using CollectionPerformanceBenchmark;

namespace CollectionPerformanceBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var benchmark = new CollectionBenchmark();
            var results = benchmark.RunAllBenchmarks();

            Console.WriteLine("\nРезультаты замеров:");
            PrintResultsTable(results);
        }

        private static void PrintResultsTable(List<CollectionBenchmark.BenchmarkResult> results)
        {
            Console.WriteLine($"{"Коллекция",-20} {"Операция",-35} {"Среднее (мс)",-15} {"Мин (мс)",-12} {"Макс (мс)",-12}");
            Console.WriteLine(new string('-', 95));

            foreach (var result in results)
            {
                Console.WriteLine($"{result.CollectionType,-20} {result.Operation,-35} {result.AverageTimeMs,15:F2} {result.MinTimeMs,12:F2} {result.MaxTimeMs,12:F2}");
            }
        }
    }
}
