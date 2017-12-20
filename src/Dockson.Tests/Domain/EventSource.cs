using System;
using System.Collections.Generic;
using System.Linq;
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

		private readonly Cache<Type, Action<object>> _handlers;

		public EventSource()
		{
			Name = "some-service";
			Version = "2.13.4";
			CommitHash = Guid.NewGuid().ToString();
			Timestamp = new DateTime(2017, 11, 30, 11, 47, 00);
			Groups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		}

		public EventSource(object projection) : this()
		{
			var names = new HashSet<string>
			{
				nameof(IProjection<string>.Project),
				nameof(IProjection<string, string>.Start),
				nameof(IProjection<string, string>.Finish)
			};

			_handlers = new Cache<Type, Action<object>>(key =>
			{
				var method = projection
					.GetType()
					.GetMethods()
					.Where(m => m.IsPublic && names.Contains(m.Name))
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

		public EventSource AdvanceTo(DateTime nextTime)
		{
			Timestamp = nextTime;
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

		public EventSource BuildFailed(Action<Notification> modify = null)
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

			Dispatch(new BuildFailed(notification));

			return this;
		}

		public EventSource ProductionDeployment(Action<Notification> modify = null)
		{
			var notification = new Notification
			{
				Name = Name,
				Version = Version,
				Timestamp = Timestamp,
				Groups = Groups
			};
			modify?.Invoke(notification);

			Dispatch(new ProductionDeployment(notification));

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
