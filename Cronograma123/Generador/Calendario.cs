using System.IO;
using System.Text.Json;

namespace Cronogramador
{
    public class Calendario
    {
        public enum Completitud
        {
            completa,
            fechaInicioPosteriorAFin,
            festivoFueraCalendario
        };

        class Data
        {
            public DateTime diaInicio { get; set; }
            public DateTime diaFin { get; set; }
            public HashSet<DateTime> festivos { get; set; }
        };

        DateTime diaInicio;
        DateTime diaFin;
        HashSet<DateTime> festivos;

        public Calendario()
        {
            diaInicio = new DateTime();
            diaFin = new DateTime();
            festivos = new HashSet<DateTime>();
        }

        public IReadOnlyList<DateTime> ObtenFestivos()
        {
            var lista = new List<DateTime>(festivos);
            lista.Sort();
            return lista;
        }

        public void Imprime(bool listarFestivos)
        {
            Console.WriteLine("| Calendario");
            Console.WriteLine(String.Format("|     Dia de inicio :{0}", diaInicio.ToString("dd/MM/yyyy")));
            Console.WriteLine(String.Format("|     Dia de fin    :{0}", diaFin.ToString("dd/MM/yyyy")));
            Console.WriteLine(String.Format("|     Festivos      :{0}", festivos.Count));
            if (listarFestivos)
            {
                var lista = new List<DateTime>(festivos);
                lista.Sort();
                foreach (DateTime f in lista) { Console.WriteLine("|         " + f.ToString("dd/MM/yyyy")); }
            }
        }

        public void PonDiaInicio(DateTime diaActual)
        {
            diaInicio = diaActual;
        }

        public void PonDiaFin(DateTime diaActual)
        {
            diaFin = diaActual;
        }

        public void AnyadeFestivo(DateTime fecha)
        {
            festivos.Add(fecha);
        }

        public void EliminaFestivo(DateTime fecha)
        {
            festivos.Remove(fecha);
        }

        public bool EsFestivo(DateTime fecha)
        {
            return festivos.Contains(fecha);
        }

        public DateTime ObtenDiaInicio() { return diaInicio; }
        public DateTime ObtenDiaFin() { return diaFin; }

        public Completitud CompruebaCompleta()
        {
            Completitud completitud = Completitud.completa;

            if (diaInicio > diaFin)
            {
                Console.WriteLine("La fecha de inicio no puede ser posterior a la fecha de fin");
                completitud = Completitud.fechaInicioPosteriorAFin;
                return completitud;
            }

            int i = 0;
            var listaFestivos = new List<DateTime>(festivos);

            while (completitud == Completitud.completa && i < listaFestivos.Count)
            {
                if (listaFestivos[i] > diaFin || listaFestivos[i] < diaInicio)
                {
                    //Utils.MuestraError("El festivo " + listaFestivos[i].ToString("dd/MM/yyyy") + " esta fuera del calendario");
                    completitud = Completitud.festivoFueraCalendario;
                }

                i++;
            }

            return completitud;
        }

        public void Reinicia()
        {
            diaInicio = new DateTime();
            diaFin = new DateTime();
            festivos.Clear();
        }

        public void Guarda(string nombreFichero)
        {
            var stream = new FileStream(nombreFichero, FileMode.Create, FileAccess.Write);

            var writer = new StreamWriter(stream);

            var data = new Data();

            data.diaInicio = diaInicio;
            data.diaFin = diaFin;
            data.festivos = festivos;

            JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerOptions.Default);
            options.WriteIndented = true;
            writer.Write(JsonSerializer.Serialize<Data>(data, options));
            writer.Close();
        }

        public void Carga(string nombreFichero)
        {
            var stream = new FileStream(nombreFichero, FileMode.Open, FileAccess.Read);

            var reader = new StreamReader(stream);

            var data = new Data();

            string text = reader.ReadToEnd();

            data = JsonSerializer.Deserialize<Data>(text);

            diaInicio = data.diaInicio;
            diaFin = data.diaFin;
            festivos = data.festivos;

            reader.Close();
        }

        public Calendario Clonar()
        {
            var otro = new Calendario();

            otro.diaInicio = diaInicio;
            otro.diaFin = diaFin;
            otro.festivos = new HashSet<DateTime>(festivos);

            return otro;
        }

    }
}
