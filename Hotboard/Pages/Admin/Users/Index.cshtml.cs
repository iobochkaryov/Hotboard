using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotboard.Data;
using Hotboard.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Hotboard.Pages.Admin.Users
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;

        public IndexModel(UserManager<IdentityUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public class UserRow
        {
            public string Id { get; set; } = null!;
            public string? UserName { get; set; }
            public string? CreatedBy { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public List<UserRow> Users { get; set; } = new();

        public async Task OnGetAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var metas = await _db.UserMetas.ToListAsync();
            Users = users.Select(u => {
                var m = metas.FirstOrDefault(x => x.UserId == u.Id);
                return new UserRow {
                    Id = u.Id,
                    UserName = u.UserName,
                    CreatedBy = m == null ? "-" : (_userManager.Users.FirstOrDefault(x => x.Id == m.CreatedByUserId)?.UserName ?? "-"),
                    CreatedAt = m?.CreatedAt
                };
            }).ToList();
        }
    }
}
