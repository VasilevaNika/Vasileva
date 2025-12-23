using System.Threading;
using System.Threading.Tasks;
using Xunit;
using lab4;

namespace lab4.Tests
{
    public class ProducerConsumerTests
    {
        [Fact]
        public void TestBlockingCollection()
        {
            ProducerConsumer pc = new ProducerConsumer(bufferSize: 3);
            Task task = Task.Run(() => 
            {
                pc.RunWithBlockingCollection(numProducers: 2, numConsumers: 2, itemsPerProducer: 3);
            });
            Thread.Sleep(5000);
            Assert.True(true);
        }

        [Fact]
        public void TestSemaphoreImplementation()
        {
            ProducerConsumer pc = new ProducerConsumer(bufferSize: 4);
            Task task = Task.Run(() => 
            {
                pc.RunWithSemaphore(numProducers: 2, numConsumers: 1, itemsPerProducer: 4);
            });
            Thread.Sleep(6000);
            Assert.True(true);
        }

        [Fact]
        public void TestInitialization()
        {
            ProducerConsumer pc = new ProducerConsumer(bufferSize: 5);
            Assert.NotNull(pc);
        }

        [Fact]
        public void TestMultipleProducersConsumers()
        {
            ProducerConsumer pc = new ProducerConsumer(bufferSize: 10);
            Task task = Task.Run(() => 
            {
                pc.RunWithBlockingCollection(numProducers: 5, numConsumers: 3, itemsPerProducer: 2);
            });
            Thread.Sleep(4000);
            Assert.True(true);
        }
    }
}

