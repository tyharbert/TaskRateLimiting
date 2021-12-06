using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace TaskRateLimiting.Tests
{
    public class TaskContext_DebounceShould
    {
        [Fact]
        public void Debounce_WithRapidCalls_CompletesOnce()
        {
            // arrange
            TaskContext taskContext = new TaskContext();
            CancellationTokenSource cts = new CancellationTokenSource();
            List<Task> tasks = new List<Task>();
            int callCounter = 0;

            // act
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(taskContext.Debounce(TimeSpan.FromMilliseconds(100), cts.Token)
                    .ContinueWith(taskContext => callCounter++, TaskContinuationOptions.OnlyOnRanToCompletion));
            }

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException)
            {

            }

            // assert
            Assert.Equal(1, callCounter);
        }

        [Fact]
        public void Debounce_WithDelayedCalls_CompletesTwice()
        {
            // arrange
            TaskContext taskContext = new TaskContext();
            CancellationTokenSource cts = new CancellationTokenSource();
            List<Task> tasks = new List<Task>();
            int callCounter = 0;

            // act
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(taskContext.Debounce(TimeSpan.FromMilliseconds(100), cts.Token)
                    .ContinueWith(taskContext => callCounter++, TaskContinuationOptions.OnlyOnRanToCompletion));

                if (i == 4)
                {
                    Thread.Sleep(150);
                }
            }

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException)
            {

            }

            // assert
            Assert.Equal(2, callCounter);
        }

        [Fact]
        public void Debounce_WhenTokenIsCancelled_CompletesNever()
        {
            // arrange
            TaskContext taskContext = new TaskContext();
            CancellationTokenSource cts = new CancellationTokenSource();
            List<Task> tasks = new List<Task>();
            int callCounter = 0;

            // act
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(taskContext.Debounce(TimeSpan.FromMilliseconds(100), cts.Token)
                    .ContinueWith(taskContext => callCounter++, TaskContinuationOptions.OnlyOnRanToCompletion));
            }

            cts.Cancel();

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException)
            {

            }

            // assert
            Assert.Equal(0, callCounter);
        }
    }
}
