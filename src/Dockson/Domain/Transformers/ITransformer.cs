using System;

namespace Dockson.Domain.Transformers
{
	public interface ITransformer
	{
		void Transform(Notification notification, Action<object> dispatch);
	}
}
