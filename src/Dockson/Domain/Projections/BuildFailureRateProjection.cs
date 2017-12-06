using System;
using System.Collections.Generic;
using Dockson.Domain.Events;
using Dockson.Domain.Views;

namespace Dockson.Domain.Projections
{
	public class BuildFailureRateProjection
	{
		private readonly BuildFailureRateView _view;
		private readonly Dictionary<string, Dictionary<DayDate, Counts>> _trackers;

		public BuildFailureRateProjection(BuildFailureRateView view)
		{
			_view = view;
			_trackers = new Dictionary<string, Dictionary<DayDate, Counts>>(StringComparer.OrdinalIgnoreCase);
		}

		public void Project(BuildSucceeded message, Action<object> dispatch)
		{
			EnsureGroups(message.Groups);

			var key = new DayDate(message.TimeStamp);

			foreach (var @group in message.Groups)
			{
				EnsureTrackers(@group, key);

				var counts = _trackers[group][key];
				counts.Successes++;

				UpdateView(@group, key, counts);
			}
		}

		public void Project(BuildFailed message, Action<object> dispatch)
		{
			EnsureGroups(message.Groups);

			var key = new DayDate(message.TimeStamp);

			foreach (var @group in message.Groups)
			{
				EnsureTrackers(@group, key);

				var counts = _trackers[group][key];
				counts.Failures++;

				UpdateView(@group, key, counts);
			}
		}

		private void EnsureGroups(IEnumerable<string> groups) => groups
			.Each(group => _view.TryAdd(@group, new BuildFailureRateGroupSummary()));

		private void EnsureTrackers(string group, DayDate key)
		{
			_trackers.TryAdd(group, new Dictionary<DayDate, Counts>());
			_trackers[group].TryAdd(key, new Counts());
		}

		private void UpdateView(string @group, DayDate key, Counts counts)
		{
			var failureRate = ((double)counts.Failures / (double)counts.Total) * 100;

			_view[@group].Daily.TryAdd(key, new BuildFailureRateSummary());
			_view[@group].Daily[key].FailureRate = failureRate;
		}

		private class Counts
		{
			public int Failures { get; set; }
			public int Successes { get; set; }
			public int Total => Successes + Failures;
		}
	}
}
