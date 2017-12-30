using System;
using System.Collections.Generic;
using Dockson.Domain.Transformers.Build;

namespace Dockson.Domain.Projections
{
	public class BuildFailureRateProjection : IProjection<BuildFailed, BuildSucceeded>
	{
		private readonly Action<string, DayDate, RateView> _updateView;
		private readonly Cache<string, Cache<DayDate, Counts>> _trackers;

		public BuildFailureRateProjection(Action<string, DayDate, RateView> updateView)
		{
			_updateView = updateView;
			_trackers = new Cache<string, Cache<DayDate, Counts>>(
				StringComparer.OrdinalIgnoreCase,
				key => new Cache<DayDate, Counts>(x => new Counts())
			);
		}

		public void Finish(BuildSucceeded message)
		{
			Project(message.Timestamp, message.Groups, counts => counts.Successes++);
		}

		public void Start(BuildFailed message)
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

				_updateView(@group, day, new RateView
				{
					Rate = (counts.Failures / counts.Total) * 100
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
