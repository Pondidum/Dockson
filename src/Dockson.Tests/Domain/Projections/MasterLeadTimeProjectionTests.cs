using System;
using System.Collections.Generic;
using Dockson.Domain;
using Dockson.Domain.Events;
using Dockson.Domain.Projections;
using Dockson.Domain.Views;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Projections
{
	public class MasterLeadTimeProjectionTests
	{
		private readonly MasterLeadTimeView _view;
		private readonly MasterLeadTimeProjection _projection;
		private readonly DateTime _now;

		public MasterLeadTimeProjectionTests()
		{
			_view = new MasterLeadTimeView();
			_projection = new MasterLeadTimeProjection(_view);
			_now = new DateTime(2017, 11, 30, 11, 47, 00);
		}

		[Fact]
		public void When_projecting_one_commit()
		{
			_projection.Project(CreateCommit(TimeSpan.Zero, TimeSpan.FromHours(1)), message => { });
			var day = new DayDate(_now);

			_view.ShouldSatisfyAllConditions(
				() => _view.Days.ShouldBe(new[] { day }),
				() => _view.Medians.ShouldContainKeyAndValue(day, TimeSpan.FromHours(1).TotalMinutes),
				() => _view.StandardDeviations.ShouldContainKeyAndValue(day, 0)
			);
		}

		[Fact]
		public void When_projecting_two_commits()
		{
			_projection.Project(CreateCommit(TimeSpan.Zero, TimeSpan.FromHours(1)), message => { });
			_projection.Project(CreateCommit(TimeSpan.FromHours(4), TimeSpan.FromHours(5)), message => { });
		}

		private MasterCommit CreateCommit(TimeSpan featureTime, TimeSpan masterTime) => new MasterCommit(
			CreateNotification(masterTime, "master"),
			CreateNotification(featureTime, "feature-whatever")
		);

		private Notification CreateNotification(TimeSpan offset, string branch) => new Notification
		{
			Type = Stages.Commit,
			TimeStamp = _now.Add(offset),
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
