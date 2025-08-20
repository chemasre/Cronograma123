namespace Cronogramador
{
    public class Config
    {
        public int filaTituloAsignatura { get; set; } = 2;
        public int columnaTituloAsignatura { get; set; } = 3;
        public int tamanyoTituloAsignatura { get; set; } = 24;
        public XlRgbColor colorTituloAsignatura { get; set; } = XlRgbColor.rgbSteelBlue;
        public XlRgbColor colorTextoTituloAsignatura { get; set; } = XlRgbColor.rgbWhite;


        public int filaInicioMeses { get; set; } = 4;
        public int columnaInicioMeses { get; set; } = 3;

        public int tamanyoTituloMes { get; set; } = 16;
        public XlRgbColor colorTituloMes { get; set; } = XlRgbColor.rgbSteelBlue;
        public XlRgbColor colorTextoTituloMes { get; set; }  = XlRgbColor.rgbWhite;

        public XlRgbColor colorDiaSemana { get; set; } = XlRgbColor.rgbLightSteelBlue;

        public XlRgbColor[] coloresUFs { get; set; } = new XlRgbColor[] { XlRgbColor.rgbNavyBlue,
                                                                   XlRgbColor.rgbDarkOrange,
                                                                   XlRgbColor.rgbDarkGreen,
                                                                   XlRgbColor.rgbDarkRed,
                                                                   XlRgbColor.rgbDarkSlateGrey,
                                                                   XlRgbColor.rgbIndigo,
                                                                   XlRgbColor.rgbOlive,
                                                                   XlRgbColor.rgbBlack,
                                                                   XlRgbColor.rgbTeal,
                                                                   XlRgbColor.rgbDeepPink

                                                                 };

        public XlRgbColor colorTextoUFs { get; set; } = XlRgbColor.rgbWhite;
        public XlRgbColor colorFestivos { get; set; } = XlRgbColor.rgbRed;
        public XlRgbColor colorTextoFestivos { get; set; } = XlRgbColor.rgbWhite;
        public XlRgbColor colorFinesDeSemana { get; set; } = XlRgbColor.rgbGray;
        public XlRgbColor colorTextoFinesDeSemana { get; set; } = XlRgbColor.rgbDarkGray;

        public int filaInicioUFs { get; set; } = 5;
        public int columnaInicioUFs { get; set; } = 11;

        public int anchoColumnaTitulosUFs { get; set; } = 35;

        public bool empezarUfsEnDiaNuevo { get; set; } = false;
        public bool estiloContinuo { get; set; } = false;
    }
}
