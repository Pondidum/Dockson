using System;
using System.Collections.Generic;
using System.Linq;
using Dockson.Domain.Transformers.MasterCommit;
using Dockson.Domain.Views;

namespace Dockson.Domain.Projections.MasterLeadTime
{
	public class MasterLeadTimeProjection : IProjection<MasterCommit>
	{
		private readonly LeadTimeView _view;
		private readonly Cache<string, Cache<DayDate, List<TimeSpan>>> _source;

		public MasterLeadTimeProjection(LeadTimeView view)
		{
			_view = view;
			_source = new Cache<string, Cache<DayDate, List<TimeSpan>>>(
				StringComparer.OrdinalIgnoreCase,
				key => new Cache<DayDate, List<TimeSpan>>(x => new List<TimeSpan>()));
		}

		public void Project(MasterCommit message)
		{
			var key = new DayDate(message.Timestamp);
			var leadTime = message.LeadTime;

			foreach (var @group in message.Groups)
			{
				var groupTimes = _source[@group];

				groupTimes[key].Add(leadTime);

				var deltas = groupTimes[key]
					.Select(spans => spans.TotalMinutes)
					.ToArray();

				_view.TryAdd(group, new GroupSummary<LeadTimeSummary>());

				_view[group].Daily[key] = new LeadTimeSummary
				{
					Median = deltas.Any() ? deltas.Median() : 0,
					Deviation = deltas.Any() ? deltas.StandardDeviation() : 0
				};
			}
		}
	}
}
