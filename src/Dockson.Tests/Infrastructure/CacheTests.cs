using System.Collections.Generic;
using Dockson.Infrastructure;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Infrastructure
{
	public class CacheTests
	{
		[Fact]
		public void When_serializing()
		{
			var container = new TestContainer();
			container.Keys["one"].Add(1);

			var json = JsonConvert.SerializeObject(container);

			json.ShouldBe("{\"Keys\":{\"one\":[1]}}");
		}

		[Fact]
		public void When_deserializing()
		{
			var json = "{\"Keys\":{\"one\":[1]}}";
			var container = JsonConvert.DeserializeObject<TestContainer>(json);

			container.Keys["one"].ShouldBe(new[] { 1 });
		}

		private class TestContainer
		{
			public Cache<string, List<int>> Keys { get; set; }

			public TestContainer()
			{
				Keys = new Cache<string, List<int>>(key => new List<int>());
			}
		}
	}
}
