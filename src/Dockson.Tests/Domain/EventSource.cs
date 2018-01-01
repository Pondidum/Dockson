using System;
using System.Collections.Generic;
using Dockson.Domain;
using Dockson.Domain.Projections;
using Dockson.Domain.Transformers.Build;
using Dockson.Domain.Transformers.Commits;
using Dockson.Domain.Transformers.Deployment;

namespace Dockson.Tests.Domain
{
	internal class EventSource
	{
		public string Name { get; set; }
		public string Version { get; set; }
		public string CommitHash { get; set; }
		public DateTime Timestamp { get; set; }
		public HashSet<string> Groups { get; set; }
		public DayDate CurrentDay => new DayDate(Timestamp);

		private readonly Action<Notification> _projector;

		public EventSource(Action<Notification> projector)
		{
			_projector = projector;

			Name = "some-service";
			Version = "2.13.4";
			CommitHash = Guid.NewGuid().ToString();
			Timestamp = new DateTime(2017, 11, 30, 11, 47, 00);
			Groups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		}

		public static EventSource For<TState, TMessage>(IProjection<TState, TMessage> projection, Action<EventSource> customise = null)
			where TState : new()
			=> CreateSource(dist => dist.AddProjection(projection), customise);

		public static EventSource For<TState, TStart, TFinish>(IProjection<TState, TStart, TFinish> projection, Action<EventSource> customise = null)
			where TState : new()
			=> CreateSource(dist => dist.AddProjection(projection), customise);

		private static EventSource CreateSource(Action<Distributor> customiseDistributor, Action<EventSource> customiseEventSource = null)
		{
			var dist = new Distributor(new DictionaryStateStore(), new ViewStore());
			dist.AddTransformer(new CommitsTransformer());
			dist.AddTransformer(new BuildTransformer());
			dist.AddTransformer(new DeploymentTransformer());

			customiseDistributor(dist);

			var es = new EventSource(dist.Project);
			customiseEventSource?.Invoke(es);
			return es;
		}

		private EventSource Dispatch(Notification notification)
		{
			_projector(notification);
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

		public EventSource AdvanceTo(DateTime nextTime)
		{
			Timestamp = nextTime;
			return this;
		}

		public EventSource BranchCommit()
		{
			Dispatch(CreateNotification("feature-whatever"));
			return this;
		}

		public EventSource MasterCommit()
		{
			Dispatch(CreateNotification("master"));
			return this;
		}

		public EventSource BuildSucceeded(Action<Notification> modify = null)
		{
			var notification = new Notification
			{
				Type = Stages.Build,
				Timestamp = Timestamp,
				Name = Name,
				Version = Version,
				Status = "success",
				Groups = Groups,
				Tags =
				{
					{ "commit", CommitHash }
				}
			};
			modify?.Invoke(notification);

			Dispatch(notification);

			return this;
		}

		public EventSource BuildFailed(Action<Notification> modify = null)
		{
			var notification = new Notification
			{
				Type = Stages.Build,
				Timestamp = Timestamp,
				Name = Name,
				Version = Version,
				Status = "failure",
				Groups = Groups,
				Tags =
				{
					{ "commit", CommitHash }
				}
			};
			modify?.Invoke(notification);

			Dispatch(notification);

			return this;
		}

		public EventSource ProductionDeployment(Action<Notification> modify = null)
		{
			var notification = new DeploymentNotification
			{
				Type = Stages.Deploy,
				Timestamp = Timestamp,
				Name = Name,
				Version = Version,
				Status = "success",
				Groups = Groups,
				Environment = "production"
			};
			modify?.Invoke(notification);

			Dispatch(notification);

			return this;
		}


		private CommitNotification CreateNotification(string branch) => new CommitNotification()
		{
			Type = Stages.Commit,
			Timestamp = Timestamp,
			Source = "github",
			Name = Name,
			Version = "1.0.0",
			Groups = Groups,
			Branch = branch,
			Commit = CommitHash
		};
	}
}
