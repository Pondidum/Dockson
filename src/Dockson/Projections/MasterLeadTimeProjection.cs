using System;
using System.Collections.Generic;
using System.Linq;
using Dockson.Events;
using Dockson.Views;

namespace Dockson.Projections
{
	public class MasterLeadTimeProjection
	{
		private readonly MasterLeadTimeView _view;
		private readonly Dictionary<DateTime, List<TimeSpan>> _source;

		public MasterLeadTimeProjection(MasterLeadTimeView view)
		{
			_view = view;
			_source = new Dictionary<DateTime, List<TimeSpan>>();
		}

		public void Project(MasterCommit message, Action<object> dispatch)
		{
			var key = message.TimeStamp.Date;
			var leadTime = message.LeadTime;

			if (_source.ContainsKey(key) == false)
				_source[key] = new List<TimeSpan>();

			_source[key].Add(leadTime);

			_view.StandardDeviations[key] = _source[key].Select(spans => spans.TotalMinutes).StandardDeviation();
			_view.Medians[key] = _source[key].Select(spans => spans.TotalMinutes).Median();
			_view.Days.Add(key);
		}
	}
}
