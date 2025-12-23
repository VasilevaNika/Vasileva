using System.Collections;

namespace CollectionsLab;

public class DoublyLinkedList : IList, ICollection, IEnumerable
{
    private Node? _head;
    private Node? _tail;
    private int _count;
    private int _version;

    private class Node
    {
        public object? Value;
        public Node? Next;
        public Node? Previous;

        public Node(object? value)
        {
            Value = value;
        }
    }

    public DoublyLinkedList()
    {
        _head = null;
        _tail = null;
        _count = 0;
        _version = 0;
    }

    public int Count => _count;

    public bool IsReadOnly => false;

    public bool IsFixedSize => false;

    public bool IsSynchronized => false;

    public object SyncRoot => this;

    public object? this[int index]
    {
        get
        {
            if (index < 0 || index >= _count)
                throw new ArgumentOutOfRangeException(nameof(index));
            
            return GetNodeAt(index).Value;
        }
        set
        {
            if (index < 0 || index >= _count)
                throw new ArgumentOutOfRangeException(nameof(index));
            
            GetNodeAt(index).Value = value;
            _version++;
        }
    }

    public int Add(object? value)
    {
        AddLast(value);
        return _count - 1;
    }

    public void AddLast(object? value)
    {
        Node newNode = new Node(value);
        
        if (_tail == null)
        {
            _head = newNode;
            _tail = newNode;
        }
        else
        {
            _tail.Next = newNode;
            newNode.Previous = _tail;
            _tail = newNode;
        }
        
        _count++;
        _version++;
    }

    public void AddFirst(object? value)
    {
        Node newNode = new Node(value);
        
        if (_head == null)
        {
            _head = newNode;
            _tail = newNode;
        }
        else
        {
            newNode.Next = _head;
            _head.Previous = newNode;
            _head = newNode;
        }
        
        _count++;
        _version++;
    }

    public void Clear()
    {
        Node? current = _head;
        while (current != null)
        {
            Node? next = current.Next;
            current.Previous = null;
            current.Next = null;
            current = next;
        }
        
        _head = null;
        _tail = null;
        _count = 0;
        _version++;
    }

    public bool Contains(object? value)
    {
        return IndexOf(value) >= 0;
    }

    public void CopyTo(Array array, int index)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));
        
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index));
        
        if (array.Length - index < _count)
            throw new ArgumentException("Destination array is not long enough.");
        
        Node? current = _head;
        int arrayIndex = index;
        while (current != null)
        {
            array.SetValue(current.Value, arrayIndex++);
            current = current.Next;
        }
    }

    public IEnumerator GetEnumerator()
    {
        int version = _version;
        Node? current = _head;
        
        while (current != null)
        {
            if (version != _version)
                throw new InvalidOperationException("Collection was modified during enumeration.");
            
            yield return current.Value;
            current = current.Next;
        }
    }

    public int IndexOf(object? value)
    {
        Node? current = _head;
        int index = 0;
        
        while (current != null)
        {
            if (Equals(current.Value, value))
                return index;
            
            current = current.Next;
            index++;
        }
        
        return -1;
    }

    public void Insert(int index, object? value)
    {
        if (index < 0 || index > _count)
            throw new ArgumentOutOfRangeException(nameof(index));
        
        if (index == 0)
        {
            AddFirst(value);
            return;
        }
        
        if (index == _count)
        {
            AddLast(value);
            return;
        }
        
        Node nodeAtPosition = GetNodeAt(index);
        Node newNode = new Node(value);
        
        newNode.Previous = nodeAtPosition.Previous;
        newNode.Next = nodeAtPosition;
        
        if (nodeAtPosition.Previous != null)
            nodeAtPosition.Previous.Next = newNode;
        
        nodeAtPosition.Previous = newNode;
        
        _count++;
        _version++;
    }

    public void Remove(object? value)
    {
        Node? current = _head;
        
        while (current != null)
        {
            if (Equals(current.Value, value))
            {
                RemoveNode(current);
                return;
            }
            
            current = current.Next;
        }
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _count)
            throw new ArgumentOutOfRangeException(nameof(index));
        
        Node nodeToRemove = GetNodeAt(index);
        RemoveNode(nodeToRemove);
    }

    public void RemoveFirst()
    {
        if (_head == null)
            throw new InvalidOperationException("The list is empty.");
        
        RemoveNode(_head);
    }

    public void RemoveLast()
    {
        if (_tail == null)
            throw new InvalidOperationException("The list is empty.");
        
        RemoveNode(_tail);
    }

    private Node GetNodeAt(int index)
    {
        if (index < 0 || index >= _count)
            throw new ArgumentOutOfRangeException(nameof(index));
        
        if (index < _count / 2)
        {
            Node? current = _head;
            for (int i = 0; i < index; i++)
            {
                current = current!.Next;
            }
            return current!;
        }
        else
        {
            Node? current = _tail;
            for (int i = _count - 1; i > index; i--)
            {
                current = current!.Previous;
            }
            return current!;
        }
    }

    private void RemoveNode(Node node)
    {
        if (node.Previous != null)
            node.Previous.Next = node.Next;
        else
            _head = node.Next;
        
        if (node.Next != null)
            node.Next.Previous = node.Previous;
        else
            _tail = node.Previous;
        
        node.Previous = null;
        node.Next = null;
        
        _count--;
        _version++;
    }
}

