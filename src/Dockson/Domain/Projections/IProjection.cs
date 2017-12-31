namespace Dockson.Domain.Projections
{
	public interface IProjection<TState, TMessage> where TState : new()
	{
		TState State { get; set; }
		void Project(TMessage message);
	}

	public interface IProjection<TState, TStartMessage, TFinishMessage> where TState : new()
	{
		TState State { get; set; }
		void Start(TStartMessage message);
		void Finish(TFinishMessage message);
	}
}
