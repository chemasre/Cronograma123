using System.Diagnostics;

namespace Programacion123
{
    partial class Utils
    {
        public static string WeekdayToText(DayOfWeek diaActual, bool breve = false)
        {
            if (diaActual == DayOfWeek.Monday) { return breve ? "Lu." : "Lunes"; }
            else if (diaActual == DayOfWeek.Tuesday) { return breve ? "Ma." : "Martes"; }
            else if (diaActual == DayOfWeek.Wednesday) { return breve ? "Mi." : "Miércoles"; }
            else if (diaActual == DayOfWeek.Thursday) { return breve ? "Ju." : "Jueves"; }
            else if (diaActual == DayOfWeek.Friday) { return breve ? "Vi." : "Viernes"; }
            else if (diaActual == DayOfWeek.Saturday) { return breve ? "Sá." : "Sábado"; }
            else // diaActual == DayOfWeek.Sunday
            { return breve ? "Do." : "Domingo"; }

        }

        public static int WeekdayToIndex(DayOfWeek dia)
        {
            if (dia == DayOfWeek.Sunday) { return 7; }
            else if (dia == DayOfWeek.Monday) { return 1; }
            else if (dia == DayOfWeek.Tuesday) { return 2; }
            else if (dia == DayOfWeek.Wednesday) { return 3; }
            else if (dia == DayOfWeek.Thursday) { return 4; }
            else if (dia == DayOfWeek.Friday) { return 5; }
            else // dia == DayOfWeek.Saturday
            { return 6; }
        }

        public static DayOfWeek IndexToWeekday(int dia)
        {
            if (dia == 7) { return DayOfWeek.Sunday; }
            else if (dia == 1) { return DayOfWeek.Monday; }
            else if (dia == 2) { return DayOfWeek.Tuesday; }
            else if (dia == 3) { return DayOfWeek.Wednesday; }
            else if (dia == 4) { return DayOfWeek.Thursday; }
            else if (dia == 5) { return DayOfWeek.Friday; }
            else // dia == 6
            { return DayOfWeek.Saturday; }
        }

        public static string MonthToText(int mes)
        {
            if (mes == 1) { return "Enero"; }
            else if (mes == 2) { return "Febrero"; }
            else if (mes == 3) { return "Marzo"; }
            else if (mes == 4) { return "Abril"; }
            else if (mes == 5) { return "Mayo"; }
            else if (mes == 6) { return "Junio"; }
            else if (mes == 7) { return "Julio"; }
            else if (mes == 8) { return "Agosto"; }
            else if (mes == 9) { return "Septiembre"; }
            else if (mes == 10) { return "Octubre"; }
            else if (mes == 11) { return "Noviembre"; }
            else // mes == 12
            { return "Diciembre"; }

        }

        public enum FormatDateOptions
        {
            numericYearMonthDay,
            numericMonthDay
        }

        public static string FormatDate(DateTime d, FormatDateOptions options = FormatDateOptions.numericYearMonthDay)
        {
            if (options == FormatDateOptions.numericYearMonthDay) { return d.ToShortDateString(); }
            else // options == FormatDateOptions.numericMonthDay
            { return String.Format("{0:0}/{1:0}", d.Day, d.Month); }
        }

        public static string FormatEntity<T>(T entity, EntityFormatContent formatContent) where T : Entity
        {
            string content;
            if (formatContent == EntityFormatContent.Title) { content = entity.Title.Value; }
            else // formatContent == EntityFormatContent.description
            { content = entity.Description.Value; }
            if (content.Length > 100) { content = content.Substring(0, Math.Min(100, content.Length)) + "..."; }

            return content;
        }

        public static string FormatEntity<T>(T entity, int index, EntityFormatContent formatContent, EntityFormatIndex formatIndex) where T : Entity
        {
            string content = FormatEntity<T>(entity, formatContent);
            string prefix;
            if (formatIndex == EntityFormatIndex.None) { prefix = ""; }
            else if (formatIndex == EntityFormatIndex.Number) { prefix = (index + 1).ToString() + ": "; }
            else // formatIndex == EntityFormatIndex.character
            { prefix = FormatLetterPrefixLowercase(index) + ": "; }

            return prefix + content;
        }

        static string letters = "abcdefghijklmnñopqrstuvwxyz";

        public static string FormatLetterPrefixLowercase(int index)
        {
            return (index / letters.Length > 0 ? FormatLetterPrefixLowercase(index / letters.Length) : "") + letters[index % letters.Length];
        }

        public static string FormatStartDayHour(Subject.SchoolDayHour dayHour, WeekSchedule weekSchedule)
        {
            return FormatStartDayHour(dayHour.day, dayHour.hour, weekSchedule);
        }

        public static string FormatEndDayHour(Subject.SchoolDayHour dayHour, WeekSchedule weekSchedule)
        {
            return FormatEndDayHour(dayHour.day, dayHour.hour, weekSchedule);
        }

        public static string FormatStartDayHour(DateTime day, float hour, WeekSchedule weekSchedule)
        {
            return Utils.WeekdayToText(day.DayOfWeek) + " " +
                    Utils.FormatDate(day, Utils.FormatDateOptions.numericMonthDay) +
                    (hour != 0 ? " +" + hour + "h" : "");
        }

        public static string FormatEndDayHour(DateTime day, float hour, WeekSchedule weekSchedule)
        {
            return Utils.WeekdayToText(day.DayOfWeek) + " " +
                    Utils.FormatDate(day, Utils.FormatDateOptions.numericMonthDay) +
                    (hour != weekSchedule.HoursPerWeekDay[day.DayOfWeek] ? " +" + hour + "h" : "");
        }

        public static string FormatEvaluableActivity(int blockIndex, ActivityEvaluationType evaluationType, int activityIndex)
        {
            return String.Format(evaluationType == ActivityEvaluationType.Continous ? "B{0}-A{1}" : "B{0}-EX{1}", blockIndex + 1, activityIndex + 1);
        }

        internal static string FormatContentPoint(int contentIndex, int pointIndex)
        {
            return String.Format("{0}.{1}", contentIndex + 1, pointIndex + 1);
        }

        internal static string FormatLearningResultCriteria(int learningResultIndex, int criteriaIndex)
        {
            return String.Format("{0}.{1}", learningResultIndex + 1, criteriaIndex + 1);
        }

        internal static string FormatToFit(string text, int length = 12, bool allowEnlarge = true, bool allowShrink = true)
        {
            string result = text;
            if(text.Length < 3) { result = text; }
            else if(text.Length - 3 < length) { result = text; }
            else if(allowShrink) { result = text.Substring(0, length - 3) + "..."; }

            if(result.Length < length && allowEnlarge)
            {
                while(result.Length < length)
                {
                    result = result + ' ';
                }
            }

            return result;
        }
    }
}
