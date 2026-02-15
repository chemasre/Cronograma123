namespace Programacion123
{
    public class WeekSchedule : Entity
    {
        internal DictionaryProperty<DayOfWeek, int> HoursPerWeekDay { get; } = new DictionaryProperty<DayOfWeek, int>();

        public const uint flagHoursPerWeekDay = 1 << 2; 

        public WeekSchedule()
        {
            StorageClassId = "weekschedule";

            Title.Value = "Título del horario";
            Description.Value = "Descripción del horario";

            HoursPerWeekDay.OnAdded += (k, v) => { Flags.Add(ref validationFlags, flagHoursPerWeekDay); InvokeOnUpdated(); };
            HoursPerWeekDay.OnRemoved += (k) => { Flags.Add(ref validationFlags, flagHoursPerWeekDay); InvokeOnUpdated(); };
            HoursPerWeekDay.OnUpdated += (k, v) => { Flags.Add(ref validationFlags, flagHoursPerWeekDay); InvokeOnUpdated(); };

            // Add flags

            Flags.Add(ref validationFlags, flagHoursPerWeekDay);
        }

        public override ValidationResult Validate(bool force = false)
        {
            base.Validate(force);

            if(Flags.Test(validationFlags, flagHoursPerWeekDay) || force)
            {
                Utils.Log("Checking week schedule has at least one hour", "hoursPerWeekDay");

                validationFails.RemoveAll(e => e.code == ValidationCode.weekScheduleOneHourMinimum);
                int total = 0;
                HoursPerWeekDay.ToList().ForEach(e => total += e.Value);
                if (total <= 0)
                {
                    validationFails.Add(ValidationResult.Create(ValidationCode.weekScheduleOneHourMinimum));
                }
            }

            // Remove flags

            Flags.Remove(ref validationFlags, flagHoursPerWeekDay);

            // FIX: Causes an exception (randomly)
            //foreach(ValidationResult fail in validationFails) { Utils.Log(fail.ToString() + " (" + fail.index + ")", "FAILED"); }

            if(validationFails.Count == 0) { return ValidationResult.Create(ValidationCode.success); }
            else { return validationFails[0]; }

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
