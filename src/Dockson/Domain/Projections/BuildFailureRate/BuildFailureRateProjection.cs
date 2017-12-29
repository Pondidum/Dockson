using System;
using System.Collections.Generic;
using Dockson.Domain.Transformers.Build;

namespace Dockson.Domain.Projections.BuildFailureRate
{
	public class BuildFailureRateProjection : IProjection<BuildSucceeded>, IProjection<BuildFailed>
	{
		private readonly Action<string, DayDate, BuildFailureRateSummary> _updateView;
		private readonly Cache<string, Cache<DayDate, Counts>> _trackers;

		public BuildFailureRateProjection(Action<string, DayDate, BuildFailureRateSummary> updateView)
		{
			_updateView = updateView;
			_trackers = new Cache<string, Cache<DayDate, Counts>>(
				StringComparer.OrdinalIgnoreCase,
				key => new Cache<DayDate, Counts>(x => new Counts())
			);
		}

		public void Project(BuildSucceeded message)
		{
			Project(message.Timestamp, message.Groups, counts => counts.Successes++);
		}

		public void Project(BuildFailed message)
		{
			Project(message.Timestamp, message.Groups, counts => counts.Failures++);
		}

		private void Project(DateTime timestamp, IEnumerable<string> groups, Action<Counts> action)
		{
			var day = new DayDate(timestamp);

			foreach (var @group in groups)
			{
				var counts = _trackers[group][day];
				action(counts);

				_updateView(@group, day, new BuildFailureRateSummary
				{
					FailureRate = (counts.Failures / counts.Total) * 100
				});
			}
		}

		private class Counts
		{
			public int Failures { get; set; }
			public int Successes { get; set; }
			public double Total => Successes + Failures;
		}
	}
}
