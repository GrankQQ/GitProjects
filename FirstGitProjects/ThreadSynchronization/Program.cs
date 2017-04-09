using System;
using System.Collections.Generic;
using System.Threading;

namespace ThreadSynchronization
{
    class Program
    {
        static SemaphoreSlim _semaphore = new SemaphoreSlim(4);
        private static AutoResetEvent _workerEvent = new AutoResetEvent(false);
        private static AutoResetEvent _mainEvent = new AutoResetEvent(false);
        static ManualResetEventSlim _mainSlimEvent = new ManualResetEventSlim(false);
        static CountdownEvent _countdown = new CountdownEvent(2);
        static Barrier _barrier = new Barrier(2, b => Console.WriteLine("End of phars {0}", b.CurrentPhaseNumber + 1));
        static ReaderWriterLockSlim _rw = new ReaderWriterLockSlim();
        static Dictionary<int, int> _items = new Dictionary<int, int>();
        //volatile多用于多线程的环境，当一个变量定义为volatile时，读取这个变量的值时候每次都是从momery里面读取而不是从cache读。这样做是为了保证读取该变量的信息都是最新的
        static volatile bool _isCompleted = false;

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
             *Mutex互斥量是全局的操作系统对象！可用于在不同的程序中同步线程。
             */

            /*
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

            /* 当主程序启动时，定义了两个AutoResetEvent实例。其中一个是从子线程向主线程发信号，另一个实例是从主线程向子线程发信号。我们向AutoResetEvent构造方法传入false，定义了这两个实例的初始状态为unsingaled。
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

            #region 2.6.2 ManualResetEventSlim 整个工作方式有点像人群通过大门

            /* ManualResetEventSlim的整个工作方式有点像人群通过大门。一直保持大门敞开直到手动调用Reset方法。
             * 当调用Set时，相当于打开了大门从而允许准备好的线程接收信号并继续工作
             */
            /*
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
           */

            #endregion

            #region 2.7.2 CountDownEvent 可以调用多个线程，并且等待返回信号，如果有线程没有返回信号，则会继续等待

            /*
            Console.WriteLine("Starting two operations");
            var t1 = new Thread(() => PerformOperation("Operation 1 is completed",4));
            var t2 = new Thread(() => PerformOperation("Operation 2 is completed", 8));
            t1.Start();
            t2.Start();
            _countdown.Wait();
            Console.WriteLine("Both operations have been completed.");
            _countdown.Dispose();
            */

            #endregion

            #region 2.8.2 Barrier 用于组织多个线程及时在某个时刻碰面 多线程迭代计算时非常有用，可以在每个迭代结束前执行一些计算

            /*
            var t1 = new Thread(() => PlayMusic("the guitarist", "play an amazing solo",5));
            var t2 = new Thread(() => PlayMusic("the singer", "sing his song",2));
            t1.Start();
            t2.Start();
            */

            #endregion

            #region 2.9.2 ReaderWriterLockSlim 允许多个线程同时读取，以及独占写

            /*
            new Thread(Read) { IsBackground = true }.Start();
            new Thread(Read) { IsBackground = true }.Start();
            new Thread(Read) { IsBackground = true }.Start();

            new Thread(() => Write("Thread 1")) { IsBackground=true }.Start();
            new Thread(() => Write("Thread 2")) { IsBackground = true }.Start();

            Thread.Sleep(TimeSpan.FromSeconds(30));
            */

            #endregion

            #region 2.10.2 SpinWait 用户模式使线程等待

            var t1 = new Thread(UserModeWait);
            var t2 = new Thread(HybridSpinWait);

            Console.WriteLine("Running user mode waiting");
            t1.Start();
            Thread.Sleep(20);
            _isCompleted = true;
            Thread.Sleep(TimeSpan.FromSeconds(1));
            _isCompleted = false;
            Console.WriteLine("Running hybrid SpinWait construct waiting");
            t2.Start();
            Thread.Sleep(5);
            _isCompleted = true;

            #endregion
        }

        static void UserModeWait()
        {
            while (!_isCompleted)
            {
                Console.WriteLine(".");
            }
            Console.WriteLine();
            Console.WriteLine("Waiting is complete");
        }

        static void HybridSpinWait()
        {
            var w = new SpinWait();
            while (!_isCompleted)
            {
                w.SpinOnce();
                Console.WriteLine(w.NextSpinWillYield);
            }
            Console.WriteLine("Waiting is complete");
        }

        static void Read()
        {
            Console.WriteLine("Reading contents of a dictionary");
            while (true)
            {
                try
                {
                    _rw.EnterReadLock();
                    foreach(var key in _items.Keys)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(0.1));
                    }
                }
                finally
                {
                    _rw.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// 当发现需要修改时用EnterWriteLock获取写锁写入数据
        /// </summary>
        /// <param name="threadName"></param>
        static void Write(string threadName)
        {
            while (true)
            {
                try
                {
                    int newKey = new Random().Next(250);
                    //读写锁
                    _rw.EnterUpgradeableReadLock();
                    if (!_items.ContainsKey(newKey))
                    {
                        try
                        {
                            _rw.EnterWriteLock();
                            _items[newKey] = 1;
                            Console.WriteLine("New key {0} is added to a dictionary by a {1}",newKey,threadName);
                        }
                        finally
                        {
                            _rw.ExitWriteLock();
                        }
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(0.1));
                }
                finally
                {
                    _rw.ExitUpgradeableReadLock();
                }
            }
        }

        static void PlayMusic(string name,string message,int seconds)
        {
            for(int i = 1; i < 3; i++)
            {
                Console.WriteLine("---------------------------------");
                Thread.Sleep(TimeSpan.FromSeconds(seconds));
                Console.WriteLine("{0} starts to {1}", name, message);
                Thread.Sleep(TimeSpan.FromSeconds(seconds));
                Console.WriteLine("{0} finishes to {1}", name, message);
                _barrier.SignalAndWait();
            }
        }

        static void PerformOperation(string message, int seconds)
        {
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
            Console.WriteLine(message);
            _countdown.Signal();
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
