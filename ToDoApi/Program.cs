using Microsoft.EntityFrameworkCore;
using ToDoApi.Data;
using ToDoApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(opt => 
    opt.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

app.MapGet("api/todo", async (AppDbContext context) =>
{
    var items = await context.Todos.ToListAsync();
    
    return Results.Ok(items);
});

app.MapPost("api/todo", async (AppDbContext context, ToDo toDo) =>
{
    await context.Todos.AddAsync(toDo);
    
    await context.SaveChangesAsync();

    return Results.Created($"api/todo/{toDo.Id}", toDo);
});

app.MapPut("api/todo/{id}", async (AppDbContext context, int id, ToDo toDo) =>
{
    var toDoModel = await context.Todos.FirstOrDefaultAsync(t => t.Id == id);

    if(toDoModel == null)
    {
        return Results.NotFound();
    }
    
    toDoModel.ToDoName = toDo.ToDoName;

    await context.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("api/todo/{id}", async (AppDbContext context, int id) =>
{
    var toDoModel = await context.Todos.FirstOrDefaultAsync(x => x.Id == id);
    if(toDoModel is null)
    {
        return Results.NotFound();
    }

    context.Todos.Remove(toDoModel);
    await context.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();

