using System;
using System.Collections.Generic;
using System.Linq;

namespace Dockson.Domain.Projections
{
	public class IntervalProjection<TMessage> : IProjection<IntervalState, TMessage>
		where TMessage : IProjectable
	{
		private readonly Action<string, DayDate, TrendView> _updateView;

		public IntervalProjection(Action<string, DayDate, TrendView> updateView)
		{
			_updateView = updateView;
			State = new IntervalState();
		}

		public IntervalState State { get; set; }

		public void Project(TMessage message)
		{
			var builds = State.Builds;
			var buildTime = message.Timestamp;
			var day = new DayDate(buildTime);

			foreach (var group in message.Groups)
			{
				var lastBuild = builds.LastOrDefault(build => build.Group.EqualsIgnore(group));

				builds.Add(new IntervalState.Interval
				{
					Timestamp = buildTime,
					Group = group,
					ElapsedMinutes = lastBuild != null
						? (buildTime - lastBuild.Timestamp).TotalMinutes
						: 0
				});

				var deltas = builds
					.Where(d => d.Group.EqualsIgnore(group))
					.Where(d => day.Includes(d.Timestamp) && d.ElapsedMinutes > 0)
					.Select(d => d.ElapsedMinutes)
					.ToArray();

				_updateView(@group, day, new TrendView
				{
					Median = deltas.Any() ? deltas.Median() : 0,
					Deviation = deltas.Any() ? deltas.StandardDeviation() : 0
				});
			}
		}

		
	}

	public class IntervalState
	{
		public IntervalState()
		{
			Builds = new List<Interval>();
		}

		public List<Interval> Builds { get; set; }

		public class Interval
		{
			public string Group;
			public DateTime Timestamp;
			public double ElapsedMinutes;
		}
	}
}
