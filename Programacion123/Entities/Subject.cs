using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Programacion123
{
    internal struct DictionaryProperty<K, T> where K : notnull
    {
        public void Add(K key, T value) { dictionary.Add(key, value); OnAdded?.Invoke(key, value); }
        public void Remove(K key) { dictionary.Remove(key); OnRemoved?.Invoke(key); }
        public int Count { get => dictionary.Count; }
        public delegate void AddedHandler(K key, T value);
        public delegate void RemovedHandler(K key);
        public event AddedHandler OnAdded;
        public event RemovedHandler OnRemoved;

        Dictionary<K, T> dictionary;

        public DictionaryProperty()
        {
            dictionary = new Dictionary<K,T>();
        }
    }

    internal struct ListProperty<T>
    {
        public void Add(T value) { list.Add(value); OnAdded?.Invoke(value); }
        public void Add(List<T> other) { foreach(T e in other) { list.Add(e); OnAdded?.Invoke(e); }  }
        public void CopyTo(List<T> other) { other.AddRange(list); }
        public void Remove(T value) { list.Remove(value); OnRemoved?.Invoke(value); }
        public bool Contains(T value) { return list.Contains(value); }
        public int Count { get { return list.Count; } }
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

    internal class Subject : Entity
    {
        class SerializableData
        {
            public string title { get; set; }
            public List<int> UnitsSequence { get; set; }
            public HashSet<KeyValuePair<int, int>> hoursByUnit { get; set; }
            public HashSet<KeyValuePair<int, string>> titlesByUnit { get; set; }
            public HashSet<KeyValuePair<DayOfWeek, int>> hoursPerWeekDay { get; set; }

        };

        internal ListProperty<int> UnitsSequence { get; } = new ListProperty<int>();

        Dictionary<int, int> hoursByUnit;
        Dictionary<int, string> titlesPerUnit;
        Dictionary<DayOfWeek, int> daysPerUnit;

        public Subject()
        {
            UnitsSequence = new ListProperty<int>();
            UnitsSequence.OnAdded += (e) => { };

            hoursByUnit = new Dictionary<int, int>();
            titlesPerUnit = new Dictionary<int, string>();
            daysPerUnit = new Dictionary<DayOfWeek, int>();
        }

        public void AddUnit(int unit, string title, int hours) { UnitsSequence.Add(unit); hoursByUnit[unit] = hours; titlesPerUnit[unit] = title; }
        public void RemoveUnit(int unit) { titlesPerUnit.Remove(unit); hoursByUnit.Remove(unit); UnitsSequence.Remove(unit); }
        public bool ExistsUnit(int unit) { return UnitsSequence.Contains(unit); }
        public int GetUnitsCount() { return UnitsSequence.Count; }
        public int GetUnitByOrderIndex(int i) { return UnitsSequence[i]; }
        public int GetUnitHours(int unit) { return hoursByUnit[unit]; }
        public string GetUnitTitle(int unit) { return titlesPerUnit[unit]; }

        public void AddWeekDay(DayOfWeek diaActual, int horas) { daysPerUnit[diaActual] = horas; }
        public void RemoveWeekDay(DayOfWeek diaActual) { daysPerUnit.Remove(diaActual); }
        public bool HasWeekDay(DayOfWeek diaActual) { return daysPerUnit.ContainsKey(diaActual); }
        public int GetWeekDayHours(DayOfWeek diaActual) { if (daysPerUnit.ContainsKey(diaActual)) { return daysPerUnit[diaActual]; } else { return 0; } }

        public override ValidationResult Validate()
        {

            ValidationResult completa = ValidationResult.success;

            if (UnitsSequence.Count <= 0) { completa = ValidationResult.unitsMissing; }
            else if (daysPerUnit.Count <= 0) { completa = ValidationResult.weekDayMissing; }

            return completa;
        }

        public void ResetWeekSchedule()
        {
            daysPerUnit.Clear();
        }

        public void ResetUnits()
        {
            UnitsSequence.Clear();
            hoursByUnit.Clear();
            titlesPerUnit.Clear();
        }

        public override void Save(string fileName)
        {
            var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write);

            var writer = new StreamWriter(stream);

            var data = new SerializableData();

            data.title = Title;
            data.UnitsSequence = new List<int>();
            UnitsSequence.CopyTo(data.UnitsSequence);
            data.hoursByUnit = new HashSet<KeyValuePair<int, int>>(hoursByUnit);
            data.titlesByUnit = new HashSet<KeyValuePair<int, string>>(titlesPerUnit);
            data.hoursPerWeekDay = new HashSet<KeyValuePair<DayOfWeek, int>>(daysPerUnit);

            JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerOptions.Default);
            options.WriteIndented = true;
            writer.Write(JsonSerializer.Serialize<SerializableData>(data, options));
            writer.Close();
        }

        public override void Load(string fileName)
        {
            var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            var reader = new StreamReader(stream);

            var data = new SerializableData();

            string text = reader.ReadToEnd();

            data = JsonSerializer.Deserialize<SerializableData>(text);

            Title = data.title;
            UnitsSequence.Clear();
            UnitsSequence.Add(data.UnitsSequence);
            hoursByUnit = new Dictionary<int, int>(data.hoursByUnit);
            titlesPerUnit = new Dictionary<int, string>(data.titlesByUnit);
            daysPerUnit = new Dictionary<DayOfWeek, int>(data.hoursPerWeekDay);

            reader.Close();
        }
    }

}
