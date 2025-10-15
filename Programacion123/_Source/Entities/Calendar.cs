namespace Programacion123
{
    public class Calendar : Entity
    {
        public Property<DateTime> StartDay { get; } = new(DateTime.MinValue);
        public Property<DateTime> EndDay { get; } = new(DateTime.MinValue);

        public SetProperty<DateTime> FreeDays { get; } = new SetProperty<DateTime>();

        public uint flagStartDay                  = 1 << 2; 
        public uint flagEndDay                    = 1 << 3; 
        public uint flagFreeDays                  = 1 << 4; 


        public Calendar()
        {
            DateTime date = DateTime.Now.Date;
            StartDay.Value = date;
            EndDay.Value = date;

            StorageClassId = "calendar";

            Title.Value = "Título del calendario";
            Description.Value = "Descripción del calendario";

            StartDay.OnSetted += (previous, current) => { if(previous != current) { Flags.Add(ref validationFlags, flagStartDay); } };
            EndDay.OnSetted += (previous, current) => { if(previous != current) { Flags.Add(ref validationFlags, flagEndDay); } };

            FreeDays.OnAdded += (element) => { Flags.Add(ref validationFlags, flagFreeDays); };
            FreeDays.OnRemoved += (element) => { Flags.Add(ref validationFlags, flagFreeDays); };

            Flags.Add(ref validationFlags, flagStartDay);
            Flags.Add(ref validationFlags, flagEndDay);
            Flags.Add(ref validationFlags, flagFreeDays);

        }

        public IReadOnlyList<DateTime> GetOrderedFreedays()
        {
            var lista = FreeDays.ToList();
            lista.Sort();
            return lista;
        }

        public override ValidationResult Validate(bool force = false)
        {
            base.Validate(force);

            if (Flags.Test(validationFlags, flagStartDay | flagEndDay) || force)
            {
                Utils.Log("Checking start day after end day", "startDay, endDay");

                validationFails.RemoveAll(e => e.code == ValidationCode.calendarStartDayAfterEndDay);

                if(StartDay.Value > EndDay.Value)
                {
                    validationFails.Add(ValidationResult.Create(ValidationCode.calendarStartDayAfterEndDay));
                }
            }


            if (Flags.Test(validationFlags, flagStartDay | flagEndDay | flagFreeDays) || force)
            {
                Utils.Log("Checking free days outside start or end day", "startDay, endDay, freeDays");

                validationFails.RemoveAll(e => e.code == ValidationCode.calendarFreeDayBeforeStartOrAfterEnd);

                int i = 0;
                var listaFestivos = FreeDays.ToList();

                while (i < listaFestivos.Count)
                {
                    if (listaFestivos[i] > EndDay.Value || listaFestivos[i] < StartDay.Value)
                    {
                        validationFails.Add(ValidationResult.Create(ValidationCode.calendarFreeDayBeforeStartOrAfterEnd));
                    }

                    i++;
                }

            }

            if (Flags.Test(validationFlags, flagStartDay | flagEndDay | flagFreeDays) || force)
            {
                Utils.Log("Checking no school days", "startDay, endDay, freeDays");

                validationFails.RemoveAll(e => e.code == ValidationCode.calendarNoSchoolDays);

                DateTime d = StartDay.Value;
                bool foundSchoolDay = false;

                while (d <= EndDay.Value && !foundSchoolDay)
                {
                    if (!FreeDays.Contains(d) && d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday) { foundSchoolDay = true; }
                    else { d = d.AddDays(1); }
                }

                if (!foundSchoolDay)
                {
                    validationFails.Add(ValidationResult.Create(ValidationCode.calendarNoSchoolDays));
                }
            }

            Flags.Remove(ref validationFlags, flagStartDay);
            Flags.Remove(ref validationFlags, flagEndDay);
            Flags.Remove(ref validationFlags, flagFreeDays);

            foreach(ValidationResult fail in validationFails) { Utils.Log(fail.ToString() + " (" + fail.index + ")", "FAILED"); }

            if(validationFails.Count == 0) { return ValidationResult.Create(ValidationCode.success); }
            else { return validationFails[0]; }

        }

        public void Reset()
        {
            StartDay.Value = new DateTime();
            EndDay.Value = new DateTime();
            FreeDays.Clear();
        }

        public override void LoadOrCreate(string storageId, string? parentStorageId = null)
        {
            base.LoadOrCreate(storageId, parentStorageId);

            if (!Storage.ExistsData<CalendarData>(storageId, StorageClassId, parentStorageId)) { Save(parentStorageId); }

            var data = Storage.LoadData<CalendarData>(storageId, StorageClassId, parentStorageId);

            Title.Value = data.Title;

            StartDay.Value = data.StartDay;
            EndDay.Value = data.EndDay;
            FreeDays.Clear();
            FreeDays.Add(data.FreeDays.ToList<DateTime>());

        }

        public override bool Exists(string storageId, string? parentStorageId)
        {
            return Storage.ExistsData<CalendarData>(storageId, StorageClassId, parentStorageId);
        }

        public override void Save(string? parentStorageId = null)
        {
            base.Save(parentStorageId);

            var data = new CalendarData();

            data.Title = Title.Value;

            data.StartDay = StartDay.Value;
            data.EndDay = EndDay.Value;
            data.FreeDays = new HashSet<DateTime>(FreeDays.ToList());

            Storage.SaveData<CalendarData>(StorageId, StorageClassId, data, parentStorageId);
        }

        public override void Delete(string? parentStorageId = null)
        {
            base.Delete(parentStorageId);

            Storage.DeleteData(StorageId, StorageClassId, parentStorageId);
        }


        public Calendar Clone()
        {
            var other = new Calendar();

            other.StartDay.Value = StartDay.Value;
            other.EndDay.Value = EndDay.Value;
            other.FreeDays.Add(FreeDays.ToList());

            return other;
        }

    }
}
