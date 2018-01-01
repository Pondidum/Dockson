using System.Collections.Generic;

namespace Dockson.Domain
{
	public class ViewStore
	{
		public Dictionary<string, GroupView> View { get; private set; }

		public ViewStore()
		{
			View = new Dictionary<string, GroupView>();
		}

		public void Save()
		{
		}

		public void Load()
		{
		}

		private GroupView For(string group)
		{
			View.TryAdd(group, new GroupView());
			return View[group];
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
}
