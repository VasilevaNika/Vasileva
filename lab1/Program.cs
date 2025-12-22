using Lab1.Services;

namespace Lab1;

class Program
{
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        DemoPerson();
        await DemoPersonSerializer();
        DemoFileResourceManager();
    }

    static void DemoPerson()
    {
        var person = new Person
        {
            FirstName = "Роман",
            LastName = "Павлов",
            Age = 27,
            Password = "13pon",
            Id = "001",
            BirthDate = new DateTime(1997, 8, 12),
            Email = "roman@yandex.ru",
            PhoneNumber = "+7-912-345-67-89"
        };
        Console.WriteLine($"Полное имя: {person.FullName}");
        Console.WriteLine($"Возраст: {person.Age}");
        Console.WriteLine($"Взрослый: {person.IsAdult}");
        Console.WriteLine($"Email: {person.Email}");
        Console.WriteLine($"Телефон: {person.PhoneNumber}");
        Console.WriteLine($"Дата рождения: {person.BirthDate:yyyy-MM-dd}");
        Console.WriteLine($"ID: {person.Id}");
        try
        {
            person.Email = "bademail";
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    static async Task DemoPersonSerializer()
    {
        var serializer = new PersonSerializer();
        var person = new Person
        {
            FirstName = "Екатерина",
            LastName = "Волкова",
            Age = 29,
            Password = "paso223",
            Id = "002",
            BirthDate = new DateTime(1995, 4, 18),
            Email = "ekaterina@mail.ru",
            PhoneNumber = "+7-987-654-32-10"
        };
        var json = serializer.SerializeToJson(person);
        Console.WriteLine(json);
        var deserializedPerson = serializer.DeserializeFromJson(json);
        Console.WriteLine($"Имя: {deserializedPerson.FullName}");
        Console.WriteLine($"Email: {deserializedPerson.Email}");
        string filePath = "person.json";
        serializer.SaveToFile(person, filePath);
        Console.WriteLine($"Сохранено в {filePath}");
        var loadedPerson = serializer.LoadFromFile(filePath);
        Console.WriteLine($"Загружено: {loadedPerson.FullName}");
        string asyncFilePath = "person_async.json";
        await serializer.SaveToFileAsync(person, asyncFilePath);
        var asyncLoadedPerson = await serializer.LoadFromFileAsync(asyncFilePath);
        Console.WriteLine($"Асинхронно загружено: {asyncLoadedPerson.FullName}");
        var people = new List<Person>
        {
            new Person { FirstName = "Максим", LastName = "Симанов", Age = 31, Email = "maxim@inbox.ru", Id = "003", PhoneNumber = "+7-911-222-33-44" },
            new Person { FirstName = "Анна", LastName = "Попова", Age = 24, Email = "anna@rambler.ru", Id = "004", PhoneNumber = "+7-912-333-44-55" }
        };
        string listFilePath = "people.json";
        serializer.SaveListToFile(people, listFilePath);
        Console.WriteLine($"Список сохранен в {listFilePath}");
        var loadedPeople = serializer.LoadListFromFile(listFilePath);
        Console.WriteLine($"Загружено {loadedPeople.Count} человек:");
        foreach (var p in loadedPeople)
        {
            Console.WriteLine($"{p.FullName} - {p.Email}");
        }
    }

    static void DemoFileResourceManager()
    {
        string testFilePath = "test_file.txt";
        using (var manager = new FileResourceManager(testFilePath, FileMode.Create))
        {
            manager.OpenForWriting();
            manager.WriteLine("Строка 1");
            manager.WriteLine("Строка 2");
            manager.WriteLine("Строка 3");
            Console.WriteLine("Записано 3 строки");
        }
        using (var manager = new FileResourceManager(testFilePath))
        {
            manager.OpenForReading();
            string content = manager.ReadAllText();
            Console.WriteLine("Содержимое файла:");
            Console.WriteLine(content);
            var fileInfo = manager.GetFileInfo();
            Console.WriteLine($"Размер: {fileInfo.Length} байт");
            Console.WriteLine($"Создан: {fileInfo.CreationTime:yyyy-MM-dd HH:mm:ss}");
        }
        using (var manager = new FileResourceManager(testFilePath, FileMode.Append))
        {
            manager.AppendText("Новая строка\n");
            Console.WriteLine("Текст добавлен");
        }
        using (var manager = new FileResourceManager(testFilePath))
        {
            manager.OpenForReading();
            string finalContent = manager.ReadAllText();
            Console.WriteLine("Финальное содержимое:");
            Console.WriteLine(finalContent);
        }
    }
}
