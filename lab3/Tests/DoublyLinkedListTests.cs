using Xunit;
using CollectionsLab;

namespace CollectionsLab.Tests;

public class DoublyLinkedListTests
{
    [Fact]
    public void Constructor_CreatesEmptyList()
    {
        var list = new DoublyLinkedList();
        
        Assert.Equal(0, list.Count);
        Assert.False(list.IsReadOnly);
        Assert.False(list.IsFixedSize);
    }

    [Fact]
    public void Add_Item_IncreasesCount()
    {
        var list = new DoublyLinkedList();
        
        list.Add("первый");
        list.Add("второй");
        
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void Add_Item_ReturnsIndex()
    {
        var list = new DoublyLinkedList();
        
        int index1 = list.Add("первый");
        int index2 = list.Add("второй");
        
        Assert.Equal(0, index1);
        Assert.Equal(1, index2);
    }

    [Fact]
    public void AddLast_Item_AddsToEnd()
    {
        var list = new DoublyLinkedList();
        
        list.AddLast("первый");
        list.AddLast("второй");
        
        Assert.Equal(2, list.Count);
        Assert.Equal("первый", list[0]);
        Assert.Equal("второй", list[1]);
    }

    [Fact]
    public void AddFirst_Item_AddsToBeginning()
    {
        var list = new DoublyLinkedList();
        
        list.AddFirst("второй");
        list.AddFirst("первый");
        
        Assert.Equal(2, list.Count);
        Assert.Equal("первый", list[0]);
        Assert.Equal("второй", list[1]);
    }

    [Fact]
    public void Indexer_Get_ReturnsCorrectValue()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        list.Add("второй");
        list.Add("третий");
        
        Assert.Equal("первый", list[0]);
        Assert.Equal("второй", list[1]);
        Assert.Equal("третий", list[2]);
    }

    [Fact]
    public void Indexer_Get_InvalidIndex_ThrowsException()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        
        Assert.Throws<ArgumentOutOfRangeException>(() => list[-1]);
        Assert.Throws<ArgumentOutOfRangeException>(() => list[1]);
    }

    [Fact]
    public void Indexer_Set_UpdatesValue()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        list.Add("второй");
        
        list[0] = "updated";
        
        Assert.Equal("updated", list[0]);
        Assert.Equal("второй", list[1]);
    }

    [Fact]
    public void Indexer_Set_InvalidIndex_ThrowsException()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        
        Assert.Throws<ArgumentOutOfRangeException>(() => list[-1] = "value");
        Assert.Throws<ArgumentOutOfRangeException>(() => list[1] = "value");
    }

    [Fact]
    public void Contains_ExistingItem_ReturnsTrue()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        list.Add("второй");
        
        Assert.True(list.Contains("первый"));
        Assert.True(list.Contains("второй"));
    }

    [Fact]
    public void Contains_NonExistingItem_ReturnsFalse()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        
        Assert.False(list.Contains("второй"));
    }

    [Fact]
    public void IndexOf_ExistingItem_ReturnsCorrectIndex()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        list.Add("второй");
        list.Add("третий");
        
        Assert.Equal(0, list.IndexOf("первый"));
        Assert.Equal(1, list.IndexOf("второй"));
        Assert.Equal(2, list.IndexOf("третий"));
    }

    [Fact]
    public void IndexOf_NonExistingItem_ReturnsMinusOne()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        
        Assert.Equal(-1, list.IndexOf("второй"));
    }

    [Fact]
    public void Insert_AtBeginning_InsertsItem()
    {
        var list = new DoublyLinkedList();
        list.Add("второй");
        
        list.Insert(0, "первый");
        
        Assert.Equal(2, list.Count);
        Assert.Equal("первый", list[0]);
        Assert.Equal("второй", list[1]);
    }

    [Fact]
    public void Insert_AtMiddle_InsertsItem()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        list.Add("третий");
        
        list.Insert(1, "второй");
        
        Assert.Equal(3, list.Count);
        Assert.Equal("первый", list[0]);
        Assert.Equal("второй", list[1]);
        Assert.Equal("третий", list[2]);
    }

    [Fact]
    public void Insert_AtEnd_InsertsItem()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        
        list.Insert(1, "второй");
        
        Assert.Equal(2, list.Count);
        Assert.Equal("первый", list[0]);
        Assert.Equal("второй", list[1]);
    }

    [Fact]
    public void Insert_InvalidIndex_ThrowsException()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        
        Assert.Throws<ArgumentOutOfRangeException>(() => list.Insert(-1, "item"));
        Assert.Throws<ArgumentOutOfRangeException>(() => list.Insert(2, "item"));
    }

    [Fact]
    public void Remove_ExistingItem_RemovesItem()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        list.Add("второй");
        list.Add("третий");
        
        list.Remove("второй");
        
        Assert.Equal(2, list.Count);
        Assert.False(list.Contains("второй"));
        Assert.Equal("первый", list[0]);
        Assert.Equal("третий", list[1]);
    }

    [Fact]
    public void Remove_FirstItem_RemovesItem()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        list.Add("второй");
        
        list.Remove("первый");
        
        Assert.Equal(1, list.Count);
        Assert.Equal("второй", list[0]);
    }

    [Fact]
    public void Remove_LastItem_RemovesItem()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        list.Add("второй");
        
        list.Remove("второй");
        
        Assert.Equal(1, list.Count);
        Assert.Equal("первый", list[0]);
    }

    [Fact]
    public void Remove_NonExistingItem_DoesNothing()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        
        list.Remove("второй");
        
        Assert.Equal(1, list.Count);
        Assert.True(list.Contains("первый"));
    }

    [Fact]
    public void RemoveAt_ValidIndex_RemovesItem()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        list.Add("второй");
        list.Add("третий");
        
        list.RemoveAt(1);
        
        Assert.Equal(2, list.Count);
        Assert.Equal("первый", list[0]);
        Assert.Equal("третий", list[1]);
    }

    [Fact]
    public void RemoveAt_FirstIndex_RemovesItem()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        list.Add("второй");
        
        list.RemoveAt(0);
        
        Assert.Equal(1, list.Count);
        Assert.Equal("второй", list[0]);
    }

    [Fact]
    public void RemoveAt_LastIndex_RemovesItem()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        list.Add("второй");
        
        list.RemoveAt(1);
        
        Assert.Equal(1, list.Count);
        Assert.Equal("первый", list[0]);
    }

    [Fact]
    public void RemoveAt_InvalidIndex_ThrowsException()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        
        Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveAt(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveAt(1));
    }

    [Fact]
    public void RemoveFirst_RemovesFirstItem()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        list.Add("второй");
        
        list.RemoveFirst();
        
        Assert.Equal(1, list.Count);
        Assert.Equal("второй", list[0]);
    }

    [Fact]
    public void RemoveFirst_EmptyList_ThrowsException()
    {
        var list = new DoublyLinkedList();
        
        Assert.Throws<InvalidOperationException>(() => list.RemoveFirst());
    }

    [Fact]
    public void RemoveLast_RemovesLastItem()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        list.Add("второй");
        
        list.RemoveLast();
        
        Assert.Equal(1, list.Count);
        Assert.Equal("первый", list[0]);
    }

    [Fact]
    public void RemoveLast_EmptyList_ThrowsException()
    {
        var list = new DoublyLinkedList();
        
        Assert.Throws<InvalidOperationException>(() => list.RemoveLast());
    }

    [Fact]
    public void Clear_RemovesAllItems()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        list.Add("второй");
        list.Add("третий");
        
        list.Clear();
        
        Assert.Equal(0, list.Count);
        Assert.False(list.Contains("первый"));
    }

    [Fact]
    public void CopyTo_ValidArray_CopiesItems()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        list.Add("второй");
        list.Add("третий");
        
        object[] array = new object[5];
        list.CopyTo(array, 1);
        
        Assert.Null(array[0]);
        Assert.Equal("первый", array[1]);
        Assert.Equal("второй", array[2]);
        Assert.Equal("третий", array[3]);
        Assert.Null(array[4]);
    }

    [Fact]
    public void CopyTo_NullArray_ThrowsException()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        
        Assert.Throws<ArgumentNullException>(() => list.CopyTo(null!, 0));
    }

    [Fact]
    public void CopyTo_InvalidIndex_ThrowsException()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        object[] array = new object[1];
        
        Assert.Throws<ArgumentOutOfRangeException>(() => list.CopyTo(array, -1));
    }

    [Fact]
    public void CopyTo_ArrayTooSmall_ThrowsException()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        list.Add("второй");
        object[] array = new object[1];
        
        Assert.Throws<ArgumentException>(() => list.CopyTo(array, 0));
    }

    [Fact]
    public void GetEnumerator_EnumeratesAllItems()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        list.Add("второй");
        list.Add("третий");
        
        var items = new List<object?>();
        foreach (var item in list)
        {
            items.Add(item);
        }
        
        Assert.Equal(3, items.Count);
        Assert.Equal("первый", items[0]);
        Assert.Equal("второй", items[1]);
        Assert.Equal("третий", items[2]);
    }

    [Fact]
    public void GetEnumerator_ModificationDuringEnumeration_ThrowsException()
    {
        var list = new DoublyLinkedList();
        list.Add("первый");
        list.Add("второй");
        
        var enumerator = list.GetEnumerator();
        enumerator.MoveNext();
        list.Add("третий");
        
        Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
    }

    [Fact]
    public void GetNodeAt_OptimizedForEnd_WorksCorrectly()
    {
        var list = new DoublyLinkedList();
        for (int i = 0; i < 10; i++)
        {
            list.Add($"item{i}");
        }
        
        Assert.Equal("item9", list[9]);
        Assert.Equal("item8", list[8]);
    }

    [Fact]
    public void MultipleOperations_WorksCorrectly()
    {
        var list = new DoublyLinkedList();
        
        list.Add("a");
        list.Add("b");
        list.Add("c");
        Assert.Equal(3, list.Count);
        
        list.Insert(1, "x");
        Assert.Equal(4, list.Count);
        Assert.Equal("x", list[1]);
        
        list.Remove("b");
        Assert.Equal(3, list.Count);
        Assert.False(list.Contains("b"));
        
        list[0] = "A";
        Assert.Equal("A", list[0]);
        
        list.Clear();
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void AddFirstAndAddLast_WorksCorrectly()
    {
        var list = new DoublyLinkedList();
        
        list.AddLast("middle");
        list.AddFirst("first");
        list.AddLast("last");
        
        Assert.Equal(3, list.Count);
        Assert.Equal("first", list[0]);
        Assert.Equal("middle", list[1]);
        Assert.Equal("last", list[2]);
    }
}

