using System;
using System.Collections.Generic;
using System.Linq;
using Dockson.Domain;
using Dockson.Domain.Transformers.Build;
using Dockson.Domain.Transformers.Commits;
using Dockson.Domain.Transformers.Deployment;

namespace Dockson.Infrastructure.Validation
{
	public class ValidationProjector : IProjector
	{
		private readonly IProjector _internal;
		private readonly List<Validator> _validators;

		public ValidationProjector(IProjector @internal)
		{
			_internal = @internal;
			_validators = new List<Validator>();

			Add(new NotificationValidator());
			Add(new CommitNotificationValidator());
			Add(new BuildNotificationValidator());
			Add(new DeploymentNotificationValidator());
		}

		private void Add<TNotification>(INotificationValidator<TNotification> validator) where TNotification : Notification
		{
			_validators.Add(new Validator(typeof(TNotification), notification => validator.Validate((TNotification)notification)));
		}

		public void Project(Notification notification)
		{
			var messages = _validators
				.Where(v => v.CanValidate(notification.GetType()))
				.SelectMany(v => v.Validate(notification))
				.ToArray();

			if (messages.Any())
				throw new NotificationValidationException(notification, messages);

			_internal.Project(notification);
		}

		private class Validator
		{
			private readonly Type _type;
			private readonly Func<object, IEnumerable<string>> _validate;

			public Validator(Type type, Func<object, IEnumerable<string>> validate)
			{
				_type = type;
				_validate = validate;
			}

			public bool CanValidate(Type type) => _type.IsAssignableFrom(type);
			public IEnumerable<string> Validate(object notification) => _validate(notification);
		}
	}
}
