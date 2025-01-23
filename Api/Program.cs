using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContextPool<TodoDbContext>(
    options => options.UseSqlServer(
        builder.Configuration["ConnectionString"]
        ));
builder.Services.AddOpenApi();
builder.Services.AddCors(options => options.AddDefaultPolicy(
    builder => builder.AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins("https://localhost:5000")
));

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();
app.UseHttpsRedirection();
app.UseCors();
app.UseExceptionHandler(e => e.Run(async context =>
        await TypedResults.Problem().ExecuteAsync(context)));

app.MapGet("/", () => "Alive!");
app.MapGet("/todos", async (TodoDbContext db) => await db.Todos.ToListAsync());
app.MapPost("/todos", async (Todo todo, TodoDbContext db) =>
{
    todo.UserEmail = "";
    await db.Todos.AddAsync(todo);
    await db.SaveChangesAsync();
    return TypedResults.Created($"/todos/{todo.Id}", todo);
});

app.Run();
class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    public DbSet<Todo> Todos => Set<Todo>();
}

class Todo
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public bool IsComplete { get; set; }
    public string? UserEmail { get; set; }
}
