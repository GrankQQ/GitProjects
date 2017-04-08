using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ThreadSynchronization
{
    class Program
    {
        static SemaphoreSlim _semaphore = new SemaphoreSlim(4);
        private static AutoResetEvent _workerEvent = new AutoResetEvent(false);
        private static AutoResetEvent _mainEvent = new AutoResetEvent(false);
        static ManualResetEventSlim _mainSlimEvent = new ManualResetEventSlim(false);

        static void Main(string[] args)
        {
            #region 2.2.2 InterLock 加减原子操作

            /*
            Console.WriteLine("Incorrect counter");

            var c = new Counter();

            var t1 = new Thread(() => TestCounter(c));
            var t2 = new Thread(() => TestCounter(c));
            var t3 = new Thread(() => TestCounter(c));
            t1.Start();
            t2.Start();
            t3.Start();
            t1.Join();
            t2.Join();
            t3.Join();
            Console.WriteLine("Total countt; {0}", c.Count);
            Console.WriteLine("---------------------------");
            Console.WriteLine("Correct counter");

            var c1 = new CounterNoLock();

            t1 = new Thread(() => TestCounter(c1));
            t2 = new Thread(() => TestCounter(c1));
            t3 = new Thread(() => TestCounter(c1));
            t1.Start();
            t2.Start();
            t3.Start();
            t1.Join();
            t2.Join();
            t3.Join();
            Console.WriteLine("Total countt; {0}", c1.Count);

            Console.ReadLine();
            */

            #endregion

            #region 2.3.2 Muex互斥量

            /*当主程序启动时，定义一个指定名称的互斥量，设置initialOwner标志为false。意味着，如果互斥量被创建，则允许程序获取该互斥量。如果没有获取到互斥量，则走else.
            //Mutex互斥量是全局的操作系统对象！可用于在不同的程序中同步线程。

            const string MutexName = "CSharpThreadingCookbook";

            using(var m = new Mutex(false, MutexName))
            {
                if (!m.WaitOne(TimeSpan.FromSeconds(5), false))
                {
                    Console.WriteLine("Second instance is running!");
                }
                else
                {
                    Console.WriteLine("Running!");
                    Console.ReadLine();
                    m.ReleaseMutex();
                }
            }
            */

            #endregion

            #region 2.4.2 SemaphoreSlime 限制了同时访问同一个资源的线程数量

            /*
            //SemaphoreSlime 限制了同一个资源的线程数量，如果超过，则需要等待 ，需要SemaphoreSlime.Wait配合           

            for(int i = 1; i <= 6; i++)
            {
                string threadName = "Thread " + i;
                int secondsTowait = 2 + 2 * i;
                var t = new Thread(() => AccessDatabase(threadName, secondsTowait));
                t.Start();
            }
            Console.ReadLine();
            */

            #endregion

            #region 2.5.2 AutoResetEvent

            /* 当主程序启动时，定义了两个AutoResetEvent实例。藏品睛个是从子线程向主线程发信号，另一个实例是从主线程向子线程发信号。我们向AutoResetEvent构造方法传入false，定义了这两个实例的初始状态为unsingaled。
             * 这意味着任何线程调用这两个对象中的任何一个的WaitOne方法将会被阻塞，直到我们调用了Set方法。如果初始事件状态为true，那么AutoResetEvent实例的状态为signaled，如果线程调用WaitOne方法则会被立即处理。
             * 然后事件状态自动变为unsignaled，所以需要再对该实例调用一次Set方法，以便让其他的线程对该实例调用WaitOne方法从而继续执行。
             * 使用的是内核时间模式，所以等待时间不能太长。
             */

            /*
            var t = new Thread(() => Process(10));
            t.Start();
            Console.WriteLine("First operation is completed!");
            Console.WriteLine("Performing an operation on a main thread");
            Thread.Sleep(TimeSpan.FromSeconds(5));
            _mainEvent.Set();
            Console.WriteLine("Now running the second operation on a second thread");
            _workerEvent.WaitOne();
            Console.WriteLine("Second operation is completed!");
            */

            #endregion

            #region 2.6.2 ManualResetEventSlim

            var t1 = new Thread(() => TravelThroughGates("Thread 1", 5));
            var t2 = new Thread(() => TravelThroughGates("Thread 2", 6));
            var t3 = new Thread(() => TravelThroughGates("Thread 3", 12));
            t1.Start();
            t2.Start();
            t3.Start();
            Thread.Sleep(TimeSpan.FromSeconds(6));
            Console.WriteLine("The gates are now open!");
            _mainSlimEvent.Set();
            Thread.Sleep(TimeSpan.FromSeconds(2));
            _mainSlimEvent.Reset();
            Console.WriteLine("The gates have been closed!");
            Thread.Sleep(TimeSpan.FromSeconds(10));
            Console.WriteLine("The gates are now open for the second time!");
            _mainSlimEvent.Set();
            Thread.Sleep(TimeSpan.FromSeconds(2));
            Console.WriteLine("The gates have been closed!");
            _mainSlimEvent.Reset();

            #endregion
        }

        static void TravelThroughGates(string threadName, int seconds)
        {
            Console.WriteLine("{0} falls to sleep", threadName);
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
            Console.WriteLine("{0} waits for the gates to open!", threadName);
            _mainSlimEvent.Wait();
            Console.WriteLine("{0} enters the gates!", threadName);
        }

        static void Process(int seconds)
        {
            Console.WriteLine("Starting a long running word...");
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
            Console.WriteLine("Work is done!");
            _workerEvent.Set();
            Console.WriteLine("Waiting for a main thread to complete its work");
            _mainEvent.WaitOne();
            Console.WriteLine("Starting second operation...");
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
            Console.WriteLine("Work is done!");
            _workerEvent.Set();
        }

        static void AccessDatabase(string name,int seconds)
        {
            Console.WriteLine("{0} waits to access a database", name);
            _semaphore.Wait();
            Console.WriteLine("{0} was granted an access to a database", name);
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
            Console.WriteLine("{0} is completed", name);
            _semaphore.Release();
        }

        static void TestCounter(CounterBase c)
        {
            for(int i = 0; i < 100000; i++)
            {
                c.Increment();
                c.Decrement();
            }
        }

        class Counter : CounterBase
        {
            private int _count;
            public int Count { get { return _count; } }

            public override void Increment()
            {
                _count++;
            }

            public override void Decrement()
            {
                _count--;
            }
        }

        class CounterNoLock: CounterBase
        {
            private int _count;

            public int Count { get { return _count; } }

            public override void Increment()
            {
                Interlocked.Increment(ref _count);
            }

            public override void Decrement()
            {
                Interlocked.Decrement(ref _count);
            }
        }

        abstract class CounterBase
        {
            public abstract void Increment();

            public abstract void Decrement();
        }
    }
}
