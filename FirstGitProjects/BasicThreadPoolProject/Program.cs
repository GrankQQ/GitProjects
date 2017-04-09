using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;

namespace BasicThreadPoolProject
{
    class Program
    {
        private delegate string RunOnThreadPool(out int threadId);
        static Timer _timer;

        static void Main(string[] args)
        {
            #region 3.2 BeginInvoke、EndInvoke异步线程对，用于执行异步操作，以及完成异步操作之后的回调操作，其中Callback使用的是线程池

            /*
            int threadId = 0;

            RunOnThreadPool poolDelegate = Test;

            var t = new Thread(() =>Test(out threadId));
            t.Start();
            t.Join();
            Console.WriteLine("Thread Id: {0}", threadId);

            IAsyncResult r = poolDelegate.BeginInvoke(out threadId, Callback, "a delegate asynchronous call");
            r.AsyncWaitHandle.WaitOne();

            string result = poolDelegate.EndInvoke(out threadId, r);

            Console.WriteLine("Thread pool worker thread id: {0}", threadId);
            Console.WriteLine(result);

            Thread.Sleep(TimeSpan.FromSeconds(12));
            */

            #endregion

            #region 3.3 通过线程池，加入异步操作

            /*
            ThreadPool.QueueUserWorkItem(AsyncOperation);
            Thread.Sleep(TimeSpan.FromSeconds(1));
            ThreadPool.QueueUserWorkItem(AsyncOperation, "async state");
            Thread.Sleep(TimeSpan.FromSeconds(1));

            ThreadPool.QueueUserWorkItem(state => {
                Console.WriteLine("Operation state: {0}", state);
                Console.WriteLine("Worker thread id: {0}",Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }, "lambda state");

            Thread.Sleep(TimeSpan.FromSeconds(5));
            */

            #endregion

            #region 3.4 比较创建线程和线程池中加入线程，使用线程，时间会短，但耗费的内存和线程数多，使用线程池时间会长，但是节省了内存和线程数

            /*
            const int numberOfOperations = 500;
            var sw = new Stopwatch();
            sw.Start();
            UseThreads(numberOfOperations);
            sw.Stop();
            Console.WriteLine("Execution time using threads: {0}", sw.ElapsedMilliseconds);            

            sw.Reset();
            sw.Start();
            UseThreadPool(numberOfOperations);
            sw.Stop();
            Console.WriteLine("Execution time using threads: {0}", sw.ElapsedMilliseconds);
            */

            #endregion

            #region 3.5 取消任务

            /*
            using(var cts = new CancellationTokenSource())
            {
                CancellationToken token = cts.Token;
                ThreadPool.QueueUserWorkItem(_ => AsyncOperation1(token));
                Thread.Sleep(TimeSpan.FromSeconds(2));
                cts.Cancel();
            }

            using (var cts = new CancellationTokenSource())
            {
                CancellationToken token = cts.Token;
                ThreadPool.QueueUserWorkItem(_ => AsyncOperation2(token));
                Thread.Sleep(TimeSpan.FromSeconds(2));
                cts.Cancel();
            }

            using (var cts = new CancellationTokenSource())
            {
                CancellationToken token = cts.Token;
                ThreadPool.QueueUserWorkItem(_ => AsyncOperation3(token));
                Thread.Sleep(TimeSpan.FromSeconds(2));
                cts.Cancel();
            }

            Thread.Sleep(TimeSpan.FromSeconds(10));
            */

            #endregion

            #region 3.6 在线程池中使用等待事件处理器及超时

            /*
            RunOperations(TimeSpan.FromSeconds(5));
            RunOperations(TimeSpan.FromSeconds(7));
            */

            #endregion

            #region 3.7 使用计时器

            /*
            Console.WriteLine("Press 'Enter' to stop the timer...");
            DateTime start = DateTime.Now;
            _timer = new Timer(_ => TimerOperation(start), null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));

            Thread.Sleep(TimeSpan.FromSeconds(6));

            _timer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(4));

            Console.ReadLine();

            _timer.Dispose();
            */

            #endregion

            #region 3.8 BackgroundWorker 基于事件的异步模式 Event-based Asynchronous Pattern 简称EAP

            var bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;

            bw.DoWork += Worker_DoWork;
            bw.ProgressChanged += Worker_ProgressChanged;
            bw.RunWorkerCompleted += Worker_Completed;

            bw.RunWorkerAsync();

            Console.WriteLine("Press C to cancel work");
            do
            {
                if (Console.ReadKey(true).KeyChar == 'C')
                {
                    bw.CancelAsync();
                }
            }
            while (bw.IsBusy);

            #endregion
        }

        static void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Console.WriteLine("DoWork thread pool thread id: {0}", Thread.CurrentThread.ManagedThreadId);
            var bw = (BackgroundWorker)sender;
            for(int i=1;i<=100; i++)
            {
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                if (i % 10 == 0)
                {
                    bw.ReportProgress(i);
                }

                Thread.Sleep(TimeSpan.FromSeconds(0.1));
            }
            e.Result = 42;
        }

        static void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine("{0}% completed. Progress thread pool thread id: {1}", e.ProgressPercentage, Thread.CurrentThread.ManagedThreadId);
        }

        static void Worker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("Completed thread pool thread id: {0}", Thread.CurrentThread.ManagedThreadId);
            if (e.Error != null)
            {
                Console.WriteLine("Exception {0} has occured.", e.Error.Message);
            }
            else if (e.Cancelled)
            {
                Console.WriteLine("Operation has been canceled");
            }
            else
            {
                Console.WriteLine("The answer is: {0}", e.Result);
            }
        }

        static void TimerOperation(DateTime start)
        {
            TimeSpan elapsed = DateTime.Now - start;
            Console.WriteLine("{0} seconds from {1}. Timer thread pool thread id: {2}",elapsed.Seconds, start, Thread.CurrentThread.ManagedThreadId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workerOperationTimeout"></param>
        static void RunOperations(TimeSpan workerOperationTimeout)
        {
            using(var evt = new ManualResetEvent(false))
            {
                using(var cts = new CancellationTokenSource())
                {
                    Console.WriteLine("Registering timeout operations...");
                    //设置一个超时时间，以及超时或回调方法
                    var worker = ThreadPool.RegisterWaitForSingleObject(evt, (state, isTimedOut) => WorkerOperationWait(cts, isTimedOut), null, workerOperationTimeout, true);

                    Console.WriteLine("Starting long running operation...");

                    ThreadPool.QueueUserWorkItem(_ => WorkerOperation(cts.Token, evt));

                    Thread.Sleep(workerOperationTimeout.Add(TimeSpan.FromSeconds(2)));
                    worker.Unregister(evt);
                }
            }
        }

        /// <summary>
        /// 设置一个6秒的操作
        /// </summary>
        /// <param name="token"></param>
        /// <param name="evt"></param>
        static void WorkerOperation(CancellationToken token,ManualResetEvent evt)
        {
            for(int i = 0; i < 6; i++)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            evt.Set();
        }

        /// <summary>
        /// 超时或回调方法
        /// </summary>
        /// <param name="cts"></param>
        /// <param name="isTimedOut"></param>
        static void WorkerOperationWait(CancellationTokenSource cts, bool isTimedOut)
        {
            if (isTimedOut)
            {
                cts.Cancel();
                Console.WriteLine("Worker operation timed out and was canceled.");
            }
            else
            {
                Console.WriteLine("Worker operation succeded");
            }
        }

        /// <summary>
        /// 检查IsCancellationRequested来判断是否被取消
        /// </summary>
        /// <param name="token"></param>
        static void AsyncOperation1(CancellationToken token)
        {
            Console.WriteLine("Starting the first task");
            for(int i = 0; i < 5; i++)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("The first task has been canceled.");
                    return;
                }
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            Console.WriteLine("The second task has  completed successfully");
        }

        /// <summary>
        /// 抛出一个OperationCancelledException异常
        /// </summary>
        /// <param name="token"></param>
        static void AsyncOperation2(CancellationToken token)
        {
            try
            {
                Console.WriteLine("Starting the second task");

                for(int i = 0; i < 5; i++)
                {
                    token.ThrowIfCancellationRequested();
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
                Console.WriteLine("The second task has completed successfully");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("The second task has been canceled.");
            }
        }

        /// <summary>
        /// 注册一个回调函数，当取消操作时调用
        /// </summary>
        /// <param name="token"></param>
        private static void AsyncOperation3(CancellationToken token)
        {
            bool cancellationFlag = false;
            token.Register(() => cancellationFlag = true );
            Console.WriteLine("Starting the third task");
            for(int i = 0; i < 5; i++)
            {
                if (cancellationFlag)
                {
                    Console.WriteLine("The third task has been canceled.");
                    return;
                }
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            Console.WriteLine("The third task has completed successfully");
        }

        /// <summary>
        /// 使用线程添加操作
        /// </summary>
        /// <param name="numberOfOperations"></param>
        static void UseThreads(int numberOfOperations)
        {
            using(var countdown = new CountdownEvent(numberOfOperations))
            {
                Console.WriteLine("Scheduling work by creating threads");
                for(int i = 0; i < numberOfOperations; i++)
                {
                    var thread = new Thread(() => {
                        Console.WriteLine("{0},",Thread.CurrentThread.ManagedThreadId);
                        Thread.Sleep(TimeSpan.FromSeconds(0.1));
                        countdown.Signal();
                    });
                    thread.Start();
                }
                countdown.Wait();
                Console.WriteLine();
            }
        }

        /// <summary>
        /// 使用线程池添加操作
        /// </summary>
        /// <param name="numberOfOperations"></param>
        static void UseThreadPool(int numberOfOperations)
        {
            using(var countdown = new CountdownEvent(numberOfOperations))
            {
                for(int i = 0; i < numberOfOperations; i++)
                {
                    ThreadPool.QueueUserWorkItem(_ => {
                        Console.WriteLine("{0},", Thread.CurrentThread.ManagedThreadId);
                        Thread.Sleep(TimeSpan.FromSeconds(0.1));
                        countdown.Signal();
                    });
                }
                countdown.Wait();
                Console.WriteLine();
            }
        }

        private static void AsyncOperation(object state)
        {
            Console.WriteLine("Operation state: {0}", state??"(null)");
            Console.WriteLine("Worker thread id: {0}", Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(TimeSpan.FromSeconds(2));
        }

        private static void Callback(IAsyncResult ar)
        {
            Console.WriteLine("Starting a callback...");
            Console.WriteLine("State passed to a callback:{0}", ar.AsyncState);
            Console.WriteLine("Is thread pool thread: {0}", Thread.CurrentThread.IsThreadPoolThread);
            Console.WriteLine("Thread pool worker thread id: {0}",Thread.CurrentThread.ManagedThreadId);
        }

        private static string Test(out int threadId)
        {
            Console.WriteLine("Starting...");
            Console.WriteLine("Is thread pool thread: {0}", Thread.CurrentThread.IsThreadPoolThread);
            Thread.Sleep(TimeSpan.FromSeconds(2));
            threadId = Thread.CurrentThread.ManagedThreadId;
            return string.Format("Thread pool worker thread id was {0}", threadId);
        }
    }
}
