namespace Dockson.Domain
{
	public static class ViewStoreExtensions
	{
		public static void UpdateMasterCommitLeadTime(this IViewStore store, string group, DayDate day, TrendView newSummary)
			=> store.For(group).MasterCommitLeadTime[day] = newSummary;

		public static void UpdateMasterCommitInterval(this IViewStore store, string group, DayDate day, TrendView newSummary)
			=> store.For(group).MasterCommitInterval[day] = newSummary;

		public static void UpdateBuildLeadTime(this IViewStore store, string group, DayDate day, TrendView newSummary)
			=> store.For(group).BuildLeadTime[day] = newSummary;

		public static void UpdateBuildInterval(this IViewStore store, string group, DayDate day, TrendView newSummary)
			=> store.For(group).BuildInterval[day] = newSummary;

		public static void UpdateBuildRecoveryTime(this IViewStore store, string group, DayDate day, TrendView newSummary)
			=> store.For(group).BuildRecoveryTime[day] = newSummary;

		public static void UpdateBuildFailureRate(this IViewStore store, string group, DayDate day, RateView newSummary)
			=> store.For(group).BuildFailureRate[day] = newSummary;

		public static void UpdateDeploymentLeadTime(this IViewStore store, string group, DayDate day, TrendView newSummary)
			=> store.For(group).DeploymentLeadTime[day] = newSummary;

		public static void UpdateDeploymentInterval(this IViewStore store, string group, DayDate day, TrendView newSummary)
			=> store.For(group).DeploymentInterval[day] = newSummary;
	}
}
