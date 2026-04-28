using CS392_Demo3.Models;
using CS392_Demo3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CS392_Demo3.Pages.ProductPages
{
    public class DeleteModel : PageModel
    {
        private readonly MongoDBService _mongoService;

        public Product Product { get; set; }

        public DeleteModel(MongoDBService mongoService)
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

        public async Task<IActionResult> OnPostAsync(string id)
        {
            await _mongoService.DeleteAsync(id);
            return RedirectToPage("Index");
        }
    }
}
