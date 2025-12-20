using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Xunit;

namespace CollectionPerformanceBenchmark.Tests
{
    public class CollectionTests
    {
        private const int TestSize = 1000;

        [Fact]
        public void List_AddToEnd_ShouldAddElements()
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
        public void List_AddToBeginning_ShouldAddElements()
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
        public void List_AddToMiddle_ShouldAddElements()
        {
            var list = new List<int> { 0, 1, 2, 3, 4 };

            list.Insert(2, 99);

            Assert.Equal(6, list.Count);
            Assert.Equal(99, list[2]);
            Assert.Equal(2, list[3]);
        }

        [Fact]
        public void List_RemoveFromBeginning_ShouldRemoveElements()
        {
            var list = Enumerable.Range(0, TestSize).ToList();

            for (int i = 0; i < TestSize; i++)
            {
                list.RemoveAt(0);
            }

            Assert.Empty(list);
        }

        [Fact]
        public void List_RemoveFromEnd_ShouldRemoveElements()
        {
            var list = Enumerable.Range(0, TestSize).ToList();

            for (int i = 0; i < TestSize; i++)
            {
                list.RemoveAt(list.Count - 1);
            }

            Assert.Empty(list);
        }

        [Fact]
        public void List_RemoveFromMiddle_ShouldRemoveElements()
        {
            var list = Enumerable.Range(0, 10).ToList();

            list.RemoveAt(5);

            Assert.Equal(9, list.Count);
            Assert.DoesNotContain(5, list);
        }

        [Fact]
        public void List_Search_ShouldFindElement()
        {
            var list = Enumerable.Range(0, TestSize).ToList();

            var found = list.Contains(500);

            Assert.True(found);
        }

        [Fact]
        public void List_GetByIndex_ShouldReturnCorrectElement()
        {
            var list = Enumerable.Range(0, TestSize).ToList();

            var value = list[500];

            Assert.Equal(500, value);
        }

        [Fact]
        public void LinkedList_AddToEnd_ShouldAddElements()
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
        public void LinkedList_AddToBeginning_ShouldAddElements()
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
        public void LinkedList_RemoveFromBeginning_ShouldRemoveElements()
        {
            var list = new LinkedList<int>(Enumerable.Range(0, TestSize));

            for (int i = 0; i < TestSize; i++)
            {
                list.RemoveFirst();
            }

            Assert.Empty(list);
        }

        [Fact]
        public void LinkedList_RemoveFromEnd_ShouldRemoveElements()
        {
            var list = new LinkedList<int>(Enumerable.Range(0, TestSize));

            for (int i = 0; i < TestSize; i++)
            {
                list.RemoveLast();
            }

            Assert.Empty(list);
        }

        [Fact]
        public void LinkedList_Search_ShouldFindElement()
        {
            var list = new LinkedList<int>(Enumerable.Range(0, TestSize));

            var found = list.Contains(500);

            Assert.True(found);
        }

        [Fact]
        public void Queue_Enqueue_ShouldAddElements()
        {
            var queue = new Queue<int>();

            for (int i = 0; i < TestSize; i++)
            {
                queue.Enqueue(i);
            }

            Assert.Equal(TestSize, queue.Count);
        }

        [Fact]
        public void Queue_Dequeue_ShouldRemoveElementsInFIFOOrder()
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
        public void Queue_Search_ShouldFindElement()
        {
            var queue = new Queue<int>(Enumerable.Range(0, TestSize));

            var found = queue.Contains(500);

            Assert.True(found);
        }

        [Fact]
        public void Stack_Push_ShouldAddElements()
        {
            var stack = new Stack<int>();

            for (int i = 0; i < TestSize; i++)
            {
                stack.Push(i);
            }

            Assert.Equal(TestSize, stack.Count);
        }

        [Fact]
        public void Stack_Pop_ShouldRemoveElementsInLIFOOrder()
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
        public void Stack_Search_ShouldFindElement()
        {
            var stack = new Stack<int>(Enumerable.Range(0, TestSize));

            var found = stack.Contains(500);

            Assert.True(found);
        }

        [Fact]
        public void ImmutableList_AddToEnd_ShouldAddElements()
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
        public void ImmutableList_AddToBeginning_ShouldAddElements()
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
        public void ImmutableList_RemoveFromBeginning_ShouldRemoveElements()
        {
            var list = ImmutableList<int>.Empty.AddRange(Enumerable.Range(0, TestSize));

            for (int i = 0; i < TestSize; i++)
            {
                list = list.RemoveAt(0);
            }

            Assert.Empty(list);
        }

        [Fact]
        public void ImmutableList_RemoveFromEnd_ShouldRemoveElements()
        {
            var list = ImmutableList<int>.Empty.AddRange(Enumerable.Range(0, TestSize));

            for (int i = 0; i < TestSize; i++)
            {
                list = list.RemoveAt(list.Count - 1);
            }

            Assert.Empty(list);
        }

        [Fact]
        public void ImmutableList_Search_ShouldFindElement()
        {
            var list = ImmutableList<int>.Empty.AddRange(Enumerable.Range(0, TestSize));

            var found = list.Contains(500);

            Assert.True(found);
        }

        [Fact]
        public void ImmutableList_GetByIndex_ShouldReturnCorrectElement()
        {
            var list = ImmutableList<int>.Empty.AddRange(Enumerable.Range(0, TestSize));

            var value = list[500];

            Assert.Equal(500, value);
        }

        [Fact]
        public void ImmutableList_ShouldBeImmutable()
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
