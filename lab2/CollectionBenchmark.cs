using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace CollectionPerformanceBenchmark
{
    public class CollectionBenchmark
    {
        private const int CollectionSize = 100_000;
        private const int WarmupIterations = 3;
        private const int MeasurementIterations = 5;

        public class BenchmarkResult
        {
            public string CollectionType { get; set; } = string.Empty;
            public string Operation { get; set; } = string.Empty;
            public double AverageTimeMs { get; set; }
            public double MinTimeMs { get; set; }
            public double MaxTimeMs { get; set; }
            public List<double> AllMeasurements { get; set; } = new();
        }

        private readonly List<BenchmarkResult> _results = new();

        public List<BenchmarkResult> RunAllBenchmarks()
        {
            _results.Clear();

            Console.WriteLine("Начало замеров производительности");
            Console.WriteLine($"Размер коллекции: {CollectionSize:N0} элементов");
            Console.WriteLine($"Количество итераций: {MeasurementIterations}");

            BenchmarkList();
            BenchmarkLinkedList();
            BenchmarkQueue();
            BenchmarkStack();
            BenchmarkImmutableList();

            return _results;
        }

        private void BenchmarkList()
        {
            Console.WriteLine("\nList<T>");

            MeasureOperation("List<T>", "Добавление в конец", () =>
            {
                var list = new List<int>();
                for (int i = 0; i < CollectionSize; i++)
                    list.Add(i);
                return list;
            });

            MeasureOperation("List<T>", "Добавление в начало", () =>
            {
                var list = new List<int>();
                for (int i = 0; i < CollectionSize; i++)
                    list.Insert(0, i);
                return list;
            });

            MeasureOperation("List<T>", "Добавление в середину", () =>
            {
                var list = new List<int>();
                for (int i = 0; i < CollectionSize; i++)
                    list.Insert(list.Count / 2, i);
                return list;
            });

            MeasureOperation("List<T>", "Удаление из начала", () =>
            {
                var list = Enumerable.Range(0, CollectionSize).ToList();
                while (list.Count > 0)
                    list.RemoveAt(0);
                return list;
            });

            MeasureOperation("List<T>", "Удаление из конца", () =>
            {
                var list = Enumerable.Range(0, CollectionSize).ToList();
                while (list.Count > 0)
                    list.RemoveAt(list.Count - 1);
                return list;
            });

            MeasureOperation("List<T>", "Удаление из середины", () =>
            {
                var list = Enumerable.Range(0, CollectionSize).ToList();
                while (list.Count > 0)
                    list.RemoveAt(list.Count / 2);
                return list;
            });

            MeasureOperation("List<T>", "Поиск элемента", () =>
            {
                var list = Enumerable.Range(0, CollectionSize).ToList();
                var found = list.Contains(CollectionSize / 2);
                return list;
            });

            MeasureOperation("List<T>", "Получение по индексу", () =>
            {
                var list = Enumerable.Range(0, CollectionSize).ToList();
                var value = list[CollectionSize / 2];
                return list;
            });
        }

        private void BenchmarkLinkedList()
        {
            Console.WriteLine("\nLinkedList<T>");

            MeasureOperation("LinkedList<T>", "Добавление в конец", () =>
            {
                var list = new LinkedList<int>();
                for (int i = 0; i < CollectionSize; i++)
                    list.AddLast(i);
                return list;
            });

            MeasureOperation("LinkedList<T>", "Добавление в начало", () =>
            {
                var list = new LinkedList<int>();
                for (int i = 0; i < CollectionSize; i++)
                    list.AddFirst(i);
                return list;
            });

            MeasureOperation("LinkedList<T>", "Добавление в середину", () =>
            {
                var list = new LinkedList<int>();
                var middle = list.AddFirst(0);
                for (int i = 1; i < CollectionSize; i++)
                {
                    if (i % 2 == 0)
                        middle = list.AddAfter(middle, i);
                    else
                        middle = list.AddBefore(middle, i);
                }
                return list;
            });

            MeasureOperation("LinkedList<T>", "Удаление из начала", () =>
            {
                var list = new LinkedList<int>(Enumerable.Range(0, CollectionSize));
                while (list.Count > 0)
                    list.RemoveFirst();
                return list;
            });

            MeasureOperation("LinkedList<T>", "Удаление из конца", () =>
            {
                var list = new LinkedList<int>(Enumerable.Range(0, CollectionSize));
                while (list.Count > 0)
                    list.RemoveLast();
                return list;
            });

            MeasureOperation("LinkedList<T>", "Удаление из середины", () =>
            {
                var list = new LinkedList<int>(Enumerable.Range(0, CollectionSize));
                while (list.Count > 0)
                {
                    var middle = GetMiddleNode(list);
                    if (middle != null)
                        list.Remove(middle);
                    else
                        break;
                }
                return list;
            });

            MeasureOperation("LinkedList<T>", "Поиск элемента", () =>
            {
                var list = new LinkedList<int>(Enumerable.Range(0, CollectionSize));
                var found = list.Contains(CollectionSize / 2);
                return list;
            });

            MeasureOperation("LinkedList<T>", "Получение по индексу", () =>
            {
                var list = new LinkedList<int>(Enumerable.Range(0, CollectionSize));
                var node = list.First;
                for (int i = 0; i < CollectionSize / 2 && node != null; i++)
                    node = node.Next;
                var value = node?.Value ?? 0;
                return list;
            });
        }

        private void BenchmarkQueue()
        {
            Console.WriteLine("\nQueue<T>");

            MeasureOperation("Queue<T>", "Добавление в конец (Enqueue)", () =>
            {
                var queue = new Queue<int>();
                for (int i = 0; i < CollectionSize; i++)
                    queue.Enqueue(i);
                return queue;
            });

            MeasureOperation("Queue<T>", "Удаление из начала (Dequeue)", () =>
            {
                var queue = new Queue<int>(Enumerable.Range(0, CollectionSize));
                while (queue.Count > 0)
                    queue.Dequeue();
                return queue;
            });

            MeasureOperation("Queue<T>", "Поиск элемента", () =>
            {
                var queue = new Queue<int>(Enumerable.Range(0, CollectionSize));
                var found = queue.Contains(CollectionSize / 2);
                return queue;
            });
        }

        private void BenchmarkStack()
        {
            Console.WriteLine("\nStack<T>");

            MeasureOperation("Stack<T>", "Добавление в конец (Push)", () =>
            {
                var stack = new Stack<int>();
                for (int i = 0; i < CollectionSize; i++)
                    stack.Push(i);
                return stack;
            });

            MeasureOperation("Stack<T>", "Удаление из конца (Pop)", () =>
            {
                var stack = new Stack<int>(Enumerable.Range(0, CollectionSize));
                while (stack.Count > 0)
                    stack.Pop();
                return stack;
            });

            MeasureOperation("Stack<T>", "Поиск элемента", () =>
            {
                var stack = new Stack<int>(Enumerable.Range(0, CollectionSize));
                var found = stack.Contains(CollectionSize / 2);
                return stack;
            });
        }

        private void BenchmarkImmutableList()
        {
            Console.WriteLine("\nImmutableList<T>");

            MeasureOperation("ImmutableList<T>", "Добавление в конец", () =>
            {
                var list = ImmutableList<int>.Empty;
                for (int i = 0; i < CollectionSize; i++)
                    list = list.Add(i);
                return list;
            });

            MeasureOperation("ImmutableList<T>", "Добавление в начало", () =>
            {
                var list = ImmutableList<int>.Empty;
                for (int i = 0; i < CollectionSize; i++)
                    list = list.Insert(0, i);
                return list;
            });

            MeasureOperation("ImmutableList<T>", "Добавление в середину", () =>
            {
                var list = ImmutableList<int>.Empty;
                for (int i = 0; i < CollectionSize; i++)
                    list = list.Insert(list.Count / 2, i);
                return list;
            });

            MeasureOperation("ImmutableList<T>", "Удаление из начала", () =>
            {
                var list = ImmutableList<int>.Empty.AddRange(Enumerable.Range(0, CollectionSize));
                while (list.Count > 0)
                    list = list.RemoveAt(0);
                return list;
            });

            MeasureOperation("ImmutableList<T>", "Удаление из конца", () =>
            {
                var list = ImmutableList<int>.Empty.AddRange(Enumerable.Range(0, CollectionSize));
                while (list.Count > 0)
                    list = list.RemoveAt(list.Count - 1);
                return list;
            });

            MeasureOperation("ImmutableList<T>", "Удаление из середины", () =>
            {
                var list = ImmutableList<int>.Empty.AddRange(Enumerable.Range(0, CollectionSize));
                while (list.Count > 0)
                    list = list.RemoveAt(list.Count / 2);
                return list;
            });

            MeasureOperation("ImmutableList<T>", "Поиск элемента", () =>
            {
                var list = ImmutableList<int>.Empty.AddRange(Enumerable.Range(0, CollectionSize));
                var found = list.Contains(CollectionSize / 2);
                return list;
            });

            MeasureOperation("ImmutableList<T>", "Получение по индексу", () =>
            {
                var list = ImmutableList<int>.Empty.AddRange(Enumerable.Range(0, CollectionSize));
                var value = list[CollectionSize / 2];
                return list;
            });
        }

        private void MeasureOperation<T>(string collectionType, string operation, Func<T> action)
        {
            for (int i = 0; i < WarmupIterations; i++)
            {
                action();
            }

            var measurements = new List<double>();
            for (int i = 0; i < MeasurementIterations; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                var sw = Stopwatch.StartNew();
                action();
                sw.Stop();

                measurements.Add(sw.Elapsed.TotalMilliseconds);
            }

            var result = new BenchmarkResult
            {
                CollectionType = collectionType,
                Operation = operation,
                AverageTimeMs = measurements.Average(),
                MinTimeMs = measurements.Min(),
                MaxTimeMs = measurements.Max(),
                AllMeasurements = measurements
            };

            _results.Add(result);
            Console.WriteLine($"  {operation}: {result.AverageTimeMs:F2} мс (мин: {result.MinTimeMs:F2}, макс: {result.MaxTimeMs:F2})");
        }

        private LinkedListNode<int>? GetMiddleNode(LinkedList<int> list)
        {
            if (list.Count == 0) return null;
            if (list.Count == 1) return list.First;

            int middleIndex = list.Count / 2;
            var current = list.First;
            for (int i = 0; i < middleIndex && current != null; i++)
            {
                current = current.Next;
            }
            return current;
        }
    }
}
