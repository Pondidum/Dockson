using System;
using System.Collections.Generic;
using System.Linq;
using Dockson.Events;
using Dockson.Views;

namespace Dockson.Projections
{
	public class MasterIntervalProjection
	{
		private readonly MasterIntervalView _view;
		private readonly List<DateTime> _source;

		public MasterIntervalProjection(MasterIntervalView view)
		{
			_view = view;
			_source = new List<DateTime>();
		}

		public void Project(MasterCommit message, Action<object> dispatch)
		{
			var commitTime = message.TimeStamp;
			var key = commitTime.Date;

			//improvement: just calculate new delta and append to _source
			_source.Add(commitTime); //assumes sorted for now!

			var deltas = _source
				.Skip(1)
				.Select((timestamp, i) => new { Timestamp = timestamp, Elapsed = timestamp - _source[i] })
				.Where(d => d.Timestamp.Date == key)
				.Select(d => d.Elapsed.TotalMinutes)
				.ToArray();

			_view.Medians[key] = deltas.Any() ? deltas.Median() : 0;
			_view.StandardDeviations[key] = deltas.Any() ? deltas.StandardDeviation() : 0;
			_view.Days.Add(key);
		}
	}
}
