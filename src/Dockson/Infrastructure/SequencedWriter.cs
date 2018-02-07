using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Hosting;

namespace Dockson.Infrastructure
{
	public class SequencedWriter<T> : IHostedService
	{
		private readonly Task _consumer;
		private readonly BufferBlock<T> _queue;
		private readonly CancellationTokenSource _completion;

		public SequencedWriter(Action<T> write)
		{
			_queue = new BufferBlock<T>(new DataflowBlockOptions
			{
				BoundedCapacity = 1000 // guesses!
			});

			_completion = new CancellationTokenSource();
			_consumer = new Task(async () => await Consumer(write), _completion.Token);
		}

		public void Write(T thing)
		{
			_queue.Post(thing);
		}

		private async Task Consumer(Action<T> write)
		{
			while (await _queue.OutputAvailableAsync() && !_completion.IsCancellationRequested)
			{
				var element = _queue.Receive();
				write(element);
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
