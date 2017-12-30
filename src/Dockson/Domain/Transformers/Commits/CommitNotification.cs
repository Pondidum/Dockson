namespace Dockson.Domain.Transformers.Commits
{
	public class CommitNotification : Notification
	{
		public string Branch { get; set; }
		public string Commit { get; set; }
	}
}
