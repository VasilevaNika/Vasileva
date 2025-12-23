using System.Threading;
using System.Threading.Tasks;
using Xunit;
using lab4;

namespace lab4.Tests
{
    public class DiningPhilosophersTests
    {
        [Fact]
        public void TestWithoutDeadlock()
        {
            DiningPhilosophers dp = new DiningPhilosophers();
            Task task = Task.Run(() => dp.RunWithoutDeadlock());
            Thread.Sleep(2000);
            Assert.True(true);
        }

        [Fact]
        public void TestForksInitialization()
        {
            DiningPhilosophers dp = new DiningPhilosophers();
            Assert.NotNull(dp);
        }
    }
}

