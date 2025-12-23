using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace lab4
{
    public class ProducerConsumer
    {
        private readonly int bufferSize;
        private readonly BlockingCollection<int> buffer;
        private int itemsProduced;
        private int itemsConsumed;

        public ProducerConsumer(int bufferSize = 5)
        {
            this.bufferSize = bufferSize;
            buffer = new BlockingCollection<int>(bufferSize);
            itemsProduced = 0;
            itemsConsumed = 0;
        }

        public void RunWithBlockingCollection(int numProducers = 3, int numConsumers = 2, int itemsPerProducer = 5)
        {
            Console.WriteLine($"Буфер: {bufferSize}, Производителей: {numProducers}, Потребителей: {numConsumers}");

            CancellationTokenSource cts = new CancellationTokenSource();
            itemsProduced = 0;
            itemsConsumed = 0;

            Task[] producers = new Task[numProducers];
            for (int i = 0; i < numProducers; i++)
            {
                int producerId = i;
                producers[i] = Task.Run(() => ProducerWithBlockingCollection(producerId, itemsPerProducer, cts.Token));
            }

            Task[] consumers = new Task[numConsumers];
            for (int i = 0; i < numConsumers; i++)
            {
                int consumerId = i;
                consumers[i] = Task.Run(() => ConsumerWithBlockingCollection(consumerId, cts.Token));
            }

            Task.WaitAll(producers);
            Console.WriteLine("Производители закончили");

            buffer.CompleteAdding();

            Task.WaitAll(consumers);
            Console.WriteLine("Потребители закончили");

            Console.WriteLine($"Произведено: {itemsProduced}, Потреблено: {itemsConsumed}");
        }

        private void ProducerWithBlockingCollection(int producerId, int itemsToProduce, CancellationToken cancellationToken)
        {
            for (int i = 0; i < itemsToProduce && !cancellationToken.IsCancellationRequested; i++)
            {
                int item = Random.Shared.Next(1, 100);
                Thread.Sleep(Random.Shared.Next(300, 800));

                buffer.Add(item, cancellationToken);
                Interlocked.Increment(ref itemsProduced);
                Console.WriteLine($"Производитель {producerId} создал {item} (в буфере: {buffer.Count})");
            }
            Console.WriteLine($"Производитель {producerId} закончил");
        }

        private void ConsumerWithBlockingCollection(int consumerId, CancellationToken cancellationToken)
        {
            try
            {
                foreach (int item in buffer.GetConsumingEnumerable(cancellationToken))
                {
                    Thread.Sleep(Random.Shared.Next(400, 1000));
                    Interlocked.Increment(ref itemsConsumed);
                    Console.WriteLine($"Потребитель {consumerId} взял {item} (в буфере: {buffer.Count})");
                }
            }
            catch (OperationCanceledException)
            {
            }
            Console.WriteLine($"Потребитель {consumerId} закончил");
        }

        public void RunWithSemaphore(int numProducers = 3, int numConsumers = 2, int itemsPerProducer = 5)
        {
            Console.WriteLine($"Буфер: {bufferSize}, Производителей: {numProducers}, Потребителей: {numConsumers}");

            Queue<int> manualBuffer = new Queue<int>();
            object bufferLock = new object();
            SemaphoreSlim emptySlots = new SemaphoreSlim(bufferSize, bufferSize);
            SemaphoreSlim filledSlots = new SemaphoreSlim(0, bufferSize);
            bool isProductionComplete = false;
            itemsProduced = 0;
            itemsConsumed = 0;

            CancellationTokenSource cts = new CancellationTokenSource();

            Task[] producers = new Task[numProducers];
            for (int i = 0; i < numProducers; i++)
            {
                int producerId = i;
                producers[i] = Task.Run(() => ProducerWithSemaphore(producerId, itemsPerProducer, manualBuffer, bufferLock, emptySlots, filledSlots, cts.Token));
            }

            Task[] consumers = new Task[numConsumers];
            for (int i = 0; i < numConsumers; i++)
            {
                int consumerId = i;
                consumers[i] = Task.Run(() => ConsumerWithSemaphore(consumerId, manualBuffer, bufferLock, emptySlots, filledSlots, () => 
                {
                    lock (bufferLock) { return isProductionComplete; }
                }, cts.Token));
            }

            Task.WaitAll(producers);
            Console.WriteLine("Производители закончили");

            lock (bufferLock)
            {
                isProductionComplete = true;
            }
            for (int i = 0; i < numConsumers; i++)
            {
                filledSlots.Release();
            }

            Task.WaitAll(consumers);
            Console.WriteLine("Потребители закончили");

            Console.WriteLine($"Произведено: {itemsProduced}, Потреблено: {itemsConsumed}");
        }

        private void ProducerWithSemaphore(int producerId, int itemsToProduce, Queue<int> buffer, object bufferLock, SemaphoreSlim emptySlots, SemaphoreSlim filledSlots, CancellationToken cancellationToken)
        {
            for (int i = 0; i < itemsToProduce && !cancellationToken.IsCancellationRequested; i++)
            {
                int item = Random.Shared.Next(1, 100);
                Thread.Sleep(Random.Shared.Next(300, 800));

                emptySlots.Wait(cancellationToken);

                lock (bufferLock)
                {
                    buffer.Enqueue(item);
                    Interlocked.Increment(ref itemsProduced);
                    Console.WriteLine($"Производитель {producerId} создал {item} (в буфере: {buffer.Count})");
                }

                filledSlots.Release();
            }
            Console.WriteLine($"Производитель {producerId} закончил");
        }

        private void ConsumerWithSemaphore(int consumerId, Queue<int> buffer, object bufferLock, SemaphoreSlim emptySlots, SemaphoreSlim filledSlots, Func<bool> isProductionComplete, CancellationToken cancellationToken)
        {
            while (true)
            {
                if (!filledSlots.Wait(100, cancellationToken))
                {
                    lock (bufferLock)
                    {
                        if (isProductionComplete() && buffer.Count == 0)
                        {
                            break;
                        }
                    }
                    continue;
                }

                int item = 0;
                bool shouldBreak = false;
                bool itemDequeued = false;
                lock (bufferLock)
                {
                    if (buffer.Count == 0)
                    {
                        if (isProductionComplete())
                        {
                            shouldBreak = true;
                        }
                        else
                        {
                            filledSlots.Release();
                        }
                    }
                    else
                    {
                        item = buffer.Dequeue();
                        itemDequeued = true;
                    }
                }

                if (shouldBreak)
                {
                    break;
                }

                if (!itemDequeued)
                {
                    continue;
                }

                Thread.Sleep(Random.Shared.Next(400, 1000));
                Interlocked.Increment(ref itemsConsumed);
                Console.WriteLine($"Потребитель {consumerId} взял {item} (в буфере: {buffer.Count})");

                emptySlots.Release();
            }
            Console.WriteLine($"Потребитель {consumerId} закончил");
        }
    }
}

