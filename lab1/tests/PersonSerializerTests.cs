using Lab1;
using System.Text.Json;
using Xunit;

namespace Lab1.Tests;

public class PersonSerializerTests : IDisposable
{
    private readonly PersonSerializer _serializer;
    private readonly string _testDirectory;
    private readonly List<string> _testFiles;

    public PersonSerializerTests()
    {
        _serializer = new PersonSerializer();
        _testDirectory = Path.Combine(Path.GetTempPath(), "Lab1Tests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDirectory);
        _testFiles = new List<string>();
    }

    [Fact]
    public void SerializeToJson_ShouldReturnValidJson()
    {
        var person = new Person
        {
            FirstName = "Роман",
            LastName = "Павлов",
            Age = 27,
            Email = "roman@yandex.ru",
            Id = "123",
            PhoneNumber = "+7-912-345-67-89"
        };
        var json = _serializer.SerializeToJson(person);
        Assert.NotNull(json);
        Assert.Contains("Роман", json);
        Assert.Contains("Кузнецов", json);
        Assert.Contains("123", json);
    }

    [Fact]
    public void DeserializeFromJson_ShouldReturnPersonObject()
    {
        var originalPerson = new Person
        {
            FirstName = "Николай",
            LastName = "Орлов",
            Age = 33,
            Email = "nikolay@gmail.com",
            Id = "789",
            PhoneNumber = "+7-987-654-32-10"
        };
        var json = JsonSerializer.Serialize(originalPerson);
        var deserializedPerson = _serializer.DeserializeFromJson(json);
        Assert.NotNull(deserializedPerson);
        Assert.Equal(originalPerson.FirstName, deserializedPerson.FirstName);
        Assert.Equal(originalPerson.LastName, deserializedPerson.LastName);
        Assert.Equal(originalPerson.Age, deserializedPerson.Age);
        Assert.Equal(originalPerson.Email, deserializedPerson.Email);
        Assert.Equal(originalPerson.Id, deserializedPerson.Id);
        Assert.Equal(originalPerson.PhoneNumber, deserializedPerson.PhoneNumber);
    }

    [Fact]
    public void DeserializeFromJson_ShouldThrowArgumentException_WhenJsonIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => _serializer.DeserializeFromJson(""));
        Assert.Throws<ArgumentException>(() => _serializer.DeserializeFromJson("   "));
    }

    [Fact]
    public void DeserializeFromJson_ShouldThrowArgumentException_WhenJsonIsNull()
    {
        Assert.Throws<ArgumentException>(() => _serializer.DeserializeFromJson(null!));
    }

    [Fact]
    public void SaveToFile_ShouldCreateFileWithJsonContent()
    {
        var person = new Person
        {
            FirstName = "Екатерина",
            LastName = "Волкова",
            Age = 29,
            Email = "ekaterina@mail.ru",
            Id = "555"
        };
        var filePath = Path.Combine(_testDirectory, "test_person.json");
        _testFiles.Add(filePath);
        _serializer.SaveToFile(person, filePath);
        Assert.True(File.Exists(filePath));
        var content = File.ReadAllText(filePath);
        Assert.Contains("Екатерина", content);
        Assert.Contains("Волкова", content);
    }

    [Fact]
    public void SaveToFile_ShouldCreateDirectory_WhenDirectoryDoesNotExist()
    {
        var person = new Person
        {
            FirstName = "Максим",
            LastName = "Симанов",
            Age = 24,
            Email = "maxim@inbox.ru",
            Id = "788"
        };
        var subDirectory = Path.Combine(_testDirectory, "subdir");
        var filePath = Path.Combine(subDirectory, "test.json");
        _testFiles.Add(filePath);
        _serializer.SaveToFile(person, filePath);
        Assert.True(Directory.Exists(subDirectory));
        Assert.True(File.Exists(filePath));
    }

    [Fact]
    public void SaveToFile_ShouldThrowArgumentNullException_WhenPersonIsNull()
    {
        var filePath = Path.Combine(_testDirectory, "test.json");
        _testFiles.Add(filePath);
        Assert.Throws<ArgumentNullException>(() => _serializer.SaveToFile(null!, filePath));
    }

    [Fact]
    public void LoadFromFile_ShouldReturnPersonObject()
    {
        var person = new Person
        {
            FirstName = "Анна",
            LastName = "Попова",
            Age = 31,
            Email = "anna@rambler.ru",
            Id = "666"
        };
        var filePath = Path.Combine(_testDirectory, "load_test.json");
        _testFiles.Add(filePath);
        _serializer.SaveToFile(person, filePath);
        var loadedPerson = _serializer.LoadFromFile(filePath);
        Assert.NotNull(loadedPerson);
        Assert.Equal(person.FirstName, loadedPerson.FirstName);
        Assert.Equal(person.LastName, loadedPerson.LastName);
        Assert.Equal(person.Age, loadedPerson.Age);
        Assert.Equal(person.Email, loadedPerson.Email);
        Assert.Equal(person.Id, loadedPerson.Id);
    }

    [Fact]
    public void LoadFromFile_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
    {
        var filePath = Path.Combine(_testDirectory, "nonexistent.json");
        Assert.Throws<FileNotFoundException>(() => _serializer.LoadFromFile(filePath));
    }

    [Fact]
    public async Task SaveToFileAsync_ShouldCreateFileWithJsonContent()
    {
        var person = new Person
        {
            FirstName = "Илья",
            LastName = "Викулин",
            Age = 36,
            Email = "ilyav@mail.ru",
            Id = "111"
        };
        var filePath = Path.Combine(_testDirectory, "async_test.json");
        _testFiles.Add(filePath);
        await _serializer.SaveToFileAsync(person, filePath);
        Assert.True(File.Exists(filePath));
        var content = await File.ReadAllTextAsync(filePath);
        Assert.Contains("Игорь", content);
    }

    [Fact]
    public async Task SaveToFileAsync_ShouldThrowArgumentNullException_WhenPersonIsNull()
    {
        var filePath = Path.Combine(_testDirectory, "async_null_test.json");
        _testFiles.Add(filePath);
        await Assert.ThrowsAsync<ArgumentNullException>(() => _serializer.SaveToFileAsync(null!, filePath));
    }

    [Fact]
    public async Task LoadFromFileAsync_ShouldReturnPersonObject()
    {
        var person = new Person
        {
            FirstName = "Татьяна",
            LastName = "Амбарова",
            Age = 26,
            Email = "tatyana@list.ru",
            Id = "222"
        };
        var filePath = Path.Combine(_testDirectory, "async_load_test.json");
        _testFiles.Add(filePath);
        await _serializer.SaveToFileAsync(person, filePath);
        var loadedPerson = await _serializer.LoadFromFileAsync(filePath);
        Assert.NotNull(loadedPerson);
        Assert.Equal(person.FirstName, loadedPerson.FirstName);
        Assert.Equal(person.LastName, loadedPerson.LastName);
    }

    [Fact]
    public async Task LoadFromFileAsync_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
    {
        var filePath = Path.Combine(_testDirectory, "async_nonexistent.json");
        await Assert.ThrowsAsync<FileNotFoundException>(() => _serializer.LoadFromFileAsync(filePath));
    }

    [Fact]
    public void SaveListToFile_ShouldSaveListOfPersons()
    {
        var people = new List<Person>
        {
            new Person { FirstName = "Софья", LastName = "Михалева", Age = 22, Email = "yuri.m@mail.ru", Id = "301" },
            new Person { FirstName = "Светлана", LastName = "Елизарова", Age = 28, Email = "svetlana.n@gmail.com", Id = "302" },
            new Person { FirstName = "Евгений", LastName = "Елагин", Age = 35, Email = "pavel.f@yandex.ru", Id = "303" }
        };
        var filePath = Path.Combine(_testDirectory, "list_test.json");
        _testFiles.Add(filePath);
        _serializer.SaveListToFile(people, filePath);
        Assert.True(File.Exists(filePath));
        var content = File.ReadAllText(filePath);
        Assert.Contains("Юрий", content);
        Assert.Contains("Светлана", content);
        Assert.Contains("Павел", content);
    }

    [Fact]
    public void SaveListToFile_ShouldThrowArgumentNullException_WhenListIsNull()
    {
        var filePath = Path.Combine(_testDirectory, "null_list_test.json");
        _testFiles.Add(filePath);
        Assert.Throws<ArgumentNullException>(() => _serializer.SaveListToFile(null!, filePath));
    }

    [Fact]
    public void LoadListFromFile_ShouldReturnListOfPersons()
    {
        var people = new List<Person>
        {
            new Person { FirstName = "Константин", LastName = "Морозов", Age = 25, Email = "konstantin.m@inbox.ru", Id = "401" },
            new Person { FirstName = "Марина", LastName = "Павлова", Age = 30, Email = "marina.p@rambler.ru", Id = "402" }
        };
        var filePath = Path.Combine(_testDirectory, "load_list_test.json");
        _testFiles.Add(filePath);
        _serializer.SaveListToFile(people, filePath);
        var loadedPeople = _serializer.LoadListFromFile(filePath);
        Assert.NotNull(loadedPeople);
        Assert.Equal(2, loadedPeople.Count);
        Assert.Equal("Константин", loadedPeople[0].FirstName);
        Assert.Equal("Марина", loadedPeople[1].FirstName);
    }

    [Fact]
    public void LoadListFromFile_ShouldReturnEmptyList_WhenFileIsEmpty()
    {
        var filePath = Path.Combine(_testDirectory, "empty_list_test.json");
        _testFiles.Add(filePath);
        File.WriteAllText(filePath, "");
        var loadedPeople = _serializer.LoadListFromFile(filePath);
        Assert.NotNull(loadedPeople);
        Assert.Empty(loadedPeople);
    }

    [Fact]
    public void LoadListFromFile_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
    {
        var filePath = Path.Combine(_testDirectory, "nonexistent_list.json");
        Assert.Throws<FileNotFoundException>(() => _serializer.LoadListFromFile(filePath));
    }

    public void Dispose()
    {
        foreach (var file in _testFiles)
        {
            try
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
            catch { }
        }
        try
        {
            if (Directory.Exists(_testDirectory))
                Directory.Delete(_testDirectory, true);
        }
        catch { }
    }
}
