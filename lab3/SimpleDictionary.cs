using System.Collections;
using System.Collections.Generic;

namespace CollectionsLab;

public class SimpleDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
    where TKey : notnull
{
    private const int DefaultCapacity = 16;
    private const double LoadFactor = 0.75;
    
    private Bucket[] _buckets;
    private int _count;
    private int _version;

    private struct Bucket
    {
        public Entry? First;
    }

    private class Entry
    {
        public TKey Key;
        public TValue Value;
        public Entry? Next;
        public int HashCode;

        public Entry(TKey key, TValue value, int hashCode, Entry? next)
        {
            Key = key;
            Value = value;
            HashCode = hashCode;
            Next = next;
        }
    }

    public SimpleDictionary() : this(DefaultCapacity)
    {
    }

    public SimpleDictionary(int capacity)
    {
        if (capacity < 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));
        
        int size = GetPrime(capacity > 0 ? capacity : DefaultCapacity);
        _buckets = new Bucket[size];
        _count = 0;
        _version = 0;
    }

    public SimpleDictionary(IDictionary<TKey, TValue> dictionary) : this(dictionary?.Count ?? DefaultCapacity)
    {
        if (dictionary == null)
            throw new ArgumentNullException(nameof(dictionary));
        
        foreach (var kvp in dictionary)
        {
            Add(kvp.Key, kvp.Value);
        }
    }

    public TValue this[TKey key]
    {
        get
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            
            Entry? entry = FindEntry(key);
            if (entry == null)
                throw new KeyNotFoundException($"The given key '{key}' was not present in the dictionary.");
            
            return entry.Value;
        }
        set
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            
            Insert(key, value, false);
        }
    }

    public ICollection<TKey> Keys => new KeyCollection(this);

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

    public ICollection<TValue> Values => new ValueCollection(this);

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

    public int Count => _count;

    public bool IsReadOnly => false;

    public void Add(TKey key, TValue value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        
        Insert(key, value, true);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        if (_count > 0)
        {
            Array.Clear(_buckets);
            _count = 0;
            _version++;
        }
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        if (item.Key == null)
            return false;
        
        Entry? entry = FindEntry(item.Key);
        if (entry == null)
            return false;
        
        return EqualityComparer<TValue>.Default.Equals(entry.Value, item.Value);
    }

    public bool ContainsKey(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        
        return FindEntry(key) != null;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));
        
        if (arrayIndex < 0 || arrayIndex > array.Length)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        
        if (array.Length - arrayIndex < _count)
            throw new ArgumentException("Destination array is not long enough.");
        
        int index = arrayIndex;
        foreach (var kvp in this)
        {
            array[index++] = kvp;
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        int version = _version;
        for (int i = 0; i < _buckets.Length; i++)
        {
            Entry? entry = _buckets[i].First;
            while (entry != null)
            {
                if (version != _version)
                    throw new InvalidOperationException("Collection was modified during enumeration.");
                
                yield return new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
                entry = entry.Next;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool Remove(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        
        int hashCode = GetHashCode(key);
        int bucketIndex = hashCode % _buckets.Length;
        
        Entry? current = _buckets[bucketIndex].First;
        Entry? previous = null;
        
        while (current != null)
        {
            if (current.HashCode == hashCode && EqualityComparer<TKey>.Default.Equals(current.Key, key))
            {
                if (previous == null)
                    _buckets[bucketIndex].First = current.Next;
                else
                    previous.Next = current.Next;
                
                _count--;
                _version++;
                return true;
            }
            
            previous = current;
            current = current.Next;
        }
        
        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        Entry? entry = FindEntry(item.Key);
        if (entry == null)
            return false;
        
        if (!EqualityComparer<TValue>.Default.Equals(entry.Value, item.Value))
            return false;
        
        return Remove(item.Key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        
        Entry? entry = FindEntry(key);
        if (entry == null)
        {
            value = default(TValue)!;
            return false;
        }
        
        value = entry.Value;
        return true;
    }

    private Entry? FindEntry(TKey key)
    {
        int hashCode = GetHashCode(key);
        int bucketIndex = hashCode % _buckets.Length;
        
        Entry? entry = _buckets[bucketIndex].First;
        while (entry != null)
        {
            if (entry.HashCode == hashCode && EqualityComparer<TKey>.Default.Equals(entry.Key, key))
                return entry;
            
            entry = entry.Next;
        }
        
        return null;
    }

    private void Insert(TKey key, TValue value, bool add)
    {
        int hashCode = GetHashCode(key);
        int bucketIndex = hashCode % _buckets.Length;
        
        Entry? entry = _buckets[bucketIndex].First;
        while (entry != null)
        {
            if (entry.HashCode == hashCode && EqualityComparer<TKey>.Default.Equals(entry.Key, key))
            {
                if (add)
                    throw new ArgumentException($"An item with the same key has already been added. Key: '{key}'");
                
                entry.Value = value;
                _version++;
                return;
            }
            
            entry = entry.Next;
        }
        
        _buckets[bucketIndex].First = new Entry(key, value, hashCode, _buckets[bucketIndex].First);
        _count++;
        _version++;
        
        if (_count > _buckets.Length * LoadFactor)
        {
            Resize();
        }
    }

    private void Resize()
    {
        int newSize = GetPrime(_buckets.Length * 2);
        Bucket[] newBuckets = new Bucket[newSize];
        
        for (int i = 0; i < _buckets.Length; i++)
        {
            Entry? entry = _buckets[i].First;
            while (entry != null)
            {
                Entry? next = entry.Next;
                int newBucketIndex = entry.HashCode % newSize;
                entry.Next = newBuckets[newBucketIndex].First;
                newBuckets[newBucketIndex].First = entry;
                entry = next;
            }
        }
        
        _buckets = newBuckets;
    }

    private int GetHashCode(TKey key)
    {
        return key.GetHashCode() & 0x7FFFFFFF;
    }

    private static int GetPrime(int min)
    {
        int[] primes = { 3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919, 1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591, 17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437, 187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263, 1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369 };
        
        foreach (int prime in primes)
        {
            if (prime >= min)
                return prime;
        }
        
        for (int i = min | 1; i < int.MaxValue; i += 2)
        {
            if (IsPrime(i))
                return i;
        }
        
        return min;
    }

    private static bool IsPrime(int candidate)
    {
        if ((candidate & 1) == 0)
            return candidate == 2;
        
        int limit = (int)Math.Sqrt(candidate);
        for (int divisor = 3; divisor <= limit; divisor += 2)
        {
            if (candidate % divisor == 0)
                return false;
        }
        
        return true;
    }

    private class KeyCollection : ICollection<TKey>, IEnumerable<TKey>, IEnumerable
    {
        private readonly SimpleDictionary<TKey, TValue> _dictionary;

        public KeyCollection(SimpleDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        public int Count => _dictionary.Count;

        public bool IsReadOnly => true;

        public void Add(TKey item) => throw new NotSupportedException();

        public void Clear() => throw new NotSupportedException();

        public bool Contains(TKey item) => _dictionary.ContainsKey(item);

        public void CopyTo(TKey[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            
            if (array.Length - arrayIndex < Count)
                throw new ArgumentException("Destination array is not long enough.");
            
            int index = arrayIndex;
            foreach (var kvp in _dictionary)
            {
                array[index++] = kvp.Key;
            }
        }

        public IEnumerator<TKey> GetEnumerator()
        {
            foreach (var kvp in _dictionary)
            {
                yield return kvp.Key;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Remove(TKey item) => throw new NotSupportedException();
    }

    private class ValueCollection : ICollection<TValue>, IEnumerable<TValue>, IEnumerable
    {
        private readonly SimpleDictionary<TKey, TValue> _dictionary;

        public ValueCollection(SimpleDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        public int Count => _dictionary.Count;

        public bool IsReadOnly => true;

        public void Add(TValue item) => throw new NotSupportedException();

        public void Clear() => throw new NotSupportedException();

        public bool Contains(TValue item)
        {
            foreach (var kvp in _dictionary)
            {
                if (EqualityComparer<TValue>.Default.Equals(kvp.Value, item))
                    return true;
            }
            return false;
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            
            if (array.Length - arrayIndex < Count)
                throw new ArgumentException("Destination array is not long enough.");
            
            int index = arrayIndex;
            foreach (var kvp in _dictionary)
            {
                array[index++] = kvp.Value;
            }
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (var kvp in _dictionary)
            {
                yield return kvp.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Remove(TValue item) => throw new NotSupportedException();
    }
}
