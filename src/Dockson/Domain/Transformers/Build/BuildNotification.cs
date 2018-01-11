namespace Dockson.Domain.Transformers.Build
{
	public class BuildNotification : Notification
	{
		public string Version { get; set; }
		public string Status { get; set; }
	}
}
