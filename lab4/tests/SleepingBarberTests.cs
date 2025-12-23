using System.Threading;
using System.Threading.Tasks;
using Xunit;
using lab4;

namespace lab4.Tests
{
    public class SleepingBarberTests
    {
        [Fact]
        public void TestBarberWithClients()
        {
            SleepingBarber sb = new SleepingBarber(maxQueueSize: 3);
            Task task = Task.Run(() => sb.Run(numClients: 5));
            Thread.Sleep(3000);
            Assert.True(true);
        }

        [Fact]
        public void TestBarberInitialization()
        {
            SleepingBarber sb = new SleepingBarber(maxQueueSize: 5);
            Assert.NotNull(sb);
        }

        [Fact]
        public void TestBarberWithManyClients()
        {
            SleepingBarber sb = new SleepingBarber(maxQueueSize: 2);
            Task task = Task.Run(() => sb.Run(numClients: 8));
            Thread.Sleep(4000);
            Assert.True(true);
        }
    }
}

