using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Rest.Models
{
    public class ProductDBContext : DbContext
    {
        public ProductDBContext(DbContextOptions<ProductDBContext> options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
    }
}
