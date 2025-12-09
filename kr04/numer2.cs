using System;
using System. Threading;

class Program{

    public static void Main()
    {
        object bigObj = new byte[10 * 1024 * 1024];
        Console.WriteLine($"поколение: {GC.GetGeneration(bigObj)}");

        for (int i = 1; i <= 3; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Console.WriteLine($"После GC.Collect() #{i}, поколение: {GC.GetGeneration(bigObj)}");
        }

        object obj = null;
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Console.WriteLine("Объект может быть освобождён.");
    }
}