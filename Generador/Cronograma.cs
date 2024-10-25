using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Cronogramador
{
    public class Cronograma
    {
        public enum TipoDia
        {
            lectivo,
            festivo,
            finDeSemana
        };

        public struct HorasUF
        {
            public int uf;
            public int horas;
        };

        public struct ContenidoDia
        {
            public TipoDia tipo;
            public List<HorasUF> horasUF;
        };

        Calendario calendario;
        Asignatura asignatura;

        bool empezarUFsEnDiaNuevo;
        bool estiloContinuo;

        public Cronograma(Calendario _calendario, Asignatura _asignatura, bool _empezarUfsEnDiaNuevo, bool _estiloContinuo)
        {
            calendario = _calendario;
            asignatura = _asignatura;
            empezarUFsEnDiaNuevo = _empezarUfsEnDiaNuevo;
            estiloContinuo = _estiloContinuo;
        }

        public bool CompruebaCorrecto()
        {
            bool correcto = true;

            Dictionary<DateTime, ContenidoDia> contenido = ObtenContenido();

            foreach(DateTime d in contenido.Keys) { if(d > calendario.ObtenDiaFin()) { correcto = false; } }

            if(!correcto) { Utils.MuestraError("El cronograma va más allá del último día del calendario"); }

            return correcto;

        }

        public void GeneraExcel(string nombreFichero)
        {
            Dictionary<DateTime, ContenidoDia> contenido = ObtenContenido();

            // Obtenemos el conjunto de meses que cubre el cronograma

            var meses = new HashSet<int>();

            foreach (DateTime d in contenido.Keys)
            {
                int mes = d.Month;
                int anyo = d.Year;

                int mesAnyo = anyo * 100 + mes;

                if (!meses.Contains(mesAnyo)) { meses.Add(mesAnyo); }
            }

            // Lo pasamos a una lista y la ordenamos

            var mesesOrdenados = new List<int>(meses);
            mesesOrdenados.Sort();

            // Abrimos el fichero excel

            Application excel;
            Workbooks libros;
            Workbook libro;
            Worksheet hoja;
            object nulo = System.Reflection.Missing.Value;

            int cursorFila = Config.filaInicioMeses;
            int cursorColumna = Config.columnaInicioMeses;

            excel =  new Application();

            if (excel == null)
            {
                throw new Exception("Excel no esta instalado en el sistema");
            }

             excel.DisplayAlerts = false;

            libros = excel.Workbooks;
            libro = libros.Add(nulo);
            hoja = (Worksheet)libro.Worksheets.get_Item(1);

            // Ponemos el titulo

            hoja.Cells[Config.filaTituloAsignatura, Config.columnaTituloAsignatura] = asignatura.ObtenNombre();
            hoja.Cells[Config.filaTituloAsignatura, Config.columnaTituloAsignatura].Font.Size = Config.tamanyoTituloAsignatura;
            hoja.Cells[Config.filaTituloAsignatura, Config.columnaTituloAsignatura].Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            hoja.Cells[Config.filaTituloAsignatura, Config.columnaTituloAsignatura].Interior.Color = Config.colorTituloAsignatura;
            hoja.Cells[Config.filaTituloAsignatura, Config.columnaTituloAsignatura].Font.Color = Config.colorTextoTituloAsignatura;
            hoja.Range[hoja.Cells[Config.filaTituloAsignatura, Config.columnaTituloAsignatura], hoja.Cells[Config.filaTituloAsignatura, Config.columnaTituloAsignatura + 6]].Merge();

            // Rellenamos los meses

            for (int i = 0; i < mesesOrdenados.Count; i ++)
            {

                int mesAnyo = mesesOrdenados[i];
                int mes = mesAnyo % 100;
                int anyo = mesAnyo / 100;

                DateTime primerDia = new DateTime(anyo, mes, 1);
                DateTime ultimoDia = primerDia.AddDays(DateTime.DaysInMonth(anyo, mes) - 1);

                cursorColumna = Config.columnaInicioMeses;

                hoja.Cells[cursorFila, cursorColumna] = Utils.TraduceMes(mes) + " " + anyo;
                hoja.Cells[cursorFila, cursorColumna].Font.Size = Config.tamanyoTituloMes;
                hoja.Cells[cursorFila, cursorColumna].Font.Bold = true;
                hoja.Cells[cursorFila, cursorColumna].Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                hoja.Cells[cursorFila, cursorColumna].Interior.Color = Config.colorTituloMes;
                hoja.Cells[cursorFila, cursorColumna].Font.Color = Config.colorTextoTituloMes;
                hoja.Range[hoja.Cells[cursorFila, cursorColumna], hoja.Cells[cursorFila, cursorColumna + 6]].Merge();


                cursorFila++;

                for(int j = 1; j <= 7; j ++)
                {
                    hoja.Cells[cursorFila, cursorColumna] = Utils.TraduceDiaSemana(Utils.IndiceADiaSemana(j), true);
                    hoja.Cells[cursorFila, cursorColumna].Interior.Color = Config.colorDiaSemana;
                    hoja.Cells[cursorFila, cursorColumna].Font.Bold = true;
                    hoja.Cells[cursorFila, cursorColumna].Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;


                    cursorColumna++;
                }

                cursorFila ++;

                int anteriorUF = 0;

                for(DateTime dia = primerDia; dia <= ultimoDia; dia = dia.AddDays(1))
                {
                    cursorColumna = Config.columnaInicioMeses + Utils.DiaSemanaAIndice(dia.DayOfWeek) - 1;

                    hoja.Cells[cursorFila, cursorColumna] = dia.Day;
                    hoja.Cells[cursorFila, cursorColumna].Borders.LineStyle = XlLineStyle.xlContinuous;
                    hoja.Cells[cursorFila, cursorColumna].Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;


                    XlRgbColor color = XlRgbColor.rgbWhite;
                    uint colorRGB = 0;

                    bool ponerColor = false;
                    bool ponerColorRGB = false;

                    List<XlRgbColor> coloresGradiente = null;
                    bool ponerGradiente = false;

                    if(contenido.ContainsKey(dia))
                    {
                        ContenidoDia contenidoDia = contenido[dia];

                        if (contenidoDia.tipo == TipoDia.festivo)
                        {   
                            color = Config.colorFestivos;
                            ponerColor = true;
                        }
                        else if(contenidoDia.tipo == TipoDia.finDeSemana)
                        {   
                            color = Config.colorFinesDeSemana;
                            ponerColor = true;
                        }
                        else // contenidoDia.tipo == TipoDia.lectivo
                        {
                            List<HorasUF> horasUF = contenidoDia.horasUF;

                            uint r = 0;
                            uint g = 0;
                            uint b = 0;

                            bool tieneUFs = false;

                            if(horasUF.Count == 0)
                            {
                                if(estiloContinuo && anteriorUF > 0) { color = Config.coloresUFs[anteriorUF - 1]; ponerColor = true; }
                            }
                            else if (horasUF.Count == 1)
                            {
                                color = Config.coloresUFs[(horasUF[0].uf - 1) % Config.coloresUFs.Length];
                                ponerColor = true;
                            }
                            else if (horasUF.Count >= 2)
                            {
                                coloresGradiente = new List<XlRgbColor>();

                                for (int j = 0; j < horasUF.Count; j++)
                                {
                                    coloresGradiente.Add(Config.coloresUFs[(horasUF[j].uf - 1) % Config.coloresUFs.Length]);
                                }

                                ponerGradiente = true;
                            }

                            if(horasUF.Count > 0) { anteriorUF =  horasUF[horasUF.Count - 1].uf; }

                        }

                    }

                    if(ponerColor)
                    {
                        hoja.Cells[cursorFila, cursorColumna].Interior.Color = color;
                    }
                    else if(ponerGradiente)
                    {
                        hoja.Cells[cursorFila, cursorColumna].Interior.Pattern = XlPattern.xlPatternLinearGradient;
                        hoja.Cells[cursorFila, cursorColumna].Interior.Gradient.Degree = 0;
                        hoja.Cells[cursorFila, cursorColumna].Interior.Gradient.ColorStops.Clear();

                        for (int j = 0; j < coloresGradiente.Count; j ++)
                        {
                            hoja.Cells[cursorFila, cursorColumna].Interior.Gradient.ColorStops.Add(j * (1.0f / (coloresGradiente.Count - 1))).Color = coloresGradiente[j];
                        }

                    }

                    if (dia.DayOfWeek == DayOfWeek.Sunday) { cursorFila++; }

                }

                cursorFila = cursorFila + 2;

            }

            // Rellenamos las UFs

            cursorFila = Config.filaInicioUFs;
            cursorColumna = Config.columnaInicioUFs;

            for(int i = 0; i < asignatura.ObtenNumUFs(); i ++)
            {
                int uf = asignatura.ObtenUFPorIndice(i);
                hoja.Cells[cursorFila, cursorColumna] = "UF" + uf;
                hoja.Cells[cursorFila, cursorColumna].Interior.Color = Config.coloresUFs[(uf - 1) % Config.coloresUFs.Length];
                hoja.Cells[cursorFila, cursorColumna].Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                hoja.Cells[cursorFila, cursorColumna + 1] = asignatura.ObtenHorasUF(uf) + "h";
                hoja.Cells[cursorFila, cursorColumna + 1].Borders.LineStyle = XlLineStyle.xlContinuous;

                cursorFila ++;

            }

            // Guardamos el fichero

            libro.SaveAs(Directory.GetCurrentDirectory() +  "\\" + nombreFichero,
                        XlFileFormat.xlOpenXMLWorkbook, nulo,
                        nulo, nulo, nulo, XlSaveAsAccessMode.xlExclusive,
                        nulo, nulo, nulo, nulo, nulo);

            // Cerramos excel

            libro.Close(true, nulo, nulo);
            libros.Close();
            excel.Quit();

            Marshal.ReleaseComObject(hoja);
            Marshal.ReleaseComObject(libro);
            Marshal.ReleaseComObject(libros);
            Marshal.ReleaseComObject(excel);

        }

        Dictionary<DateTime, ContenidoDia> ObtenContenido()
        {
            var contenido = new Dictionary<DateTime, ContenidoDia>();
            ContenidoDia contenidoDia = new ContenidoDia();

            int numUFs = asignatura.ObtenNumUFs();

            DateTime diaActual = calendario.ObtenDiaInicio();

            int indiceUF = 0;
            int horasNoAsignadasUF = asignatura.ObtenHorasUF(asignatura.ObtenUFPorIndice(0));

            while (indiceUF < numUFs)
            {
                int horasDia;

                if (diaActual.DayOfWeek == DayOfWeek.Saturday || diaActual.DayOfWeek == DayOfWeek.Sunday)
                {
                    contenidoDia.tipo = TipoDia.finDeSemana;

                    horasDia = 0;
                }
                else if (calendario.EsFestivo(diaActual))
                {
                    contenidoDia.tipo = TipoDia.festivo;

                    horasDia = 0;
                }
                else
                {
                    contenidoDia.tipo = TipoDia.lectivo;
                    contenidoDia.horasUF = new List<HorasUF>();

                    horasDia = asignatura.ObtenHorasDiaSemana(diaActual.DayOfWeek);
                }

                while (horasDia > 0 && indiceUF < numUFs)
                {
                    bool avanzarUF = false;

                    if (horasNoAsignadasUF < horasDia)
                    {
                        var horasUF = new HorasUF() { uf = asignatura.ObtenUFPorIndice(indiceUF), horas = horasNoAsignadasUF };
                        contenidoDia.horasUF.Add(horasUF);

                        if (empezarUFsEnDiaNuevo)
                        {
                            horasDia = 0;
                        }
                        else
                        {
                            horasDia -= horasNoAsignadasUF;
                        }

                        horasNoAsignadasUF = 0;

                        avanzarUF = true;

                    }
                    else
                    {
                        var horasUF = new HorasUF() { uf = asignatura.ObtenUFPorIndice(indiceUF), horas = horasDia };
                        contenidoDia.horasUF.Add(horasUF);

                        horasNoAsignadasUF -= horasDia;
                        horasDia = 0;

                        if (horasNoAsignadasUF == 0)
                        {
                            avanzarUF = true;
                        }
                    }

                    if (avanzarUF)
                    {
                        indiceUF++;
                        if (indiceUF < numUFs)
                        {
                            horasNoAsignadasUF = asignatura.ObtenHorasUF(asignatura.ObtenUFPorIndice(indiceUF));
                        }

                    }

                }

                contenido[diaActual] = contenidoDia;

                diaActual = diaActual.AddDays(1);

            }

            return contenido;
        }
    }
}
