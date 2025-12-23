using CollectionsLab;

namespace CollectionsLab;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine();

        TestList();
        Console.WriteLine();

        TestDictionary();
        Console.WriteLine();

        TestLinkedList();
    }

    static void TestList()
    {
        Console.WriteLine("SimpleList:");
        var list = new SimpleList();

        list.Add("кофе");
        list.Add("чай");
        list.Add("сок");
        Console.WriteLine($"Количество: {list.Count}");

        for (int i = 0; i < list.Count; i++)
        {
            Console.WriteLine($"[{i}] = {list[i]}");
        }

        Console.WriteLine($"Индекс 'чай': {list.IndexOf("чай")}");
        Console.WriteLine($"Содержит 'сок': {list.Contains("сок")}");

        list.Insert(1, "вода");
        foreach (var item in list)
        {
            Console.WriteLine($"- {item}");
        }

        list.Remove("чай");
        Console.WriteLine($"Количество после удаления: {list.Count}");
    }

    static void TestDictionary()
    {
        Console.WriteLine("SimpleDictionary:");
        var dict = new SimpleDictionary<string, int>();

        dict.Add("математика", 5);
        dict.Add("физика", 4);
        dict["химия"] = 5;
        Console.WriteLine($"Количество: {dict.Count}");

        foreach (var kvp in dict)
        {
            Console.WriteLine($"{kvp.Key} = {kvp.Value}");
        }

        Console.WriteLine($"Содержит 'физика': {dict.ContainsKey("физика")}");
        if (dict.TryGetValue("математика", out int value))
        {
            Console.WriteLine($"Оценка по математике: {value}");
        }

        dict["математика"] = 4;
        Console.WriteLine($"математика = {dict["математика"]}");

        dict.Remove("физика");
        Console.WriteLine($"Количество после удаления: {dict.Count}");
    }

    static void TestLinkedList()
    {
        Console.WriteLine("DoublyLinkedList:");
        var list = new DoublyLinkedList();

        list.AddLast("понедельник");
        list.AddLast("вторник");
        list.AddLast("среда");
        Console.WriteLine($"Количество: {list.Count}");

        for (int i = 0; i < list.Count; i++)
        {
            Console.WriteLine($"[{i}] = {list[i]}");
        }

        list.AddFirst("воскресенье");
        Console.WriteLine($"Первый элемент: {list[0]}");

        list.Insert(2, "понедельник2");
        foreach (var item in list)
        {
            Console.WriteLine($"- {item}");
        }

        list.RemoveFirst();
        list.RemoveLast();
        Console.WriteLine($"Количество: {list.Count}");
        Console.WriteLine($"Первый: {list[0]}");
        Console.WriteLine($"Последний: {list[list.Count - 1]}");
    }
}
