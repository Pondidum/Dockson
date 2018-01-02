using System.Collections.Generic;
using Dockson.Domain;

namespace Dockson.Tests.Domain
{
	public class DictionaryViewStore : IViewStore
	{
		public Dictionary<string, GroupView> View { get; }

		public DictionaryViewStore()
		{
			View = new Dictionary<string, GroupView>();
		}

		public void Save()
		{
		}

		public void Load()
		{
		}

		public GroupView For(string @group)
		{
			View.TryAdd(@group, new GroupView());
			return View[@group];
		}
	}
}
