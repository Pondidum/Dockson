namespace Dockson.Domain.Transformers.Deployment
{
	public class DeploymentNotification : Notification
	{
		public string Environment { get; set; }
		public string Version { get; set; }
		public string Status { get; set; }
	}
}
