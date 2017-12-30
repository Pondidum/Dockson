using System;
using Dockson.Domain;
using Dockson.Domain.Projections;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Projections
{
	public class BuildLeadTimeProjectionTests
	{
		private const string Team = "team-one";

		private readonly View _view;

		private readonly EventSource _serviceOne;
		private readonly EventSource _serviceTwo;

		public BuildLeadTimeProjectionTests()
		{
			_view = new View();
			var projection = new BuildLeadTimeProjection(_view.UpdateBuildLeadTime);

			_serviceOne = EventSource.For(projection, s =>
			{
				s.Name = "service-one";
				s.Groups.Add(Team);
			});

			_serviceTwo = EventSource.For(projection, s =>
			{
				s.Name = "service-two";
				s.Groups.Add(Team);
			});
		}

		[Fact]
		public void When_projecting_one_commit()
		{
			_serviceOne.MasterCommit();

			_view.ShouldBeEmpty();
		}

		[Fact]
		public void When_projecting_one_build()
		{
			_serviceOne.BuildSucceeded();

			_view.ShouldBeEmpty();
		}

		[Fact]
		public void When_projecting_a_commit_and_matching_build()
		{
			_serviceOne
				.BranchCommit()
				.MasterCommit()
				.Advance(TimeSpan.FromMinutes(20))
				.BuildSucceeded();

			var summary = _view[_serviceOne.Name].BuildLeadTime[_serviceOne.CurrentDay];

			summary.ShouldSatisfyAllConditions(
				() => summary.Median.ShouldBe(20),
				() => summary.Deviation.ShouldBe(0)
			);
		}

		[Fact]
		public void When_projecting_two_commits_and_matching_builds()
		{
			_serviceOne
				.BranchCommit()
				.MasterCommit()
				.Advance(TimeSpan.FromMinutes(20))
				.BuildSucceeded()
				.Advance(TimeSpan.FromMinutes(20))
				.NewCommitHash()
				.BranchCommit()
				.MasterCommit()
				.Advance(TimeSpan.FromMinutes(15))
				.BuildSucceeded();

			var summary = _view[_serviceOne.Name].BuildLeadTime[_serviceOne.CurrentDay];

			summary.ShouldSatisfyAllConditions(
				() => summary.Median.ShouldBe(17.5),
				() => summary.Deviation.ShouldBe(3.53, tolerance: 0.01)
			);
		}

		[Fact]
		public void When_projecting_two_commits_and_matching_builds_for_different_services()
		{
			_serviceOne
				.BranchCommit()
				.MasterCommit()
				.Advance(TimeSpan.FromMinutes(20))
				.BuildSucceeded();

			_serviceTwo
				.Advance(TimeSpan.FromMinutes(40))
				.BranchCommit()
				.MasterCommit()
				.Advance(TimeSpan.FromMinutes(15))
				.BuildSucceeded();

			var one = _view[_serviceOne.Name].BuildLeadTime[_serviceOne.CurrentDay];
			var two = _view[_serviceTwo.Name].BuildLeadTime[_serviceTwo.CurrentDay];
			var team = _view[Team].BuildLeadTime[_serviceOne.CurrentDay];

			_view.ShouldSatisfyAllConditions(
				() => one.Median.ShouldBe(20),
				() => one.Deviation.ShouldBe(0),
				() => two.Median.ShouldBe(15),
				() => two.Deviation.ShouldBe(0),
				() => team.Median.ShouldBe(17.5),
				() => team.Deviation.ShouldBe(3.53, tolerance: 0.01)
			);
		}
	}
}
