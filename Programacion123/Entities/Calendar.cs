using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Programacion123
{
    internal class Calendar : Entity
    {
        public DateTime StartDay { get; set; }
        public DateTime EndDay { get; set; }

        public SetProperty<DateTime> FreeDays { get; } = new SetProperty<DateTime>();

        public Calendar()
        {
            StartDay = new DateTime();
            EndDay = new DateTime();

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
            ValidationResult validation = ValidationResult.success;

            if (StartDay > EndDay)
            {
                //Console.WriteLine("La fecha de inicio no puede ser posterior a la fecha de fin");
                validation = ValidationResult.startDayAfterEndDay;
                return validation;
            }

            int i = 0;
            var listaFestivos = FreeDays.ToList();

            while (validation == ValidationResult.success && i < listaFestivos.Count)
            {
                if (listaFestivos[i] > EndDay || listaFestivos[i] < StartDay)
                {
                    //Utils.MuestraError("El festivo " + listaFestivos[i].ToString("dd/MM/yyyy") + " esta fuera del calendario");
                    validation = ValidationResult.freeDayOutsideCalendar;
                }

                i++;
            }

            return validation;
        }

        public void Reset()
        {
            StartDay = new DateTime();
            EndDay = new DateTime();
            FreeDays.Clear();
        }

        public void Load(string storageId)
        {
            base.Load(storageId);

            var data = Storage.LoadData<CalendarData>(storageId, StorageClassId);

            StartDay = data.StartDay;
            EndDay = data.EndDay;
            FreeDays.Clear();
            FreeDays.Add(data.FreeDays.ToList<DateTime>());

        }

        public override void Save()
        {
            base.Save();

            var data = new CalendarData();

            data.StartDay = StartDay;
            data.EndDay = EndDay;
            data.FreeDays = new HashSet<DateTime>(FreeDays.ToList());

            Storage.SaveData<CalendarData>(StorageId, StorageClassId, data);
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
