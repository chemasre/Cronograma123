using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Cronogramador;

namespace CronogramaMe
{
    /// <summary>
    /// Interaction logic for Horario.xaml
    /// </summary>
    public partial class Horario : Window
    {
        Asignatura asignatura;

        public Horario(Asignatura a)
        {
            asignatura = a;

            InitializeComponent();

            ActualizaDias();
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private string DiaANombre(DayOfWeek d)
        {
            if (d == DayOfWeek.Monday) { return "Lunes"; }
            else if (d == DayOfWeek.Tuesday) { return "Martes"; }
            else if (d == DayOfWeek.Wednesday) { return "Miércoles"; }
            else if (d == DayOfWeek.Thursday) { return "Jueves"; }
            else // d == DayOfWeek.Friday
            { return "Viernes"; }
        }

        private DayOfWeek NombreADia(string n)
        {
            if (n == "Lunes") { return DayOfWeek.Monday; }
            else if (n == "Martes") { return DayOfWeek.Tuesday; }
            else if (n == "Miércoles") { return DayOfWeek.Wednesday; }
            else if (n == "Jueves") { return DayOfWeek.Thursday; }
            else  // n == "Viernes"
            { return DayOfWeek.Friday; }
        }



        private void ActualizaDias()
        {
            ListaDias.Items.Clear();

            List<string> dias;
            for(int i = 0; i < 5; i++)
            {
                DayOfWeek diaSemana = (DayOfWeek)(i + 1);
                if(asignatura.TieneDiaSemana(diaSemana))
                {
                    int horas = asignatura.ObtenHorasDiaSemana(diaSemana);
                    ListaDias.Items.Add(DiaANombre(diaSemana) + " " + horas + (horas > 1 ? " horas" : " hora"));
                }

            }

        }


        private void AnyadirDia_Click(object sender, RoutedEventArgs e)
        {            
            asignatura.AnyadeDiaSemana((DayOfWeek)(DiaAnyadir.SelectedIndex + 1), HorasAnyadir.SelectedIndex + 1);
            ActualizaDias();
            if (DiaAnyadir.SelectedIndex < DiaAnyadir.Items.Count) { DiaAnyadir.SelectedIndex ++; }
        }

        private void QuitarDia_Click(object sender, RoutedEventArgs e)
        {
            DayOfWeek dia = (DayOfWeek)(DiaQuitar.SelectedIndex + 1);
            if(asignatura.TieneDiaSemana(dia))
            {
                asignatura.EliminaDiaSemana(dia);
                ActualizaDias();
                if (DiaQuitar.SelectedIndex < DiaQuitar.Items.Count) { DiaQuitar.SelectedIndex++; }
            }
            else
            {
                MessageBox.Show("No tienes ese día en la lista");
            }
        }

        private void ListaDias_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListaDias.SelectedValue != null)
            {
                string diaLine = ListaDias.SelectedValue.ToString();
                string[] diaParts = diaLine.Split(' ');
                DayOfWeek dia = NombreADia(diaParts[0]);
                DiaAnyadir.SelectedIndex = ((int)dia - 1);
                int horas = Int32.Parse(diaParts[1]);
                HorasAnyadir.SelectedIndex = horas - 1;
                DiaQuitar.SelectedIndex = ((int)dia - 1);

            }
        }
    }
}
