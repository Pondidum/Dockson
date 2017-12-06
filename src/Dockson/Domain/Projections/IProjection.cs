namespace Dockson.Domain.Projections
{
	public interface IProjection<TMessage>
	{
		void Project(TMessage message);
	}
}
