using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TaskManager2
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a scheduler that uses two threads.
            
            LimitedTaskScheduler lcts = new LimitedTaskScheduler(3);
            List<Task> tasks = new List<Task>();

            // Create a TaskFactory and pass it our custom scheduler.
            TaskFactory factory = new TaskFactory(lcts);
            CancellationTokenSource cts = new CancellationTokenSource();

            // Use our factory to run a set of tasks.
            Object locker = new Object();

            for (int i = 1; i <= 5; i++)
            {
               Random random = new Random();
               int taskNumber = i;
               Task t = factory.StartNew(() => 
               {
             /*
             for (int i = 0; i < 2; i++)
             {
                lock (locker) {
                   Console.WriteLine("{0} in task t-{1} on thread {2}; ",
                      i, iteration, Thread.CurrentThread.ManagedThreadId);
                   
                }
             }
             */
             Console.WriteLine($"Задача {taskNumber} началась в потоке {Thread.CurrentThread.ManagedThreadId}");
             Thread.Sleep(random.Next(0,3000));
             Console.WriteLine($"Задача {taskNumber} выполняется в потоке {Thread.CurrentThread.ManagedThreadId}");
             Thread.Sleep(random.Next(0,3000));
             Console.WriteLine($"Задача {taskNumber} завершилась в потоке {Thread.CurrentThread.ManagedThreadId}");
             //Thread.Sleep(random.Next(0,2000));
               }, cts.Token);
               tasks.Add(t);
            }
      // Use it to run a second set of tasks.
      

            Task.WaitAll(tasks.ToArray());
            cts.Dispose();
            Console.WriteLine("\n\nSuccessful completion.");
        }
    }
}