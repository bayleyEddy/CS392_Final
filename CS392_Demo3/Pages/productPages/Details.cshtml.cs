using CS392_Demo3.Models;
using CS392_Demo3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CS392_Demo3.Pages.ProductPages
{
    public class DetailsModel : PageModel
    {
        private readonly MongoDBService _mongoService;

        public Product Product { get; set; }

        public DetailsModel(MongoDBService mongoService)
        {
            _mongoService = mongoService;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            Product = await _mongoService.GetAsync(id);

            if (Product == null)
                return RedirectToPage("Index");

            return Page();
        }
    }
}
