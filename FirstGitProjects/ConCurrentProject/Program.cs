using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace ConCurrentProject
{
    class Program
    {
        const string Item = "Dictionary item";
        public static string CurrentItem;
        static void Main(string[] args)
        {

            #region 6.2 使用ConcurrentDictionary            

            /*
             * ConcurrentDictionary写操作比使用锁的通常的字典要慢得多
             * ConcurrentDictionary的实现使用了细粒度锁(fine-grained locking)技术，这在多线程写入方面比使用锁的通常的字典（也称为粗粒度锁）的可伸缩性更好。
             * 当只用一个线程时，并发字典非常慢，但是扩展到5到6个线程（如果有足够的CPU核心来同时运行它们 ），并发字典的性能会更好。
             */
            /*
            var concurrentDictionary = new ConcurrentDictionary<int, string>();
            var dictionary = new Dictionary<int, string>();

            var sw = new Stopwatch();

            sw.Start();
            for(int i = 0; i < 1000000; i++)
            {
                lock(dictionary)
                {
                    dictionary[i] = Item;
                }
            }

            sw.Stop();
            Console.WriteLine("Writing to dictionary with a lock: {0}", sw.Elapsed);

            sw.Restart();
            for(int i = 0; i < 1000000; i++)
            {
                concurrentDictionary[i] = Item;
            }
            sw.Stop();
            Console.WriteLine("Writing to a concurrent dictionary: {0}", sw.Elapsed);

            sw.Restart();
            for(int i = 0; i < 1000000; i++)
            {
                lock (dictionary)
                {
                    CurrentItem = dictionary[i];
                }
            }
            sw.Stop();
            Console.WriteLine("Reading from dictionary with a lock: {0}", sw.Elapsed);

            sw.Restart();
            for(int i = 0; i < 1000000; i++)
            {
                CurrentItem = concurrentDictionary[i];
            }
            sw.Stop();
            Console.WriteLine("Reading from a concurrent dictionary: {0}", sw.Elapsed);

            Console.ReadLine();
            */

            #endregion

            #region 6.3 使用ConcurrentQueue实现异步处理

            /*
            Task t = RunProgram();
            t.Wait();

            Console.ReadLine();
            */

            #endregion

            #region 6.4 改变ConcurrentStack异步处理顺序

            /*
            Task t = RunProgram4();
            t.Wait();

            Console.ReadLine();
            */

            #endregion

            #region 6.5 使用ConcurrentBag 创建一个可扩展的爬虫

            /*
            CreateLinks();
            Task t = RunProgram5();
            t.Wait();

            Console.ReadLine();
            */

            #endregion

            #region 6.6 使用 BlockingCollection进行异步处理

            Console.WriteLine("Usint a Queue inside of BlockingCollection"); ;
            Console.WriteLine();
            Task t = RunProgram6();
            t.Wait();

            Console.WriteLine();
            Console.WriteLine("Using a Stack inside of BlockingCollection");
            Console.WriteLine();
            t = RunProgram6(new ConcurrentStack<CustomTask6>());
            t.Wait();

            Console.ReadLine();

            #endregion
        }

        #region 6.6 使用 BlockingCollection进行异步处理

        static async Task RunProgram6(IProducerConsumerCollection<CustomTask6> collection = null)
        {
            var taskCollection = new BlockingCollection<CustomTask6>();
            if (null != collection)
                taskCollection = new BlockingCollection<CustomTask6>(collection);

            var taskSource = Task.Run(() => TaskProducer6(taskCollection));

            Task[] processors = new Task[4];
            for(int i = 1; i <= 4; i++)
            {
                string processorId = "Processor " + i;
                processors[i - 1] = Task.Run(() => TaskProcessor6(taskCollection, processorId));
            }

            await taskSource;

            await Task.WhenAll(processors);
        }

        static async Task TaskProducer6(BlockingCollection<CustomTask6> collection)
        {
            for(int i = 1; i <= 20; i++)
            {
                await Task.Delay(20);
                var workItem = new CustomTask6 { Id = i };
                collection.Add(workItem);
                Console.WriteLine("Task {0} have been posted", workItem.Id);
            }
            collection.CompleteAdding();
        }

        static async Task TaskProcessor6(BlockingCollection<CustomTask6> collection, string name)
        {
            await GetRandomDelay6();
            foreach(CustomTask6 item in collection.GetConsumingEnumerable())
            {
                Console.WriteLine("Task {0} have been processed by {1}", item.Id, name);
                await GetRandomDelay6();
            }
        }

        static Task GetRandomDelay6()
        {
            int delay = new Random(DateTime.Now.Millisecond).Next(1, 500);
            return Task.Delay(delay);
        }

        class CustomTask6
        {
            public int Id { get; set; }
        }

        #endregion

        #region 6.5 使用ConcurrentBag 创建一个可扩展的爬虫

        static Dictionary<string, string[]> _contentEmulation = new Dictionary<string, string[]>();

        static async Task RunProgram5()
        {
            var bag = new ConcurrentBag<CrawlingTask>();

            string[] urls = new[] { "http://microsoft.com", "http://google.com", "http://facebook.com/", "http://twitter.com/" };

            var crawlers = new Task[4];
            for (int i = 1;i<= 4; i++)
            {
                string crawlerName = "Crawler " + i.ToString();
                bag.Add(new CrawlingTask { UrlToCrawl = urls[i - 1], ProducerName = "root" });
                crawlers[i - 1] = Task.Run(() => Crawl(bag, crawlerName));
            }

            await Task.WhenAll(crawlers);
        }

        static async Task Crawl(ConcurrentBag<CrawlingTask> bag, string crawlerName)
        {
            CrawlingTask task;
            while(bag.TryTake(out task))
            {
                IEnumerable<string> urls = await GetLinksFromContent(task);
                if (urls != null)
                {
                    foreach(var url in urls)
                    {
                        var t = new CrawlingTask
                        {
                            UrlToCrawl = url,
                            ProducerName = crawlerName
                        };

                        bag.Add(t);
                    }
                }
                Console.WriteLine("Indexing url {0} posted by {1} is completed by {2}!", task.UrlToCrawl, task.ProducerName, crawlerName);
            }                
        }

        static async Task<IEnumerable<string>> GetLinksFromContent(CrawlingTask task)
        {
            await GetRandomDelay5();

            if (_contentEmulation.ContainsKey(task.UrlToCrawl))
                return _contentEmulation[task.UrlToCrawl];

            return null;
        }

        static void CreateLinks()
        {
            _contentEmulation["http://microsoft.com/"] = new[] { "http://microsoft.com/a.html", "http://microsoft.com/b.html" };
            _contentEmulation["http://microsoft.com/a.html"] = new[] { "http://microsoft.com/c.html", "http://microsoft.com/d.html" };
            _contentEmulation["http://microsoft.com/b.html"] = new[] { "http://microsoft.com/e.html" };
            _contentEmulation["http://google.com"] = new[] { "http://google.com/a.html", "http://google.com/b.html" };
            _contentEmulation["http://google.com/a.html"] = new[] { "http://google.com/c.html", "http://google.com/d.html" };
            _contentEmulation["http://google.com/b.html"] = new[] { "http://google.com/e.html", "http://google.com/f.html" };
            _contentEmulation["http://google.com/c.html"] = new[] { "http://google.com/h.html", "http://google.com/i.html" };

            _contentEmulation["http://facebook.com"] = new[] { "http://facebook.com/a.html", "http://facebook.com/b.html" };
            _contentEmulation["http://facebook.com/a.html"] = new[] { "http://facebook.com/c.html", "http://facebook.com/d.html" };
            _contentEmulation["http://facebook.com/b.html"] = new[] { "http://facebook.com/e.html" };
            _contentEmulation["http://twitter.com"] = new[] { "http://twitter.com/a.html", "http://twitter.com/b.html" };
            _contentEmulation["http://twitter.com/a.html"] = new[] { "http://twitter.com/c.html", "http://twitter.com/d.html" };
            _contentEmulation["http://twitter.com/b.html"] = new[] { "http://twitter.com/e.html" };
            _contentEmulation["http://twitter.com/c.html"] = new[] { "http://twitter.com/f.html", "http://twitter.com/g.html" };
            _contentEmulation["http://twitter.com/d.html"] = new[] { "http://twitter.com/h.html" };
            _contentEmulation["http://twitter.com/e.html"] = new[] { "http://twitter.com/i.html" };
        }

        static Task GetRandomDelay5()
        {
            int delay = new Random(DateTime.Now.Millisecond).Next(150, 200);
            return Task.Delay(delay);
        }

        class CrawlingTask
        {
            public string UrlToCrawl { get; set; }

            public string ProducerName { get; set; }
        }

        #endregion

        #region 6.4 改变ConcurrentStack异步处理顺序

        static async Task RunProgram4()
        {
            var taskStack = new ConcurrentStack<CustomTask4>();
            var cts = new CancellationTokenSource();

            var taskSource = Task.Run(() => TaskProducer4(taskStack));

            Task[] processors = new Task[4];
            for(int i = 1; i <= 4; i++)
            {
                string processorId = i.ToString();
                processors[i - 1] = Task.Run(() => TaskProcessor4(taskStack, "Processor " + processorId, cts.Token));
            }

            await taskSource;
            cts.CancelAfter(TimeSpan.FromSeconds(2));

            await Task.WhenAll(processors);
        }

        static async Task TaskProducer4(ConcurrentStack<CustomTask4> stack)
        {
            for(int i = 1; i <= 20; i++)
            {
                await Task.Delay(50);
                var workItem = new CustomTask4 { Id = i };
                stack.Push(workItem);
                Console.WriteLine("Task {0} has been posted", workItem.Id);
            }
        }

        static async Task TaskProcessor4(ConcurrentStack<CustomTask4> stack, string name, CancellationToken token)
        {
            await GetRandomDelay4();
            do
            {
                CustomTask4 workItem;
                bool popSuccesful = stack.TryPop(out workItem);
                if (popSuccesful)
                {
                    Console.WriteLine("Task {0} has been processed by {1}", workItem.Id, name);
                }

                await GetRandomDelay4();
            }
            while (!token.IsCancellationRequested);
        }

        static Task GetRandomDelay4()
        {
            int delay = new Random(DateTime.Now.Millisecond).Next(1, 500);
            return Task.Delay(delay);
        }

        class CustomTask4
        {
            public int Id { get; set; }
        }

        #endregion

        #region 6.3 使用ConcurrentQueue实现异步处理

        static async Task RunProgram()
        {
            var taskQueue = new ConcurrentQueue<CustomTask>();
            var cts = new CancellationTokenSource();

            var taskSource = Task.Run(() => TaskProducer(taskQueue));

            Task[] processors = new Task[4];
            for(int i = 1; i <= 4; i++)
            {
                string processorId = i.ToString();
                processors[i - 1] = Task.Run(() => TaskProcessor(taskQueue, "Processor " + processorId, cts.Token));
            }

            await taskSource;
            cts.CancelAfter(TimeSpan.FromSeconds(2));

            await Task.WhenAll(processors);
        }

        static async Task TaskProducer(ConcurrentQueue<CustomTask> queue)
        {
            for(int i = 1; i <= 20; i++)
            {
                await Task.Delay(50);
                var workItem = new CustomTask { Id = i };
                queue.Enqueue(workItem);
                Console.WriteLine("Task {0} has been posted", workItem.Id);
            }
        }

        static async Task TaskProcessor(ConcurrentQueue<CustomTask> queue, string name, CancellationToken token)
        {
            CustomTask workItem;
            bool dequeueSuccessful = false;

            await GetRandomDelay();
            do
            {
                dequeueSuccessful = queue.TryDequeue(out workItem);
                if (dequeueSuccessful)
                {
                    Console.WriteLine("Task {0} has been processed by {1}", workItem.Id, name);
                }

                await GetRandomDelay();
            }
            while (!token.IsCancellationRequested);
        }

        static Task GetRandomDelay()
        {
            int delay = new Random(DateTime.Now.Millisecond).Next(1, 500);
            return Task.Delay(delay);
        }

        class CustomTask
        {
            public int Id { get; set; }
        }

        #endregion
    }
}
