namespace Dockson.Domain
{
	public interface IProjector
	{
		void Project(Notification notification);
	}
}
