namespace Dockson.Domain.Projections
{
	public interface IProjection<TMessage>
	{
		void Project(TMessage message);
	}

	public interface IProjection<TStartMessage, TFinishMessage>
	{
		void Start(TStartMessage message);
		void Finish(TFinishMessage message);
	}
}
