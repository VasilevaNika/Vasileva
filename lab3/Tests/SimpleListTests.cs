using Xunit;
using CollectionsLab;

namespace CollectionsLab.Tests;

public class SimpleListTests
{
    [Fact]
    public void Constructor_Default_CreatesEmptyList()
    {
        var list = new SimpleList();
        
        Assert.Equal(0, list.Count);
        Assert.False(list.IsReadOnly);
        Assert.False(list.IsFixedSize);
    }

    [Fact]
    public void Constructor_WithCapacity_CreatesEmptyList()
    {
        var list = new SimpleList(10);
        
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void Constructor_NegativeCapacity_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new SimpleList(-1));
    }

    [Fact]
    public void Add_Item_IncreasesCount()
    {
        var list = new SimpleList();
        
        list.Add("стол");
        list.Add("стул");
        
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void Add_Item_ReturnsIndex()
    {
        var list = new SimpleList();
        
        int index1 = list.Add("книга");
        int index2 = list.Add("ручка");
        
        Assert.Equal(0, index1);
        Assert.Equal(1, index2);
    }

    [Fact]
    public void Indexer_Get_ReturnsCorrectValue()
    {
        var list = new SimpleList();
        list.Add("окно");
        list.Add("дверь");
        
        Assert.Equal("окно", list[0]);
        Assert.Equal("дверь", list[1]);
    }

    [Fact]
    public void Indexer_Get_InvalidIndex_ThrowsException()
    {
        var list = new SimpleList();
        list.Add("лампа");
        
        Assert.Throws<ArgumentOutOfRangeException>(() => list[-1]);
        Assert.Throws<ArgumentOutOfRangeException>(() => list[1]);
    }

    [Fact]
    public void Indexer_Set_UpdatesValue()
    {
        var list = new SimpleList();
        list.Add("компьютер");
        list.Add("монитор");
        
        list[0] = "ноутбук";
        
        Assert.Equal("ноутбук", list[0]);
        Assert.Equal("монитор", list[1]);
    }

    [Fact]
    public void Indexer_Set_InvalidIndex_ThrowsException()
    {
        var list = new SimpleList();
        list.Add("item1");
        
        Assert.Throws<ArgumentOutOfRangeException>(() => list[-1] = "value");
        Assert.Throws<ArgumentOutOfRangeException>(() => list[1] = "value");
    }

    [Fact]
    public void Contains_ExistingItem_ReturnsTrue()
    {
        var list = new SimpleList();
        list.Add("телефон");
        list.Add("планшет");
        
        Assert.True(list.Contains("телефон"));
        Assert.True(list.Contains("планшет"));
    }

    [Fact]
    public void Contains_NonExistingItem_ReturnsFalse()
    {
        var list = new SimpleList();
        list.Add("мышь");
        
        Assert.False(list.Contains("клавиатура"));
    }

    [Fact]
    public void Contains_Null_ReturnsTrueWhenNullExists()
    {
        var list = new SimpleList();
        list.Add("item1");
        list.Add(null);
        
        Assert.True(list.Contains(null));
    }

    [Fact]
    public void IndexOf_ExistingItem_ReturnsCorrectIndex()
    {
        var list = new SimpleList();
        list.Add("яблоко");
        list.Add("банан");
        list.Add("апельсин");
        
        Assert.Equal(0, list.IndexOf("яблоко"));
        Assert.Equal(1, list.IndexOf("банан"));
        Assert.Equal(2, list.IndexOf("апельсин"));
    }

    [Fact]
    public void IndexOf_NonExistingItem_ReturnsMinusOne()
    {
        var list = new SimpleList();
        list.Add("груша");
        
        Assert.Equal(-1, list.IndexOf("виноград"));
    }

    [Fact]
    public void Remove_ExistingItem_RemovesItem()
    {
        var list = new SimpleList();
        list.Add("красный");
        list.Add("синий");
        list.Add("зеленый");
        
        list.Remove("синий");
        
        Assert.Equal(2, list.Count);
        Assert.False(list.Contains("синий"));
        Assert.Equal("красный", list[0]);
        Assert.Equal("зеленый", list[1]);
    }

    [Fact]
    public void Remove_NonExistingItem_DoesNothing()
    {
        var list = new SimpleList();
        list.Add("белый");
        
        list.Remove("черный");
        
        Assert.Equal(1, list.Count);
        Assert.True(list.Contains("белый"));
    }

    [Fact]
    public void RemoveAt_ValidIndex_RemovesItem()
    {
        var list = new SimpleList();
        list.Add("item1");
        list.Add("item2");
        list.Add("item3");
        
        list.RemoveAt(1);
        
        Assert.Equal(2, list.Count);
        Assert.Equal("item1", list[0]);
        Assert.Equal("item3", list[1]);
    }

    [Fact]
    public void RemoveAt_InvalidIndex_ThrowsException()
    {
        var list = new SimpleList();
        list.Add("item1");
        
        Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveAt(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveAt(1));
    }

    [Fact]
    public void Insert_ValidIndex_InsertsItem()
    {
        var list = new SimpleList();
        list.Add("понедельник");
        list.Add("среда");
        
        list.Insert(1, "вторник");
        
        Assert.Equal(3, list.Count);
        Assert.Equal("понедельник", list[0]);
        Assert.Equal("вторник", list[1]);
        Assert.Equal("среда", list[2]);
    }

    [Fact]
    public void Insert_AtBeginning_InsertsItem()
    {
        var list = new SimpleList();
        list.Add("item2");
        
        list.Insert(0, "item1");
        
        Assert.Equal(2, list.Count);
        Assert.Equal("item1", list[0]);
        Assert.Equal("item2", list[1]);
    }

    [Fact]
    public void Insert_AtEnd_InsertsItem()
    {
        var list = new SimpleList();
        list.Add("item1");
        
        list.Insert(1, "item2");
        
        Assert.Equal(2, list.Count);
        Assert.Equal("item1", list[0]);
        Assert.Equal("item2", list[1]);
    }

    [Fact]
    public void Insert_InvalidIndex_ThrowsException()
    {
        var list = new SimpleList();
        list.Add("item1");
        
        Assert.Throws<ArgumentOutOfRangeException>(() => list.Insert(-1, "item"));
        Assert.Throws<ArgumentOutOfRangeException>(() => list.Insert(2, "item"));
    }

    [Fact]
    public void Clear_RemovesAllItems()
    {
        var list = new SimpleList();
        list.Add("item1");
        list.Add("item2");
        list.Add("item3");
        
        list.Clear();
        
        Assert.Equal(0, list.Count);
        Assert.False(list.Contains("item1"));
    }

    [Fact]
    public void CopyTo_ValidArray_CopiesItems()
    {
        var list = new SimpleList();
        list.Add("item1");
        list.Add("item2");
        list.Add("item3");
        
        object[] array = new object[5];
        list.CopyTo(array, 1);
        
        Assert.Null(array[0]);
        Assert.Equal("item1", array[1]);
        Assert.Equal("item2", array[2]);
        Assert.Equal("item3", array[3]);
        Assert.Null(array[4]);
    }

    [Fact]
    public void CopyTo_NullArray_ThrowsException()
    {
        var list = new SimpleList();
        list.Add("item1");
        
        Assert.Throws<ArgumentNullException>(() => list.CopyTo(null!, 0));
    }

    [Fact]
    public void CopyTo_InvalidIndex_ThrowsException()
    {
        var list = new SimpleList();
        list.Add("item1");
        object[] array = new object[1];
        
        Assert.Throws<ArgumentOutOfRangeException>(() => list.CopyTo(array, -1));
    }

    [Fact]
    public void CopyTo_ArrayTooSmall_ThrowsException()
    {
        var list = new SimpleList();
        list.Add("item1");
        list.Add("item2");
        object[] array = new object[1];
        
        Assert.Throws<ArgumentException>(() => list.CopyTo(array, 0));
    }

    [Fact]
    public void GetEnumerator_EnumeratesAllItems()
    {
        var list = new SimpleList();
        list.Add("зима");
        list.Add("весна");
        list.Add("лето");
        
        var items = new List<object?>();
        foreach (var item in list)
        {
            items.Add(item);
        }
        
        Assert.Equal(3, items.Count);
        Assert.Equal("зима", items[0]);
        Assert.Equal("весна", items[1]);
        Assert.Equal("лето", items[2]);
    }

    [Fact]
    public void Capacity_ExpandsWhenNeeded()
    {
        var list = new SimpleList(2);
        
        for (int i = 0; i < 10; i++)
        {
            list.Add($"item{i}");
        }
        
        Assert.Equal(10, list.Count);
        for (int i = 0; i < 10; i++)
        {
            Assert.Equal($"item{i}", list[i]);
        }
    }

    [Fact]
    public void MultipleOperations_WorksCorrectly()
    {
        var list = new SimpleList();
        
        list.Add("один");
        list.Add("два");
        list.Add("три");
        Assert.Equal(3, list.Count);
        
        list.Insert(1, "полтора");
        Assert.Equal(4, list.Count);
        Assert.Equal("полтора", list[1]);
        
        list.Remove("два");
        Assert.Equal(3, list.Count);
        Assert.False(list.Contains("два"));
        
        list[0] = "единица";
        Assert.Equal("единица", list[0]);
        
        list.Clear();
        Assert.Equal(0, list.Count);
    }
}

