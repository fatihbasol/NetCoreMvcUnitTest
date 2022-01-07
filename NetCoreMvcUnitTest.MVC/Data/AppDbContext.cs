using Microsoft.EntityFrameworkCore;
using NetCoreMvcUnitTest.MVC.Models;

namespace NetCoreMvcUnitTest.MVC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        DbSet<Product> Products { get; set; }
    }
}
