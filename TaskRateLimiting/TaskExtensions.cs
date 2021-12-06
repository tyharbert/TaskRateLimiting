using System;
using System.Threading.Tasks;

namespace TaskRateLimiting
{
    public static class TaskExtensions
    {
        public static Task Subscribe(this Task task, Action<Task> continuationAction)
        {
            return task.ContinueWith(continuationAction, TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}