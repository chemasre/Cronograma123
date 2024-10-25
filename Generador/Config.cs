using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cronogramador
{
    class Config
    {
        public const int filaTituloAsignatura = 2;
        public const int columnaTituloAsignatura = 3;
        public const int tamanyoTituloAsignatura = 24;
        public static XlRgbColor colorTituloAsignatura = XlRgbColor.rgbSteelBlue;
        public static XlRgbColor colorTextoTituloAsignatura = XlRgbColor.rgbWhite;


        public const int filaInicioMeses = 4;
        public const int columnaInicioMeses = 3;

        public const int tamanyoTituloMes = 16;
        public static XlRgbColor colorTituloMes = XlRgbColor.rgbSteelBlue;
        public static XlRgbColor colorTextoTituloMes = XlRgbColor.rgbWhite;

        public static XlRgbColor colorDiaSemana = XlRgbColor.rgbLightSteelBlue;

        public static XlRgbColor[] coloresUFs = new XlRgbColor[] { XlRgbColor.rgbSkyBlue,
                                                                   XlRgbColor.rgbTeal,
                                                                   XlRgbColor.rgbLimeGreen,
                                                                   XlRgbColor.rgbOliveDrab,
                                                                   XlRgbColor.rgbSlateBlue,
                                                                   XlRgbColor.rgbDarkOrange,
                                                                   XlRgbColor.rgbGold
                                                                 };
        public static XlRgbColor colorFestivos = XlRgbColor.rgbRed;
        public static XlRgbColor colorFinesDeSemana = XlRgbColor.rgbSilver;

        public const int filaInicioUFs = 5;
        public const int columnaInicioUFs = 11;
    }
}
