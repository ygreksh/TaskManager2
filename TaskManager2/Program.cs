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
            // Кастомный планировщик с количеством потоков = 3.
            
            LimitedTaskScheduler lcts = new LimitedTaskScheduler(3);
            List<Task> tasks = new List<Task>();

            // Фабрика с кастомным планировщиком
            TaskFactory factory = new TaskFactory(lcts);
            CancellationTokenSource cts = new CancellationTokenSource();


            for (int i = 1; i <= 5; i++)
            {
               Random random = new Random();
               int taskNumber = i;
               Task t = factory.StartNew(() => 
               {
                Console.WriteLine($"Задача {taskNumber} началась в потоке {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(random.Next(0,3000));
                Console.WriteLine($"Задача {taskNumber} выполняется в потоке {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(random.Next(0,3000));
                Console.WriteLine($"Задача {taskNumber} завершилась в потоке {Thread.CurrentThread.ManagedThreadId}");
                //Thread.Sleep(random.Next(0,2000));
               }, cts.Token);
               tasks.Add(t);
            }
            //Подождать и завершить Main только после завершения всех задач
            Task.WaitAll(tasks.ToArray());
            cts.Dispose();
            Console.WriteLine("\n\nSuccessful completion.");
        }
    }
}