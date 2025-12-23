using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CollectionPerformanceBenchmark.Tests
{
    public class ListTests
    {
        private const int TestSize = 1000;

        [Fact]
        public void AddToEnd_ShouldAddElements()
        {
            var list = new List<int>();

            for (int i = 0; i < TestSize; i++)
            {
                list.Add(i);
            }

            Assert.Equal(TestSize, list.Count);
            Assert.Equal(0, list[0]);
            Assert.Equal(TestSize - 1, list[TestSize - 1]);
        }

        [Fact]
        public void AddToBeginning_ShouldAddElements()
        {
            var list = new List<int>();

            for (int i = 0; i < TestSize; i++)
            {
                list.Insert(0, i);
            }

            Assert.Equal(TestSize, list.Count);
            Assert.Equal(TestSize - 1, list[0]);
            Assert.Equal(0, list[TestSize - 1]);
        }

        [Fact]
        public void AddToMiddle_ShouldAddElements()
        {
            var list = new List<int> { 0, 1, 2, 3, 4 };

            list.Insert(2, 99);

            Assert.Equal(6, list.Count);
            Assert.Equal(99, list[2]);
            Assert.Equal(2, list[3]);
        }

        [Fact]
        public void RemoveFromBeginning_ShouldRemoveElements()
        {
            var list = Enumerable.Range(0, TestSize).ToList();

            for (int i = 0; i < TestSize; i++)
            {
                list.RemoveAt(0);
            }

            Assert.Empty(list);
        }

        [Fact]
        public void RemoveFromEnd_ShouldRemoveElements()
        {
            var list = Enumerable.Range(0, TestSize).ToList();

            for (int i = 0; i < TestSize; i++)
            {
                list.RemoveAt(list.Count - 1);
            }

            Assert.Empty(list);
        }

        [Fact]
        public void RemoveFromMiddle_ShouldRemoveElements()
        {
            var list = Enumerable.Range(0, 10).ToList();

            list.RemoveAt(5);

            Assert.Equal(9, list.Count);
            Assert.DoesNotContain(5, list);
        }

        [Fact]
        public void Search_ShouldFindElement()
        {
            var list = Enumerable.Range(0, TestSize).ToList();

            var found = list.Contains(500);

            Assert.True(found);
        }

        [Fact]
        public void GetByIndex_ShouldReturnCorrectElement()
        {
            var list = Enumerable.Range(0, TestSize).ToList();

            var value = list[500];

            Assert.Equal(500, value);
        }
    }
}

