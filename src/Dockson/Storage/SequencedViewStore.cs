using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dockson.Domain;
using Dockson.Infrastructure;
using Microsoft.Extensions.Hosting;

namespace Dockson.Storage
{
	public class SequencedViewStore : IViewStore, IHostedService
	{
		private readonly IViewStore _inner;
		private readonly SequencedAction _queue;

		public SequencedViewStore(IViewStore inner)
		{
			_inner = inner;
			_queue = new SequencedAction();
		}

		public Dictionary<string, GroupView> View => _inner.View;

		public void Load() => _inner.Load();
		public void Save() => _queue.Later(_inner.Save).Wait();
		public GroupView For(string @group) => _inner.For(@group);

		public async Task StartAsync(CancellationToken cancellationToken) => await _queue.StartAsync(cancellationToken);
		public async Task StopAsync(CancellationToken cancellationToken) => await _queue.StopAsync(cancellationToken);
	}
}
