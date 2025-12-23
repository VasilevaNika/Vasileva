using System;
using System.Threading;
using System.Threading.Tasks;

namespace lab4
{
    public class DiningPhilosophers
    {
        private const int NUM_PHILOSOPHERS = 5;
        private object[] forks;
        private SemaphoreSlim[] forksSemaphore;

        public DiningPhilosophers()
        {
            forks = new object[NUM_PHILOSOPHERS];
            forksSemaphore = new SemaphoreSlim[NUM_PHILOSOPHERS];
            
            for (int i = 0; i < NUM_PHILOSOPHERS; i++)
            {
                forks[i] = new object();
                forksSemaphore[i] = new SemaphoreSlim(1, 1);
            }
        }

        public void RunWithDeadlock()
        {
            Console.WriteLine("Версия с deadlock");
            Task[] philosophers = new Task[NUM_PHILOSOPHERS];

            for (int i = 0; i < NUM_PHILOSOPHERS; i++)
            {
                int philosopherId = i;
                philosophers[i] = Task.Run(() => PhilosopherWithDeadlock(philosopherId));
            }

            Thread.Sleep(5000);
            Console.WriteLine("Deadlock произошел");
        }

        private void PhilosopherWithDeadlock(int id)
        {
            int leftFork = id;
            int rightFork = (id + 1) % NUM_PHILOSOPHERS;

            while (true)
            {
                Think(id);

                lock (forks[leftFork])
                {
                    Console.WriteLine($"Философ {id} взял левую вилку {leftFork}");
                    Thread.Sleep(100);

                    lock (forks[rightFork])
                    {
                        Console.WriteLine($"Философ {id} взял правую вилку {rightFork}");
                        Eat(id);
                    }
                }
            }
        }

        public void RunWithoutDeadlock()
        {
            Console.WriteLine("Версия без deadlock");
            CancellationTokenSource cts = new CancellationTokenSource();
            Task[] philosophers = new Task[NUM_PHILOSOPHERS];

            for (int i = 0; i < NUM_PHILOSOPHERS; i++)
            {
                int philosopherId = i;
                philosophers[i] = Task.Run(() => PhilosopherWithoutDeadlock(philosopherId, cts.Token));
            }

            Thread.Sleep(10000);
            cts.Cancel();

            Task.WaitAll(philosophers);
            Console.WriteLine("Все философы закончили");
        }

        private void PhilosopherWithoutDeadlock(int id, CancellationToken cancellationToken)
        {
            int leftFork = id;
            int rightFork = (id + 1) % NUM_PHILOSOPHERS;

            if (id == 0)
            {
                leftFork = (id + 1) % NUM_PHILOSOPHERS;
                rightFork = id;
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                Think(id);

                forksSemaphore[leftFork].Wait();
                Console.WriteLine($"Философ {id} взял левую вилку {leftFork}");
                
                try
                {
                    forksSemaphore[rightFork].Wait();
                    Console.WriteLine($"Философ {id} взял правую вилку {rightFork}");
                    Eat(id);
                }
                finally
                {
                    forksSemaphore[rightFork].Release();
                    Console.WriteLine($"Философ {id} положил правую вилку {rightFork}");
                }
                
                forksSemaphore[leftFork].Release();
                Console.WriteLine($"Философ {id} положил левую вилку {leftFork}");
            }
        }

        private void Think(int id)
        {
            Console.WriteLine($"Философ {id} думает");
            Thread.Sleep(Random.Shared.Next(500, 1500));
        }

        private void Eat(int id)
        {
            Console.WriteLine($"Философ {id} ест");
            Thread.Sleep(Random.Shared.Next(500, 1500));
        }
    }
}

