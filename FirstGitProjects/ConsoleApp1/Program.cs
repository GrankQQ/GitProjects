using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            #region 

            /*
            Console.WriteLine("Start program...");
            Thread t = new Thread(PrintNumbersWithDelay);
            Thread t2 = new Thread(DoNothing);
            Console.WriteLine(t.ThreadState.ToString());
            t2.Start();
            t.Start();
            for (int i = 1; i < 30; i++)
            {
                Console.WriteLine(t.ThreadState);
            }
            Thread.Sleep(TimeSpan.FromSeconds(6));

            Console.WriteLine(t.ThreadState.ToString());
            Console.WriteLine(t2.ThreadState);
            //t.Join();//wait t
            //PrintNumbers();
            Console.ReadLine();
            */

            #endregion

            #region ThreadPriority 1.8.2
            /*
            Console.WriteLine("Current thread priority:{0}", Thread.CurrentThread.Priority);
            Console.WriteLine("Running on all cores available");
            RunThreads();
            Thread.Sleep(TimeSpan.FromSeconds(2));
            Console.WriteLine("Running on a single core");
            Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(1);
            RunThreads();
            Console.ReadLine();
            */

            #endregion

            #region Back thread and front thread

            /*
            var sampleForeground = new ThreadSample2(10);
            var sampleBackground = new ThreadSample2(20);

            var threadOne = new Thread(sampleForeground.CountNumbers);
            threadOne.Name = "ForegroundThread";
            var threadTow = new Thread(sampleBackground.CountNumbers);
            threadTow.Name = "BackgroundThread";
            threadTow.IsBackground = true;

            threadOne.Start();
            threadTow.Start();
            */

            #endregion

            #region 1.9.2

            /*
            var sample = new ThreadSample3(10);

            var threadOne = new Thread(sample.CountNumbers);
            threadOne.Name = "ThreadOne";
            threadOne.Start();
            threadOne.Join();
            Console.WriteLine("----------------");

            var threadTwo = new Thread(Count);
            threadTwo.Name = "ThreadTwo";
            threadTwo.Start(8);
            threadTwo.Join();
            Console.WriteLine("----------------");

            var threadThree = new Thread(() => CountNumbers(12));
            threadThree.Name = "ThreadThree";
            threadThree.Start();
            threadThree.Join();
            Console.WriteLine("----------------");

            int i = 10;
            var threadFour = new Thread(() => PrintNumber(i));
            i = 20;
            var threadFive = new Thread(() => PrintNumber(i));
            threadFour.Start();
            threadFive.Start();

            Console.ReadLine();
            */

            #endregion

            #region 1.10.1

            /*
            Console.WriteLine("Incorrect counter");

            var c = new Counter();

            var t1 = new Thread(() =>TestCounter(c));
            var t2 = new Thread(() => TestCounter(c));
            var t3 = new Thread(() => TestCounter(c));
            t1.Start();
            t2.Start();
            t3.Start();
            t1.Join();
            t2.Join();
            t3.Join();

            Console.WriteLine("Total count: {0}", c.Count);
            Console.WriteLine("--------------------------");
            Console.WriteLine("Correct counter");

            var c1 = new CounterWithLock();
            t1 = new Thread(() => TestCounter(c1));
            t2 = new Thread(() => TestCounter(c1));
            t3 = new Thread(() => TestCounter(c1));
            t1.Start();
            t2.Start();
            t3.Start();
            t1.Join();
            t2.Join();
            t3.Join();
            Console.WriteLine("Total count: {0}", c1.Count);

            Console.ReadLine();
            */

            #endregion

            #region 1.11.1

            /*
            object lock1 = new object();
            object lock2 = new object();

            new Thread(() => LockTooMuch(lock1, lock2)).Start();

            lock (lock2)
            {
                Thread.Sleep(1000);
                Console.WriteLine(("Monitor.TryEnter allows not to get stuck, returning false after a specified timeout is elapsed"));
                if (Monitor.TryEnter(lock1, TimeSpan.FromSeconds(5)))
                {
                    Console.WriteLine("Acquired a protected resource succesfully");
                }
                else
                {
                    Console.WriteLine("Timeout acquiring a resource!");
                }
            }
            new Thread(() => LockTooMuch(lock1, lock2)).Start();

            Console.WriteLine("-----------------------");
            lock (lock2)
            {
                Console.WriteLine("This will be a deadlock!");
                Thread.Sleep(1000);
                lock (lock1)
                {
                    Console.WriteLine("Acquired a protected resource succesfully");
                }
            }
            */

            #endregion

            #region 1.12.1

            var t = new Thread(FaultyThread);
            t.Start();
            t.Join();

            try
            {
                t = new Thread(BadFaultyThread);
                t.Start();
            }
            catch(Exception ex)
            {
                Console.WriteLine("We won't get here!");
            }
            Console.ReadLine();

            #endregion

        }

        static void BadFaultyThread()
        {
            Console.WriteLine("Starting a faulty thread...");
            Thread.Sleep(TimeSpan.FromSeconds(2));
            throw new Exception("Boom!");
        }

        static void FaultyThread()
        {
            try
            {
                Console.WriteLine("Starting a faulty thread...");
                Thread.Sleep(TimeSpan.FromSeconds(1));
                throw new Exception("Boom!");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception handled: {0}",ex.Message);
            }
        }

        static void LockTooMuch(object lock1, object lock2)
        {
            lock (lock1)
            {
                Thread.Sleep(1000);
                lock (lock2) ;
            }
        }

        static void TestCounter(CounterBase c)
        {
            for(int i = 0; i < 100000; i++)
            {
                c.Increment();
                c.Decrement();
            }
        }

        class Counter: CounterBase
        {
            public int Count { get; private set; }
            public override void Increment()
            {
                Count++;
            }

            public override void Decrement()
            {
                Count--;
            }
        }

        class CounterWithLock : CounterBase
        {
            private readonly object _syncRoot = new object();

            public int Count { get; private set; }

            public override void Increment()
            {
                lock (_syncRoot)
                {
                    Count++;
                }
            }

            public override void Decrement()
            {
                lock (_syncRoot)
                {
                    Count--;
                }
            }
        }

        abstract class CounterBase
        {
            public abstract void Increment();
            public abstract void Decrement();
        }

        static void Count(object iterations)
        {
            CountNumbers((int)iterations);
        }

        static void CountNumbers(int iterations)
        {
            for(int i = 1; i <= iterations; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
                Console.WriteLine("{0} prints {1}", Thread.CurrentThread.Name, i);
            }
        }

        static void PrintNumber(int number)
        {
            Console.WriteLine(number);
        }

        class ThreadSample3
        {
            private readonly int _iterations;

            public ThreadSample3(int iterations)
            {
                _iterations = iterations;
            }
            public void CountNumbers()
            {
                for(int i = 1; i < _iterations; i++)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(0.5));
                    Console.WriteLine("{0} prints {1}", Thread.CurrentThread.Name, i);
                }
            }
        }

        class ThreadSample2
        {
            private readonly int _iterations;

            public ThreadSample2(int iterations)
            {
                _iterations = iterations;
            }
            public void CountNumbers()
            {
                for(int i = 0; i < _iterations; i++)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(0.5));
                    Console.WriteLine("{0} prints {1}",Thread.CurrentThread.Name, i);
                }
            }

        }

        static void RunThreads()
        {
            var sample = new ThreadSample();

            var threadOne = new Thread(sample.CountNumbers);
            threadOne.Name = "ThreadOne";
            var threadTwo = new Thread(sample.CountNumbers);
            threadTwo.Name = "ThreadTwo";

            threadOne.Priority = ThreadPriority.Highest;
            threadTwo.Priority = ThreadPriority.Lowest;
            threadOne.Start();
            threadTwo.Start();
            Thread.Sleep(TimeSpan.FromSeconds(2));
            sample.Stop();
        }

        static void DoNothing()
        {
            Thread.Sleep(TimeSpan.FromSeconds(2));
        }

        static void PrintNumbers()
        {
            Console.WriteLine("Starting....");
            for (int i = 1; i < 10; i++)

            {
                Console.WriteLine(i);
            }
        }

        static void PrintNumbersWithDelay()
        {
            Console.WriteLine("Starting....");
            Console.WriteLine(Thread.CurrentThread.ThreadState);
            for (int i = 1; i < 10; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));//thread sleep
                Console.WriteLine(i);
            }
        }

        class ThreadSample
        {
            private bool _isStopped = false;
            public void Stop()
            {
                _isStopped = true;
            }

            public void CountNumbers()
            {
                long counter = 0;

                while (!_isStopped)
                {
                    counter++;
                }

                Console.WriteLine("{0} with {1,11} priority has a count = {2,13}", Thread.CurrentThread.Name, Thread.CurrentThread.Priority, counter.ToString("N0"));
            }
        }
    }
}
