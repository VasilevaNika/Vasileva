using System.Text;
using System.Text.Json;

namespace Lab1;

public class PersonSerializer
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    private static readonly SemaphoreSlim _fileSemaphore = new(1, 1);
    private static readonly string _logFilePath = "errors.log";

    public string SerializeToJson(Person person)
    {
        try
        {
            return JsonSerializer.Serialize(person, _jsonOptions);
        }
        catch (Exception ex)
        {
            LogError($"Ошибка при сериализации: {ex.Message}");
            throw;
        }
    }

    public Person DeserializeFromJson(string json)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("JSON строка не может быть пустой");
            }

            var person = JsonSerializer.Deserialize<Person>(json, _jsonOptions);
            if (person == null)
            {
                throw new InvalidOperationException("Не удалось десериализовать объект Person");
            }
            return person;
        }
        catch (Exception ex)
        {
            LogError($"Ошибка при десериализации: {ex.Message}");
            throw;
        }
    }

    public void SaveToFile(Person person, string filePath)
    {
        _fileSemaphore.Wait();
        try
        {
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            var json = SerializeToJson(person);
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, json, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            LogError($"Ошибка при сохранении в файл {filePath}: {ex.Message}");
            throw;
        }
        finally
        {
            _fileSemaphore.Release();
        }
    }

    public Person LoadFromFile(string filePath)
    {
        _fileSemaphore.Wait();
        try
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Файл не найден: {filePath}");
            }

            var json = File.ReadAllText(filePath, Encoding.UTF8);
            return DeserializeFromJson(json);
        }
        catch (Exception ex)
        {
            LogError($"Ошибка при загрузке из файла {filePath}: {ex.Message}");
            throw;
        }
        finally
        {
            _fileSemaphore.Release();
        }
    }

    public async Task SaveToFileAsync(Person person, string filePath)
    {
        await _fileSemaphore.WaitAsync();
        try
        {
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            var json = SerializeToJson(person);
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(filePath, json, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            LogError($"Ошибка при асинхронном сохранении в файл {filePath}: {ex.Message}");
            throw;
        }
        finally
        {
            _fileSemaphore.Release();
        }
    }

    public async Task<Person> LoadFromFileAsync(string filePath)
    {
        await _fileSemaphore.WaitAsync();
        try
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Файл не найден: {filePath}");
            }

            var json = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
            return DeserializeFromJson(json);
        }
        catch (Exception ex)
        {
            LogError($"Ошибка при асинхронной загрузке из файла {filePath}: {ex.Message}");
            throw;
        }
        finally
        {
            _fileSemaphore.Release();
        }
    }

    public void SaveListToFile(List<Person> people, string filePath)
    {
        _fileSemaphore.Wait();
        try
        {
            if (people == null)
            {
                throw new ArgumentNullException(nameof(people));
            }

            var json = JsonSerializer.Serialize(people, _jsonOptions);
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, json, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            LogError($"Ошибка при сохранении списка в файл {filePath}: {ex.Message}");
            throw;
        }
        finally
        {
            _fileSemaphore.Release();
        }
    }

    public List<Person> LoadListFromFile(string filePath)
    {
        _fileSemaphore.Wait();
        try
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Файл не найден: {filePath}");
            }

            var json = File.ReadAllText(filePath, Encoding.UTF8);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Person>();
            }

            var people = JsonSerializer.Deserialize<List<Person>>(json, _jsonOptions);
            return people ?? new List<Person>();
        }
        catch (Exception ex)
        {
            LogError($"Ошибка при загрузке списка из файла {filePath}: {ex.Message}");
            throw;
        }
        finally
        {
            _fileSemaphore.Release();
        }
    }

    private static void LogError(string message)
    {
        try
        {
            var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";
            File.AppendAllText(_logFilePath, logMessage, Encoding.UTF8);
        }
        catch
        {
        }
    }
}
