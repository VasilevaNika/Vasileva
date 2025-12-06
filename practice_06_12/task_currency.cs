class Program
{
    static void Main()
    {
        Dictionary<string, double> rates = new Dictionary<string, double>
        {
            { "USD", 1.0 },
            { "EUR", 0.86 },
            { "RUB", 76.09 },
            { "GBP", 0.75 }
        };
        
        List<string> history = new List<string>();
        
        while (true)
        {
            Console.WriteLine("\nВалюты: USD, EUR, RUB, GBP");
            Console.WriteLine("'выход' или 'история'");
            
            Console.Write("\nИз: ");
            string from = Console.ReadLine();
            
            if (from == "выход") break;
            
            if (from == "история")
            {
                Console.WriteLine("\nИстория:");
                foreach (string h in history)
                    Console.WriteLine(h);
                continue;
            }
            Console.Write("В: ");
            string to = Console.ReadLine().ToUpper();
            
            Console.Write("Сумма: ");
            if (!double.TryParse(Console.ReadLine(), out double amount))
            {
                Console.WriteLine("Неверная сумма!");
                continue;
            }
            
            double result = amount * rates[to] / rates[from];
            double rate = rates[to] / rates[from];
            
            string operation = $"{amount:F2} {from} = {result:F2} {to} (курс: {rate:F4})";
            Console.WriteLine($"Результат: {operation}");
            
            history.Add(operation);
        }
    }
}