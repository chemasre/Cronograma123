namespace Programacion123
{
    public struct SetProperty<T>
    {
        public void Add(T value) { set.Add(value); OnAdded?.Invoke(value); }
        public void Add(List<T> other) { foreach(T e in other) { set.Add(e); OnAdded?.Invoke(e); }  }
        public void Set(List<T> other)
        {
            foreach(T e in set) { OnRemoved?.Invoke(e); }
            set.Clear();
            foreach(T e in other) { set.Add(e); OnAdded?.Invoke(e); }
        }
        public void Remove(T value) { set.Remove(value); OnRemoved?.Invoke(value); }
        public int Count { get => set.Count; }
        public void Clear() { foreach(T e in set) { OnRemoved?.Invoke(e); }; set.Clear(); }
        public bool Contains(T value) { return set.Contains(value); }
        public List<T> ToList() { return set.ToList<T>(); }
        public T? Find(Predicate<T> criteria) { return set.ToList<T>().Find(criteria); }
        public delegate void AddedHandler(T value);
        public delegate void RemovedHandler(T value);
        public event AddedHandler OnAdded;
        public event RemovedHandler OnRemoved;

        HashSet<T> set;

        public SetProperty()
        {
            set = new HashSet<T>();
        }
    }


    public struct DictionaryProperty<K, T> where K : notnull
    {
        public void Add(K key, T value) { dictionary.Add(key, value); OnAdded?.Invoke(key, value); }
        public void Add(List<KeyValuePair<K, T>> other) { foreach(KeyValuePair<K, T> e in other) { dictionary.Add(e.Key, e.Value); OnAdded?.Invoke(e.Key, e.Value); }  }
        public void Set(List<KeyValuePair<K, T>> other)
        {
            foreach(KeyValuePair<K, T> e in dictionary) { OnRemoved?.Invoke(e.Key); }
            dictionary.Clear();
            foreach(KeyValuePair<K, T> e in other) { dictionary.Add(e.Key, e.Value); OnAdded?.Invoke(e.Key, e.Value); }
        }
        public void Set(K key, T value) { dictionary[key] = value; OnUpdated?.Invoke(key, value); }
        public void Remove(K key) { dictionary.Remove(key); OnRemoved?.Invoke(key); }
        public int Count { get => dictionary.Count; }
        public List< KeyValuePair<K, T> > ToList() { return new List<KeyValuePair<K, T>>(dictionary); }
        public T this[K key] { get { return dictionary[key]; } }
        public void Clear() { foreach(KeyValuePair<K, T> e in dictionary) { OnRemoved?.Invoke(e.Key); }; dictionary.Clear(); }
        public bool ContainsKey(K key) { return dictionary.ContainsKey(key); }
        public delegate void AddedHandler(K key, T value);
        public delegate void RemovedHandler(K key);
        public delegate void UpdatedHandler(K key, T value);
        public event AddedHandler OnAdded;
        public event RemovedHandler OnRemoved;
        public event UpdatedHandler OnUpdated;

        Dictionary<K, T> dictionary;

        public DictionaryProperty()
        {
            dictionary = new Dictionary<K,T>();
        }
    }

    public struct ListProperty<T>
    {
        public void Add(T value) { list.Add(value); OnAdded?.Invoke(value); }
        public void Add(List<T> other) { foreach(T e in other) { list.Add(e); OnAdded?.Invoke(e); }  }
        public void Set(List<T> other)
        {
            while(list.Count > 0) { T value = list[0]; list.RemoveAt(0); OnRemoved?.Invoke(value); }
            foreach(T e in other) { list.Add(e); OnAdded?.Invoke(e); }
        }
        public List<T> ToList() { return new List<T>(list); }
        public void Remove(T value) { list.Remove(value); OnRemoved?.Invoke(value); }
        public bool Contains(T value) { return list.Contains(value); }
        public int Count { get { return list.Count; } }
        public T? Find(Predicate<T> criteria) { return list.Find(criteria); }
        public T this[int index] { get { return list[index]; } }
        public void Clear() { while(list.Count > 0) { T value = list[0]; list.RemoveAt(0); OnRemoved?.Invoke(value); } }
        public delegate void AddedHandler(T value);
        public delegate void RemovedHandler(T value);
        public event AddedHandler OnAdded;
        public event RemovedHandler OnRemoved;

        List<T> list;

        public ListProperty()
        {
            list = new List<T>();
        }
    }
}
