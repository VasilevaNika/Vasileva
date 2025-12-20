using Lab1;

namespace Lab1;

class Program
{
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        try
        {
            TestPerson();
            await TestPersonSerializer();
            TestFileResourceManager();

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    // Тест Person
    static void TestPerson()
    {
        Console.WriteLine("Тест Person");

        var person = new Person
        {
            FirstName = "Георгий",
            LastName = "Пономарев",
            Age = 30,
            Password = "gosha123",
            Id = "001",
            BirthDate = new DateTime(1995, 3, 12),
            Email = "gosha.ponomarev@mail.ru",
            PhoneNumber = "+7-902-228-67-77"
        };

        Console.WriteLine($"Полное имя: {person.FullName}");
        Console.WriteLine($"Взрослый: {person.IsAdult}");
        Console.WriteLine($"Email: {person.Email}");
        Console.WriteLine($"Телефон: {person.PhoneNumber}");
        Console.WriteLine($"Дата рождения: {person.BirthDate:yyyy-MM-dd}");

        try
        {
            person.Email = "bademail";
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Валидация Email: {ex.Message}");
        }
    }

    // Тест PersonSerializer
    static async Task TestPersonSerializer()
    {
        Console.WriteLine("\nТест PersonSerializer");

        var serializer = new PersonSerializer();
        var person = new Person
        {
            FirstName = "Андрей",
            LastName = "Григорьев",
            Age = 24,
            Password = "an228rey",
            Id = "002",
            BirthDate = new DateTime(2001, 7, 8),
            Email = "andrey2001@gmail.com",
            PhoneNumber = "+7-902-314-22-85"
        };

        string json = serializer.SerializeToJson(person);
        Console.WriteLine("JSON:");
        Console.WriteLine(json);

        var deserializedPerson = serializer.DeserializeFromJson(json);
        Console.WriteLine($"Десериализовано: {deserializedPerson.FullName}");
        Console.WriteLine($"Пароль: '{deserializedPerson.Password}'");

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
            new Person { FirstName = "Роман", LastName = "Симанов", Age = 31, Email = "roman.m@yandex.ru", Id = "003", PhoneNumber = "+7-911-132-23-59" },
            new Person { FirstName = "Наталья", LastName = "Андреева", Age = 19, Email = "sofia.n@gmail.com", Id = "004", PhoneNumber = "+7-911-888-46-32" }
        };

        string listFilePath = "people.json";
        serializer.SaveListToFile(people, listFilePath);
        Console.WriteLine($"Список сохранен в {listFilePath}");

        var loadedPeople = serializer.LoadListFromFile(listFilePath);
        Console.WriteLine($"Загружено {loadedPeople.Count} объектов:");
        foreach (var p in loadedPeople)
        {
            Console.WriteLine($"{p.FullName} - {p.Email}");
        }
    }

    // Тест FileResourceManager
    static void TestFileResourceManager()
    {
        Console.WriteLine("\nТест FileResourceManager");

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
    }
}
