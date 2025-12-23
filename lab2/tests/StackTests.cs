using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CollectionPerformanceBenchmark.Tests
{
    public class StackTests
    {
        private const int TestSize = 1000;

        [Fact]
        public void Push_ShouldAddElements()
        {
            var stack = new Stack<int>();

            for (int i = 0; i < TestSize; i++)
            {
                stack.Push(i);
            }

            Assert.Equal(TestSize, stack.Count);
        }

        [Fact]
        public void Pop_ShouldRemoveElementsInLIFOOrder()
        {
            var stack = new Stack<int>();
            for (int i = 0; i < TestSize; i++)
            {
                stack.Push(i);
            }

            for (int i = TestSize - 1; i >= 0; i--)
            {
                var value = stack.Pop();
                Assert.Equal(i, value);
            }
            Assert.Empty(stack);
        }

        [Fact]
        public void Search_ShouldFindElement()
        {
            var stack = new Stack<int>(Enumerable.Range(0, TestSize));

            var found = stack.Contains(500);

            Assert.True(found);
        }
    }
}

