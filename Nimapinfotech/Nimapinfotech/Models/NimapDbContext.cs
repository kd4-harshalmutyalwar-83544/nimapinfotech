using Microsoft.EntityFrameworkCore;

namespace Nimapinfotech.Models
{
    public class NimapDbContext : DbContext
    {
        public NimapDbContext(DbContextOptions options) : base(options) 
        { 
           
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
