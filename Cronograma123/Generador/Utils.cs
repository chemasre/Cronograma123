namespace Cronogramador
{
    class Utils
    {
        public static void MuestraError(string error)
        {
            Console.WriteLine(error);
            Thread.Sleep(2000);
        }

        public static void MuestraMensaje(string mensaje)
        {
            Console.WriteLine(mensaje);
            Thread.Sleep(2000);
        }

        public static string TraduceDiaSemana(DayOfWeek diaActual, bool breve = false)
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

        public static int DiaSemanaAIndice(DayOfWeek dia)
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

        public static DayOfWeek IndiceADiaSemana(int dia)
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

        public static string TraduceMes(int mes)
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
    }
}
