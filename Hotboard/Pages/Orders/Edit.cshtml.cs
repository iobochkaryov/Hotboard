using Hotboard.Data;
using Hotboard.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotboard.Pages.Orders
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Order Order { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                Order = new Order();
                return Page();
            }

            Order = await _db.Orders.FindAsync(id);
            if (Order == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid) return Page();

            if (Order.Id == 0)
            {
                // set CSR to the current user who creates the order
                Order.CSR = User?.Identity?.Name ?? Order.CSR;
                _db.Orders.Add(Order);
            }
            else
            {
                // preserve CSR when updating existing order
                var existing = await _db.Orders.FindAsync(Order.Id);
                if (existing == null) return NotFound();

                existing.SO = Order.SO;
                existing.Del = Order.Del;
                existing.PO = Order.PO;
                existing.Rec = Order.Rec;
                existing.MO = Order.MO;
                existing.RO = Order.RO;
                existing.LinkForOrder = Order.LinkForOrder;
                existing.LinkForIncomingPO = Order.LinkForIncomingPO;
                existing.LinkForMO = Order.LinkForMO;
                existing.LinkForRO = Order.LinkForRO;
                existing.Customer = Order.Customer;
                existing.Worker = Order.Worker;
                // do not change existing.CSR
                existing.Courier = Order.Courier;
                existing.Comment = Order.Comment;
                // update status from the form when saving manually
                existing.Status = Order.Status;
            }

            await _db.SaveChangesAsync();
            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (Order.Id != 0)
            {
                var o = await _db.Orders.FindAsync(Order.Id);
                if (o != null) { _db.Orders.Remove(o); await _db.SaveChangesAsync(); }
            }
            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPostNextStepAsync()
        {
            if (Order.Id == 0) return RedirectToPage("Index");

            var o = await _db.Orders.FindAsync(Order.Id);
            if (o == null) return NotFound();

            o.Status = o.Status switch
            {
                OrderStatus.NotReady => OrderStatus.Waiting,
                OrderStatus.Waiting => OrderStatus.Ready,
                OrderStatus.Ready => OrderStatus.Printed,
                OrderStatus.Printed => OrderStatus.Posted,
                OrderStatus.Posted => OrderStatus.CalledIn,
                OrderStatus.CalledIn => OrderStatus.Complete,
                _ => o.Status
            };

            await _db.SaveChangesAsync();
            // After advancing status, return to the orders list
            return RedirectToPage("Index");
        }

        // New handler for setting status immediately; creates order if new, or updates existing
        public async Task<IActionResult> OnPostSetStatusAsync(string status)
        {
            if (!Enum.TryParse<OrderStatus>(status, ignoreCase: true, out var s))
            {
                return BadRequest();
            }

            if (Order.Id == 0)
            {
                // create new order with selected status
                Order.Status = s;
                Order.CSR = User?.Identity?.Name ?? Order.CSR;
                _db.Orders.Add(Order);
            }
            else
            {
                var o = await _db.Orders.FindAsync(Order.Id);
                if (o == null) return NotFound();
                o.Status = s;
                // optionally update other fields from bound Order
                o.SO = Order.SO;
                o.Del = Order.Del;
                o.PO = Order.PO;
                o.Rec = Order.Rec;
                o.MO = Order.MO;
                o.RO = Order.RO;
                o.LinkForOrder = Order.LinkForOrder;
                o.LinkForIncomingPO = Order.LinkForIncomingPO;
                o.LinkForMO = Order.LinkForMO;
                o.LinkForRO = Order.LinkForRO;
                o.Customer = Order.Customer;
                o.Worker = Order.Worker;
                o.Courier = Order.Courier;
                o.Comment = Order.Comment;
            }

            await _db.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
