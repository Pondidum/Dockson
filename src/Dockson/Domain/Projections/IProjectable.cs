using System;
using System.Collections.Generic;

namespace Dockson.Domain.Projections
{
	public interface IProjectable
	{
		DateTime Timestamp { get; }
		HashSet<string> Groups { get; }
	}
}
