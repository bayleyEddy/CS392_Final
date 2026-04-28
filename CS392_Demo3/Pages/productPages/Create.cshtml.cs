using CS392_Demo3.Models;
using CS392_Demo3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace CS392_Demo3.Pages.ProductPages
{
    public class CreateModel : PageModel
    {
        private readonly MongoDBService _mongoService;

        [BindProperty]
        public Product Product { get; set; } = new Product();

        public CreateModel(MongoDBService mongoService)
        {
            _mongoService = mongoService;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Product.specifications");
            ModelState.Remove("Product.reviews");
            ModelState.Remove("Product.sizes");
            ModelState.Remove("Product.brand");
            ModelState.Remove("Product.Id");

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    if (error.Value.Errors.Count > 0)
                        Console.WriteLine($"Field: {error.Key}, Error: {error.Value.Errors[0].ErrorMessage}");
                }
                return Page();
            }

            if (!string.IsNullOrWhiteSpace(Product.tagsString))
            {
                Product.tags = Product.tagsString
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .ToList();
            }

            Product.specifications ??= new Specifications();
            Product.reviews ??= new List<Review>();
            Product.sizes ??= new List<string>();

            await _mongoService.CreateAsync(Product);
            return RedirectToPage("Index");
        }
    }
}