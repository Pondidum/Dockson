using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Dockson.Domain;
using Dockson.Domain.Projections;
using Dockson.Domain.Transformers;
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

		private readonly Cache<Type, Action<object>> _handlers;
		private readonly List<ITransformer> _transformers;
		private readonly Action<Notification> _projector;

		public EventSource()
		{
			Name = "some-service";
			Version = "2.13.4";
			CommitHash = Guid.NewGuid().ToString();
			Timestamp = new DateTime(2017, 11, 30, 11, 47, 00);
			Groups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			_transformers = new List<ITransformer>
			{
				new CommitsTransformer(),
				new BuildTransformer(),
				new DeploymentTransformer()
			};
		}

		public EventSource(Action<Notification> projector) : this()
		{
			_projector = projector;
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
					.SingleOrDefault(m => m.GetParameters().Single().ParameterType.IsAssignableFrom(key));

				if (method == null)
					return value => { };

				return value => method.Invoke(projection, new[] { value });
			});
		}

		private EventSource Dispatch(Notification notification)
		{
			if (_projector != null)
			{
				_projector(notification);
				return this;
			}

			var events = new List<object>();

			_transformers.ForEach(tx => tx.Transform(notification, events.Add));
			events.ForEach(message => _handlers[message.GetType()].Invoke(message));

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
			var notification = new Notification
			{
				Type = Stages.Deploy,
				Timestamp = Timestamp,
				Name = Name,
				Version = Version,
				Status = "success",
				Groups = Groups,
				Tags =
				{
					{ "environment", "production" }
				}
			};
			modify?.Invoke(notification);

			Dispatch(notification);

			return this;
		}


		private Notification CreateNotification(string branch) => new Notification
		{
			Type = Stages.Commit,
			Timestamp = Timestamp,
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
