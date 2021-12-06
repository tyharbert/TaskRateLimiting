using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaskRateLimiting
{
    public class TaskDebounceContext
    {
        private Task _debounceTask;
        private CancellationTokenSource _debounceTaskCancellationTokenSource;

        public Task Debounce(TimeSpan duration, CancellationToken cancellationToken)
        {
            return ExecuteDebounce(duration, cancellationToken);
        }

        public Task Debounce(TimeSpan duration)
        {
            return ExecuteDebounce(duration, null);
        }

        private Task ExecuteDebounce(TimeSpan duration, CancellationToken? cancellationToken)
        {
            if (PreviousTaskNotComplete())
            {
                CancelDebounceTask();
            }

            CreateNewCancellationTokenSource(cancellationToken);
            return CreateNewDebounceTask(duration, GetToken());
        }

        private bool PreviousTaskNotComplete()
        {
            return _debounceTask != null && !_debounceTask.IsCompleted;
        }

        private CancellationToken GetToken()
        {
            return _debounceTaskCancellationTokenSource.Token;
        }

        private void CreateNewCancellationTokenSource(CancellationToken? cancellationToken)
        {
            _debounceTaskCancellationTokenSource = cancellationToken.HasValue
                ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken.Value)
                : new CancellationTokenSource();
        }

        private Task CreateNewDebounceTask(TimeSpan duration, CancellationToken token)
        {
            return _debounceTask = Task.Run(async () =>
            {
                await Task.Delay(duration, token);

                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }
            }, token);
        }

        private void CancelDebounceTask()
        {
            _debounceTaskCancellationTokenSource.Cancel();
        }
    }
}