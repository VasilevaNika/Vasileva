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
            
            int delay = random.Next(100, 5001);
            Thread.Sleep(delay);
        }
    }

    static void Main()
    {
        for (int i = 0; i < 3; i++)
        {
            new Thread(Worker).Start();
        }

        int randomDelay = random.Next(100, 5001);
        Thread.Sleep(randomDelay);
        
        Console.WriteLine($"Всего элементов: {list.Count}");
    }
}