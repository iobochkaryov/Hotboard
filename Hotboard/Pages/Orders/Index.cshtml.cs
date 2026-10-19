using Hotboard.Data;
using Hotboard.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace Hotboard.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public List<Order> Orders { get; set; } = new();

        public string? DefaultFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Filter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        public async Task OnGetAsync()
        {
            var q = _db.Orders.AsQueryable();

            if (!string.IsNullOrWhiteSpace(Filter))
            {
                q = q.Where(o => o.Worker == Filter || o.CSR == Filter || o.Courier == Filter);
            }

            if (!string.IsNullOrWhiteSpace(Search))
            {
                var s = Search.ToLower();
                q = q.Where(o => (o.SO ?? "").ToLower().Contains(s)
                    || (o.Del ?? "").ToLower().Contains(s)
                    || (o.PO ?? "").ToLower().Contains(s)
                    || (o.Rec ?? "").ToLower().Contains(s)
                    || (o.MO ?? "").ToLower().Contains(s)
                    || (o.RO ?? "").ToLower().Contains(s)
                    || (o.Customer ?? "").ToLower().Contains(s)
                    || (o.Worker ?? "").ToLower().Contains(s)
                    || (o.CSR ?? "").ToLower().Contains(s)
                    || (o.Courier ?? "").ToLower().Contains(s)
                    || (o.Comment ?? "").ToLower().Contains(s)
                );
            }

            var list = await q.ToListAsync();
            Orders = list.OrderBy(o => GetStatusOrder(o.Status)).ThenBy(o => o.Id).ToList();

            // determine default filter based on current user's role
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                var roles = await _userManager.GetRolesAsync(currentUser);
                var role = roles.FirstOrDefault()?.ToLowerInvariant();
                DefaultFilter = role switch
                {
                    "shipping" => "Shipping",
                    "receiving" => "Receiving",
                    "manufacturing" => "Manufacturing",
                    "sealshop" => "Sealshop",
                    "traffic" => "Traffic",
                    _ => "" // admin, csr, or others -> All
                };
            }
        }

        private int GetStatusOrder(OrderStatus s) => s switch
        {
            OrderStatus.NotReady => 0,
            OrderStatus.Waiting => 1,
            OrderStatus.Ready => 2,
            OrderStatus.Printed => 3,
            OrderStatus.Posted => 4,
            OrderStatus.CalledIn => 5,
            OrderStatus.Complete => 6,
            _ => 7
        };
    }
}
