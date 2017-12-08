using System;
using System.Collections.Generic;
using Dockson.Domain.Transformers.Build;
using Dockson.Domain.Views;

namespace Dockson.Domain.Projections.BuildRecoveryTime
{
	public class BuildRecoveryTimeProjection : IProjection<BuildFixed>
	{
		private readonly BuildRecoveryTimeView _view;
		private readonly Cache<string, List<double>> _recoveries;

		public BuildRecoveryTimeProjection(BuildRecoveryTimeView view)
		{
			_view = view;
			_recoveries = new Cache<string, List<double>>(
				StringComparer.OrdinalIgnoreCase,
				key => new List<double>());
		}

		public void Project(BuildFixed message)
		{
			var day = new DayDate(message.FixedTimestamp);
			foreach (var @group in message.Groups)
			{
				_recoveries[group].Add(message.RecoveryTime.TotalMinutes);

				_view.TryAdd(group, new GroupSummary<BuildRecoveryTimeSummary>());
				_view[group].Daily[day] = new BuildRecoveryTimeSummary
				{
					Median = _recoveries[group].Median(),
					Deviation = _recoveries[group].StandardDeviation()
				};
			}
		}
	}

	public class BuildRecoveryTimeView : Dictionary<string, GroupSummary<BuildRecoveryTimeSummary>>
	{
	}

	public class BuildRecoveryTimeSummary
	{
		public double Median { get; set; }
		public double Deviation { get; set; }
	}
}
