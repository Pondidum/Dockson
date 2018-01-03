using System.Collections.Generic;
using Dockson.Infrastructure.Validation;

namespace Dockson.Domain.Transformers.Commits
{
	public class CommitNotificationValidator : INotificationValidator<CommitNotification>
	{
		public IEnumerable<string> Validate(CommitNotification notification)
		{
			if (string.IsNullOrWhiteSpace(notification.Branch))
				yield return "You must specify the project's branch";

			if (string.IsNullOrWhiteSpace(notification.Commit))
				yield return "You must specify the project's Commit Hash";
		}
	}
}
