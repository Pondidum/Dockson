using System.Collections.Generic;
using System.IO;
using Dockson.Domain;
using Dockson.Infrastructure;
using Newtonsoft.Json;

namespace Dockson.Storage
{
	public class ViewStore : IViewStore
	{
		private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
		{
			Formatting = Formatting.Indented
		};

		private readonly IFileSystem _fileSystem;
		private readonly string _viewFile;

		public ViewStore(IFileSystem fileSystem, Settings settings)
		{
			_fileSystem = fileSystem;
			_viewFile = Path.Combine(settings.StoragePath, "view.json");
			View = new Dictionary<string, GroupView>();
		}

		public Dictionary<string, GroupView> View { get; private set; }

		public void Save()
		{
			var json = JsonConvert.SerializeObject(View, Settings);
			_fileSystem.WriteFile(_viewFile, json);
		}

		public void Load()
		{
			var json = _fileSystem.ReadFile(_viewFile);

			View = string.IsNullOrWhiteSpace(json) == false
				? JsonConvert.DeserializeObject<Dictionary<string, GroupView>>(json)
				: new Dictionary<string, GroupView>();
		}

		public GroupView For(string group)
		{
			View.TryAdd(group, new GroupView());
			return View[group];
		}
	}
}
