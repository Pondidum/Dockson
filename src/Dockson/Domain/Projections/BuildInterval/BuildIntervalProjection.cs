using System;
using System.Collections.Generic;
using System.Linq;
using Dockson.Domain.Transformers.Build;
using Dockson.Domain.Views;

namespace Dockson.Domain.Projections.BuildInterval
{
	public class BuildIntervalProjection : IProjection<BuildSucceeded>
	{
		private readonly BuildIntervalView _view;
		private readonly List<BuildDelta> _builds;

		public BuildIntervalProjection(BuildIntervalView view)
		{
			_view = view;
			_builds = new List<BuildDelta>();
		}

		public void Project(BuildSucceeded message)
		{
			var buildTime = message.Timestamp;

			foreach (var group in message.Groups)
			{
				var lastBuild = _builds.LastOrDefault(build => build.Group.EqualsIgnore(group));

				_builds.Add(new BuildDelta
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

			_view.TryAdd(group, new GroupSummary<BuildIntervalSummary>());

			_view[group].Daily[key] = new BuildIntervalSummary
			{
				Median = deltas.Any() ? deltas.Median() : 0,
				Deviation = deltas.Any() ? deltas.StandardDeviation() : 0
			};
		}

		private class BuildDelta
		{
			public string Group;
			public DateTime Timestamp;
			public double ElapsedMinutes;
		}
	}
}
