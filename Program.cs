using Microsoft.EntityFrameworkCore;
using Rest.DataAccess;
using Rest.Models;

var builder = WebApplication.CreateBuilder();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ProductDBContext>(options =>
options.UseSqlServer(connectionString));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

var _dbContext = new ProductDBContext(app.Services.CreateScope().ServiceProvider.GetRequiredService<DbContextOptions<ProductDBContext>>());

app.Run(async (context) =>
{
    var response = context.Response;
    var request = context.Request;
    var path = request.Path;

    if ((path == "/api/products" || path == "/api/change") && request.Method == "GET")
    {
        await GetAllProducts(response);
    }
    else if (path == "/api/products" && request.Method == "PUT")
    {
        await UpdateProduct(response, request);
    }
    else
    {
        response.ContentType = "text/html; charset=utf-8";
        await response.SendFileAsync("html/index.html");
    }
});

app.Run();

async Task GetAllProducts(HttpResponse response)
{
    await response.WriteAsJsonAsync(_dbContext.Products.ToList());
}
async Task UpdateProduct(HttpResponse response, HttpRequest request)
{
    try
    {
        Product? productData = await request.ReadFromJsonAsync<Product>();
        if (productData != null)
        {
            var product = _dbContext.Products.FirstOrDefault(u => u.Id == productData.Id);
            if (product != null)
            {
                product.Count = productData.Count;
                await _dbContext.SaveChangesAsync();
                await response.WriteAsJsonAsync(product);
            }
            else
            {
                response.StatusCode = 404;
                await response.WriteAsJsonAsync(new { message = "Пользователь не найден" });
            }
        }
        else
        {
            throw new Exception("Некорректные данные");
        }
    }
    catch (Exception)
    {
        response.StatusCode = 400;
        await response.WriteAsJsonAsync(new { message = "Некорректные данные" });
    }
}