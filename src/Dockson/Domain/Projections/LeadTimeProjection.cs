using System;
using System.Collections.Generic;
using Dockson.Domain.Views;

namespace Dockson.Domain.Projections
{
	public class LeadTimeProjection<TStart, TFinish> : IProjection<TStart, TFinish>
		where TStart : IProjectable
		where TFinish : IProjectable
	{
		private readonly Action<string, DayDate, LeadTimeSummary> _updateView;
		private readonly Func<TStart, string> _startIdentity;
		private readonly Func<TFinish, string> _finishIdentity;
		private readonly Dictionary<string, DateTime> _commits;
		private readonly Cache<string, List<double>> _times;

		public LeadTimeProjection(Action<string, DayDate, LeadTimeSummary> updateView, Func<TStart, string> startIdentity, Func<TFinish, string> finishIdentity)
		{
			_updateView = updateView;
			_startIdentity = startIdentity;
			_finishIdentity = finishIdentity;
			_commits = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
			_times = new Cache<string, List<double>>(
				StringComparer.OrdinalIgnoreCase,
				key => new List<double>());
		}

		public void Start(TStart message)
		{
			_commits[_startIdentity(message)] = message.Timestamp;
		}

		public void Finish(TFinish message)
		{
			var id = _finishIdentity(message);
			var day = new DayDate(message.Timestamp);

			DateTime startTimestamp;

			if (_commits.Remove(id, out startTimestamp))
			{
				var leadTime = message.Timestamp - startTimestamp;

				foreach (var @group in message.Groups)
				{
					_times[group].Add(leadTime.TotalMinutes);

					_updateView(group, day, new LeadTimeSummary
					{
						Median = _times[@group].Median(),
						Deviation = _times[@group].StandardDeviation()
					});
				}
			}
		}
	}
}
