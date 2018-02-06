using System.IO;
using Dockson.Domain;
using Dockson.Infrastructure;
using Newtonsoft.Json;

namespace Dockson.Storage
{
	public class NotificationStore : INotificationStore
	{
		private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.Auto,
			Formatting = Formatting.None
		};

		private readonly IFileSystem _fileSystem;
		private readonly string _logFile;

		public NotificationStore(IFileSystem fileSystem, Settings settings)
		{
			_fileSystem = fileSystem;
			_logFile = Path.Combine(settings.StoragePath, "notifications.json");
		}

		public void Append(Notification notification)
		{
			_fileSystem.AppendFile(_logFile, JsonConvert.SerializeObject(notification, Settings));
		}
	}
}
