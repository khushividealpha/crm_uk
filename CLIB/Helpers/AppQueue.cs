using System.Collections.Concurrent;
using System.Threading;

namespace CLIB.Helpers
{
    public class AppQueue<T> : IDisposable
    {
        public delegate Task OnDequeue(T item);
        private readonly ConcurrentQueue<T> queue = new ConcurrentQueue<T>();
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(0); // Start with 0 permits
        private readonly OnDequeue _dequeueHandler;
        private readonly CancellationTokenSource _cts = new();
        private Task _processingTask;
        private bool _disposed = false;

        public AppQueue(OnDequeue dequeueHandler)
        {
            _dequeueHandler = dequeueHandler ?? throw new ArgumentNullException(nameof(dequeueHandler));
            _processingTask = Task.Run(() => ProcessQueueAsync(_cts.Token));
        }

        public void Enqueue(T item)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(AppQueue<T>));

            queue.Enqueue(item);
            semaphoreSlim.Release(); // Signal that an item is available
        }

        private async Task ProcessQueueAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Wait for an item to be available or cancellation
                    await semaphoreSlim.WaitAsync(cancellationToken).ConfigureAwait(false);

                    if (queue.TryDequeue(out var item) && item != null)
                    {
                         _dequeueHandler.Invoke(item);
                    }
                }
                catch (OperationCanceledException)
                {
                    // Expected during shutdown
                    break;
                }
                catch (Exception ex)
                {
                    // Log error here if needed
                    Console.WriteLine($"Error processing queue item: {ex.Message}");
                    // Continue processing other items
                }
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            // Signal cancellation
            _cts.Cancel();

            try
            {
                // Wait for the processing task to complete
                _processingTask?.GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
                // Expected
            }

            _cts.Dispose();
            semaphoreSlim.Dispose();

            // Clear the queue
            while (queue.TryDequeue(out _)) { }
        }
    }
}