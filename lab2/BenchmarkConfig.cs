using System.Collections.Generic;

namespace CollectionPerformanceBenchmark
{
    public static class BenchmarkConfig
    {
        public const int CollectionSize = 100_000;
        public const int WarmupIterations = 3;
        public const int MeasurementIterations = 5;
    }

    public class BenchmarkResult
    {
        public string CollectionType { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public double AverageTimeMs { get; set; }
        public double MinTimeMs { get; set; }
        public double MaxTimeMs { get; set; }
        public List<double> AllMeasurements { get; set; } = new();
    }
}

