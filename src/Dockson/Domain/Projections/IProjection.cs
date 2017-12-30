namespace Dockson.Domain.Projections
{
	public interface IProjection<TMessage>
	{
//		TState State { get; set; }
		void Project(TMessage message);
	}

	public interface IProjection<TState, TStartMessage, TFinishMessage>
	{
		TState State { get; set; }
		void Start(TStartMessage message);
		void Finish(TFinishMessage message);
	}
}
