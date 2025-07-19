namespace Programacion123
{
    class Utils
    {
        public static void ShowError(string error)
        {
            Console.WriteLine(error);
            Thread.Sleep(2000);
        }

        public static void ShowMessage(string mensaje)
        {
            Console.WriteLine(mensaje);
            Thread.Sleep(2000);
        }

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

        public static string FormatDate(DateTime d)
        {
            return d.ToShortDateString();
        }

        public static string FormatEntity<T>(T entity, EntityFormatContent formatContent) where T:Entity
        {
            string content;
            if(formatContent == EntityFormatContent.title) { content = entity.Title; }
            else // formatContent == EntityFormatContent.description
            { content = entity.Description; }
            if(content.Length > 100) { content = content.Substring(0, Math.Min(100, content.Length)) + "..."; }

            return content;
        }

        public static string FormatEntity<T>(T entity, int index, EntityFormatContent formatContent, EntityFormatIndex formatIndex) where T:Entity
        {
            string content = FormatEntity<T>(entity, formatContent);
            string prefix;
            if(formatIndex == EntityFormatIndex.none) { prefix = ""; }
            else if(formatIndex == EntityFormatIndex.number) { prefix = (index + 1).ToString() + ": "; }
            else // formatIndex == EntityFormatIndex.character
            { prefix = System.Text.Encoding.ASCII.GetString(new byte[] { (byte)(65 + index) }).ToLower() + ": "; }

            return prefix + content;
        }



    }
}
