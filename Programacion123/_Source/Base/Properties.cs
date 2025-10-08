namespace Programacion123
{
    public class EntityProperty<T> : Property<T> where T: Entity
    {
        public override T Value
        {
            get
            {
                return base.Value;
            }
            
            set
            {
                if(base.Value != null)
                {
                    base.Value.OnUpdated -= OnEntityUpdatedHandler;
                }

                base.Value = value;

                if(value != null)
                {
                    value.OnUpdated += OnEntityUpdatedHandler;
                }
            }
        }

        public EntityProperty(T initialValue) : base(initialValue)
        {
            if(initialValue != null) { initialValue.OnUpdated += OnEntityUpdatedHandler; }
        }

        public delegate void EntityChangedHandler(Entity entity);
        public event EntityChangedHandler OnEntityUpdated;

        void OnEntityUpdatedHandler(Entity entity)
        {
            OnEntityUpdated?.Invoke(entity);
        }
    }

    public class Property<T>
    {
        public virtual T Value { get { return internalValue; } set { T previousValue = internalValue; internalValue = value; OnSetted?.Invoke(previousValue, value); } }

        public delegate void SettedHandler(T previousValue, T value);
        public event SettedHandler OnSetted;

        public Property(T initialValue) { internalValue = initialValue; }

        T internalValue;

    }

    public class SetProperty<T>
    {
        public virtual void Add(T value) { set.Add(value); OnAdded?.Invoke(value); }
        public virtual void Remove(T value) { set.Remove(value); OnRemoved?.Invoke(value); }

        public void Add(List<T> other) { foreach (T e in other) { Add(e); } }
        public void Set(List<T> other)
        {
            List<T> current = set.ToList<T>();
            foreach(T e in current) { Remove(e); }
            foreach (T e in other) { Add(e); }
        }
        public int Count { get => set.Count; }
        public void Clear()
        {
            List<T> list = set.ToList<T>();
            foreach (T e in list) { Remove(e); };
            set.Clear();
        }

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

    public class SetEntityProperty<T> : SetProperty<T> where T: Entity
    {
        public delegate void EntityChangedHandler(Entity entity);
        public event EntityChangedHandler OnEntityUpdated;

        public override void Add(T value) { base.Add(value); if(value != null) { value.OnUpdated += OnEntityUpdatedHandler; } }
        public override void Remove(T value) { if(value != null) { value.OnUpdated -= OnEntityUpdatedHandler; } base.Remove(value); }

        void OnEntityUpdatedHandler(Entity entity)
        {
            OnEntityUpdated?.Invoke(entity);
        }
    }

    public class DictionaryProperty<K, T> where K : notnull
    {
        public virtual void Add(K key, T value) { dictionary.Add(key, value); OnAdded?.Invoke(key, value); }
        public virtual void Remove(K key) { dictionary.Remove(key); OnRemoved?.Invoke(key); }

        public void Add(List<KeyValuePair<K, T>> other) { foreach (KeyValuePair<K, T> e in other) { Add(e.Key, e.Value); } }
        public void Set(List<KeyValuePair<K, T>> other)
        {
            List<K> current = dictionary.Keys.ToList<K>();
            foreach (K key in current) { Remove(key); }
            foreach (KeyValuePair<K, T> e in other) { Add(e.Key, e.Value); }
        }
        
        public void Set(K key, T value) { dictionary[key] = value; OnUpdated?.Invoke(key, value); }
        public int Count { get => dictionary.Count; }
        public List<KeyValuePair<K, T>> ToList() { return new List<KeyValuePair<K, T>>(dictionary); }
        public T this[K key] { get { return dictionary[key]; } }
        public void Clear()
        {
            List<K> list = dictionary.Keys.ToList<K>();
            foreach (K key in list) { Remove(key); }
        }

        public bool ContainsKey(K key) { return dictionary.ContainsKey(key); }
        public delegate void AddedHandler(K key, T value);
        public delegate void RemovedHandler(K key);
        public delegate void UpdatedHandler(K key, T value);
        public event AddedHandler OnAdded;
        public event RemovedHandler OnRemoved;
        public event UpdatedHandler OnUpdated;

        Dictionary<K, T> dictionary;

        protected T GetValue(K key) { return dictionary[key]; } 

        public DictionaryProperty()
        {
            dictionary = new Dictionary<K, T>();
        }
    }

    public class DictionaryEntityProperty<K, T>: DictionaryProperty<K, T> where T: Entity
    {
        public delegate void EntityChangedHandler(Entity entity);
        public event EntityChangedHandler OnEntityUpdated;

        public override void Add(K key, T value) { base.Add(key, value); if(value != null) { value.OnUpdated += OnEntityUpdatedHandler; } }
        public override void Remove(K key) { if(GetValue(key) != null) { GetValue(key).OnUpdated -= OnEntityUpdatedHandler; } base.Remove(key); }

        void OnEntityUpdatedHandler(Entity entity)
        {
            OnEntityUpdated?.Invoke(entity);
        }

    }


    public class ListProperty<T>
    {
        public virtual void Add(T value) { list.Add(value); OnAdded?.Invoke(value); }
        public virtual void Remove(T value) { list.Remove(value); OnRemoved?.Invoke(value); }

        public void Add(List<T> other) { foreach (T e in other) { Add(e); } }
        public void Set(List<T> other)
        {
            List<T> current = list.ToList<T>();
            foreach (T e in current) { Remove(e); }
            foreach (T e in other) { Add(e); }
        }
        public List<T> ToList() { return new List<T>(list); }
        public bool Contains(T value) { return list.Contains(value); }
        public int Count { get { return list.Count; } }
        public T? Find(Predicate<T> criteria) { return list.Find(criteria); }
        public T this[int index] { get { return list[index]; } }
        public void Clear()
        {
            List<T> current = list.ToList<T>();
            foreach(T e in current) { Remove(e); }
        }
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

    public class ListEntityProperty<T> : ListProperty<T> where T: Entity
    {
        public delegate void EntityChangedHandler(Entity entity);
        public event EntityChangedHandler OnEntityUpdated;

        public override void Add(T value) { base.Add(value); if(value != null) { value.OnUpdated += OnEntityUpdatedHandler; } }
        public override void Remove(T value) { if(value != null) { value.OnUpdated -= OnEntityUpdatedHandler; }  base.Remove(value); }

        void OnEntityUpdatedHandler(Entity entity)
        {
            OnEntityUpdated?.Invoke(entity);
        }
    }
}
