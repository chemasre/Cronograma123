namespace Programacion123
{
    public class Calendar : Entity
    {
        public Property<DateTime> StartDay { get; } = new(DateTime.MinValue);
        public Property<DateTime> EndDay { get; } = new(DateTime.MinValue);

        public SetProperty<DateTime> FreeDays { get; } = new SetProperty<DateTime>();

        bool checkStartDay;
        bool checkEndDay;
        bool checkFreeDays;

        ValidationResult cachedResult;

        public Calendar()
        {
            DateTime date = DateTime.Now.Date;
            StartDay.Value = date;
            EndDay.Value = date;

            StorageClassId = "calendar";

            Title.Value = "Título del calendario";
            Description.Value = "Descripción del calendario";

            StartDay.OnSetted += (previous, current) => { if(previous != current) { checkStartDay = true; } };
            EndDay.OnSetted += (previous, current) => { if(previous != current) { checkStartDay = true; } };

            FreeDays.OnAdded += (element) => { checkFreeDays = true; };
            FreeDays.OnRemoved += (element) => { checkFreeDays = true; };

            checkStartDay = true;
            checkEndDay   = true;
            checkFreeDays = true;

        }

        public IReadOnlyList<DateTime> GetOrderedFreedays()
        {
            var lista = FreeDays.ToList();
            lista.Sort();
            return lista;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            checkStartDay = true;
            checkEndDay = true;
            checkFreeDays = true;
        }

        public override ValidationResult Validate()
        {
            ValidationResult baseResult = base.Validate();

            if(baseResult.code != ValidationCode.success) { return baseResult; }

            if(!checkStartDay && !checkEndDay && !checkFreeDays) { return cachedResult; }
           
            bool invalid = false;

            if (!invalid && checkFreeDays)
            {
                int i = 0;
                var listaFestivos = FreeDays.ToList();

                while (!invalid && i < listaFestivos.Count)
                {
                    if (listaFestivos[i] > EndDay.Value || listaFestivos[i] < StartDay.Value)
                    {
                        cachedResult = ValidationResult.Create(ValidationCode.calendarFreeDayBeforeStartOrAfterEnd);
                        invalid = true;
                    }

                    i++;
                }

                checkFreeDays = false;
            }

            if (!invalid && (checkStartDay || checkEndDay))
            {
                DateTime d = StartDay.Value;
                bool foundSchoolDay = false;

                while (d <= EndDay.Value && !foundSchoolDay)
                {
                    if (!FreeDays.Contains(d) && d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday) { foundSchoolDay = true; }
                    else { d = d.AddDays(1); }
                }

                if (!foundSchoolDay)
                {
                    cachedResult = ValidationResult.Create(ValidationCode.calendarNoSchoolDays);
                    invalid = true;
                }

                checkStartDay = false;
                checkEndDay = false;
            }

            if(!invalid)
            {
                cachedResult = ValidationResult.Create(ValidationCode.success);
            }

            return cachedResult;
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
