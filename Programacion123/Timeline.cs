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

namespace Programacion123
{
    internal class Timeline
    {
        Config config;

        public enum Validation
        {
            success,
            existDaysBeyondEnd,
            excelNotFound,
            fileSaveError
        };

        public enum DayType
        {
            schoolDay,
            festivity,
            weekend
        };

        public struct HoursUnit
        {
            public int unit;
            public int hours;
        };

        public struct DaySchedule
        {
            public DayType type;
            public List<HoursUnit> hoursUnits;
        };

        Calendar calendar;
        Subject subject;

        bool errorSaving;


        public Timeline(Calendar _calendar, Subject _subject)
        {
            calendar = _calendar;
            subject = _subject;

            config = new Config();
        }

        public Config GetConfiguration()
        {
            return config;
        }

        public void ResetConfiguration()
        {
            config = new Config();
        }


        public void SaveConfiguration(string fileName)
        {
            var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write);

            var writer = new StreamWriter(stream);

            JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerOptions.Default);
            options.WriteIndented = true;
            writer.Write(JsonSerializer.Serialize<Config>(config, options));
            writer.Close();
        }

        public void LoadConfiguration(string fileName)
        {
            var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            var reader = new StreamReader(stream);

            string text = reader.ReadToEnd();

            config = JsonSerializer.Deserialize<Config>(text);

            reader.Close();
        }

        public bool CheckExcelAvailable()
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

        public Validation Validate()
        {
            Validation validation = Validation.success;

            Dictionary<DateTime, DaySchedule> contenido = ObtenContenido();

            if (errorSaving)
            {
                validation = Validation.fileSaveError;
            }

            if (validation != Validation.success) { return validation; }

            foreach (DateTime d in contenido.Keys)
            {
                if (d > calendar.EndDay) { validation = Validation.existDaysBeyondEnd; break; }
            }

            return validation;

        }

        public void GenerateExcelSheet(string nombreFichero)
        {
            Dictionary<DateTime, DaySchedule> contenido = ObtenContenido();

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

            int cursorFila = config.monthsStartRow;
            int cursorColumna = config.monthsStartColumn;

            excel =  new Application();

            excel.DisplayAlerts = false;

            libros = excel.Workbooks;
            libro = libros.Add(nulo);
            hoja = (Worksheet)libro.Worksheets.get_Item(1);

            // Ponemos el titulo

            hoja.Cells[config.subjectTitleRow, config.subjectTitleColumn] = subject.Title;
            hoja.Cells[config.subjectTitleRow, config.subjectTitleColumn].Font.Size = config.subjectTitleSize;
            hoja.Cells[config.subjectTitleRow, config.subjectTitleColumn].Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            hoja.Cells[config.subjectTitleRow, config.subjectTitleColumn].Interior.Color = config.subjectTitleColor;
            hoja.Cells[config.subjectTitleRow, config.subjectTitleColumn].Font.Color = config.subjectTitleTextColor;
            hoja.Range[hoja.Cells[config.subjectTitleRow, config.subjectTitleColumn], hoja.Cells[config.subjectTitleRow, config.subjectTitleColumn + 6]].Merge();

            // Rellenamos los meses

            for (int i = 0; i < mesesOrdenados.Count; i ++)
            {

                int mesAnyo = mesesOrdenados[i];
                int mes = mesAnyo % 100;
                int anyo = mesAnyo / 100;

                DateTime primerDia = new DateTime(anyo, mes, 1);
                DateTime ultimoDia = primerDia.AddDays(DateTime.DaysInMonth(anyo, mes) - 1);

                cursorColumna = config.monthsStartColumn;

                hoja.Cells[cursorFila, cursorColumna] = Utils.MonthToText(mes) + " " + anyo;
                hoja.Cells[cursorFila, cursorColumna].Font.Size = config.monthTitleSize;
                hoja.Cells[cursorFila, cursorColumna].Font.Bold = true;
                hoja.Cells[cursorFila, cursorColumna].Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                hoja.Cells[cursorFila, cursorColumna].Interior.Color = config.monthTitleColor;
                hoja.Cells[cursorFila, cursorColumna].Font.Color = config.monthTitleTextColor;
                hoja.Range[hoja.Cells[cursorFila, cursorColumna], hoja.Cells[cursorFila, cursorColumna + 6]].Merge();


                cursorFila++;

                for(int j = 1; j <= 7; j ++)
                {
                    hoja.Cells[cursorFila, cursorColumna] = Utils.WeekdayToText(Utils.IndexToWeekday(j), true);
                    hoja.Cells[cursorFila, cursorColumna].Interior.Color = config.weekDayColor;
                    hoja.Cells[cursorFila, cursorColumna].Font.Bold = true;
                    hoja.Cells[cursorFila, cursorColumna].Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;


                    cursorColumna++;
                }

                cursorFila ++;

                int anteriorUF = 0;

                for(DateTime dia = primerDia; dia <= ultimoDia; dia = dia.AddDays(1))
                {
                    cursorColumna = config.monthsStartColumn + Utils.WeekdayToIndex(dia.DayOfWeek) - 1;

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
                        DaySchedule contenidoDia = contenido[dia];

                        if (contenidoDia.type == DayType.festivity)
                        {   
                            color = config.freeDaysColor;
                            colorTexto = config.freeDaysTextColor;
                            ponerColor = true;
                            ponerColorTexto = true;
                        }
                        else if(contenidoDia.type == DayType.weekend)
                        {   
                            color = config.weekendColor;
                            colorTexto = config.weekendTextColor;
                            ponerColor = true;
                            ponerColorTexto = true;
                        }
                        else // contenidoDia.tipo == TipoDia.lectivo
                        {
                            List<HoursUnit> horasUF = contenidoDia.hoursUnits;

                            uint r = 0;
                            uint g = 0;
                            uint b = 0;

                            bool tieneUFs = false;

                            if(horasUF.Count == 0)
                            {
                                if(config.continousStyle && anteriorUF > 0)
                                {
                                    color = config.unitsColor[anteriorUF - 1];
                                    colorTexto = config.unitsTextColor;
                                    ponerColor = true;
                                    ponerColorTexto = true;
                                 }
                            }
                            else if (horasUF.Count == 1)
                            {
                                color = config.unitsColor[(horasUF[0].unit - 1) % config.unitsColor.Length];
                                colorTexto = config.unitsTextColor;
                                ponerColor = true;
                                ponerColorTexto = true;
                            }
                            else if (horasUF.Count >= 2)
                            {
                                coloresGradiente = new List<XlRgbColor>();

                                for (int j = 0; j < horasUF.Count; j++)
                                {
                                    coloresGradiente.Add(config.unitsColor[(horasUF[j].unit - 1) % config.unitsColor.Length]);
                                }

                                ponerGradiente = true;

                                colorTexto = config.unitsTextColor;
                                ponerColorTexto = true;
                            }

                            if (horasUF.Count > 0) { anteriorUF =  horasUF[horasUF.Count - 1].unit; }

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

            cursorFila = config.unitsStartRow;
            cursorColumna = config.unitsStartColumn;

            hoja.Columns[cursorColumna + 1].ColumnWidth = config.unitysTitleColumnWidth;

            for(int i = 0; i < subject.UnitsSequence.Count; i ++)
            {
                int uf = subject.UnitsSequence[i].Id;
                hoja.Cells[cursorFila, cursorColumna] = uf;
                hoja.Cells[cursorFila, cursorColumna].Interior.Color = config.unitsColor[(uf - 1) % config.unitsColor.Length];
                hoja.Cells[cursorFila, cursorColumna].Font.Color = config.unitsTextColor;
                hoja.Cells[cursorFila, cursorColumna].Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                hoja.Cells[cursorFila, cursorColumna + 1] = subject.UnitsById[uf].Title;
                hoja.Cells[cursorFila, cursorColumna + 1].Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                hoja.Cells[cursorFila, cursorColumna + 1].Borders.LineStyle = XlLineStyle.xlContinuous;
                hoja.Cells[cursorFila, cursorColumna + 2] = subject.UnitsById[uf].Hours + "h";
                hoja.Cells[cursorFila, cursorColumna + 2].Borders.LineStyle = XlLineStyle.xlContinuous;

                cursorFila ++;

            }

            // Rellenamos la leyenda

            cursorFila++;

            hoja.Cells[cursorFila, cursorColumna] = "";
            hoja.Cells[cursorFila, cursorColumna].Interior.Color = config.weekendColor;
            hoja.Cells[cursorFila, cursorColumna].Font.Color = config.weekendTextColor;
            hoja.Cells[cursorFila, cursorColumna].Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            hoja.Cells[cursorFila, cursorColumna + 1].Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            hoja.Cells[cursorFila, cursorColumna + 1] = "Fin de semana";
            hoja.Cells[cursorFila, cursorColumna + 1].Borders.LineStyle = XlLineStyle.xlContinuous;

            if (calendar.FreeDays.Count > 0)
            {
                cursorFila ++;

                hoja.Cells[cursorFila, cursorColumna] = "";
                hoja.Cells[cursorFila, cursorColumna].Interior.Color = config.freeDaysColor;
                hoja.Cells[cursorFila, cursorColumna].Font.Color = config.freeDaysTextColor;
                hoja.Cells[cursorFila, cursorColumna].Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                hoja.Cells[cursorFila, cursorColumna + 1] = "Festivo";
                hoja.Cells[cursorFila, cursorColumna + 1].Borders.LineStyle = XlLineStyle.xlContinuous;
            }


            // Guardamos el fichero

            errorSaving = false;

            try
            {
                libro.SaveAs(nombreFichero,
                            XlFileFormat.xlOpenXMLWorkbook, nulo,
                            nulo, nulo, nulo, XlSaveAsAccessMode.xlExclusive,
                            nulo, nulo, nulo, nulo, nulo);
            }
            catch (Exception e)
            {
                errorSaving = true;
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

        Dictionary<DateTime, DaySchedule> ObtenContenido()
        {
            var contenido = new Dictionary<DateTime, DaySchedule>();
            DaySchedule contenidoDia = new DaySchedule();

            int numUFs = subject.UnitsSequence.Count;

            DateTime diaActual = calendar.StartDay;

            int indiceUF = 0;
            int horasNoAsignadasUF = subject.UnitsById[subject.UnitsSequence[0].Id].Hours;

            while (indiceUF < numUFs)
            {
                int horasDia;

                if (diaActual.DayOfWeek == DayOfWeek.Saturday || diaActual.DayOfWeek == DayOfWeek.Sunday)
                {
                    contenidoDia.type = DayType.weekend;

                    horasDia = 0;
                }
                else if (calendar.FreeDays.Contains(diaActual))
                {
                    contenidoDia.type = DayType.festivity;

                    horasDia = 0;
                }
                else
                {
                    contenidoDia.type = DayType.schoolDay;
                    contenidoDia.hoursUnits = new List<HoursUnit>();

                    horasDia = subject.WeekSchedule.HoursPerWeekDay[diaActual.DayOfWeek];
                }

                while (horasDia > 0 && indiceUF < numUFs)
                {
                    bool avanzarUF = false;

                    if (horasNoAsignadasUF < horasDia)
                    {
                        var horasUF = new HoursUnit() { unit = subject.UnitsSequence[indiceUF].Id, hours = horasNoAsignadasUF };
                        contenidoDia.hoursUnits.Add(horasUF);

                        if (config.startUnitsInNewDay)
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
                        var horasUF = new HoursUnit() { unit = subject.UnitsSequence[indiceUF].Id, hours = horasDia };
                        contenidoDia.hoursUnits.Add(horasUF);

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
                            horasNoAsignadasUF = subject.UnitsById[subject.UnitsSequence[indiceUF].Id].Hours;
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
