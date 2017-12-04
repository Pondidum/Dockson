﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dockson.Domain.Events;
using Dockson.Domain.Views;

namespace Dockson.Domain.Projections
{
	public class MasterLeadTimeProjection
	{
		private readonly MasterLeadTimeView _view;
		private readonly Dictionary<DayDate, List<TimeSpan>> _source;

		public MasterLeadTimeProjection(MasterLeadTimeView view)
		{
			_view = view;
			_source = new Dictionary<DayDate, List<TimeSpan>>();
		}

		public void Project(MasterCommit message, Action<object> dispatch)
		{
			var key = new DayDate(message.TimeStamp);
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
