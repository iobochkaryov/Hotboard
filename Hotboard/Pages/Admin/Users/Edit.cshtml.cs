using System;
using System.Linq;
using System.Threading.Tasks;
using Hotboard.Data;
using Hotboard.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hotboard.Pages.Admin.Users
{
    public class EditModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;

        public EditModel(UserManager<IdentityUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [BindProperty(SupportsGet = true)]
        public string? Id { get; set; }

        [BindProperty]
        public string? UserName { get; set; }

        [BindProperty]
        public string? Password { get; set; }

        [BindProperty]
        public string Role { get; set; } = "admin";

        public bool IsNew => string.IsNullOrEmpty(Id);

        public async Task<IActionResult> OnGetAsync()
        {
            if (IsNew) return Page();
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null) return NotFound();
            UserName = user.UserName;
            var roles = await _userManager.GetRolesAsync(user);
            Role = roles.FirstOrDefault() ?? "admin";
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (IsNew)
            {
                var u = new IdentityUser { UserName = UserName, Email = UserName + "@local" };
                var createResult = await _userManager.CreateAsync(u, string.IsNullOrEmpty(Password) ? "Admin123!" : Password);
                if (!createResult.Succeeded)
                {
                    ModelState.AddModelError("", string.Join("; ", createResult.Errors.Select(e => e.Description)));
                    return Page();
                }
                // assign role
                if (!string.IsNullOrEmpty(Role)) await _userManager.AddToRoleAsync(u, Role);
                // add metadata
                _db.UserMetas.Add(new UserMeta { UserId = u.Id, CreatedByUserId = (await _userManager.GetUserAsync(User))?.Id, CreatedAt = DateTime.UtcNow });
                await _db.SaveChangesAsync();
                return RedirectToPage("/Admin/Users/Index");
            }

            var user = await _userManager.FindByIdAsync(Id);
            if (user == null) return NotFound();

            if (!string.Equals(user.UserName, UserName, StringComparison.OrdinalIgnoreCase))
            {
                user.UserName = UserName;
                user.Email = UserName + "@local";
                var ur = await _userManager.UpdateAsync(user);
                if (!ur.Succeeded) { ModelState.AddModelError("", string.Join(';', ur.Errors.Select(e => e.Description))); return Page(); }
            }

            if (!string.IsNullOrEmpty(Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var rr = await _userManager.ResetPasswordAsync(user, token, Password);
                if (!rr.Succeeded) { ModelState.AddModelError("", string.Join(';', rr.Errors.Select(e => e.Description))); return Page(); }
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains(Role))
            {
                // remove existing roles and add new
                foreach (var r in roles) await _userManager.RemoveFromRoleAsync(user, r);
                await _userManager.AddToRoleAsync(user, Role);
            }

            return RedirectToPage("/Admin/Users/Index");
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (IsNew) return RedirectToPage("/Admin/Users/Index");
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null) return NotFound();
            var dr = await _userManager.DeleteAsync(user);
            if (!dr.Succeeded) { ModelState.AddModelError("", string.Join(';', dr.Errors.Select(e => e.Description))); return Page(); }
            // remove metadata
            var m = await _db.UserMetas.FindAsync(Id);
            if (m != null) { _db.UserMetas.Remove(m); await _db.SaveChangesAsync(); }
            return RedirectToPage("/Admin/Users/Index");
        }
    }
}
