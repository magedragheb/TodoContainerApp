using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<TodoDbContext>(options =>
    options.UseSqlServer(builder.Configuration["connectionkv"]));

builder.Services.AddOpenApi();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("EntraId"));
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(p =>
        p.AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins(builder.Configuration["fronturl"]!)
));

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler(e => e.Run(async context =>
        await TypedResults.Problem().ExecuteAsync(context)));

app.MapGet("/", (IConfiguration config) =>
{
    var response = new Dictionary<string, string>();
    foreach (var item in config.AsEnumerable())
    {
        response.Add(item.Key, item.Value ?? "null");
    }
    return TypedResults.Ok(response);
}).RequireAuthorization();
app.MapGet("/todos", async (TodoDbContext db) => await db.Todos.ToListAsync()).RequireAuthorization();
app.MapPost("/todos", async (Todo todo, TodoDbContext db) =>
{
    todo.UserEmail = "mail@mail.com";
    await db.Todos.AddAsync(todo);
    await db.SaveChangesAsync();
    return TypedResults.Created($"/todos/{todo.Id}", todo);
}).RequireAuthorization();

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
