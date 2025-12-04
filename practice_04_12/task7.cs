using System;
using System.Threading;
using System.Collections.Generic;

class Program
{
    static Mutex mutex = new Mutex();
    static List<string> list = new List<string>();

    static void Worker()
    {
        for (int i = 0; i < 5; i++)
        {
            string data = $"Поток {Thread.CurrentThread.ManagedThreadId} - запись {i}";
            
            mutex.WaitOne();
            list.Add(data);
            Console.WriteLine(data);
            mutex.ReleaseMutex();
            
            Thread.Sleep(10);
        }
    }

    static void Main()
    {
        for (int i = 0; i < 3; i++)
        {
            new Thread(Worker).Start();

        }
        
        Thread.Sleep(1000);
        Console.WriteLine($"Всего элементов: {list.Count}");
    }
}