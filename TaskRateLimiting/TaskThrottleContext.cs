using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaskRateLimiting
{
    public class TaskThrottleContext
    {
        private readonly static Task _cancelledTask;
        private Task _throttleTask;

        static TaskThrottleContext()
        {
            var tcs = new CancellationTokenSource();
            tcs.Cancel();
            _cancelledTask = Task.FromCanceled(tcs.Token);
        }

        public Task Throttle(TimeSpan duration, CancellationToken cancellationToken)
        {
            return ExecuteThrottle(duration, cancellationToken);
        }

        public Task Throttle(TimeSpan duration)
        {
            return ExecuteThrottle(duration, null);
        }

        private Task ExecuteThrottle(TimeSpan duration, CancellationToken? cancellationToken)
        {
            if (PreviousTaskNotComplete())
            {
                return GetCancelledTask();
            }

            return cancellationToken.HasValue
                ? CreateNewThrottleTask(duration, cancellationToken.Value)
                : CreateNewThrottleTask(duration);
        }

        private Task GetCancelledTask()
        {
            return _cancelledTask;
        }

        private bool PreviousTaskNotComplete()
        {
            return _throttleTask != null && !_throttleTask.IsCompleted;
        }

        private Task CreateNewThrottleTask(TimeSpan duration)
        {
            return _throttleTask = Task.Run(async () =>
            {
                await Task.Delay(duration);
            });
        }

        private Task CreateNewThrottleTask(TimeSpan duration, CancellationToken token)
        {
            return _throttleTask = Task.Run(async () =>
            {
                await Task.Delay(duration, token);

                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }
            }, token);
        }
    }
}