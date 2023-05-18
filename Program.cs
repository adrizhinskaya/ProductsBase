using Microsoft.EntityFrameworkCore;
using Rest.DataAccess;
using Rest.Models;
using System.Text.RegularExpressions;
 
// ��������� ������
List<Person> users = new List<Person>
{
    new() { Id = Guid.NewGuid().ToString(), Name = "����", Count = 1 },
    new() { Id = Guid.NewGuid().ToString(), Name = "����", Count = 1 },
    new() { Id = Guid.NewGuid().ToString(), Name = "������", Count = 1 }
};

var builder = WebApplication.CreateBuilder();

builder.Services.AddDbContext<ProductContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
app.Run(async (context) =>
{
    var response = context.Response;
    var request = context.Request;
    var path = request.Path;

    if (path == "/api/users" && request.Method == "GET")
    {
        await GetAllPeople(response);
    }
    else if (path == "/api/users" && request.Method == "PUT")
    {
        await UpdatePerson(response, request);
    }
    else
    {
        response.ContentType = "text/html; charset=utf-8";
        await response.SendFileAsync("html/index.html");
    }
});

app.Run();

// ��������� ���� �������������
async Task GetAllPeople(HttpResponse response)
{
    await response.WriteAsJsonAsync(users);
}
async Task UpdatePerson(HttpResponse response, HttpRequest request)
{
    try
    {
        // �������� ������ ������������
        Person? userData = await request.ReadFromJsonAsync<Person>();
        if (userData != null)
        {
            // �������� ������������ �� id
            var user = users.FirstOrDefault(u => u.Id == userData.Id);
            // ���� ������������ ������, �������� ��� ������ � ���������� ������� �������
            if (user != null)
            {
                user.Count = userData.Count;
                await response.WriteAsJsonAsync(user);
            }
            else
            {
                response.StatusCode = 404;
                await response.WriteAsJsonAsync(new { message = "������������ �� ������" });
            }
        }
        else
        {
            throw new Exception("������������ ������");
        }
    }
    catch (Exception)
    {
        response.StatusCode = 400;
        await response.WriteAsJsonAsync(new { message = "������������ ������" });
    }
}
public class Person
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public int Count { get; set; }
}