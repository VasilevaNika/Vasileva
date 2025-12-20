# Лабораторная работа 3: Реализация коллекций и интерфейсов


## Реализованные классы

### 1. SimpleList

Простая реализация списка с использованием динамического массива.

**Реализуемые интерфейсы:**
- `IEnumerable` — поддержка перебора элементов (foreach)
- `ICollection` — базовые операции: добавление, удаление, подсчёт элементов
- `IList` — индексированный доступ, вставка, удаление по индексу

**Основные методы и свойства:**
- `Count` — количество элементов
- `Add(object? value)` — добавление элемента
- `Remove(object? value)` — удаление элемента
- `RemoveAt(int index)` — удаление по индексу
- `Clear()` — очистка списка
- `Contains(object? value)` — проверка наличия элемента
- `IndexOf(object? value)` — поиск индекса элемента
- `Insert(int index, object? value)` — вставка элемента по индексу
- `this[int index]` — индексированный доступ

### 2. SimpleDictionary<TKey, TValue>

Реализация словаря с использованием хеш-таблицы с цепочками для разрешения коллизий.

**Реализуемые интерфейсы:**
- `IDictionary<TKey, TValue>` — словарь, работа с ключами и значениями
- `IReadOnlyDictionary<TKey, TValue>` — поддержка только чтения
- `ICollection<KeyValuePair<TKey, TValue>>` — коллекция пар ключ-значение
- `IEnumerable<KeyValuePair<TKey, TValue>>` — перебор элементов

**Основные методы и свойства:**
- `Count` — количество элементов
- `Keys` — коллекция ключей
- `Values` — коллекция значений
- `Add(TKey key, TValue value)` — добавление элемента
- `Remove(TKey key)` — удаление по ключу
- `ContainsKey(TKey key)` — проверка наличия ключа
- `TryGetValue(TKey key, out TValue value)` — безопасное получение значения
- `this[TKey key]` — индексированный доступ

**Особенности:**
- Автоматическое изменение размера при превышении коэффициента загрузки (0.75)
- Использование простых чисел для размера хеш-таблицы
- Поддержка перечисления с проверкой модификации коллекции

### 3. DoublyLinkedList

Реализация двунаправленного связного списка.

**Реализуемые интерфейсы:**
- `IList` — индексированный доступ, вставка, удаление по индексу
- `ICollection` — базовые операции
- `IEnumerable` — поддержка перебора элементов

**Основные методы и свойства:**
- `Count` — количество элементов
- `Add(object? value)` / `AddLast(object? value)` — добавление в конец
- `AddFirst(object? value)` — добавление в начало
- `Remove(object? value)` — удаление элемента
- `RemoveAt(int index)` — удаление по индексу
- `RemoveFirst()` — удаление первого элемента
- `RemoveLast()` — удаление последнего элемента
- `Insert(int index, object? value)` — вставка по индексу
- `Contains(object? value)` — проверка наличия
- `IndexOf(object? value)` — поиск индекса
- `this[int index]` — индексированный доступ (оптимизирован для доступа с конца)

## Тестирование

Проект использует xUnit для автоматизированного тестирования.

### Запуск тестов

```bash
dotnet test
```

### Покрытие тестами

Все классы имеют полное покрытие тестами, включая:
- Базовые операции (добавление, удаление, поиск)
- Граничные случаи (пустые коллекции, невалидные индексы)
- Обработку ошибок (исключения)
- Перечисление элементов
- Операции с null значениями
- Модификацию во время перечисления

## Структура проекта

```
lab3/
├── CollectionsLab.csproj          # Основной проект
├── Program.cs                      # Точка входа приложения
├── SimpleList.cs                   # Реализация SimpleList
├── SimpleDictionary.cs             # Реализация SimpleDictionary
├── DoublyLinkedList.cs             # Реализация DoublyLinkedList
├── Tests/                          # Папка с тестами
│   ├── SimpleListTests.cs          # Тесты для SimpleList
│   ├── SimpleDictionaryTests.cs    # Тесты для SimpleDictionary
│   └── DoublyLinkedListTests.cs    # Тесты для DoublyLinkedList
└── README.md                       # Документация
```

## Требования

- .NET 8.0 SDK или выше
- xUnit (устанавливается автоматически через NuGet)

## Примеры использования

### SimpleList

```csharp
var list = new SimpleList();
list.Add("item1");
list.Add("item2");
list.Insert(1, "item1.5");
list.Remove("item1");
Console.WriteLine(list[0]); // "item1.5"
```

### SimpleDictionary

```csharp
var dict = new SimpleDictionary<string, int>();
dict.Add("key1", 100);
dict["key2"] = 200;
if (dict.TryGetValue("key1", out int value))
{
    Console.WriteLine(value); // 100
}
```

### DoublyLinkedList

```csharp
var list = new DoublyLinkedList();
list.AddFirst("first");
list.AddLast("last");
list.Insert(1, "middle");
list.RemoveAt(0);
```

## Особенности реализации

1. **SimpleList**: Использует динамический массив с автоматическим увеличением размера (удвоение при необходимости).

2. **SimpleDictionary**: Реализует хеш-таблицу с цепочками для разрешения коллизий. Использует простые числа для размера таблицы и автоматически изменяет размер при превышении коэффициента загрузки.

3. **DoublyLinkedList**: Двунаправленный связный список с оптимизацией доступа по индексу (выбор направления обхода в зависимости от позиции индекса).

Все реализации являются детерминированными и воспроизводимыми.

