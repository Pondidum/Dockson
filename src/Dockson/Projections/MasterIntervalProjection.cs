using System;
using System.Collections.Generic;
using System.Linq;
using Dockson.Events;
using Dockson.Views;

namespace Dockson.Projections
{
	public class MasterIntervalProjection
	{
		private readonly MasterIntervalView _view;
		private readonly List<CommitDelta> _source;

		public MasterIntervalProjection(MasterIntervalView view)
		{
			_view = view;
			_source = new List<CommitDelta>();
		}

		public void Project(MasterCommit message, Action<object> dispatch)
		{
			var commitTime = message.TimeStamp;
			var key = commitTime.Date;

			var last = _source.LastOrDefault();

			_source.Add(new CommitDelta
			{
				Timestamp = commitTime,
				ElapsedMinutes = last != null
					? (commitTime - last.Timestamp).TotalMinutes
					: 0
			});

			var deltas = _source
				.Where(d => d.Timestamp.Date == key && d.ElapsedMinutes > 0)
				.Select(d => d.ElapsedMinutes)
				.ToArray();

			_view.Medians[key] = deltas.Any() ? deltas.Median() : 0;
			_view.StandardDeviations[key] = deltas.Any() ? deltas.StandardDeviation() : 0;
			_view.Days.Add(key);
		}

		private class CommitDelta
		{
			public DateTime Timestamp;
			public double ElapsedMinutes;
		}
	}
}
