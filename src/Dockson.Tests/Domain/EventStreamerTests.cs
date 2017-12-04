﻿using System;
using System.Collections.Generic;
using Dockson.Domain;
using Dockson.Domain.Events;
using Dockson.Domain.Projections;
using Dockson.Domain.Views;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain
{
	public class EventStreamerTests
	{
		private static readonly DateTime Yesterday = new DateTime(2017, 11, 29, 11, 47, 00);
		private static readonly DateTime Today = new DateTime(2017, 11, 30, 11, 47, 00);
		private static readonly DateTime Tomorrow = new DateTime(2017, 12, 1, 11, 47, 00);

		[Fact]
		public void When_projecting_two_levels()
		{
			var intervalView = new MasterIntervalView();
			var leadTimeView = new MasterLeadTimeView();
			var projections = new List<Action<object, Action<object>>>();

			projections.Add(Wrap<Notification>(new MasterCommitsProjection().Project));
			projections.Add(Wrap<MasterCommit>(new MasterIntervalProjection(intervalView).Project));
			projections.Add(Wrap<MasterCommit>(new MasterLeadTimeProjection(leadTimeView).Project));

			var streamer = new EventStreamer(projections);

			streamer.Handle(CreateCommit(Today, 0));
			streamer.Handle(CreateCommit(Today, 1));
			streamer.Handle(CreateCommit(Today, 2));
			streamer.Handle(CreateCommit(Today, 4));
			streamer.Handle(CreateCommit(Today, 5));
			streamer.Handle(CreateCommit(Tomorrow, 2));
			streamer.Handle(CreateCommit(Tomorrow, 4));
			streamer.Handle(CreateCommit(Tomorrow, 5));

			intervalView.ShouldSatisfyAllConditions(
				() => intervalView.Daily[new DayDate(Today)].Median.ShouldBe(60), //1 hour
				() => intervalView.Daily[new DayDate(Today)].Deviation.ShouldBe(30), // half hour
				() => intervalView.Daily[new DayDate(Tomorrow)].Median.ShouldBe(120), //2 hours
				() => intervalView.Daily[new DayDate(Tomorrow)].Deviation.ShouldBe(676.165, tolerance: 0.005)
			);

			leadTimeView.ShouldSatisfyAllConditions(
				() => leadTimeView.Medians[Today.Date].ShouldBe(5),
				() => leadTimeView.StandardDeviations[Today.Date].ShouldBe(0),
				() => leadTimeView.Medians[Tomorrow.Date].ShouldBe(5),
				() => leadTimeView.StandardDeviations[Tomorrow.Date].ShouldBe(0)
			);
		}

		private Action<object, Action<object>> Wrap<TMessage>(Action<TMessage, Action<object>> projection) where TMessage : class
		{
			return (message, dispatch) =>
			{
				if (message is TMessage @event)
					projection(@event, dispatch);
			};
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

	public class EventStreamer
	{
		private readonly List<Action<object, Action<object>>> _projections;

		public EventStreamer(List<Action<object, Action<object>>> projections)
		{
			_projections = projections;
		}

		public void Handle(object @event)
		{
			var newEvents = new List<object>();

			foreach (var projection in _projections)
			{
				projection(@event, newEvents.Add);
			}

			foreach (var newEvent in newEvents)
				Handle(newEvent);
		}
	}
}
