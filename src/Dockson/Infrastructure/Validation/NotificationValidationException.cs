using System;
using Dockson.Domain;

namespace Dockson.Infrastructure.Validation
{
	public class NotificationValidationException : Exception
	{
		public Notification Notification { get; }
		public string[] Messages { get; }

		public NotificationValidationException(Notification notification, string[] messages)
		{
			Notification = notification;
			Messages = messages;
		}
	}
}