using Microsoft.EntityFrameworkCore;
using Rest.Models;

namespace Rest.DataAccess
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            string[] names = new string[5] { "Стул", "Стол", "Кресло", "Диван", "Шкаф" };

            using (var _context = new ProductDBContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ProductDBContext>>()))
            {
                if (_context.Products.Any())
                {
                    return;
                }
                for (int i = 0; i < names.Length; i++)
                {
                    await _context.Products.AddAsync(
                        new Product { Id = Guid.NewGuid(), Name = names[i], Count = 1 });
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
