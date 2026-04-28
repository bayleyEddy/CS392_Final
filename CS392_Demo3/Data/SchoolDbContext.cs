using Microsoft.EntityFrameworkCore;
using CS392_Demo3.Models;

namespace CS392_Demo3.Data
{
    public class SchoolDbContext : DbContext
    {
        public SchoolDbContext(DbContextOptions<SchoolDbContext> options) : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; } = null!;
    }
}
