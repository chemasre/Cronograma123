using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cronogramador
{
    public class Cronograma
    {
        Config config;

        public enum Resultado
        {
            exito,
            hayDiasMasAllaDelFinal,
            excelNoInstalado,
            errorAlGuardarFichero
        };

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

        bool errorGuardado;


        public Cronograma(Calendario _calendario, Asignatura _asignatura)
        {
            calendario = _calendario;
            asignatura = _asignatura;

            config = new Config();
        }

        public Config ObtenConfiguracion()
        {
            return config;
        }

        public void ReiniciaConfiguracion()
        {
            config = new Config();
        }


        public void GuardaConfiguracion(string nombreFichero)
        {
            var stream = new FileStream(nombreFichero, FileMode.Create, FileAccess.Write);

            var writer = new StreamWriter(stream);

            JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerOptions.Default);
            options.WriteIndented = true;
            writer.Write(JsonSerializer.Serialize<Config>(config, options));
            writer.Close();
        }

        public void CargaConfiguracion(string nombreFichero)
        {
            var stream = new FileStream(nombreFichero, FileMode.Open, FileAccess.Read);

            var reader = new StreamReader(stream);

            string text = reader.ReadToEnd();

            config = JsonSerializer.Deserialize<Config>(text);

            reader.Close();
        }

        public bool CompruebaExcelDisponible()
        {
            Application excel;
            excel = new Application();
            if (excel == null)
            {
                return false;
            }
            else
            {
                excel.Quit();
                Marshal.ReleaseComObject(excel);
                return true;

            }
        }

        public Resultado CompruebaResultado()
        {
            Resultado completa = Resultado.exito;

            Dictionary<DateTime, ContenidoDia> contenido = ObtenContenido();

            if (errorGuardado)
            {
                completa = Resultado.errorAlGuardarFichero;
            }

            if (completa != Resultado.exito) { return completa; }

            foreach (DateTime d in contenido.Keys)
            {
                if (d > calendario.ObtenDiaFin()) { completa = Resultado.hayDiasMasAllaDelFinal; break; }
            }

            return completa;

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

            int cursorFila = config.filaInicioMeses;
            int cursorColumna = config.columnaInicioMeses;

            excel =  new Application();

            excel.DisplayAlerts = false;

            libros = excel.Workbooks;
            libro = libros.Add(nulo);
            hoja = (Worksheet)libro.Worksheets.get_Item(1);

            // Ponemos el titulo

            hoja.Cells[config.filaTituloAsignatura, config.columnaTituloAsignatura] = asignatura.ObtenNombre();
            hoja.Cells[config.filaTituloAsignatura, config.columnaTituloAsignatura].Font.Size = config.tamanyoTituloAsignatura;
            hoja.Cells[config.filaTituloAsignatura, config.columnaTituloAsignatura].Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            hoja.Cells[config.filaTituloAsignatura, config.columnaTituloAsignatura].Interior.Color = config.colorTituloAsignatura;
            hoja.Cells[config.filaTituloAsignatura, config.columnaTituloAsignatura].Font.Color = config.colorTextoTituloAsignatura;
            hoja.Range[hoja.Cells[config.filaTituloAsignatura, config.columnaTituloAsignatura], hoja.Cells[config.filaTituloAsignatura, config.columnaTituloAsignatura + 6]].Merge();

            // Rellenamos los meses

            for (int i = 0; i < mesesOrdenados.Count; i ++)
            {

                int mesAnyo = mesesOrdenados[i];
                int mes = mesAnyo % 100;
                int anyo = mesAnyo / 100;

                DateTime primerDia = new DateTime(anyo, mes, 1);
                DateTime ultimoDia = primerDia.AddDays(DateTime.DaysInMonth(anyo, mes) - 1);

                cursorColumna = config.columnaInicioMeses;

                hoja.Cells[cursorFila, cursorColumna] = Utils.TraduceMes(mes) + " " + anyo;
                hoja.Cells[cursorFila, cursorColumna].Font.Size = config.tamanyoTituloMes;
                hoja.Cells[cursorFila, cursorColumna].Font.Bold = true;
                hoja.Cells[cursorFila, cursorColumna].Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                hoja.Cells[cursorFila, cursorColumna].Interior.Color = config.colorTituloMes;
                hoja.Cells[cursorFila, cursorColumna].Font.Color = config.colorTextoTituloMes;
                hoja.Range[hoja.Cells[cursorFila, cursorColumna], hoja.Cells[cursorFila, cursorColumna + 6]].Merge();


                cursorFila++;

                for(int j = 1; j <= 7; j ++)
                {
                    hoja.Cells[cursorFila, cursorColumna] = Utils.TraduceDiaSemana(Utils.IndiceADiaSemana(j), true);
                    hoja.Cells[cursorFila, cursorColumna].Interior.Color = config.colorDiaSemana;
                    hoja.Cells[cursorFila, cursorColumna].Font.Bold = true;
                    hoja.Cells[cursorFila, cursorColumna].Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;


                    cursorColumna++;
                }

                cursorFila ++;

                int anteriorUF = 0;

                for(DateTime dia = primerDia; dia <= ultimoDia; dia = dia.AddDays(1))
                {
                    cursorColumna = config.columnaInicioMeses + Utils.DiaSemanaAIndice(dia.DayOfWeek) - 1;

                    hoja.Cells[cursorFila, cursorColumna] = dia.Day;
                    hoja.Cells[cursorFila, cursorColumna].Borders.LineStyle = XlLineStyle.xlContinuous;
                    hoja.Cells[cursorFila, cursorColumna].Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;


                    XlRgbColor color = XlRgbColor.rgbWhite;
                    XlRgbColor colorTexto = XlRgbColor.rgbWhite;
                    uint colorRGB = 0;

                    bool ponerColor = false;
                    bool ponerColorTexto = false;
                    bool ponerColorRGB = false;

                    List<XlRgbColor> coloresGradiente = null;
                    bool ponerGradiente = false;

                    if(contenido.ContainsKey(dia))
                    {
                        ContenidoDia contenidoDia = contenido[dia];

                        if (contenidoDia.tipo == TipoDia.festivo)
                        {   
                            color = config.colorFestivos;
                            colorTexto = config.colorTextoFestivos;
                            ponerColor = true;
                            ponerColorTexto = true;
                        }
                        else if(contenidoDia.tipo == TipoDia.finDeSemana)
                        {   
                            color = config.colorFinesDeSemana;
                            colorTexto = config.colorTextoFinesDeSemana;
                            ponerColor = true;
                            ponerColorTexto = true;
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
                                if(config.estiloContinuo && anteriorUF > 0)
                                {
                                    color = config.coloresUFs[anteriorUF - 1];
                                    colorTexto = config.colorTextoUFs;
                                    ponerColor = true;
                                    ponerColorTexto = true;
                                 }
                            }
                            else if (horasUF.Count == 1)
                            {
                                color = config.coloresUFs[(horasUF[0].uf - 1) % config.coloresUFs.Length];
                                colorTexto = config.colorTextoUFs;
                                ponerColor = true;
                                ponerColorTexto = true;
                            }
                            else if (horasUF.Count >= 2)
                            {
                                coloresGradiente = new List<XlRgbColor>();

                                for (int j = 0; j < horasUF.Count; j++)
                                {
                                    coloresGradiente.Add(config.coloresUFs[(horasUF[j].uf - 1) % config.coloresUFs.Length]);
                                }

                                ponerGradiente = true;

                                colorTexto = config.colorTextoUFs;
                                ponerColorTexto = true;
                            }

                            if (horasUF.Count > 0) { anteriorUF =  horasUF[horasUF.Count - 1].uf; }

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

                    if(ponerColorTexto)
                    {
                        hoja.Cells[cursorFila, cursorColumna].Font.Color = colorTexto;
                    }

                    if (dia.DayOfWeek == DayOfWeek.Sunday) { cursorFila++; }

                }

                cursorFila = cursorFila + 2;

            }

            // Rellenamos las UFs

            cursorFila = config.filaInicioUFs;
            cursorColumna = config.columnaInicioUFs;

            hoja.Columns[cursorColumna + 1].ColumnWidth = config.anchoColumnaTitulosUFs;

            for(int i = 0; i < asignatura.ObtenNumUFs(); i ++)
            {
                int uf = asignatura.ObtenUFPorIndice(i);
                hoja.Cells[cursorFila, cursorColumna] = uf;
                hoja.Cells[cursorFila, cursorColumna].Interior.Color = config.coloresUFs[(uf - 1) % config.coloresUFs.Length];
                hoja.Cells[cursorFila, cursorColumna].Font.Color = config.colorTextoUFs;
                hoja.Cells[cursorFila, cursorColumna].Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                hoja.Cells[cursorFila, cursorColumna + 1] = asignatura.ObtenTituloUF(uf);
                hoja.Cells[cursorFila, cursorColumna + 1].Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                hoja.Cells[cursorFila, cursorColumna + 1].Borders.LineStyle = XlLineStyle.xlContinuous;
                hoja.Cells[cursorFila, cursorColumna + 2] = asignatura.ObtenHorasUF(uf) + "h";
                hoja.Cells[cursorFila, cursorColumna + 2].Borders.LineStyle = XlLineStyle.xlContinuous;

                cursorFila ++;

            }

            // Rellenamos la leyenda

            cursorFila++;

            hoja.Cells[cursorFila, cursorColumna] = "";
            hoja.Cells[cursorFila, cursorColumna].Interior.Color = config.colorFinesDeSemana;
            hoja.Cells[cursorFila, cursorColumna].Font.Color = config.colorTextoFinesDeSemana;
            hoja.Cells[cursorFila, cursorColumna].Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            hoja.Cells[cursorFila, cursorColumna + 1].Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            hoja.Cells[cursorFila, cursorColumna + 1] = "Fin de semana";
            hoja.Cells[cursorFila, cursorColumna + 1].Borders.LineStyle = XlLineStyle.xlContinuous;

            if (calendario.ObtenFestivos().Count > 0)
            {
                cursorFila ++;

                hoja.Cells[cursorFila, cursorColumna] = "";
                hoja.Cells[cursorFila, cursorColumna].Interior.Color = config.colorFestivos;
                hoja.Cells[cursorFila, cursorColumna].Font.Color = config.colorTextoFestivos;
                hoja.Cells[cursorFila, cursorColumna].Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                hoja.Cells[cursorFila, cursorColumna + 1] = "Festivo";
                hoja.Cells[cursorFila, cursorColumna + 1].Borders.LineStyle = XlLineStyle.xlContinuous;
            }


            // Guardamos el fichero

            errorGuardado = false;

            try
            {
                libro.SaveAs(nombreFichero,
                            XlFileFormat.xlOpenXMLWorkbook, nulo,
                            nulo, nulo, nulo, XlSaveAsAccessMode.xlExclusive,
                            nulo, nulo, nulo, nulo, nulo);
            }
            catch (Exception e)
            {
                errorGuardado = true;
                libros.Close();
                excel.Quit();
                return;
            }

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

                        if (config.empezarUfsEnDiaNuevo)
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
