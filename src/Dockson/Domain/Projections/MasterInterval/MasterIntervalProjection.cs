using System;
using System.Collections.Generic;
using System.Linq;
using Dockson.Domain.Transformers.MasterCommit;
using Dockson.Domain.Views;

namespace Dockson.Domain.Projections.MasterInterval
{
	public class MasterIntervalProjection : IProjection<MasterCommit>
	{
		private readonly MasterIntervalView _view;
		private readonly List<CommitDelta> _source;

		public MasterIntervalProjection(MasterIntervalView view)
		{
			_view = view;
			_source = new List<CommitDelta>();
		}

		public void Project(MasterCommit message)
		{
			var commitTime = message.Timestamp;

			foreach (var group in message.Groups)
			{
				var last = _source.LastOrDefault(commit => commit.Group.EqualsIgnore(group));

				_source.Add(new CommitDelta
				{
					Timestamp = commitTime,
					Group = group,
					ElapsedMinutes = last != null
						? (commitTime - last.Timestamp).TotalMinutes
						: 0
				});

				UpdateDailySummary(commitTime, group);
			}
		}

		private void UpdateDailySummary(DateTime commitTime, string group)
		{
			var key = new DayDate(commitTime);

			var deltas = _source
				.Where(d => d.Group.EqualsIgnore(group))
				.Where(d => key.Includes(d.Timestamp) && d.ElapsedMinutes > 0)
				.Select(d => d.ElapsedMinutes)
				.ToArray();

			_view.TryAdd(group, new GroupSummary<MasterIntervalSummary>());

			_view[group].Daily[key] = new MasterIntervalSummary
			{
				Median = deltas.Any() ? deltas.Median() : 0,
				Deviation = deltas.Any() ? deltas.StandardDeviation() : 0
			};
		}

		private class CommitDelta
		{
			public DateTime Timestamp;
			public double ElapsedMinutes;
			public string Group;
		}
	}
}
