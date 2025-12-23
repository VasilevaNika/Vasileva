using System.Collections.Immutable;
using System.Linq;
using Xunit;

namespace CollectionPerformanceBenchmark.Tests
{
    public class ImmutableListTests
    {
        private const int TestSize = 1000;

        [Fact]
        public void AddToEnd_ShouldAddElements()
        {
            var list = ImmutableList<int>.Empty;

            for (int i = 0; i < TestSize; i++)
            {
                list = list.Add(i);
            }

            Assert.Equal(TestSize, list.Count);
            Assert.Equal(0, list[0]);
            Assert.Equal(TestSize - 1, list[TestSize - 1]);
        }

        [Fact]
        public void AddToBeginning_ShouldAddElements()
        {
            var list = ImmutableList<int>.Empty;

            for (int i = 0; i < TestSize; i++)
            {
                list = list.Insert(0, i);
            }

            Assert.Equal(TestSize, list.Count);
            Assert.Equal(TestSize - 1, list[0]);
            Assert.Equal(0, list[TestSize - 1]);
        }

        [Fact]
        public void RemoveFromBeginning_ShouldRemoveElements()
        {
            var list = ImmutableList<int>.Empty.AddRange(Enumerable.Range(0, TestSize));

            for (int i = 0; i < TestSize; i++)
            {
                list = list.RemoveAt(0);
            }

            Assert.Empty(list);
        }

        [Fact]
        public void RemoveFromEnd_ShouldRemoveElements()
        {
            var list = ImmutableList<int>.Empty.AddRange(Enumerable.Range(0, TestSize));

            for (int i = 0; i < TestSize; i++)
            {
                list = list.RemoveAt(list.Count - 1);
            }

            Assert.Empty(list);
        }

        [Fact]
        public void Search_ShouldFindElement()
        {
            var list = ImmutableList<int>.Empty.AddRange(Enumerable.Range(0, TestSize));

            var found = list.Contains(500);

            Assert.True(found);
        }

        [Fact]
        public void GetByIndex_ShouldReturnCorrectElement()
        {
            var list = ImmutableList<int>.Empty.AddRange(Enumerable.Range(0, TestSize));

            var value = list[500];

            Assert.Equal(500, value);
        }

        [Fact]
        public void ShouldBeImmutable()
        {
            var list1 = ImmutableList<int>.Empty.AddRange(Enumerable.Range(0, 10));
            var list2 = list1.Add(99);

            Assert.Equal(10, list1.Count);
            Assert.Equal(11, list2.Count);
            Assert.DoesNotContain(99, list1);
            Assert.Contains(99, list2);
        }
    }
}

