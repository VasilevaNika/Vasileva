# Лабораторная работа 1

Сериализация JSON, управление ресурсами и работа с файлами в C#

## Структура проекта

```
lab1/
├── Person.cs                    # Класс Person с атрибутами JSON
├── PersonSerializer.cs          # Класс для сериализации и работы с файлами
├── FileResourceManager.cs       # Класс для управления файловыми ресурсами
├── Program.cs                   # Демонстрация работы классов
├── Lab1.csproj                  # Файл проекта
└── tests/                       # Папка с тестами
    ├── PersonTests.cs           # Тесты для класса Person
    ├── PersonSerializerTests.cs # Тесты для класса PersonSerializer
    ├── FileResourceManagerTests.cs # Тесты для класса FileResourceManager
    └── Lab1.Tests.csproj        # Файл тестового проекта
```

## Описание классов

### Person

Класс представляет информацию о человеке с использованием атрибутов JSON:

- `FirstName`, `LastName`, `Age` - основные свойства
- `Password` - свойство с атрибутом `[JsonIgnore]` (не сериализуется)
- `Id` - свойство с атрибутом `[JsonPropertyName("personId")]` (сериализуется как "personId")
- `BirthDate` - приватное поле с атрибутом `[JsonInclude]`
- `Email` - свойство с валидацией (проверка наличия символа '@')
- `PhoneNumber` - свойство с атрибутом `[JsonPropertyName("phone")]`
- `FullName` - вычисляемое свойство (только для чтения)
- `IsAdult` - вычисляемое свойство (только для чтения)

### PersonSerializer

Класс для сериализации и десериализации объектов Person в JSON:

- `SerializeToJson(Person person)` - сериализация в строку JSON
- `DeserializeFromJson(string json)` - десериализация из строки JSON
- `SaveToFile(Person person, string filePath)` - сохранение в файл (синхронно)
- `LoadFromFile(string filePath)` - загрузка из файла (синхронно)
- `SaveToFileAsync(Person person, string filePath)` - сохранение в файл (асинхронно)
- `LoadFromFileAsync(string filePath)` - загрузка из файла (асинхронно)
- `SaveListToFile(List<Person> people, string filePath)` - сохранение списка в файл
- `LoadListFromFile(string filePath)` - загрузка списка из файла

Использует `SemaphoreSlim` для потокобезопасности операций с файлами.

### FileResourceManager

Класс для управления файловыми ресурсами, реализует интерфейс `IDisposable`:

- `OpenForWriting()` - открытие файла для записи
- `OpenForReading()` - открытие файла для чтения
- `WriteLine(string text)` - запись строки в файл
- `ReadAllText()` - чтение всего содержимого файла
- `AppendText(string text)` - добавление текста в конец файла
- `GetFileInfo()` - получение информации о файле
- `Dispose()` - освобождение ресурсов

## Запуск программы

```bash
dotnet restore
dotnet build
dotnet run
```

Программа демонстрирует работу всех трех классов:
- Создание и использование объектов Person
- Сериализация и десериализация в JSON
- Работа с файлами через FileResourceManager

## Запуск тестов

```bash
dotnet test tests/Lab1.Tests.csproj
```

Или из корневой директории:

```bash
dotnet test
```

## Зависимости

### Основной проект (Lab1.csproj)
- `System.Text.Json` версия 8.0.5

### Тестовый проект (tests/Lab1.Tests.csproj)
- `Microsoft.NET.Test.Sdk` версия 17.8.0
- `xunit` версия 2.6.2
- `xunit.runner.visualstudio` версия 2.5.4
- `coverlet.collector` версия 6.0.0

## Требования

- .NET 8.0 SDK или выше
