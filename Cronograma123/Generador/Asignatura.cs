using CronogramaMe.Interfaz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cronogramador
{
    public class Asignatura
    {
        public enum Completitud
        {
            completa,
            faltanUnidades,
            faltanDiasSemana
        };

        class Data
        {
            public string nombre { get; set; }
            public List<int> ordenUFs { get; set; }
            public HashSet<KeyValuePair<int, int>> horasPorUF { get; set; }
            public HashSet<KeyValuePair<int, string>> titulosPorUF { get; set; }
            public HashSet<KeyValuePair<DayOfWeek, int>> horasPorDiaSemana { get; set; }

        };

        string nombre;
        List<int> ordenUFs;
        Dictionary<int, int> horasPorUF;
        Dictionary<int, string> titulosPorUF;
        Dictionary<DayOfWeek, int> horasPorDiaSemana;

        public Asignatura()
        {
            nombre = "";
            ordenUFs = new List<int>();
            horasPorUF = new Dictionary<int, int>();
            titulosPorUF = new Dictionary<int, string>();
            horasPorDiaSemana = new Dictionary<DayOfWeek, int>();
        }

        public void Imprime(bool listarDetalles)
        {
            Console.WriteLine("| Asignatura: " + nombre);
            Console.WriteLine(String.Format("|     UFs          :{0}", ordenUFs.Count));

            if (listarDetalles)
            {
                foreach (int i in ordenUFs) { Console.WriteLine(String.Format("|         UF{0}: {1} horas", i, horasPorUF[i])); }
            }

            Console.WriteLine(String.Format("|     Dias semana  :{0}", horasPorDiaSemana.Keys.Count));

            if (listarDetalles)
            {
                var lista = new List<DayOfWeek>(horasPorDiaSemana.Keys);
                lista.Sort();
                foreach (DayOfWeek d in lista) { Console.WriteLine(String.Format("|         {0}: {1} horas", Utils.TraduceDiaSemana(d), horasPorDiaSemana[d])); }
            }
        }

        public void PonNombre(string _nombre) { nombre = _nombre; }
        public string ObtenNombre() { return nombre; }
        public void AnyadeUF(int uf, string titulo, int horas) { ordenUFs.Add(uf); horasPorUF[uf] = horas; titulosPorUF[uf] = titulo; }
        public void EliminaUF(int uf) { titulosPorUF.Remove(uf); horasPorUF.Remove(uf); ordenUFs.Remove(uf); }
        public bool TieneUF(int uf) { return ordenUFs.Contains(uf); }
        public int ObtenNumUFs() { return ordenUFs.Count; }
        public int ObtenUFPorIndice(int i) { return ordenUFs[i]; }
        public int ObtenHorasUF(int uf) { return horasPorUF[uf]; }
        public string ObtenTituloUF(int uf) { return titulosPorUF[uf]; }

        public void AnyadeDiaSemana(DayOfWeek diaActual, int horas) { horasPorDiaSemana[diaActual] = horas; }
        public void EliminaDiaSemana(DayOfWeek diaActual) { horasPorDiaSemana.Remove(diaActual); }
        public bool TieneDiaSemana(DayOfWeek diaActual) { return horasPorDiaSemana.ContainsKey(diaActual); }
        public int ObtenHorasDiaSemana(DayOfWeek diaActual) { if (horasPorDiaSemana.ContainsKey(diaActual)) { return horasPorDiaSemana[diaActual]; } else { return 0; } }

        public Completitud CompruebaCompleta()
        {

            Completitud completa = Completitud.completa;

            if (ordenUFs.Count <= 0) { completa = Completitud.faltanUnidades; }
            else if (horasPorDiaSemana.Count <= 0) { completa = Completitud.faltanDiasSemana; }

            return completa;
        }

        public void ReiniciaHorario()
        {
            horasPorDiaSemana.Clear();
        }

        public void ReiniciaUFs()
        {
            ordenUFs.Clear();
            horasPorUF.Clear();
            titulosPorUF.Clear();
        }

        public void Guarda(string nombreFichero)
        {
            var stream = new FileStream(nombreFichero, FileMode.Create, FileAccess.Write);

            var writer = new StreamWriter(stream);

            var data = new Data();

            data.nombre = nombre;
            data.ordenUFs = ordenUFs;
            data.horasPorUF = new HashSet<KeyValuePair<int, int>>(horasPorUF);
            data.titulosPorUF = new HashSet<KeyValuePair<int, string>>(titulosPorUF);
            data.horasPorDiaSemana = new HashSet<KeyValuePair<DayOfWeek, int>>(horasPorDiaSemana);

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

            nombre = data.nombre;
            ordenUFs = data.ordenUFs;
            horasPorUF = new Dictionary<int, int>(data.horasPorUF);
            titulosPorUF = new Dictionary<int, string>(data.titulosPorUF);
            horasPorDiaSemana = new Dictionary<DayOfWeek, int>(data.horasPorDiaSemana);

            reader.Close();
        }
    }

}
