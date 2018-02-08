using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Hosting;

namespace Dockson.Infrastructure
{
	public class SequencedAction : IHostedService
	{
		private readonly Task _consumer;
		private readonly BufferBlock<Action> _queue;
		private readonly CancellationTokenSource _completion;

		public SequencedAction()
		{
			_queue = new BufferBlock<Action>(new DataflowBlockOptions
			{
				BoundedCapacity = 1000 // guesses!
			});

			_completion = new CancellationTokenSource();
			_consumer = new Task(async () => await Consumer(), _completion.Token);
		}

		public async Task Later(Action later)
		{
			var tcs = new TaskCompletionSource<bool>();
			_queue.Post(() =>
			{
				later();
				tcs.SetResult(true);
			});

			await tcs.Task;
		}

		private async Task Consumer()
		{
			while (await _queue.OutputAvailableAsync() && !_completion.IsCancellationRequested)
			{
				var action = _queue.Receive();
				action();
			}
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_consumer.Start();

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_completion.Cancel();
			_completion.Dispose();
			_consumer.Dispose();

			return Task.CompletedTask;
		}
	}
}
