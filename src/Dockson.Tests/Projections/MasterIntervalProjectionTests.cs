﻿using System;
using System.Collections.Generic;
using Dockson.Events;
using Dockson.Projections;
using Dockson.Views;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Projections
{
	public class MasterIntervalProjectionTests
	{
		private static readonly DateTime Yesterday = new DateTime(2017, 11, 29, 11, 47, 00);
		private static readonly DateTime Today = new DateTime(2017, 11, 30, 11, 47, 00);
		private static readonly DateTime Tomorrow = new DateTime(2017, 12, 1, 11, 47, 00);

		private readonly MasterIntervalView _view;
		private readonly MasterIntervalProjection _projection;

		public MasterIntervalProjectionTests()
		{
			_view = new MasterIntervalView();
			_projection = new MasterIntervalProjection(_view);
		}

		[Fact]
		public void When_projecting_one_commit()
		{
			_projection.Project(CreateCommit(Today, 0), message => { });

			_view.ShouldSatisfyAllConditions(
				() => _view.Medians.ShouldContainKeyAndValue(Today.Date, 0),
				() => _view.StandardDeviations.ShouldContainKeyAndValue(Today.Date, 0)
			);
		}

		[Fact]
		public void When_projecting_two_commits_on_the_same_day()
		{
			_projection.Project(CreateCommit(Today, 0), message => { });
			_projection.Project(CreateCommit(Today, 1), message => { });

			_view.ShouldSatisfyAllConditions(
				() => _view.Medians.ShouldContainKeyAndValue(Today.Date, 60), //1 hour
				() => _view.StandardDeviations.ShouldContainKeyAndValue(Today.Date, 0)
			);
		}

		[Fact]
		public void When_projecting_several_commits_on_the_same_day()
		{
			_projection.Project(CreateCommit(Today, 0), message => { });
			_projection.Project(CreateCommit(Today, 1), message => { });
			_projection.Project(CreateCommit(Today, 2), message => { });
			_projection.Project(CreateCommit(Today, 4), message => { });
			_projection.Project(CreateCommit(Today, 5), message => { });

			_view.ShouldSatisfyAllConditions(
				() => _view.Medians.ShouldContainKeyAndValue(Today.Date, 60), //1 hour
				() => _view.StandardDeviations.ShouldContainKeyAndValue(Today.Date, 30) // half hour
			);
		}


		[Fact]
		public void When_projecting_several_commits_on_several_days()
		{
			_projection.Project(CreateCommit(Today, 0), message => { });
			_projection.Project(CreateCommit(Today, 1), message => { });
			_projection.Project(CreateCommit(Today, 2), message => { });
			_projection.Project(CreateCommit(Today, 4), message => { });
			_projection.Project(CreateCommit(Today, 5), message => { });
			_projection.Project(CreateCommit(Tomorrow, 2), message => { });
			_projection.Project(CreateCommit(Tomorrow, 4), message => { });
			_projection.Project(CreateCommit(Tomorrow, 5), message => { });

			_view.ShouldSatisfyAllConditions(
				() => _view.Medians[Today.Date].ShouldBe(60), //1 hour
				() => _view.StandardDeviations[Today.Date].ShouldBe(30), // half hour
				() => _view.Medians[Tomorrow.Date].ShouldBe(120), //2 hours
				() => _view.StandardDeviations[Tomorrow.Date].ShouldBe(676.165, tolerance: 0.005)
			);
		}

		private MasterCommit CreateCommit(DateTime day, int hoursOffset) => new MasterCommit(
			CreateNotification(day.AddHours(hoursOffset), "master"),
			CreateNotification(day.AddHours(hoursOffset).Add(TimeSpan.FromMinutes(-5)), "feature-whatever")
		);

		private Notification CreateNotification(DateTime timestamp, string branch) => new Notification
		{
			Type = Stages.Commit,
			TimeStamp = timestamp,
			Source = "github",
			Name = "SomeService",
			Version = "1.0.0",
			Tags = new Dictionary<string, string>
			{
				{ "commit", Guid.NewGuid().ToString() },
				{ "branch", branch }
			}
		};
	}
}
