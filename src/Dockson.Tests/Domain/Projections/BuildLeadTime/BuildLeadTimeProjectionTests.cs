using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Dockson.Domain;
using Dockson.Domain.Projections;
using Dockson.Domain.Projections.BuildLeadTime;
using Dockson.Domain.Transformers.Build;
using Dockson.Domain.Transformers.MasterCommit;
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

	internal class EventSource
	{
		public string Name { get; set; }
		public string Version { get; set; }
		public string CommitHash { get; set; }
		public DateTime Timestamp { get; set; }
		public HashSet<string> Groups { get; set; }

		private readonly Cache<Type, Action<object>> _handlers;

		public EventSource()
		{
			Name = "some-service";
			Version = "2.13.4";
			CommitHash = Guid.NewGuid().ToString();
			Timestamp = DateTime.UtcNow;
			Groups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		}

		public EventSource(object projection) : this()
		{
			_handlers = new Cache<Type, Action<object>>(key =>
			{
				var method = projection
					.GetType()
					.GetMethods()
					.Where(m => m.IsPublic && m.Name == nameof(IProjection<string>.Project))
					.Where(m => m.GetParameters().Length == 1)
					.Single(m => m.GetParameters().Single().ParameterType.IsAssignableFrom(key));

				return value => method.Invoke(projection, new[] { value });
			});
		}

		private EventSource Dispatch<T>(T message)
		{
			_handlers[typeof(T)].Invoke(message);
			return this;
		}

		public EventSource NewCommitHash(string hash = null)
		{
			CommitHash = hash ?? Guid.NewGuid().ToString();
			return this;
		}
		
		public EventSource Advance(TimeSpan time)
		{
			Timestamp = Timestamp.Add(time);
			return this;
		}

		public EventSource MasterCommit(TimeSpan? sinceFeatureCommit = null) => Dispatch(new MasterCommit(
			CreateNotification(Timestamp, "master"),
			CreateNotification(Timestamp.Subtract(sinceFeatureCommit ?? TimeSpan.Zero), "feature-whatever")
		));

		public EventSource BuildSucceeded(Action<Notification> modify = null)
		{
			var notification = new Notification
			{
				Name = Name,
				Version = Version,
				Timestamp = Timestamp,
				Groups = Groups,
				Tags =
				{
					{ "commit", CommitHash }
				}
			};
			modify?.Invoke(notification);

			Dispatch(new BuildSucceeded(notification));

			return this;
		}

		private Notification CreateNotification(DateTime timestamp, string branch) => new Notification
		{
			Type = Stages.Commit,
			Timestamp = timestamp,
			Source = "github",
			Name = Name,
			Version = "1.0.0",
			Groups = Groups,
			Tags =
			{
				{ "commit", CommitHash },
				{ "branch", branch }
			}
		};

	}
}
