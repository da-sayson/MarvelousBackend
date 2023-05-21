using Microsoft.EntityFrameworkCore;

namespace MarvelousBackend
{
	public class TaskDb : DbContext
	{
		public TaskDb(DbContextOptions<TaskDb> options) : base(options) { }
		public DbSet<Task> Tasks => Set<Task>();
	}
}
