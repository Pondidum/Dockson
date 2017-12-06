using System;
using System.Collections.Generic;
using Dockson.Domain.Transformers.Build;
using Dockson.Domain.Views;

namespace Dockson.Domain.Projections.BuildFailureRate
{
	public class BuildFailureRateProjection : IProjection<BuildSucceeded>, IProjection<BuildFailed>
	{
		private readonly BuildFailureRateView _view;
		private readonly Cache<string, Cache<DayDate, Counts>> _trackers;

		public BuildFailureRateProjection(BuildFailureRateView view)
		{
			_view = view;
			_trackers = new Cache<string, Cache<DayDate, Counts>>(
				StringComparer.OrdinalIgnoreCase,
				key => new Cache<DayDate, Counts>(x => new Counts())
			);
		}

		public void Project(BuildSucceeded message)
		{
			Project(message.TimeStamp, message.Groups, counts => counts.Successes++);
		}

		public void Project(BuildFailed message)
		{
			Project(message.TimeStamp, message.Groups, counts => counts.Failures++);
		}

		private void Project(DateTime timestamp, IEnumerable<string> groups, Action<Counts> action)
		{
			var key = new DayDate(timestamp);

			foreach (var @group in groups)
			{
				var counts = _trackers[group][key];
				action(counts);

				UpdateView(@group, key, counts);
			}
		}

		private void UpdateView(string @group, DayDate key, Counts counts)
		{
			_view.TryAdd(@group, new GroupSummary<BuildFailureRateSummary>());
			_view[@group].Daily.TryAdd(key, new BuildFailureRateSummary());

			_view[@group].Daily[key].FailureRate = (counts.Failures / counts.Total) * 100;
		}

		private class Counts
		{
			public int Failures { get; set; }
			public int Successes { get; set; }
			public double Total => Successes + Failures;
		}
	}
}
