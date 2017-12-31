using System;

namespace Dockson.Domain.Projections
{
	public class LeadTimeProjection<TStart, TFinish> : IProjection<LeadTimeState, TStart, TFinish>
		where TStart : IProjectable
		where TFinish : IProjectable
	{
		private readonly Action<string, DayDate, TrendView> _updateView;
		private readonly Func<TStart, string> _startIdentity;

		private readonly Func<TFinish, string> _finishIdentity;

		public LeadTimeProjection(Action<string, DayDate, TrendView> updateView, Func<TStart, string> startIdentity, Func<TFinish, string> finishIdentity)
		{
			_updateView = updateView;
			_startIdentity = startIdentity;
			_finishIdentity = finishIdentity;
		}

		public LeadTimeState State { get; set; }

		public void Start(TStart message)
		{
			State.Commits[_startIdentity(message)] = message.Timestamp;
		}

		public void Finish(TFinish message)
		{
			var id = _finishIdentity(message);
			var day = new DayDate(message.Timestamp);

			DateTime startTimestamp;

			if (State.Commits.Remove(id, out startTimestamp))
			{
				var leadTime = message.Timestamp - startTimestamp;

				foreach (var @group in message.Groups)
				{
					State.Times[group].Add(leadTime.TotalMinutes);

					_updateView(group, day, new TrendView
					{
						Median = State.Times[@group].Median(),
						Deviation = State.Times[@group].StandardDeviation()
					});
				}
			}
		}
	}
}
