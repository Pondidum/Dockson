using System.IO;

namespace Dockson.Infrastructure
{
	public class FileSystem : IFileSystem
	{
		public string ReadFile(string path) => File.Exists(path)
			? File.ReadAllText(path)
			: string.Empty;

		public void WriteFile(string path, string contents) => File.WriteAllText(path, contents);
		public void AppendFile(string path, string line) => File.AppendAllLines(path, new[] { line });

		public void CreateDirectory(string path) => Directory.CreateDirectory(path);
	}
}
