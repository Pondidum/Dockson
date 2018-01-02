using System.IO;

namespace Dockson.Infrastructure
{
	public class FileSystem : IFileSystem
	{
		public string ReadFile(string path) => File.ReadAllText(path);
		public void WriteFile(string path, string contents) => File.WriteAllText(path, contents);
	}
}