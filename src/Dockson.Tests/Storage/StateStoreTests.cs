using System;
using System.Collections.Generic;
using Dockson.Domain.Projections;
using Dockson.Infrastructure;
using Dockson.Storage;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Storage
{
	public class StateStoreTests
	{
		private readonly FakeFileSystem _fs;

		public StateStoreTests()
		{
			_fs = new FakeFileSystem();
		}

		[Fact]
		public void When_serializing_and_deserializing()
		{
			var stamp = DateTime.UtcNow;

			var saveStore = CreateStateStore();
			var saveState = saveStore.StateFor<LeadTimeState>(typeof(BuildLeadTimeProjection));
			saveState.Commits["one"] = stamp;
			saveStore.Save();

			var loadStore = CreateStateStore();
			var loadState = loadStore.StateFor<LeadTimeState>(typeof(BuildLeadTimeProjection));

			loadState.Commits["one"].ShouldBe(stamp);
		}

		private StateStore CreateStateStore()
		{
			var settings = new Settings { StoragePath = "." };
			var loadStore = new StateStore(_fs, settings);
			loadStore.Load();

			return loadStore;
		}

		private class FakeFileSystem : IFileSystem
		{
			private readonly Dictionary<string, string> _files;

			public FakeFileSystem()
			{
				_files = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			}

			public string ReadFile(string path)
			{
				return _files.ContainsKey(path)
					? _files[path]
					: string.Empty;
			}

			public void WriteFile(string path, string contents)
			{
				_files[path] = contents;
			}

			public void AppendFile(string path, string line)
			{
				if (_files.ContainsKey(path))
					_files[path] = _files[path] + "\n" + line;
				else
					_files[path] = line;
			}

			public void CreateDirectory(string path)
			{
			}
		}
	}
}
