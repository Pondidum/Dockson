﻿using System;
using System.Text.RegularExpressions;
using Dockson.Domain;
using Dockson.Domain.Projections.BuildLeadTime;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Projections.BuildLeadTime
{
	public class BuildLeadTimeProjectionTests
	{
		private const string Team = "team-one";

		private readonly BuildLeadTimeView _view;
		private readonly BuildLeadTimeProjection _projection;
		private readonly DateTime _now;

		private readonly EventSource _serviceOne;
		private readonly EventSource _serviceTwo;

		public BuildLeadTimeProjectionTests()
		{
			_view = new BuildLeadTimeView();
			_projection = new BuildLeadTimeProjection(_view);
			_now = DateTime.UtcNow;

			_serviceOne = new EventSource(_projection) { Name = "service-one", Groups = { Team } };
			_serviceTwo = new EventSource(_projection) { Name = "service-two", Groups = { Team } };
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
				.MasterCommit()
				.Advance(TimeSpan.FromMinutes(20))
				.BuildSucceeded();

			var summary = _view[_serviceOne.Name].Daily[new DayDate(_now)];

			summary.ShouldSatisfyAllConditions(
				() => summary.Median.ShouldBe(20),
				() => summary.Deviation.ShouldBe(0)
			);
		}

		[Fact]
		public void When_projecting_two_commits_and_matching_builds()
		{
			_serviceOne
				.MasterCommit()
				.Advance(TimeSpan.FromMinutes(20))
				.BuildSucceeded()
				.Advance(TimeSpan.FromMinutes(20))
				.NewCommitHash()
				.MasterCommit()
				.Advance(TimeSpan.FromMinutes(15))
				.BuildSucceeded();

			var summary = _view[_serviceOne.Name].Daily[new DayDate(_now)];

			summary.ShouldSatisfyAllConditions(
				() => summary.Median.ShouldBe(17.5),
				() => summary.Deviation.ShouldBe(3.53, tolerance: 0.01)
			);
		}

		[Fact]
		public void When_projecting_two_commits_and_matching_builds_for_different_services()
		{
			_serviceOne
				.MasterCommit()
				.Advance(TimeSpan.FromMinutes(20))
				.BuildSucceeded();

			_serviceTwo
				.Advance(TimeSpan.FromMinutes(40))
				.MasterCommit()
				.Advance(TimeSpan.FromMinutes(15))
				.BuildSucceeded();

			var one = _view[_serviceOne.Name].Daily[new DayDate(_now)];
			var two = _view[_serviceTwo.Name].Daily[new DayDate(_now)];
			var team = _view[Team].Daily[new DayDate(_now)];

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
