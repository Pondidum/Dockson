namespace Dockson.Infrastructure
{
	public interface IFileSystem
	{
		string ReadFile(string path);
		void WriteFile(string path, string contents);
	}
}