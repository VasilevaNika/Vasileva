# Лабораторная работа 1

Cериализация JSON, управление ресурсами и работа с файлами в C#

##  Цели работы:
1. Освоить использование атрибутов JSON для управления процессом сериализации/десериализации.
2. Изучить паттерн IDisposable для ĸорреĸтного освобождения ресурсов.
3. Научиться работать с файловой системой (чтение/запись файлов).
4. Реализовать сохранение и загрузĸу объеĸтов в/из файлов JSON.
5. Понимать разницу между явным освобождением ресурсов через Dispose() и автоматичесĸим через
финализатор.

## Структура проекта

- Person.cs - класс Person с атрибутами JSON
- PersonSerializer.cs - класс для сериализации и работы с файлами
- FileResourceManager.cs - класс для управления файловыми ресурсами
- Program.cs - тесты

## Задание 1: Класс Person

Класс Person содержит свойства:
- FirstName, LastName, Age - обычные свойства
- Password - с [JsonIgnore]
- Id - с [JsonPropertyName("personId")]
- _birthDate - приватное поле с [JsonInclude]
- Email - с валидацией (проверка '@')
- PhoneNumber - с [JsonPropertyName("phone")]
- FullName - только для чтения
- IsAdult - только для чтения

## Задание 2: Класс PersonSerializer

Методы:
1. SerializeToJson - сериализация в строку
2. DeserializeFromJson - десериализация из строки
3. SaveToFile - сохранение в файл (синхронно)
4. LoadFromFile - загрузка из файла (синхронно)
5. SaveToFileAsync - сохранение в файл (асинхронно)
6. LoadFromFileAsync - загрузка из файла (асинхронно)
7. SaveListToFile - экспорт списка объектов
8. LoadListFromFile - импорт из файла

## Задание 3: Класс FileResourceManager

Класс реализует IDisposable и предоставляет методы:
- OpenForWriting - открытие для записи
- OpenForReading - открытие для чтения
- WriteLine - запись строки
- ReadAllText - чтение всего файла
- AppendText - добавление текста
- GetFileInfo - информация о файле

## Задание 4: Тестирование

В Program.cs реализованы тесты для всех классов.

## Запуск

```
dotnet restore
dotnet build
dotnet run
```
