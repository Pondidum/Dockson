using System;
using System.Threading;
using System.Threading.Tasks;
using Dockson.Domain;
using Dockson.Infrastructure;
using Microsoft.Extensions.Hosting;

namespace Dockson.Storage
{
	public class SequencedStateStore : IStateStore, IHostedService
	{
		private readonly IStateStore _inner;
		private readonly SequencedAction _queue;

		public SequencedStateStore(IStateStore inner)
		{
			_inner = inner;
			_queue = new SequencedAction();
		}

		public void Load() => _inner.Load();
		public void Save() => _queue.Later(_inner.Save).Wait();
		public TState StateFor<TState>(Type owner) where TState : new() => _inner.StateFor<TState>(owner);

		public async Task StartAsync(CancellationToken cancellationToken) => await _queue.StartAsync(cancellationToken);
		public async Task StopAsync(CancellationToken cancellationToken) => await _queue.StopAsync(cancellationToken);
	}
}
