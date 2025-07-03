using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Programacion123
{
    internal class Subject : Entity
    {
        class SerializableData
        {
            public string title { get; set; }
            public List<int> unitsOrder { get; set; }
            public HashSet<KeyValuePair<int, int>> hoursByUnit { get; set; }
            public HashSet<KeyValuePair<int, string>> titlesByUnit { get; set; }
            public HashSet<KeyValuePair<DayOfWeek, int>> hoursPerWeekDay { get; set; }

        };

        List<int> unitsOrder;
        Dictionary<int, int> hoursByUnit;
        Dictionary<int, string> titlesPerUnit;
        Dictionary<DayOfWeek, int> daysPerUnit;

        public Subject()
        {
             unitsOrder = new List<int>();
            hoursByUnit = new Dictionary<int, int>();
            titlesPerUnit = new Dictionary<int, string>();
            daysPerUnit = new Dictionary<DayOfWeek, int>();
        }

        public void AddUnit(int unit, string title, int hours) { unitsOrder.Add(unit); hoursByUnit[unit] = hours; titlesPerUnit[unit] = title; }
        public void RemoveUnit(int unit) { titlesPerUnit.Remove(unit); hoursByUnit.Remove(unit); unitsOrder.Remove(unit); }
        public bool ExistsUnit(int unit) { return unitsOrder.Contains(unit); }
        public int GetUnitsCount() { return unitsOrder.Count; }
        public int GetUnitByOrderIndex(int i) { return unitsOrder[i]; }
        public int GetUnitHours(int unit) { return hoursByUnit[unit]; }
        public string GetUnitTitle(int unit) { return titlesPerUnit[unit]; }

        public void HasWeekDay(DayOfWeek diaActual, int horas) { daysPerUnit[diaActual] = horas; }
        public void RemoveWeekDay(DayOfWeek diaActual) { daysPerUnit.Remove(diaActual); }
        public bool HasWeekDay(DayOfWeek diaActual) { return daysPerUnit.ContainsKey(diaActual); }
        public int GetWeekDayHours(DayOfWeek diaActual) { if (daysPerUnit.ContainsKey(diaActual)) { return daysPerUnit[diaActual]; } else { return 0; } }

        public override ValidationResult Validate()
        {

            ValidationResult completa = ValidationResult.success;

            if (unitsOrder.Count <= 0) { completa = ValidationResult.unitsMissing; }
            else if (daysPerUnit.Count <= 0) { completa = ValidationResult.weekDayMissing; }

            return completa;
        }

        public void ResetWeekSchedule()
        {
            daysPerUnit.Clear();
        }

        public void ResetUnits()
        {
            unitsOrder.Clear();
            hoursByUnit.Clear();
            titlesPerUnit.Clear();
        }

        public override void Save(string fileName)
        {
            var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write);

            var writer = new StreamWriter(stream);

            var data = new SerializableData();

            data.title = Title;
            data.unitsOrder = unitsOrder;
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
            unitsOrder = data.unitsOrder;
            hoursByUnit = new Dictionary<int, int>(data.hoursByUnit);
            titlesPerUnit = new Dictionary<int, string>(data.titlesByUnit);
            daysPerUnit = new Dictionary<DayOfWeek, int>(data.hoursPerWeekDay);

            reader.Close();
        }
    }

}
