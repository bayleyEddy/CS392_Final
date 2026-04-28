using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS392_Demo3.Data;
using CS392_Demo3.Models;

namespace CS392_Demo3.Pages.departmentPages
{
    public class IndexModel : PageModel
    {
        private readonly CS392_Demo3.Data.SchoolDbContext _context;

        public IndexModel(CS392_Demo3.Data.SchoolDbContext context)
        {
            _context = context;
        }

        public IList<Department> Department { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Department = await _context.Departments.ToListAsync();
        }
    }
}
