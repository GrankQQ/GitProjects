using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

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



            #endregion

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
