using System;
using System. Threading;

class Program
{
    public static void Main()
    {
        int n = 0;
        if (param is int)
        {
            n = (int)param;
        }
        else
        {
            int.TryParse(param?.ToString() ?? "0", out n);
        }

        long sum = 0;
        for (int i = 1; i <= n; i++) sum += i;

        Console.WriteLine($"Поток: сумма чисел 1..{n} = {sum}");
    }
}