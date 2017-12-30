using System.Collections.Generic;

namespace Dockson.Domain
{
	public class GroupView
	{
		public Dictionary<DayDate, TrendView> MasterCommitLeadTime { get; set; }
		public Dictionary<DayDate, TrendView> MasterCommitInterval { get; set; }

		public Dictionary<DayDate, TrendView> BuildLeadTime { get; set; }
		public Dictionary<DayDate, TrendView> BuildInterval { get; set; }
		public Dictionary<DayDate, TrendView> BuildRecoveryTime { get; set; }
		public Dictionary<DayDate, RateView> BuildFailureRate { get; set; }

		public Dictionary<DayDate, TrendView> DeploymentLeadTime { get; set; }
		public Dictionary<DayDate, TrendView> DeploymentInterval { get; set; }

		public GroupView()
		{
			MasterCommitLeadTime = new Dictionary<DayDate, TrendView>();
			MasterCommitInterval = new Dictionary<DayDate, TrendView>();
			BuildLeadTime = new Dictionary<DayDate, TrendView>();
			BuildInterval = new Dictionary<DayDate, TrendView>();
			BuildRecoveryTime = new Dictionary<DayDate, TrendView>();
			BuildFailureRate = new Dictionary<DayDate, RateView>();
			DeploymentLeadTime = new Dictionary<DayDate, TrendView>();
			DeploymentInterval = new Dictionary<DayDate, TrendView>();
		}
	}
}
