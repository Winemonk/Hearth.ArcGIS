using System.Collections.Concurrent;

namespace Hearth.ArcGIS.Infrastructure
{
    /// <summary>
    /// 数据库上下文队列任务
    /// </summary>
    public static class DbContextQueuedTask
    {
        private static readonly ConcurrentQueue<Func<Task>> _queue = new();
        private static readonly SemaphoreSlim _signal = new(0);
        private static readonly AsyncLocal<bool> _inQueue = new();
        private static readonly object _startLock = new();

        private static readonly CancellationTokenSource _shutdownCts = new();
        private static Task? _processorTask;
        private static bool _isShuttingDown = false;

        static DbContextQueuedTask() => StartProcessor();

        private static void StartProcessor()
        {
            lock (_startLock)
            {
                if (_processorTask == null || _processorTask.IsCompleted)
                {
                    _processorTask = Task.Factory.StartNew(
                        ProcessQueueAsync,
                        CancellationToken.None,
                        TaskCreationOptions.LongRunning,
                        TaskScheduler.Default).Unwrap();
                }
            }
        }

        private static async Task ProcessQueueAsync()
        {
            while (!_shutdownCts.Token.IsCancellationRequested || !_queue.IsEmpty)
            {
                try
                {
                    await _signal.WaitAsync(_shutdownCts.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    if (_queue.IsEmpty) break;
                }

                if (!_queue.TryDequeue(out var work)) continue;

                _inQueue.Value = true;
                try
                {
                    await work().ConfigureAwait(false);
                }
                catch { }
                finally
                {
                    _inQueue.Value = false;
                }
            }
        }

        /// <summary>
        /// 停止，停止后再继续添加任务会抛出异常
        /// </summary>
        public static async Task StopAsync()
        {
            lock (_startLock)
            {
                if (_isShuttingDown) return;
                _isShuttingDown = true;
            }

            _shutdownCts.Cancel();
            if (_processorTask != null)
            {
                await _processorTask.ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 排队执行任务
        /// </summary>
        /// <param name="action">待执行任务方法</param>
        /// <exception cref="ArgumentNullException">参数为空异常</exception>
        public static void Run(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (_inQueue.Value) { action(); return; }

            RunAsync(() => { action(); return Task.FromResult(0); }).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 排队执行任务
        /// </summary>
        /// <typeparam name="T">任务返回值类型</typeparam>
        /// <param name="func">待执行任务方法</param>
        /// <returns>任务返回值</returns>
        /// <exception cref="ArgumentNullException">参数为空异常</exception>
        public static T Run<T>(Func<T> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));
            if (_inQueue.Value) return func();

            return RunAsync(() => Task.FromResult(func())).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 排队执行任务
        /// </summary>
        /// <param name="task">待执行任务</param>
        /// <param name="token">取消令牌</param>
        public static Task Run(Func<Task> task, CancellationToken token = default)
            => RunAsync<object?>(async () => { await task(); return null; }, token);

        /// <summary>
        /// 排队执行任务
        /// </summary>
        /// <typeparam name="T">任务返回值类型</typeparam>
        /// <param name="task">待执行任务</param>
        /// <param name="token">取消令牌</param>
        public static Task<T> Run<T>(Func<Task<T>> task, CancellationToken token = default)
            => RunAsync(task, token);

        private static async Task<T> RunAsync<T>(Func<Task<T>> task, CancellationToken token = default)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            if (_isShuttingDown)
            {
                throw new InvalidOperationException("QueuedTask 正在关闭，不接受新任务。");
            }

            if (_inQueue.Value) return await task().ConfigureAwait(false);

            token.ThrowIfCancellationRequested();

            var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);

            _queue.Enqueue(async () =>
            {
                if (token.IsCancellationRequested)
                {
                    tcs.TrySetCanceled(token);
                    return;
                }

                try
                {
                    var result = await task().ConfigureAwait(false);
                    tcs.TrySetResult(result);
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    tcs.TrySetCanceled(token);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            });

            _signal.Release();

            if (token.CanBeCanceled)
            {
                using (token.Register(() => tcs.TrySetCanceled(token)))
                {
                    return await tcs.Task.ConfigureAwait(false);
                }
            }

            return await tcs.Task.ConfigureAwait(false);
        }
    }
}