using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace AwaitTaskProjects
{
    class Program
    {
        static void Main(string[] args)
        {
            #region 5.2 使用await操作徐伟获取异步任务结果

            /*
            Task t = AsynchronyWithTPL();
            t.Wait();

            t = AsynchronyWithAwait();
            t.Wait();

            Thread.Sleep(TimeSpan.FromSeconds(10));
            */

            #endregion

            #region 5.3 在lambda表达式中使用await操作符

            /*
            Task t = AsynchronousProcessing();
            t.Wait();

            Thread.Sleep(TimeSpan.FromSeconds(10));
            */

            #endregion

            #region 5.4 对连续的异步任务使用await操作符

            /*
            Task t = AsynchronyWithTPL4();
            t.Wait();

            t = AsynchronyWithAwait4();
            t.Wait();

            Thread.Sleep(TimeSpan.FromSeconds(10));
            */

            #endregion

            #region 5.5 对并行执行的异步任务使用await操作符

            /*
            Task t = AsynchronousProcessing5();
            t.Wait();

            Thread.Sleep(TimeSpan.FromSeconds(10));
            */

            #endregion

            #region 5.6 处理异步操作中的异常

            /*
            Task t = AsynchronousProcessing6();
            t.Wait();

            Thread.Sleep(TimeSpan.FromSeconds(10));
            */

            #endregion

        }

        async static Task AsynchronousProcessing6()
        {
            Console.WriteLine("1. Single exception");

            try
            {
                string result = await GetInfoAsync6("Task 1", 2);
                Console.WriteLine(result);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exceptin details: {0}",ex);
            }

            Console.WriteLine();
            Console.WriteLine("2. Multiple exceptions");

            Task<string> t1 = GetInfoAsync6("Task 1", 3);
            Task<string> t2 = GetInfoAsync6("Task 2", 2);
            try
            {
                string[] results = await Task.WhenAll(t1, t2);
                Console.WriteLine(results.Length);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception details: {0}",ex);
            }
            Console.WriteLine();
            Console.WriteLine("2. Multiple exceptions with AggregateException");

            t1 = GetInfoAsync6("Task 1", 3);
            t2 = GetInfoAsync6("Task 2", 2);
            Task<string[]> t3 = Task.WhenAll(t1, t2);
            try
            {
                string[] results = await t3;
                Console.WriteLine(results.Length);
            }
            catch
            {
                var ae = t3.Exception.Flatten();
                var exceptions = ae.InnerExceptions;
                Console.WriteLine("Exceptions caught: {0}",exceptions.Count);
                foreach(var e in exceptions)
                {
                    Console.WriteLine("Exception details: {0}",e);
                    Console.WriteLine();
                }
            }
        }

        async static Task<string> GetInfoAsync6(string name, int seconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            throw new Exception(string.Format("Boom from {0};", name));
        }

        async static Task AsynchronousProcessing5()
        {
            Task<string> t1 = GetInfoAsync5("Task 1", 3);
            Task<string> t2 = GetInfoAsync5("Task 2", 5);

            string[] results = await Task.WhenAll(t1, t2);
            foreach(string  result in results)
            {
                Console.WriteLine(result);
            }
        }

        //Task.Delay是启动计时器，然后等待时间运行，Task.Run是启动线程
        async static Task<string> GetInfoAsync5(string name, int seconds)
        {
            //await Task.Delay(TimeSpan.FromSeconds(seconds));
            await Task.Run(()=>
                Thread.Sleep(TimeSpan.FromSeconds(seconds)));
            return string.Format("Task {0} is running on a thread id {1}. Is thread pool thread: {2}", name, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
        }

        static Task AsynchronyWithTPL4()
        {
            var containerTask = new Task(() => {
                Task<string> t = GetInfoAsync4("TPL 1");
                t.ContinueWith(task =>
                {
                    Console.WriteLine(t.Result);
                    Task<string> t2 = GetInfoAsync4("TPL 2");
                    t2.ContinueWith(innerTask => Console.WriteLine(innerTask.Result), TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.AttachedToParent);
                    t2.ContinueWith(innerTask => Console.WriteLine(innerTask.Exception.InnerException), TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.AttachedToParent);
                }, TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.AttachedToParent);

                t.ContinueWith(task => Console.WriteLine(t.Exception.InnerException), TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.AttachedToParent);
            });

            containerTask.Start();
            return containerTask;
        }

        async static Task AsynchronyWithAwait4()
        {
            try
            {
                string result = await GetInfoAsync4("Async 1");
                Console.WriteLine(result);
                result = await GetInfoAsync4("Async 2");
                Console.WriteLine(result);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        async static Task<string> GetInfoAsync4(string name)
        {
            Console.WriteLine("Task {0} started!", name);
            await Task.Delay(TimeSpan.FromSeconds(2));
            if (name == "TPL 2")
                throw new Exception("Boom!");
            return string.Format("Task {0} is running on a thread id {1}. Is thread pool thread: {2}", name, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
        }

        async static Task AsynchronousProcessing()
        {
            Func<string, Task<string>> asyncLambda = async name => {
                await Task.Delay(TimeSpan.FromSeconds(2));
                return string .Format("Task {0} is running on a thread id {1}. Is thread pool thread: {2}", name, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
            };

            string result = await asyncLambda("async ladmbda");

            Console.WriteLine(result);
        }

        static Task AsynchronyWithTPL()
        {
            Task<string> t = GetInfoAsync("Task 1");
            Task t2 = t.ContinueWith(task => Console.WriteLine(t.Result), TaskContinuationOptions.NotOnFaulted);
            Task t3 = t.ContinueWith(task => Console.WriteLine(t.Exception.InnerException), TaskContinuationOptions.OnlyOnFaulted);
            return Task.WhenAny(t2, t3);
        }

        async static Task AsynchronyWithAwait()
        {
            try
            {
                string result = await GetInfoAsync("Task 2");
                Console.WriteLine(result);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        async static Task<string> GetInfoAsync(string name)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            //throw new Exception("Boom!");
            return string.Format("Task {0} is running on a thread id {1}. Is thread pool thread: {2}", name, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
        }
    }
}
