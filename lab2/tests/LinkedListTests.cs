using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CollectionPerformanceBenchmark.Tests
{
    public class LinkedListTests
    {
        private const int TestSize = 1000;

        [Fact]
        public void AddToEnd_ShouldAddElements()
        {
            var list = new LinkedList<int>();

            for (int i = 0; i < TestSize; i++)
            {
                list.AddLast(i);
            }

            Assert.Equal(TestSize, list.Count);
            Assert.Equal(0, list.First?.Value);
            Assert.Equal(TestSize - 1, list.Last?.Value);
        }

        [Fact]
        public void AddToBeginning_ShouldAddElements()
        {
            var list = new LinkedList<int>();

            for (int i = 0; i < TestSize; i++)
            {
                list.AddFirst(i);
            }

            Assert.Equal(TestSize, list.Count);
            Assert.Equal(TestSize - 1, list.First?.Value);
            Assert.Equal(0, list.Last?.Value);
        }

        [Fact]
        public void RemoveFromBeginning_ShouldRemoveElements()
        {
            var list = new LinkedList<int>(Enumerable.Range(0, TestSize));

            for (int i = 0; i < TestSize; i++)
            {
                list.RemoveFirst();
            }

            Assert.Empty(list);
        }

        [Fact]
        public void RemoveFromEnd_ShouldRemoveElements()
        {
            var list = new LinkedList<int>(Enumerable.Range(0, TestSize));

            for (int i = 0; i < TestSize; i++)
            {
                list.RemoveLast();
            }

            Assert.Empty(list);
        }

        [Fact]
        public void Search_ShouldFindElement()
        {
            var list = new LinkedList<int>(Enumerable.Range(0, TestSize));

            var found = list.Contains(500);

            Assert.True(found);
        }
    }
}

