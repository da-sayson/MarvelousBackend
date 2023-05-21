using MarvelousBackend;
using Microsoft.EntityFrameworkCore;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TaskDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddCors(options =>
{
	options.AddPolicy(
		name: MyAllowSpecificOrigins,
		policy =>
		{
			policy.WithOrigins("https://localhost:8888").AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
		}
	);
});

var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);

app.MapGet("/allTasks", async (TaskDb db) =>
	await db.Tasks.ToListAsync()
)
.RequireCors(MyAllowSpecificOrigins);

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
})
.RequireCors(MyAllowSpecificOrigins);

app.MapPut("/allTasks/check/{id}", async (int id, TaskDb db) =>
{
	var task = await db.Tasks.FindAsync(id);
	if (task is null) return Results.NotFound();
	if (task.Checked) return Results.Ok();

	task.Checked = true;
	task.DateCompleted = DateTime.Now;
	await db.SaveChangesAsync();

	return TypedResults.Ok(task);
})
.RequireCors(MyAllowSpecificOrigins);

app.MapPut("/allTasks/uncheck/{id}", async (int id, TaskDb db) =>
{
	var task = await db.Tasks.FindAsync(id);
	if (task is null) return Results.NotFound();
	if (!task.Checked) return Results.NoContent();

	task.Checked = false;
	task.DateCompleted = null;
	await db.SaveChangesAsync();

	return TypedResults.Ok(task);
})
.RequireCors(MyAllowSpecificOrigins);

app.MapDelete("/allTasks", async (TaskDb db) =>
{
	var taskList = await db.Tasks.ToListAsync();
	db.Tasks.RemoveRange(taskList);
	await db.SaveChangesAsync();

	return Results.Ok();
})
.RequireCors(MyAllowSpecificOrigins);

app.Run("https://localhost:8888");
