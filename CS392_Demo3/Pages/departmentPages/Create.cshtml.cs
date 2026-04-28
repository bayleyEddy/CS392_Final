using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CS392_Demo3.Data;
using CS392_Demo3.Models;
using CS392_Demo3.Constants;
using Microsoft.AspNetCore.Authorization;

namespace CS392_Demo3.Pages.departmentPages
{
    [Authorize(Roles="Admin")]
    public class CreateModel : PageModel
    {
        private readonly CS392_Demo3.Data.SchoolDbContext _context;

        public CreateModel(CS392_Demo3.Data.SchoolDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Department Department { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Departments.Add(Department);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
