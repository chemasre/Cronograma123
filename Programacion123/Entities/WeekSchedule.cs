namespace Programacion123
{
    public class WeekSchedule : Entity
    {
        internal DictionaryProperty<DayOfWeek, int> HoursPerWeekDay { get; } = new DictionaryProperty<DayOfWeek, int>();

        public WeekSchedule()
        {
            StorageClassId = "weekschedule";
            Title = "Horario sin título";
        }

        public override ValidationResult Validate()
        {
            ValidationResult validation = base.Validate();

            if(validation == ValidationResult.success)
            {
                int total = 0;
                HoursPerWeekDay.ToList().ForEach(e => total += e.Value);
                if (total <= 0) { validation = ValidationResult.oneHourMinimum; }
            }
            
            return validation;
        }

        public override bool Exists(string storageId, string? parentStorageId)
        {
            return Storage.ExistsData<WeekScheduleData>(storageId, StorageClassId, parentStorageId);
        }

        public override void Save(string? parentStorageId = null)
        {
            base.Save(parentStorageId);

            WeekScheduleData data = new();
            data.HoursPerWeekDay = new(HoursPerWeekDay.ToList());
            data.Title = Title;

            Storage.SaveData<WeekScheduleData>(StorageId, StorageClassId, data, parentStorageId);
        }

        public override void LoadOrCreate(string storageId, string? parentStorageId = null)
        {   
            base.LoadOrCreate(storageId, parentStorageId);

            bool created = false;

            if(!Storage.ExistsData<WeekScheduleData>(storageId, StorageClassId, parentStorageId)) { Save(parentStorageId); created = true; }

            var data = Storage.LoadData<WeekScheduleData>(storageId, StorageClassId, parentStorageId);

            Title = data.Title;
            HoursPerWeekDay.Set(data.HoursPerWeekDay.ToList());

        }

        public override void Delete(string? parentStorageId = null)
        {
            base.Delete(parentStorageId);

            Storage.DeleteData(StorageId, StorageClassId, parentStorageId);
        }

    }
}
