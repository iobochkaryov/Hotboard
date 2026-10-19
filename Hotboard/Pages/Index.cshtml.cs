using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hotboard.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Redirect to the Orders index page so authenticated users land on the orders list
            return RedirectToPage("/Orders/Index");
        }
    }
}
