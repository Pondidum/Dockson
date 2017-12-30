using System;
using System.Collections.Generic;
using Dockson.Domain.Transformers.Build;

namespace Dockson.Domain.Projections
{
	public class BuildFailureRateProjection : IProjection<BuildFailureState, BuildFailed, BuildSucceeded>
	{
		private readonly Action<string, DayDate, RateView> _updateView;

		public BuildFailureRateProjection(Action<string, DayDate, RateView> updateView)
		{
			_updateView = updateView;
			State = new BuildFailureState();
		}

		public void Finish(BuildSucceeded message)
		{
			Project(message.Timestamp, message.Groups, counts => counts.Successes++);
		}

		public BuildFailureState State { get; set; }

		public void Start(BuildFailed message)
		{
			Project(message.Timestamp, message.Groups, counts => counts.Failures++);
		}

		private void Project(DateTime timestamp, IEnumerable<string> groups, Action<BuildFailureState.Counts> action)
		{
			var day = new DayDate(timestamp);

			foreach (var @group in groups)
			{
				var counts = State.Builds[group][day];
				action(counts);

				_updateView(@group, day, new RateView
				{
					Rate = (counts.Failures / counts.Total) * 100
				});
			}
		}
	}
}
