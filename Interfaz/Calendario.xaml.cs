using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for Calendario.xaml
    /// </summary>
    public partial class Calendario : Window
    {
        Cronogramador.Calendario calendario;

        public Calendario(Cronogramador.Calendario c)
        {
            DateTime today = DateTime.Now;
            
            calendario = c;

            InitializeComponent();

            DiaInicio.SelectedDate= calendario.ObtenDiaInicio();
            DiaFin.SelectedDate = calendario.ObtenDiaFin();
            FestivoAnyadir.SelectedDate = calendario.ObtenDiaInicio();
            FestivoQuitar.SelectedDate = calendario.ObtenDiaInicio();


        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ActualizaDias()
        {
           IReadOnlyList<DateTime>  festivos = calendario.ObtenFestivos();

            ListaFestivos.Items.Clear();

            for(int i = 0; i < festivos.Count; i ++)
            {
                ListaFestivos.Items.Add(festivos[i].ToShortDateString());
            }
        }

        private void AnyadirFestivo_Click(object sender, RoutedEventArgs e)
        {
            DateTime dia = FestivoAnyadir.SelectedDate.GetValueOrDefault();
            if (calendario.EsFestivo(dia))
            {
                MessageBox.Show("Ya añadiste ese día");
                return;
            }

            calendario.AnyadeFestivo(dia);

            if(!calendario.CompruebaCorrecto())
            {
                MessageBox.Show("No puedes añadir ese día");
                calendario.EliminaFestivo(dia);
                return;
            }

            FestivoAnyadir.SelectedDate = dia.AddDays(1);
            ActualizaDias();
        }

        static bool ignore = false;

        private void DiaInicioChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ignore) { return; }

            DateTime dia = DiaInicio.SelectedDate.GetValueOrDefault();
            DateTime anterior = calendario.ObtenDiaInicio();
            calendario.PonDiaInicio(dia);

            if(!calendario.CompruebaCorrecto())
            {
                MessageBox.Show("No puedes poner ese día");
                calendario.PonDiaInicio(anterior);
                ignore = true;
                DiaInicio.SelectedDate = anterior;
                ignore = false;
            }
        }

        private void DiaFinChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ignore) { return; }

            DateTime dia = DiaFin.SelectedDate.GetValueOrDefault();
            DateTime anterior = calendario.ObtenDiaFin();
            calendario.PonDiaFin(dia);

            if (!calendario.CompruebaCorrecto())
            {
                MessageBox.Show("No puedes poner ese día");
                calendario.PonDiaFin(anterior);
                ignore = true;
                DiaFin.SelectedDate = anterior;
                ignore = false;
            }
        }

        private void QuitarFestivo_Click(object sender, RoutedEventArgs e)
        {
            DateTime dia = FestivoAnyadir.SelectedDate.GetValueOrDefault();
            if (!calendario.EsFestivo(dia))
            {
                MessageBox.Show("No tienes ese festivo en la lista");
                return;
            }

            calendario.EliminaFestivo(dia);

            ActualizaDias();

            FestivoAnyadir.SelectedDate = dia.AddDays(1);
        }

        private void ListaFestivos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FestivoQuitar.SelectedDate = DateTime.Parse(ListaFestivos.SelectedValue.ToString());
        }
    }
}
