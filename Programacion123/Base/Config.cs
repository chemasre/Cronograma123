using Microsoft.Office.Interop.Excel;

namespace Programacion123
{
    public class Config
    {
        public int subjectTitleRow { get; set; } = 2;
        public int subjectTitleColumn { get; set; } = 3;
        public int subjectTitleSize { get; set; } = 24;
        public XlRgbColor subjectTitleColor { get; set; } = XlRgbColor.rgbSteelBlue;
        public XlRgbColor subjectTitleTextColor { get; set; } = XlRgbColor.rgbWhite;


        public int monthsStartRow { get; set; } = 4;
        public int monthsStartColumn { get; set; } = 3;

        public int monthTitleSize { get; set; } = 16;
        public XlRgbColor monthTitleColor { get; set; } = XlRgbColor.rgbSteelBlue;
        public XlRgbColor monthTitleTextColor { get; set; }  = XlRgbColor.rgbWhite;

        public XlRgbColor weekDayColor { get; set; } = XlRgbColor.rgbLightSteelBlue;

        public XlRgbColor[] unitsColor { get; set; } = new XlRgbColor[] { XlRgbColor.rgbNavyBlue,
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

        public XlRgbColor unitsTextColor { get; set; } = XlRgbColor.rgbWhite;
        public XlRgbColor freeDaysColor { get; set; } = XlRgbColor.rgbRed;
        public XlRgbColor freeDaysTextColor { get; set; } = XlRgbColor.rgbWhite;
        public XlRgbColor weekendColor { get; set; } = XlRgbColor.rgbGray;
        public XlRgbColor weekendTextColor { get; set; } = XlRgbColor.rgbDarkGray;

        public int unitsStartRow { get; set; } = 5;
        public int unitsStartColumn { get; set; } = 11;

        public int unitysTitleColumnWidth { get; set; } = 35;

        public bool startUnitsInNewDay { get; set; } = false;
        public bool continousStyle { get; set; } = false;
    }
}
