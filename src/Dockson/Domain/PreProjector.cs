using System;
using System.Collections.Generic;
using System.Linq;

namespace Dockson.Domain
{
	public class PreProjector : IProjector
	{
		private readonly IProjector _internal;
		private readonly Func<DateTime> _getDate;

		public PreProjector(IProjector @internal, Func<DateTime> getDate = null)
		{
			_internal = @internal;
			_getDate = getDate ?? (() => DateTime.UtcNow);
		}

		public void Project(Notification notification)
		{
			if (notification.Timestamp == DateTime.MinValue)
				notification.Timestamp = _getDate();

			notification.Name = Clean(notification.Name);
			notification.Groups = new HashSet<string>(notification.Groups.Select(Clean), StringComparer.OrdinalIgnoreCase);

			_internal.Project(notification);
		}

		private string Clean(string value) => string.IsNullOrWhiteSpace(value)
			? string.Empty
			: value.Replace('.', '-');
	}
}
