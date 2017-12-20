using System;
using System.Collections.Generic;
using System.Linq;
using Dockson.Domain.Views;

namespace Dockson.Domain.Projections
{
	public class IntervalProjection<TMessage> : IProjection<TMessage>
		where TMessage : IProjectable
	{
		private readonly IntervalView _view;
		private readonly List<Interval> _builds;

		public IntervalProjection(IntervalView view)
		{
			_view = view;
			_builds = new List<Interval>();
		}

		public void Project(TMessage message)
		{
			var buildTime = message.Timestamp;

			foreach (var group in message.Groups)
			{
				var lastBuild = _builds.LastOrDefault(build => build.Group.EqualsIgnore(group));

				_builds.Add(new Interval
				{
					Timestamp = buildTime,
					Group = group,
					ElapsedMinutes = lastBuild != null
						? (buildTime - lastBuild.Timestamp).TotalMinutes
						: 0
				});

				UpdateDailySummary(buildTime, group);
			}
		}

		private void UpdateDailySummary(DateTime buildTime, string group)
		{
			var key = new DayDate(buildTime);

			var deltas = _builds
				.Where(d => d.Group.EqualsIgnore(group))
				.Where(d => key.Includes(d.Timestamp) && d.ElapsedMinutes > 0)
				.Select(d => d.ElapsedMinutes)
				.ToArray();

			_view.TryAdd(group, new GroupSummary<IntervalSummary>());

			_view[group].Daily[key] = new IntervalSummary
			{
				Median = deltas.Any() ? deltas.Median() : 0,
				Deviation = deltas.Any() ? deltas.StandardDeviation() : 0
			};
		}

		private class Interval
		{
			public string Group;
			public DateTime Timestamp;
			public double ElapsedMinutes;
		}
	}
}
