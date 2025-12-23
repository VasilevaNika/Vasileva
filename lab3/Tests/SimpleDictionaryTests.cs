using Xunit;
using CollectionsLab;
using System.Linq;

namespace CollectionsLab.Tests;

public class SimpleDictionaryTests
{
    [Fact]
    public void Constructor_Default_CreatesEmptyDictionary()
    {
        var dict = new SimpleDictionary<string, int>();
        
        Assert.Equal(0, dict.Count);
        Assert.False(dict.IsReadOnly);
    }

    [Fact]
    public void Constructor_WithCapacity_CreatesEmptyDictionary()
    {
        var dict = new SimpleDictionary<string, int>(32);
        
        Assert.Equal(0, dict.Count);
    }

    [Fact]
    public void Constructor_WithDictionary_CopiesItems()
    {
        var source = new Dictionary<string, int> { { "a", 1 }, { "b", 2 } };
        var dict = new SimpleDictionary<string, int>(source);
        
        Assert.Equal(2, dict.Count);
        Assert.Equal(1, dict["a"]);
        Assert.Equal(2, dict["b"]);
    }

    [Fact]
    public void Constructor_NullDictionary_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new SimpleDictionary<string, int>(null!));
    }

    [Fact]
    public void Add_KeyValue_AddsItem()
    {
        var dict = new SimpleDictionary<string, int>();
        
        dict.Add("студент", 100);
        
        Assert.Equal(1, dict.Count);
        Assert.True(dict.ContainsKey("студент"));
    }

    [Fact]
    public void Add_DuplicateKey_ThrowsException()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("студент", 100);
        
        Assert.Throws<ArgumentException>(() => dict.Add("студент", 200));
    }

    [Fact]
    public void Add_NullKey_ThrowsException()
    {
        var dict = new SimpleDictionary<string, int>();
        
        Assert.Throws<ArgumentNullException>(() => dict.Add(null!, 100));
    }

    [Fact]
    public void Add_KeyValuePair_AddsItem()
    {
        var dict = new SimpleDictionary<string, int>();
        
        dict.Add(new KeyValuePair<string, int>("студент", 100));
        
        Assert.Equal(1, dict.Count);
        Assert.True(dict.ContainsKey("студент"));
    }

    [Fact]
    public void Indexer_Get_ReturnsValue()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("студент", 100);
        
        Assert.Equal(100, dict["студент"]);
    }

    [Fact]
    public void Indexer_Get_NonExistingKey_ThrowsException()
    {
        var dict = new SimpleDictionary<string, int>();
        
        Assert.Throws<KeyNotFoundException>(() => dict["nonexistent"]);
    }

    [Fact]
    public void Indexer_Set_AddsOrUpdatesValue()
    {
        var dict = new SimpleDictionary<string, int>();
        
        dict["студент"] = 100;
        Assert.Equal(100, dict["студент"]);
        Assert.Equal(1, dict.Count);
        
        dict["студент"] = 200;
        Assert.Equal(200, dict["студент"]);
        Assert.Equal(1, dict.Count);
    }

    [Fact]
    public void Indexer_Set_NullKey_ThrowsException()
    {
        var dict = new SimpleDictionary<string, int>();
        
        Assert.Throws<ArgumentNullException>(() => dict[null!] = 100);
    }

    [Fact]
    public void ContainsKey_ExistingKey_ReturnsTrue()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("студент", 100);
        
        Assert.True(dict.ContainsKey("студент"));
    }

    [Fact]
    public void ContainsKey_NonExistingKey_ReturnsFalse()
    {
        var dict = new SimpleDictionary<string, int>();
        
        Assert.False(dict.ContainsKey("nonexistent"));
    }

    [Fact]
    public void ContainsKey_NullKey_ThrowsException()
    {
        var dict = new SimpleDictionary<string, int>();
        
        Assert.Throws<ArgumentNullException>(() => dict.ContainsKey(null!));
    }

    [Fact]
    public void TryGetValue_ExistingKey_ReturnsTrueAndValue()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("студент", 100);
        
        bool result = dict.TryGetValue("студент", out int value);
        
        Assert.True(result);
        Assert.Equal(100, value);
    }

    [Fact]
    public void TryGetValue_NonExistingKey_ReturnsFalse()
    {
        var dict = new SimpleDictionary<string, int>();
        
        bool result = dict.TryGetValue("nonexistent", out int value);
        
        Assert.False(result);
        Assert.Equal(0, value);
    }

    [Fact]
    public void TryGetValue_NullKey_ThrowsException()
    {
        var dict = new SimpleDictionary<string, int>();
        
        Assert.Throws<ArgumentNullException>(() => dict.TryGetValue(null!, out int _));
    }

    [Fact]
    public void Remove_ExistingKey_RemovesItem()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("студент", 100);
        dict.Add("преподаватель", 200);
        
        bool result = dict.Remove("студент");
        
        Assert.True(result);
        Assert.Equal(1, dict.Count);
        Assert.False(dict.ContainsKey("студент"));
        Assert.True(dict.ContainsKey("преподаватель"));
    }

    [Fact]
    public void Remove_NonExistingKey_ReturnsFalse()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("студент", 100);
        
        bool result = dict.Remove("nonexistent");
        
        Assert.False(result);
        Assert.Equal(1, dict.Count);
    }

    [Fact]
    public void Remove_NullKey_ThrowsException()
    {
        var dict = new SimpleDictionary<string, int>();
        
        Assert.Throws<ArgumentNullException>(() => dict.Remove(null!));
    }

    [Fact]
    public void Remove_KeyValuePair_RemovesWhenMatch()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("студент", 100);
        
        bool result = dict.Remove(new KeyValuePair<string, int>("студент", 100));
        
        Assert.True(result);
        Assert.Equal(0, dict.Count);
    }

    [Fact]
    public void Remove_KeyValuePair_WrongValue_ReturnsFalse()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("студент", 100);
        
        bool result = dict.Remove(new KeyValuePair<string, int>("студент", 200));
        
        Assert.False(result);
        Assert.Equal(1, dict.Count);
    }

    [Fact]
    public void Contains_ExistingPair_ReturnsTrue()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("студент", 100);
        
        Assert.True(dict.Contains(new KeyValuePair<string, int>("студент", 100)));
    }

    [Fact]
    public void Contains_NonExistingPair_ReturnsFalse()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("студент", 100);
        
        Assert.False(dict.Contains(new KeyValuePair<string, int>("студент", 200)));
        Assert.False(dict.Contains(new KeyValuePair<string, int>("преподаватель", 100)));
    }

    [Fact]
    public void Clear_RemovesAllItems()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("студент", 100);
        dict.Add("преподаватель", 200);
        
        dict.Clear();
        
        Assert.Equal(0, dict.Count);
        Assert.False(dict.ContainsKey("студент"));
        Assert.False(dict.ContainsKey("преподаватель"));
    }

    [Fact]
    public void CopyTo_CopiesAllItems()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("студент", 100);
        dict.Add("преподаватель", 200);
        
        var array = new KeyValuePair<string, int>[3];
        dict.CopyTo(array, 1);
        
        Assert.Equal(default(KeyValuePair<string, int>), array[0]);
        Assert.True(array[1].Key == "студент" || array[1].Key == "преподаватель");
        Assert.True(array[2].Key == "студент" || array[2].Key == "преподаватель");
    }

    [Fact]
    public void CopyTo_NullArray_ThrowsException()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("студент", 100);
        
        Assert.Throws<ArgumentNullException>(() => dict.CopyTo(null!, 0));
    }

    [Fact]
    public void GetEnumerator_EnumeratesAllItems()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("студент", 100);
        dict.Add("преподаватель", 200);
        dict.Add("деканат", 300);
        
        var items = new List<KeyValuePair<string, int>>();
        foreach (var kvp in dict)
        {
            items.Add(kvp);
        }
        
        Assert.Equal(3, items.Count);
        Assert.True(items.Any(kvp => kvp.Key == "студент" && kvp.Value == 100));
        Assert.True(items.Any(kvp => kvp.Key == "преподаватель" && kvp.Value == 200));
        Assert.True(items.Any(kvp => kvp.Key == "деканат" && kvp.Value == 300));
    }

    [Fact]
    public void Keys_Collection_ContainsAllKeys()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("студент", 100);
        dict.Add("преподаватель", 200);
        
        var keys = dict.Keys.ToList();
        
        Assert.Equal(2, keys.Count);
        Assert.Contains("студент", keys);
        Assert.Contains("преподаватель", keys);
    }

    [Fact]
    public void Values_Collection_ContainsAllValues()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("студент", 100);
        dict.Add("преподаватель", 200);
        
        var values = dict.Values.ToList();
        
        Assert.Equal(2, values.Count);
        Assert.Contains(100, values);
        Assert.Contains(200, values);
    }

    [Fact]
    public void IReadOnlyDictionary_Keys_Works()
    {
        IReadOnlyDictionary<string, int> dict = new SimpleDictionary<string, int>();
        ((SimpleDictionary<string, int>)dict).Add("студент", 100);
        
        Assert.True(dict.ContainsKey("студент"));
        Assert.Equal(100, dict["студент"]);
    }

    [Fact]
    public void Resize_WhenLoadFactorExceeded_WorksCorrectly()
    {
        var dict = new SimpleDictionary<int, string>();
        
        for (int i = 0; i < 100; i++)
        {
            dict.Add(i, $"value{i}");
        }
        
        Assert.Equal(100, dict.Count);
        
        for (int i = 0; i < 100; i++)
        {
            Assert.Equal($"value{i}", dict[i]);
        }
    }

    [Fact]
    public void MultipleOperations_WorksCorrectly()
    {
        var dict = new SimpleDictionary<string, int>();
        
        dict.Add("a", 1);
        dict.Add("b", 2);
        Assert.Equal(2, dict.Count);
        
        dict["a"] = 10;
        Assert.Equal(10, dict["a"]);
        
        dict.Remove("b");
        Assert.Equal(1, dict.Count);
        Assert.False(dict.ContainsKey("b"));
        
        dict.Clear();
        Assert.Equal(0, dict.Count);
    }

    [Fact]
    public void Enumerator_ModificationDuringEnumeration_ThrowsException()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("студент", 100);
        dict.Add("преподаватель", 200);
        
        var enumerator = dict.GetEnumerator();
        enumerator.MoveNext();
        dict.Add("деканат", 300);
        
        Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
    }
}

