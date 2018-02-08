using System.Threading;
using System.Threading.Tasks;
using Dockson.Domain;
using Microsoft.Extensions.Hosting;

namespace Dockson.Infrastructure
{
	public class SequencingProjector : IProjector, IHostedService
	{
		private readonly IProjector _inner;
		private readonly SequencedAction _queue;

		public SequencingProjector(IProjector inner)
		{
			_inner = inner;
			_queue = new SequencedAction();
		}

		public void Project(Notification notification)
		{
			_queue.Later(() => _inner.Project(notification)).Wait();
		}

		public async Task StartAsync(CancellationToken cancellationToken) => await _queue.StartAsync(cancellationToken);
		public async Task StopAsync(CancellationToken cancellationToken) => await _queue.StopAsync(cancellationToken);
	}
}
