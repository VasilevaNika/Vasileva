using System;
using System.Threading;

namespace lab4
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Лабораторная работа 4");
            Console.WriteLine("1. Обедающие философы");
            Console.WriteLine("2. Спящий парикмахер");
            Console.WriteLine("3. Производитель-Потребитель");
            Console.WriteLine("4. Все задачи");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    RunDiningPhilosophers();
                    break;
                case "2":
                    RunSleepingBarber();
                    break;
                case "3":
                    RunProducerConsumer();
                    break;
                case "4":
                    RunAll();
                    break;
                default:
                    RunAll();
                    break;
            }

            Console.ReadKey();
        }

        static void RunDiningPhilosophers()
        {
            DiningPhilosophers dp = new DiningPhilosophers();
            dp.RunWithoutDeadlock();
        }

        static void RunSleepingBarber()
        {
            SleepingBarber sb = new SleepingBarber(maxQueueSize: 5);
            sb.Run(numClients: 10);
        }

        static void RunProducerConsumer()
        {
            ProducerConsumer pc = new ProducerConsumer(bufferSize: 5);
            pc.RunWithBlockingCollection(numProducers: 3, numConsumers: 2, itemsPerProducer: 5);
            Thread.Sleep(2000);
            pc.RunWithSemaphore(numProducers: 3, numConsumers: 2, itemsPerProducer: 5);
        }

        static void RunAll()
        {
            Console.WriteLine("Задача 1: Обедающие философы");
            RunDiningPhilosophers();

            Thread.Sleep(2000);

            Console.WriteLine("Задача 2: Спящий парикмахер");
            RunSleepingBarber();

            Thread.Sleep(2000);

            Console.WriteLine("Задача 3: Производитель-Потребитель");
            ProducerConsumer pc = new ProducerConsumer(bufferSize: 5);
            pc.RunWithBlockingCollection(numProducers: 3, numConsumers: 2, itemsPerProducer: 5);
        }
    }
}
