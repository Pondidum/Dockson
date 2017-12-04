using System;
using System.Collections.Generic;
using System.Linq;
using Dockson.Domain.Events;
using Dockson.Domain.Views;

namespace Dockson.Domain.Projections
{
	public class MasterIntervalProjection
	{
		private readonly MasterIntervalView _view;
		private readonly List<CommitDelta> _source;

		public MasterIntervalProjection(MasterIntervalView view)
		{
			_view = view;
			_source = new List<CommitDelta>();
		}

		public void Project(MasterCommit message, Action<object> dispatch)
		{
			var commitTime = message.TimeStamp;

			var last = _source.LastOrDefault();

			_source.Add(new CommitDelta
			{
				Timestamp = commitTime,
				ElapsedMinutes = last != null
					? (commitTime - last.Timestamp).TotalMinutes
					: 0
			});

			UpdateDailySummary(commitTime);
			UpdateWeeklySummary(commitTime);
		}

		private void UpdateDailySummary(DateTime commitTime)
		{
			var key = commitTime.Date;
			var deltas = _source
				.Where(d => d.Timestamp.Date == key && d.ElapsedMinutes > 0)
				.Select(d => d.ElapsedMinutes)
				.ToArray();

			_view.Daily[key] = new Summary
			{
				Median = deltas.Any() ? deltas.Median() : 0,
				Deviation = deltas.Any() ? deltas.StandardDeviation() : 0
			};
		}

		private void UpdateWeeklySummary(DateTime commitTime)
		{
			var week = new WeekDate(commitTime); 

			var deltas = _source
				.Where(d => d.Timestamp.Date >= week.Start && d.Timestamp.Date <= week.Finish && d.ElapsedMinutes > 0)
				.Select(d => d.ElapsedMinutes)
				.ToArray();

			_view.Weekly[week] = new Summary
			{
				Median = deltas.Any() ? deltas.Median() : 0,
				Deviation = deltas.Any() ? deltas.StandardDeviation() : 0
			};
		}

		private class CommitDelta
		{
			public DateTime Timestamp;
			public double ElapsedMinutes;
		}
	}
}
