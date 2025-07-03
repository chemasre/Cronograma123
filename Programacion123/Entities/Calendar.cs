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
        class SerializableData
        {
            public DateTime startDay { get; set; }
            public DateTime endDay { get; set; }
            public HashSet<DateTime> freeDays { get; set; }
        };

        public DateTime StartDay { get; set; }
        public DateTime EndDay { get; set; }


        public HashSet<DateTime> FreeDays { get; set; }

        public Calendar()
        {
            StartDay = new DateTime();
            EndDay = new DateTime();
            FreeDays = new HashSet<DateTime>();
        }

        public IReadOnlyList<DateTime> GetOrderedFreedays()
        {
            var lista = new List<DateTime>(FreeDays);
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
            var listaFestivos = new List<DateTime>(FreeDays);

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

        public override void Save(string nombreFichero)
        {
            var stream = new FileStream(nombreFichero, FileMode.Create, FileAccess.Write);

            var writer = new StreamWriter(stream);

            var data = new SerializableData();

            data.startDay = StartDay;
            data.endDay = EndDay;
            data.freeDays = FreeDays;

            JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerOptions.Default);
            options.WriteIndented = true;
            writer.Write(JsonSerializer.Serialize<SerializableData>(data, options));
            writer.Close();
        }

        public override void Load(string nombreFichero)
        {
            var stream = new FileStream(nombreFichero, FileMode.Open, FileAccess.Read);

            var reader = new StreamReader(stream);

            var data = new SerializableData();

            string text = reader.ReadToEnd();

            data = JsonSerializer.Deserialize<SerializableData>(text);

            StartDay = data.startDay;
            EndDay = data.endDay;
            FreeDays = data.freeDays;

            reader.Close();
        }

        public Calendar Clone()
        {
            var otro = new Calendar();

            otro.StartDay = StartDay;
            otro.EndDay = EndDay;
            otro.FreeDays = new HashSet<DateTime>(FreeDays);

            return otro;
        }

    }
}
