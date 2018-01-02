using System.Collections.Generic;

namespace Dockson.Domain
{
	public interface IViewStore
	{
		Dictionary<string, GroupView> View { get; }

		void Save();
		void Load();
		GroupView For(string group);
	}
}
