using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace PLINQProject
{
    class Program
    {
        static void Main(string[] args)
        {
            #region 7.2 使用Parallel类

            /*
            Parallel.Invoke(() => EmulateProcessing("Task1"),
                () => EmulateProcessing("Task2"),
                () => EmulateProcessing("Task3")
                );

            var cts = new CancellationTokenSource();
            var result = Parallel.ForEach(Enumerable.Range(1, 30), new ParallelOptions
            {
                CancellationToken = cts.Token,
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                TaskScheduler = TaskScheduler.Default
            }, (i, state) =>
            {
                Console.WriteLine(i); ;
                if (i == 20)
                {
                    state.Break();
                    Console.WriteLine("Loop is stopped: {0}", state.IsStopped);
                }
            });

            Console.WriteLine("---");
            Console.WriteLine("IsCompleted: {0}", result.IsCompleted);
            Console.WriteLine("Lowest break iteration: {0}", result.LowestBreakIteration);
            Console.ReadLine();
            */

            #endregion

            #region 7.3 并行化LINQ查询

            /*
            //7s
            var sw = new Stopwatch();
            sw.Start();
            var query = from t in GetTypes()
                        select EmulateProcessing3(t);

            foreach(string typeName in query)
            {
                PrintInfo(typeName);
            }
            sw.Stop();
            Console.WriteLine("---");
            Console.WriteLine("Sequential LINQ query.");
            Console.WriteLine("Time elapsed: {0}", sw.Elapsed);
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
            Console.Clear();
            sw.Reset();

            //4s
            sw.Start();
            var parallelQuery = from t in ParallelEnumerable.AsParallel(GetTypes())
                                select EmulateProcessing3(t);

            foreach(string typeName in parallelQuery)
            {
                PrintInfo(typeName);
            }
            sw.Stop();
            Console.WriteLine("---");
            Console.WriteLine("Parallel LINQ query. The results are being merged on a single thread");
            Console.WriteLine("Time elapsed: {0}", sw.Elapsed);
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
            Console.Clear();
            sw.Reset();

            //1s
            sw.Start();
            parallelQuery = from t in GetTypes().AsParallel()
                            select EmulateProcessing3(t);

            parallelQuery.ForAll(PrintInfo);

            sw.Stop();
            Console.WriteLine("---");
            Console.WriteLine("Parallel LINQ query. The results are being processed in parallel");
            Console.WriteLine("Time elapsed: {0}", sw.Elapsed);
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
            Console.Clear();
            sw.Reset();

            //7s
            sw.Start();
            query = from t in GetTypes().AsParallel().AsSequential()
                    select EmulateProcessing3(t);

            foreach(var typeName in query)
            {
                PrintInfo(typeName);
            }

            sw.Stop();
            Console.WriteLine("---");
            Console.WriteLine("Parallel LINQ query, transformed into sequential.");
            Console.WriteLine("Time elapsed: {0}", sw.Elapsed);
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
            Console.Clear();
            */

            #endregion

            #region 7.4 调整PLINQ查询的参数

            /*
            var parallelQuery = from t in GetTypes4().AsParallel()
                                select EmulateProcessing4(t);

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(3));

            try
            {
                parallelQuery.WithDegreeOfParallelism(Environment.ProcessorCount).WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                    .WithMergeOptions(ParallelMergeOptions.Default).WithCancellation(cts.Token).ForAll(Console.WriteLine);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("---");
                Console.WriteLine("Operation has been canceled!");
            }

            Console.WriteLine("---");
            Console.WriteLine("Unordered PLINQ query execution");
            var unorderedQuery = from i in ParallelEnumerable.Range(1, 30) select i;

            foreach(var i in unorderedQuery)
            {
                Console.WriteLine(i);
            }

            Console.WriteLine("---");
            Console.WriteLine("Ordered PLINQ query execution");
            var orderedQuery = from i in ParallelEnumerable.Range(1, 30).AsOrdered() select i;

            foreach(var i in orderedQuery)
            {
                Console.WriteLine(i);
            }

            Console.ReadLine();
            */

            #endregion

            #region 7.5 处理PLINQ查询中的异常

            /*
            IEnumerable<int> numbers = Enumerable.Range(-5, 10);

            var query = from number in numbers
                        select 100 / number;

            try
            {
                foreach(var n in query)
                    Console.WriteLine(n);
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine("Divided by zero!");
            }

            Console.WriteLine("---");
            Console.WriteLine("Sequential LINQ query processing");
            Console.WriteLine();

            var parallelQuery = from number in numbers.AsParallel()
                                select 100 / number;

            try
            {
                parallelQuery.ForAll(Console.WriteLine);
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine("Divided by zero - usual exception handler!");
            }
            catch(AggregateException e)
            {
                e.Flatten().Handle(ex => 
                {
                    if(ex is DivideByZeroException)
                    {
                        Console.WriteLine("Divided by zero - aggregate exception handler!");
                        return true;
                    }
                    return false;
                });
            }

            Console.WriteLine("---");
            Console.WriteLine("Parallel LINQ query processing and results merging");

            Console.ReadLine();
            */

            #endregion

            #region 7.6 管理PLINQ查询中的数据分区

            /*
            //报错，分区错误
            var partitioner = new StringPartitioner(GetTypes6());
            var parallelQuery = from t in partitioner.AsParallel()
                                select EmulateProcessing6(t);

            parallelQuery.ForAll(PrintInfo6);

            Console.ReadLine();
            */

            #endregion

            #region 7.7 为PLINQ查询创建一个自定义的聚合器

            var parallelQuery = from t in GetTypes7().AsParallel()
                                select t;

            var parallelAggregator = parallelQuery.Aggregate(() => new ConcurrentDictionary<char, int>()
            , (taskTotal, item) => AccumulateLettersInformation(taskTotal, item)
            , (total, taskTotal) => MergeAccumulators(total, taskTotal)
            , total => total);

            Console.WriteLine();
            Console.WriteLine("There were the following letters in type names;");
            var orderedKeys = from k in parallelAggregator.Keys
                              orderby parallelAggregator[k] descending
                              select k;

            foreach(var c in orderedKeys)
            {
                Console.WriteLine("Letter '{0}' --- {1} times", c, parallelAggregator[c]);
            }

            Console.ReadLine();

            #endregion
        }

        #region 7.7 为PLINQ查询创建一个自定义的聚合器

        static ConcurrentDictionary<char, int> AccumulateLettersInformation(ConcurrentDictionary<char, int> taskTotal, string item)
        {
            foreach(var c in item)
            {
                if (taskTotal.ContainsKey(c))
                {
                    taskTotal[c] = taskTotal[c] + 1;
                }
                else
                {
                    taskTotal[c] = 1;
                }
            }
            Console.WriteLine("{0} types was aggregated on a thread id {1}", item, Thread.CurrentThread.ManagedThreadId);
            return taskTotal;
        }

        static ConcurrentDictionary<char, int>MergeAccumulators(ConcurrentDictionary<char, int> total, ConcurrentDictionary<char,int> taskTotal)
        {
            foreach(var key in taskTotal.Keys)
            {
                if (total.ContainsKey(key))
                {
                    total[key] = total[key] + taskTotal[key];
                }
                else
                {
                    total[key] = taskTotal[key];
                }
            }
            Console.WriteLine("---");
            Console.WriteLine("Total aggregate value was calculated on a thread id {0}", Thread.CurrentThread.ManagedThreadId);
            return total;
        }

        static IEnumerable<string> GetTypes7()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetExportedTypes());

            return from type in types
                   where type.Name.StartsWith("Web")
                   select type.Name;
        }

        #endregion

        #region 7.6 管理PLINQ查询中的数据分区

        static void PrintInfo6(string typeName)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(150));
            Console.WriteLine("{0} type was processed on a thread id {1}", typeName, Thread.CurrentThread.ManagedThreadId);
        }

        static string EmulateProcessing6(string typeName)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(150));
            Console.WriteLine("{0} type was processed on a thread id {1}. Has {2} length", typeName, Thread.CurrentThread.ManagedThreadId, typeName.Length % 2 == 0 ? "even" : "odd");
            return typeName;
        }

        static IEnumerable<string> GetTypes6()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetExportedTypes());

            return from type in types
                   where type.Name.StartsWith("Web")
                   select type.Name;
        }

        public class StringPartitioner: Partitioner<string>
        {
            private readonly IEnumerable<string> _data;

            public StringPartitioner(IEnumerable<string> data)
            {
                _data = data;
            }

            public override bool SupportsDynamicPartitions
            {
                get
                {
                    return false;
                }
            }

            public override IList<IEnumerator<string>> GetPartitions(int partitionCount)
            {
                var result = new List<IEnumerator<string>>(2);
                result.Add(CreateEnumerator(true));
                result.Add(CreateEnumerator(false));

                return result;
            }

            IEnumerator<string> CreateEnumerator(bool isEven)
            {
                foreach (var d in _data)
                {
                    if (!(d.Length % 2 == 0 ^ isEven))
                        yield return d;
                }
            }
        }
        

        #endregion

        #region 7.4 调整PLINQ查询的参数

        static string EmulateProcessing4(string typeName)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(new Random(DateTime.Now.Millisecond).Next(250, 350)));
            Console.WriteLine("{0} task was processed on a thread id {1}", typeName, Thread.CurrentThread.ManagedThreadId);
            return typeName;
        }

        static IEnumerable<string> GetTypes4()
        {
            return from assembly in AppDomain.CurrentDomain.GetAssemblies()
                   from type in assembly.GetExportedTypes()
                   where type.Name.StartsWith("Web")
                   orderby type.Name.Length
                   select type.Name;
        }

        #endregion

        #region 7.3 并行化LINQ查询

        static void PrintInfo(string typeName)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(150));
            Console.WriteLine("{0} type was processed on a thread id {1}", typeName, Thread.CurrentThread.ManagedThreadId);
        }

        static string EmulateProcessing3(string typeName)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(150));
            Console.WriteLine("{0} type was processed on a thread id {1}", typeName, Thread.CurrentThread.ManagedThreadId);
            return typeName;
        }

        static IEnumerable<string> GetTypes()
        {
            return from assembly in AppDomain.CurrentDomain.GetAssemblies()
                   from type in assembly.GetExportedTypes()
                   where type.Name.StartsWith("Web")
                   select type.Name;
        }

        #endregion

        #region 7.2 使用Parallel类

        static string EmulateProcessing(string taskName)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(new Random(DateTime.Now.Millisecond).Next(250, 350)));
            Console.WriteLine("{0} task was processed on a thread id {1}", taskName, Thread.CurrentThread.ManagedThreadId);
            return taskName;
        }

        #endregion
    }
}
