namespace MarvelousBackend
{
	public class Task
	{
		public int TaskId { get; set; }
		public string Name { get; set; }
		public bool Checked { get; set; }
		public DateTime? DateCompleted { get; set; }
	}
}
