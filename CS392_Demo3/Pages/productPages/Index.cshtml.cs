using CS392_Demo3.Models;
using CS392_Demo3.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CS392_Demo3.Pages.ProductPages
{
    public class IndexModel : PageModel
    {
        private readonly MongoDBService _mongoService;

        public List<Product> Products { get; set; }

        public IndexModel(MongoDBService mongoService)
        {
            _mongoService = mongoService;
        }

        public async Task OnGetAsync()
        {
            // Load all products from MongoDB
            Products = await _mongoService.GetAllAsync();
        }
    }
}
