using MarvelousBackend;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TaskDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/allTasks", async (TaskDb db) =>
	await db.Tasks.ToListAsync());

app.MapPost("/allTasks", async (MarvelousBackend.Task task, TaskDb db) =>
{
	if (String.IsNullOrEmpty(task.Name)) return Results.BadRequest();
	var newTask = new MarvelousBackend.Task
	{
		Name = task.Name,
		Checked = false,
		DateCompleted = null
	};
	db.Tasks.Add(newTask);
	await db.SaveChangesAsync();

	return Results.Created($"/allTasks/{newTask.TaskId}", newTask);
});

app.MapPut("/allTasks/check/{id}", async (int id, MarvelousBackend.Task inputTask, TaskDb db) =>
{
	var task = await db.Tasks.FindAsync(id);
	if (task is null) return Results.NotFound();
	if (task.Checked) return Results.NoContent();

	task.Checked = true;
	task.DateCompleted = DateTime.Now;
	await db.SaveChangesAsync();

	return Results.NoContent();
});

app.MapPut("/allTasks/uncheck/{id}", async (int id, MarvelousBackend.Task inputTask, TaskDb db) =>
{
	var task = await db.Tasks.FindAsync(id);
	if (task is null) return Results.NotFound();
	if (!task.Checked) return Results.NoContent();
	
	task.Checked = false;
	task.DateCompleted = null;
	await db.SaveChangesAsync();

	return Results.NoContent();
});

app.MapDelete("/allTasks/{id}", async (int id, TaskDb db) =>
{
	if (await db.Tasks.FindAsync(id) is MarvelousBackend.Task task)
	{
		db.Tasks.Remove(task);
		await db.SaveChangesAsync();
		return Results.Ok(task);
	}

	return Results.NotFound();
});


app.Run();
