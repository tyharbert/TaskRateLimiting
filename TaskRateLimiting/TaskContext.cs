using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaskRateLimiting
{
    public class TaskContext
    {
        private readonly TaskDebounceContext _taskDebounceContext;
        private readonly TaskThrottleContext _taskThrottleContext;

        public TaskContext()
        {
            _taskDebounceContext = new TaskDebounceContext();
            _taskThrottleContext = new TaskThrottleContext();
        }

        public Task Debounce(TimeSpan duration, CancellationToken cancellationToken)
        {
            return _taskDebounceContext.Debounce(duration, cancellationToken);
        }

        public Task Debounce(TimeSpan duration)
        {
            return _taskDebounceContext.Debounce(duration);
        }

        public Task Throttle(TimeSpan duration, CancellationToken cancellationToken)
        {
            return _taskThrottleContext.Throttle(duration, cancellationToken);
        }

        public Task Throttle(TimeSpan duration)
        {
            return _taskThrottleContext.Throttle(duration);
        }
    }
}