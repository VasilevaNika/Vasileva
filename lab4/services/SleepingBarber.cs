using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace lab4
{
    public class SleepingBarber
    {
        private readonly int maxQueueSize;
        private readonly ConcurrentQueue<Client> waitingRoom;
        private readonly SemaphoreSlim clientReady;
        private readonly Mutex queueMutex;
        private bool isBarberWorking;
        private int clientsServed;

        public SleepingBarber(int maxQueueSize = 5)
        {
            this.maxQueueSize = maxQueueSize;
            waitingRoom = new ConcurrentQueue<Client>();
            clientReady = new SemaphoreSlim(0, maxQueueSize);
            queueMutex = new Mutex();
            isBarberWorking = true;
            clientsServed = 0;
        }

        public void Run(int numClients = 10)
        {
            Console.WriteLine($"Очередь: {maxQueueSize}, Клиентов: {numClients}");

            Task barberTask = Task.Run(() => Barber());

            Task[] clientTasks = new Task[numClients];
            for (int i = 0; i < numClients; i++)
            {
                int clientId = i;
                clientTasks[i] = Task.Run(() => ClientArrives(clientId));
                Thread.Sleep(Random.Shared.Next(200, 800));
            }

            Task.WaitAll(clientTasks);
            Console.WriteLine("Все клиенты пришли");

            Thread.Sleep(5000);
            isBarberWorking = false;
            clientReady.Release();

            barberTask.Wait();
            Console.WriteLine($"Обслужено клиентов: {clientsServed}");
        }

        private void Barber()
        {
            Console.WriteLine("Парикмахер открыл салон");

            while (isBarberWorking || waitingRoom.Count > 0)
            {
                if (waitingRoom.Count == 0)
                {
                    Console.WriteLine("Парикмахер спит");
                }

                clientReady.Wait();

                if (waitingRoom.TryDequeue(out Client? client))
                {
                    Console.WriteLine($"Парикмахер работает с клиентом {client.Id}");
                    Thread.Sleep(Random.Shared.Next(1000, 2000));
                    Console.WriteLine($"Парикмахер закончил стрижку клиента {client.Id}");
                    clientsServed++;
                    client.CutCompleted.Set();
                }
            }

            Console.WriteLine("Парикмахер закрыл салон");
        }

        private void ClientArrives(int clientId)
        {
            Console.WriteLine($"Клиент {clientId} пришел");

            Client client = new Client(clientId);
            bool addedToQueue = false;

            queueMutex.WaitOne();
            try
            {
                if (waitingRoom.Count < maxQueueSize)
                {
                    waitingRoom.Enqueue(client);
                    addedToQueue = true;
                    Console.WriteLine($"Клиент {clientId} в очереди (всего: {waitingRoom.Count})");
                    clientReady.Release();
                }
                else
                {
                    Console.WriteLine($"Клиент {clientId} ушел - очередь полная");
                    return;
                }
            }
            finally
            {
                queueMutex.ReleaseMutex();
            }

            if (addedToQueue)
            {
                client.CutCompleted.Wait();
                Console.WriteLine($"Клиент {clientId} ушел");
            }
        }

        private class Client
        {
            public int Id { get; }
            public ManualResetEventSlim CutCompleted { get; }

            public Client(int id)
            {
                Id = id;
                CutCompleted = new ManualResetEventSlim(false);
            }
        }
    }
}

