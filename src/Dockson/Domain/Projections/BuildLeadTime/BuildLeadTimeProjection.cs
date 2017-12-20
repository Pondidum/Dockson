using System;
using System.Collections.Generic;
using Dockson.Domain.Transformers.Build;
using Dockson.Domain.Transformers.MasterCommit;
using Dockson.Domain.Views;

namespace Dockson.Domain.Projections.BuildLeadTime
{
	public class BuildLeadTimeProjection : IProjection<MasterCommit>, IProjection<BuildSucceeded>
	{
		private readonly BuildLeadTimeView _view;
		private readonly Dictionary<string, DateTime> _commits;
		private readonly Cache<string, List<double>> _times;

		public BuildLeadTimeProjection(BuildLeadTimeView view)
		{
			_view = view;
			_commits = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
			_times = new Cache<string, List<double>>(
				StringComparer.OrdinalIgnoreCase,
				key => new List<double>());
		}

		public void Project(MasterCommit message)
		{
			_commits[message.CommitHash] = message.Timestamp;
		}

		public void Project(BuildSucceeded build)
		{
			var commit = build.CommitHash;
			var day = new DayDate(build.Timestamp);

			DateTime commitTimestamp;

			if (_commits.Remove(commit, out commitTimestamp))
			{
				var leadTime = build.Timestamp - commitTimestamp;

				foreach (var @group in build.Groups)
				{
					_times[group].Add(leadTime.TotalMinutes);

					_view.TryAdd(group, new GroupSummary<BuildLeadTimeSummary>());
					_view[group].Daily[day] = new BuildLeadTimeSummary
					{
						Median = _times[group].Median(),
						Deviation = _times[group].StandardDeviation()
					};
				}
			}
		}
	}
}
