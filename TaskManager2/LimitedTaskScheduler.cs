using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TaskManager2
{
    //Кастомный планировщик задач на основе стандартного
    public class LimitedTaskScheduler : TaskScheduler
    { 
        // Флаг занятости потока задачами true - выполняется задача, false - поток свободен.
        private static bool _ThreadIsWorking;

        // список задач ожидающих выполнение 
        private LinkedList<Task> _tasks = new LinkedList<Task>();

        // Максимальное количество потоков.
        private int _maxCountThreads;
        
        // Количество текущих задач
        private int _tasksNowCounter = 0;

   // Конструктор. Параметр = максимальное количество потоков.
   public LimitedTaskScheduler(int maxCountThreads)
   {
       if (maxCountThreads < 1) throw new ArgumentOutOfRangeException("maxCountThreads");
       _maxCountThreads = maxCountThreads;
   }

   // Добавление задач в очередь.
   protected sealed override void QueueTask(Task task)
   {
       lock (_tasks)    
       {
           _tasks.AddLast(task);
           if (_tasksNowCounter < _maxCountThreads)
           {
               ++_tasksNowCounter;
               NotifyThreadPoolOfPendingWork();
           }
       }
   }

   // Запуск на выполнение очереди задач.
   private void NotifyThreadPoolOfPendingWork()
   {
       ThreadPool.QueueUserWorkItem(_ =>
       {
           // текущий поток работает и выполняет задачу.
           _ThreadIsWorking = true;
           try
           {
               // цикл обработки всех доступных задач в очереди.
               while (true)
               {
                   Task item;
                   lock (_tasks)
                   {
                       // если задач в очереди нет - выход из цикла
                       if (_tasks.Count == 0)
                       {
                           --_tasksNowCounter;
                           break;
                       }

                       // получение следующей задачи из очереди 
                       item = _tasks.First.Value;
                       _tasks.RemoveFirst();
                   }

                   // запуск на выполнение задачи
                   base.TryExecuteTask(item);
               }
           }
           // Поток закончил работу и теперь он нерабочий 
           finally { _ThreadIsWorking = false; }
       }, null);
   }

   
   // Attempts to execute the specified task on the current thread.
   protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
   {
       /*
       // If this thread isn't already processing a task, we don't support inlining
       if (!_currentThreadIsProcessingItems) return false;

       // If the task was previously queued, remove it from the queue
       if (taskWasPreviouslyQueued)
          // Try to run the task.
          if (TryDequeue(task))
            return base.TryExecuteTask(task);
          else
             return false;
       else
          return base.TryExecuteTask(task);
       */
       return false;
   }

   // удаление задачи из очереди.
   protected sealed override bool TryDequeue(Task task)
   {
       lock (_tasks) return _tasks.Remove(task);
       return false;
   }

   // Gets the maximum concurrency level supported by this scheduler.
   public sealed override int MaximumConcurrencyLevel { get { return _maxCountThreads; } }

   // получение очереди задач в виде списка
   protected sealed override IEnumerable<Task> GetScheduledTasks()
   {
       
       bool lockTaken = false;
       try
       {
           Monitor.TryEnter(_tasks, ref lockTaken);
           if (lockTaken) return _tasks;
           else throw new NotSupportedException();
       }
       finally
       {
           if (lockTaken) Monitor.Exit(_tasks);
       }
       
   }
    }
}