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
        class Data
        {
            public string nombre { get; set; }
            public List<int> ordenUFs { get; set; }
            public HashSet<KeyValuePair<int, int>> horasPorUF { get; set; }
            public HashSet<KeyValuePair<DayOfWeek, int>> horasPorDiaSemana { get; set; }

        };

        string nombre;
        List<int> ordenUFs;
        Dictionary<int, int> horasPorUF;
        Dictionary<DayOfWeek, int> horasPorDiaSemana;

        public Asignatura()
        {
            nombre = "";
            ordenUFs = new List<int>();
            horasPorUF = new Dictionary<int, int>();
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
        public void AnyadeUF(int uf, int horas) { ordenUFs.Add(uf); horasPorUF[uf] = horas; }
        public void EliminaUF(int uf) { horasPorUF.Remove(uf); ordenUFs.Remove(uf); }
        public bool TieneUF(int uf) { return ordenUFs.Contains(uf); }
        public int ObtenNumUFs() { return ordenUFs.Count; }
        public int ObtenUFPorIndice(int i) { return ordenUFs[i]; }
        public int ObtenHorasUF(int uf) { return horasPorUF[uf]; }

        public void AnyadeDiaSemana(DayOfWeek diaActual, int horas) { horasPorDiaSemana[diaActual] = horas; }
        public void EliminaDiaSemana(DayOfWeek diaActual) { horasPorDiaSemana.Remove(diaActual); }
        public bool TieneDiaSemana(DayOfWeek diaActual) { return horasPorDiaSemana.ContainsKey(diaActual); }
        public int ObtenHorasDiaSemana(DayOfWeek diaActual) { if (horasPorDiaSemana.ContainsKey(diaActual)) { return horasPorDiaSemana[diaActual]; } else { return 0; } }

        public bool CompruebaCorrecta()
        {
            bool correcta = true;

            if (ordenUFs.Count <= 0) { Utils.MuestraError("La asignatura no tiene ninguna UF"); correcta = false; }
            else if (horasPorDiaSemana.Count <= 0) { Utils.MuestraError("La asignatura no tiene ningun diaActual semanal asignado"); correcta = false; }

            return correcta;
        }

        public void Guarda(string nombreFichero)
        {
            var stream = new FileStream(nombreFichero, FileMode.Create, FileAccess.Write);

            var writer = new StreamWriter(stream);

            var data = new Data();

            data.nombre = nombre;
            data.ordenUFs = ordenUFs;
            data.horasPorUF = new HashSet<KeyValuePair<int, int>>(horasPorUF);
            data.horasPorDiaSemana = new HashSet<KeyValuePair<DayOfWeek, int>>(horasPorDiaSemana);

            writer.Write(JsonSerializer.Serialize<Data>(data));
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
            horasPorDiaSemana = new Dictionary<DayOfWeek, int>(data.horasPorDiaSemana);

            reader.Close();
        }
    }

}
