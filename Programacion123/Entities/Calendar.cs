namespace Programacion123
{
    public class Calendar : Entity
    {
        public DateTime StartDay { get; set; }
        public DateTime EndDay { get; set; }

        public SetProperty<DateTime> FreeDays { get; } = new SetProperty<DateTime>();

        public Calendar()
        {
            Title = "Calendario sin título";
            DateTime date = DateTime.Now.Date;
            StartDay = date;
            EndDay = date;

            StorageClassId = "calendar";
        }

        public IReadOnlyList<DateTime> GetOrderedFreedays()
        {
            var lista = FreeDays.ToList();
            lista.Sort();
            return lista;
        }

        public override ValidationResult Validate()
        {
            ValidationResult validation = base.Validate();

            if (validation == ValidationResult.success && StartDay > EndDay)
            {
                //Console.WriteLine("La fecha de inicio no puede ser posterior a la fecha de fin");
                validation = ValidationResult.startDayAfterEndDay;
            }

            if(validation == ValidationResult.success)
            {
                int i = 0;
                var listaFestivos = FreeDays.ToList();

                while (validation == ValidationResult.success && i < listaFestivos.Count)
                {
                    if (listaFestivos[i] > EndDay || listaFestivos[i] < StartDay)
                    {
                        //Utils.MuestraError("El festivo " + listaFestivos[i].ToString("dd/MM/yyyy") + " esta fuera del calendario");
                        validation = ValidationResult.freeDayBeforeStartOrAfterEnd;
                    }

                    i++;
                }

            }

            if(validation == ValidationResult.success)
            {
                DateTime d = StartDay;
                bool foundSchoolDay = false;

                while(d <= EndDay && !foundSchoolDay)
                {
                    if(!FreeDays.Contains(d)) { foundSchoolDay = true; }
                    else { d = d.AddDays(1); }
                }

                if(!foundSchoolDay) { validation = ValidationResult.noSchoolDays; }
            }

            return validation;
        }

        public void Reset()
        {
            StartDay = new DateTime();
            EndDay = new DateTime();
            FreeDays.Clear();
        }

        public override void Load(string storageId, string? parentStorageId = null)
        {
            base.Load(storageId, parentStorageId);

            var data = Storage.LoadData<CalendarData>(storageId, StorageClassId, parentStorageId);

            Title = data.Title;

            StartDay = data.StartDay;
            EndDay = data.EndDay;
            FreeDays.Clear();
            FreeDays.Add(data.FreeDays.ToList<DateTime>());

        }

        public override void Save(string? parentStorageId = null)
        {
            base.Save(parentStorageId);

            var data = new CalendarData();

            data.Title = Title;

            data.StartDay = StartDay;
            data.EndDay = EndDay;
            data.FreeDays = new HashSet<DateTime>(FreeDays.ToList());

            Storage.SaveData<CalendarData>(StorageId, StorageClassId, data, parentStorageId);
        }


        public Calendar Clone()
        {
            var otro = new Calendar();

            otro.StartDay = StartDay;
            otro.EndDay = EndDay;
            otro.FreeDays.Add(FreeDays.ToList());

            return otro;
        }

    }
}
