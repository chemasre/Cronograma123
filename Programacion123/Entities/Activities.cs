using System.IO;
using System.Text.Json;

namespace Programacion123
{
    internal class Activities : Entity
    {
        internal ListProperty<Unit> UnitsSequence { get; } = new ListProperty<Unit>();
        internal DictionaryProperty<int, Unit> UnitsById  { get; } = new DictionaryProperty<int, Unit>();

        internal WeekSchedule WeekSchedule { get; set; }


        public Activities()
        {
            UnitsSequence.OnAdded += (Unit e) => { UnitsById.Add(e.Id, e); };
            UnitsSequence.OnRemoved += (Unit e) => { UnitsById.Remove(e.Id); };

            StorageClassId = "subject";
        }

        public override ValidationResult Validate()
        {

            ValidationResult completa = ValidationResult.success;

            if (UnitsSequence.Count <= 0) { completa = ValidationResult.unitsMissing; }
            else if (WeekSchedule.HoursPerWeekDay.Count <= 0) { completa = ValidationResult.weekDayMissing; }

            return completa;
        }

        public void ResetWeekSchedule()
        {
            WeekSchedule.HoursPerWeekDay.Clear();
        }

        public void ResetUnits()
        {
            UnitsSequence.Clear();
            UnitsById.Clear();
        }

        public override void Save(string? parentStorageId = null)
        {
            base.Save(parentStorageId);

            var stream = new FileStream(StorageId + "." + StorageClassId, FileMode.Create, FileAccess.Write);

            var writer = new StreamWriter(stream);

            var data = new ActivitiesStorageData();

            data.Title = Title;
            data.UnitsSequenceStorageIds = Storage.GetStorageIds<Unit>(UnitsSequence.ToList());

            data.WeekScheduleStorageId = WeekSchedule.StorageId;
            WeekSchedule.Save(parentStorageId);

            JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerOptions.Default);
            options.WriteIndented = true;
            writer.Write(JsonSerializer.Serialize<ActivitiesStorageData>(data, options));
            writer.Close();
        }

        public void Load(string storageId, string? parentStorageId = null)
        {
            base.LoadOrCreate(storageId, parentStorageId);

            var stream = new FileStream(storageId + "." + StorageClassId, FileMode.Open, FileAccess.Read);
            var reader = new StreamReader(stream);

            var data = new ActivitiesStorageData();

            string text = reader.ReadToEnd();

            data = JsonSerializer.Deserialize<ActivitiesStorageData>(text);

            Title = data.Title;
            UnitsSequence.Set(Storage.LoadEntities<Unit>(data.UnitsSequenceStorageIds));

            WeekSchedule = Storage.LoadOrCreateEntity<WeekSchedule>(data.WeekScheduleStorageId);

            reader.Close();
        }
    }

}
