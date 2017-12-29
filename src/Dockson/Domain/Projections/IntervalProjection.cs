using System;
using System.Collections.Generic;
using System.Linq;
using Dockson.Domain.Views;

namespace Dockson.Domain.Projections
{
	public class IntervalProjection<TMessage> : IProjection<TMessage>
		where TMessage : IProjectable
	{
		private readonly Action<string, DayDate, IntervalSummary> _updateView;
		private readonly List<Interval> _builds;

		public IntervalProjection(Action<string, DayDate, IntervalSummary> updateView)
		{
			_updateView = updateView;
			_builds = new List<Interval>();
		}

		public void Project(TMessage message)
		{
			var buildTime = message.Timestamp;
			var day = new DayDate(buildTime);

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

				var deltas = _builds
					.Where(d => d.Group.EqualsIgnore(group))
					.Where(d => day.Includes(d.Timestamp) && d.ElapsedMinutes > 0)
					.Select(d => d.ElapsedMinutes)
					.ToArray();

				_updateView(@group, day, new IntervalSummary
				{
					Median = deltas.Any() ? deltas.Median() : 0,
					Deviation = deltas.Any() ? deltas.StandardDeviation() : 0
				});
			}
		}

		private class Interval
		{
			public string Group;
			public DateTime Timestamp;
			public double ElapsedMinutes;
		}
	}
}
