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

       for (int tCtr = 1; tCtr <= 5; tCtr++) 
       {
          int iteration = tCtr;
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
             Console.WriteLine($"Задача {iteration} выполняется в потоке {Thread.CurrentThread.ManagedThreadId} ");
          }, cts.Token);
          tasks.Add(t);
      }
      // Use it to run a second set of tasks.
      /*
      for (int tCtr = 0; tCtr <= 4; tCtr++) 
      {
         int iteration = tCtr;
         Task t1 = factory.StartNew(() => 
         {
            for (int outer = 0; outer <= 3; outer++) 
            {
               for (int i = 0x21; i <= 0x25; i++) 
               {
                  lock (locker) 
                  {
                     Console.WriteLine("'{0}' in task t1-{1} on thread {2}   ",
                        Convert.ToChar(i), iteration, Thread.CurrentThread.ManagedThreadId);
                  }
               }
            }
         }, cts.Token);
         tasks.Add(t1);
      }*/

      // Wait for the tasks to complete before displaying a completion message.
      Task.WaitAll(tasks.ToArray());
      cts.Dispose();
      Console.WriteLine("\n\nSuccessful completion.");
        }
    }
}