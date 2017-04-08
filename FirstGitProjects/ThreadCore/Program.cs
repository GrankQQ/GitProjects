using System;
using System.Threading;
using System.Diagnostics;

namespace ThreadCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start program...");
            Thread t = new Thread(PrintNumbersWithDelay);
            Thread t2 = new Thread(DoNothing);
            Console.WriteLine(t.ThreadState.ToString());
            t2.Start();
            t.Start();
            for(int i = 1; i < 30; i++)
            {
                Console.WriteLine(t.ThreadState);
            }
            Thread.Sleep(TimeSpan.FromSeconds(6));

            Console.WriteLine(t.ThreadState.ToString());
            Console.WriteLine(t2.ThreadState);
            //t.Join();//wait t
            //PrintNumbers();
            Console.ReadLine();
        }

        static void DoNothing()
        {
            Thread.Sleep(TimeSpan.FromSeconds(2));
        }

        static void PrintNumbers()
        {
            Console.WriteLine("Starting....");
            for(int i = 1; i < 10; i++)

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
                
                //Console.WriteLine("{0} with {1,11} priority has a count = {2,13}",Thread.CurrentThread.Name, Thread.CurrentThread.Priority, counter.ToString("N0"));
            }
        }
    }
}