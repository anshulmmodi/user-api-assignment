using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Middleware (logging)
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    await next();
});

List<User> users = new();

// GET
app.MapGet("/users", () => users);

// POST
app.MapPost("/users", (User user) =>
{
    if (string.IsNullOrEmpty(user.Name)) return Results.BadRequest("Invalid data");
    user.Id = users.Count + 1;
    users.Add(user);
    return Results.Ok(user);
});

// PUT
app.MapPut("/users/{id}", (int id, User updated) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    if (user == null) return Results.NotFound();

    user.Name = updated.Name;
    user.Email = updated.Email;
    return Results.Ok(user);
});

// DELETE
app.MapDelete("/users/{id}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    if (user == null) return Results.NotFound();

    users.Remove(user);
    return Results.Ok();
});

app.Run();

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }

    [EmailAddress]
    public string Email { get; set; }
}
