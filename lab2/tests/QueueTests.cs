using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CollectionPerformanceBenchmark.Tests
{
    public class QueueTests
    {
        private const int TestSize = 1000;

        [Fact]
        public void Enqueue_ShouldAddElements()
        {
            var queue = new Queue<int>();

            for (int i = 0; i < TestSize; i++)
            {
                queue.Enqueue(i);
            }

            Assert.Equal(TestSize, queue.Count);
        }

        [Fact]
        public void Dequeue_ShouldRemoveElementsInFIFOOrder()
        {
            var queue = new Queue<int>();
            for (int i = 0; i < TestSize; i++)
            {
                queue.Enqueue(i);
            }

            for (int i = 0; i < TestSize; i++)
            {
                var value = queue.Dequeue();
                Assert.Equal(i, value);
            }
            Assert.Empty(queue);
        }

        [Fact]
        public void Search_ShouldFindElement()
        {
            var queue = new Queue<int>(Enumerable.Range(0, TestSize));

            var found = queue.Contains(500);

            Assert.True(found);
        }
    }
}

