using System.Collections.Generic;

namespace Dockson.Domain
{
	public class View : Dictionary<string, GroupView>
	{
		public GroupView For(string group)
		{
			TryAdd(group, new GroupView());
			return this[group];
		}

		public void UpdateMasterCommitLeadTime(string group, DayDate day, TrendView newSummary) => For(group).MasterCommitLeadTime[day] = newSummary;
		public void UpdateMasterCommitInterval(string group, DayDate day, TrendView newSummary) => For(group).MasterCommitInterval[day] = newSummary;
		public void UpdateBuildLeadTime(string group, DayDate day, TrendView newSummary) => For(group).BuildLeadTime[day] = newSummary;
		public void UpdateBuildInterval(string group, DayDate day, TrendView newSummary) => For(group).BuildInterval[day] = newSummary;
		public void UpdateBuildRecoveryTime(string group, DayDate day, TrendView newSummary) => For(group).BuildRecoveryTime[day] = newSummary;
		public void UpdateBuildFailureRate(string group, DayDate day, RateView newSummary) => For(group).BuildFailureRate[day] = newSummary;
		public void UpdateDeploymentLeadTime(string group, DayDate day, TrendView newSummary) => For(group).DeploymentLeadTime[day] = newSummary;
		public void UpdateDeploymentInterval(string group, DayDate day, TrendView newSummary) => For(group).DeploymentInterval[day] = newSummary;
	}

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

	public class TrendView
	{
		public double Median { get; set; }
		public double Deviation { get; set; }
	}

	public class RateView
	{
		public double Rate { get; set; }
	}
}
