namespace Programacion123
{
    public class WeekSchedule : Entity
    {
        internal DictionaryProperty<DayOfWeek, int> HoursPerWeekDay { get; } = new DictionaryProperty<DayOfWeek, int>();

        bool checkHoursPerWeeek;

        ValidationResult cachedResult;

        public WeekSchedule()
        {
            StorageClassId = "weekschedule";

            Title.Value = "Título del horario";
            Description.Value = "Descripción del horario";

            HoursPerWeekDay.OnAdded += (k, v) => { checkHoursPerWeeek = true; InvokeOnUpdated(); };
            HoursPerWeekDay.OnRemoved += (k) => { checkHoursPerWeeek = true; InvokeOnUpdated(); };
            HoursPerWeekDay.OnUpdated += (k, v) => { checkHoursPerWeeek = true; InvokeOnUpdated(); };

            checkHoursPerWeeek = true;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            checkHoursPerWeeek = true;
        }

        public override ValidationResult Validate()
        {
            ValidationResult baseResult = base.Validate();

            if(baseResult.code != ValidationCode.success) { return baseResult; }

            if(!checkHoursPerWeeek) { return cachedResult; }

            bool invalid = false;

            if(!invalid && checkHoursPerWeeek)
            {
                int total = 0;
                HoursPerWeekDay.ToList().ForEach(e => total += e.Value);
                if (total <= 0)
                {
                    cachedResult = ValidationResult.Create(ValidationCode.weekScheduleOneHourMinimum);
                    invalid = true;
                }

                checkHoursPerWeeek = false;
            }

            if(!invalid)
            {
                cachedResult = ValidationResult.Create(ValidationCode.success);
            }

            return cachedResult;


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
            data.Title = Title.Value;

            Storage.SaveData<WeekScheduleData>(StorageId, StorageClassId, data, parentStorageId);
        }

        public override void LoadOrCreate(string storageId, string? parentStorageId = null)
        {
            base.LoadOrCreate(storageId, parentStorageId);

            bool created = false;

            if (!Storage.ExistsData<WeekScheduleData>(storageId, StorageClassId, parentStorageId)) { Save(parentStorageId); created = true; }

            var data = Storage.LoadData<WeekScheduleData>(storageId, StorageClassId, parentStorageId);

            Title.Value = data.Title;
            HoursPerWeekDay.Set(data.HoursPerWeekDay.ToList());

        }

        public override void Delete(string? parentStorageId = null)
        {
            base.Delete(parentStorageId);

            Storage.DeleteData(StorageId, StorageClassId, parentStorageId);
        }

    }
}
