using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;

namespace TaskParallelLibraryProject
{
    class Program
    {

        static void Main(string[] args)
        {
            #region 4.2 创建Task

            /*
            var t1 = new Task(() => TaskMethod("Task 1"));
            var t2 = new Task(() => TaskMethod("Task 2"));
            t1.Start();
            t2.Start();
            Task.Run(() => TaskMethod("Task 3"));
            Task.Factory.StartNew(() => TaskMethod("Task 4"));
            Task.Factory.StartNew(() => TaskMethod("Task 5"), TaskCreationOptions.LongRunning);
            Thread.Sleep(TimeSpan.FromMinutes(1));
            */

            #endregion

            #region 4.3 从Task中获取值

            /*
            TaskMethodInt("Main Thread Task");
            Task<int> task = CreateTask("Task 1");
            task.Start();
            int result = task.Result;
            Console.WriteLine("Result is: {0}",result);

            task = CreateTask("Task 2");
            task.RunSynchronously();
            result = task.Result;
            Console.WriteLine("Result is: {0}", result);

            task = CreateTask("Task 3");
            task.Start();

            while (!task.IsCompleted)
            {
                Console.WriteLine(task.Status);
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }

            Console.WriteLine(task.Status);
            result = task.Result;
            Console.WriteLine("Result is: {0}", result);
            Thread.Sleep(TimeSpan.FromMinutes(1));
            */

            #endregion

            #region 4.4 组合任务 父子任务、后续任务

            /*
            var firstTask = new Task<int>(() => TaskMethodCombination("First Task", 3));
            var secondTask = new Task<int>(() => TaskMethodCombination("Second Task",2));

            firstTask.ContinueWith(t => Console.WriteLine("The first answer is {0}. Thread id {1}, is thread pool thread: {2}", t.Result
                , Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread), TaskContinuationOptions.OnlyOnRanToCompletion);

            firstTask.Start();
            secondTask.Start();

            Thread.Sleep(TimeSpan.FromSeconds(4));

            Task continuation = secondTask.ContinueWith(t => Console.WriteLine("The second answer is {0}. Thread id {1}, is thread pool thread: {2}", t.Result
                , Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread), TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);

            continuation.GetAwaiter().OnCompleted(
                () => Console.WriteLine("Continuation Task Completed! Thread id {0}, is thread pool thread: {1}", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread)
            );

            Thread.Sleep(TimeSpan.FromSeconds(2));
            Console.WriteLine();

            //父子任务，子任务必须在父任务运行时创建，并正确的附加给父任务
            firstTask = new Task<int>(() => {
                var innerTask = Task.Factory.StartNew(() => TaskMethodCombination("Second Task", 5), TaskCreationOptions.AttachedToParent);
                innerTask.ContinueWith(testc => TaskMethodCombination("Third Task", 2), TaskContinuationOptions.AttachedToParent);
                return TaskMethodCombination("First Task", 2);
            });

            firstTask.Start();

            while (!firstTask.IsCompleted)
            {
                Console.WriteLine(firstTask.Status);
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
            Console.WriteLine(firstTask.Status);

            Thread.Sleep(TimeSpan.FromMinutes(3));
            */

            #endregion

            #region 4.5 将APM模式转换为任务

            /*
            int threadId;
            AsynchronousTask d = Test;
            IncompatibleAsynchronousTask e = Test;

            Console.WriteLine("Option 1");
            Task<string> task = Task<string>.Factory.FromAsync(d.BeginInvoke("AsyncTaskThread", Callback, "a delegate asynchronous call"), d.EndInvoke);

            task.ContinueWith(t => Console.WriteLine("Callback is finished, now running a continuation! Result: {0}", t.Result));

            while (!task.IsCompleted)
            {
                Console.WriteLine(task.Status);
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
            Console.WriteLine(task.Status);
            Thread.Sleep(TimeSpan.FromSeconds(1));

            Console.WriteLine("--------------------------------");
            Console.WriteLine();
            Console.WriteLine("Option 2");

            task = Task<string>.Factory.FromAsync(d.BeginInvoke, d.EndInvoke, "AsyncTaskThread", "a delegate asynchronous call");
            task.ContinueWith(t => Console.WriteLine("Task is completed, now running a continuation! Result: {0}", t.Result));
            while (!task.IsCompleted)
            {
                Console.WriteLine(task.Status);
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
            Console.WriteLine(task.Status);
            Thread.Sleep(TimeSpan.FromSeconds(1));

            Console.WriteLine("--------------------------------");
            Console.WriteLine();
            Console.WriteLine("Option 3");

            IAsyncResult ar = e.BeginInvoke(out threadId, Callback, "a delegate asynchronous call");
            ar = e.BeginInvoke(out threadId, Callback, "a delegate asynchronous call");
            task = Task<string>.Factory.FromAsync(ar, _ => e.EndInvoke(out threadId, ar));
            task.ContinueWith(t => Console.WriteLine("Task is completed, now running a continuation! Result:{0}, ThreadId: {1}", t.Result,threadId));

            while (!task.IsCompleted)
            {
                Console.WriteLine(task.Status);
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
            Console.WriteLine(task.Status);
            Thread.Sleep(TimeSpan.FromSeconds(100));
            */

            #endregion

            #region 4.6 将EAP模式转换为任务

            /*
            var tcs = new TaskCompletionSource<int>();

            var worker = new BackgroundWorker();
            worker.DoWork += (sender, eventArgs) =>
            {
                eventArgs.Result = TaskMethodForEAP("Background worker", 5);
            };

            worker.RunWorkerCompleted += (sender, eventArgs) =>
            {
                if (eventArgs.Error != null)
                {
                    tcs.SetException(eventArgs.Error);
                }
                else if (eventArgs.Cancelled)
                {
                    tcs.SetCanceled();
                }
                else
                {
                    tcs.SetResult((int)eventArgs.Result);
                }
            };

            worker.RunWorkerAsync();

            int result = tcs.Task.Result;

            Console.WriteLine("Result is: {0}",result);
            */

            #endregion

            #region 4.7 实现取消选项

            var cts = new CancellationTokenSource();
            var longTask = new Task<int>(() => TaskMethodForCancel("Task 1", 10, cts.Token), cts.Token);
            Console.WriteLine(longTask.Status);
            cts.Cancel();
            Console.WriteLine(longTask.Status);
            Console.WriteLine("First task has been cancelled befor execution");
            cts = new CancellationTokenSource();
            longTask = new Task<int>(() => TaskMethodForCancel("Task 2", 10, cts.Token), cts.Token);
            longTask.Start();
            for(int i = 0; i < 5; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
                Console.WriteLine(longTask.Status);
            }
            cts.Cancel();
            for(int i = 0; i < 5; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
                Console.WriteLine(longTask.Status);
            }

            Console.WriteLine("A task has been completed with result {0}.", longTask.Result);

            #endregion

        }

        private static int TaskMethodForCancel(string name, int seconds, CancellationToken token)
        {
            Console.WriteLine("Task {0} is running on a thread id: {1}. Is thread pool thread: {2}", name, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
            for(int i = 0; i < seconds; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                if (token.IsCancellationRequested)
                {
                    return -1;
                }
            }
            return 42 * seconds;
        }

        static int TaskMethodForEAP(string name, int seconds)
        {
            Console.WriteLine("Task {0} is running on a thread id: {1}. Is thread pool thread: {2}", name, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
            return 42 * seconds;
        }

        private delegate string AsynchronousTask(string threadName);
        private delegate string IncompatibleAsynchronousTask(out int threadId);

        private static void Callback(IAsyncResult ar)
        {
            Console.WriteLine("Starting a callback...");
            Console.WriteLine("State passed to a callback: {0}", ar.AsyncState);
            Console.WriteLine("Is thread pool thread: {0}", Thread.CurrentThread.IsThreadPoolThread);
            Console.WriteLine("Thread pool worker thread id: {0}",Thread.CurrentThread.ManagedThreadId);
        }

        private static string Test(string threadName)
        {
            Console.WriteLine("Starting...");
            Console.WriteLine("Is thread pool thread: {0}", Thread.CurrentThread.IsThreadPoolThread);
            Thread.Sleep(TimeSpan.FromSeconds(2));
            Thread.CurrentThread.Name = threadName;
            return string.Format("Thread name: {0}", Thread.CurrentThread.Name);
        }

        private static string Test(out int threadId)
        {
            Console.WriteLine("Starting...");
            Console.WriteLine("Is thread pool thread: {0}", Thread.CurrentThread.IsThreadPoolThread);
            Thread.Sleep(TimeSpan.FromSeconds(2));
            threadId = Thread.CurrentThread.ManagedThreadId;
            return string.Format("Thread pool worker thread id was: {0}", threadId);
        }

        static int TaskMethodCombination(string name, int seconds)
        {
            Console.WriteLine("Task {0} is running on a thread id {1}. Is thread pool thread: {2}",name,Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
            return 42 * seconds;
        }

        static Task<int> CreateTask(string name)
        {
            return new Task<int>(() => TaskMethodInt(name));
        }

        static int TaskMethodInt(string name)
        {
            Console.WriteLine("Task {0} is running on a thread id {1}. Is thread pool thread: {2}",name,Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
            Thread.Sleep(TimeSpan.FromSeconds(2));
            return 42;
        }

        static void TaskMethod(string name)
        {
            Console.WriteLine("Task {0} is running on a thread id {1}. Is thread pool thread: {2}",name, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
        }
    }
}
