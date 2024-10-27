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
using Message = CronogramaMe.Interfaz.MessageBox;

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
            calendario = c;

            InitializeComponent();

            DiaInicio.SelectedDate= calendario.ObtenDiaInicio();
            DiaFin.SelectedDate = calendario.ObtenDiaFin();
            FestivoAnyadir.SelectedDate = calendario.ObtenDiaInicio();
            FestivoQuitar.SelectedDate = calendario.ObtenDiaInicio();

            ActualizaDias();


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
                Message m = new Message(Message.Type.alert, "Ya añadiste ese día");
                m.ShowDialog();
                return;
            }

            calendario.AnyadeFestivo(dia);

            Cronogramador.Calendario.Completitud completitud = calendario.CompruebaCompleta();
            if(completitud == Cronogramador.Calendario.Completitud.festivoFueraCalendario)
            {
                Message m = new Message(Message.Type.alert, "No puedes añadir ese día porque no está entre las fechas de inicio y fin del curso");
                m.ShowDialog();

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

            Cronogramador.Calendario.Completitud completitud = calendario.CompruebaCompleta();
            if (completitud == Cronogramador.Calendario.Completitud.festivoFueraCalendario)
            {
                Message m = new Message(Message.Type.alert, "No puedes poner ese día como inicio porque quedarían festivos fuera de las fechas de inicio y fin del curso");
                m.ShowDialog();
                calendario.PonDiaInicio(anterior);
                ignore = true;
                DiaInicio.SelectedDate = anterior;
                ignore = false;
                return;
            }
            else if(completitud == Cronogramador.Calendario.Completitud.fechaInicioPosteriorAFin)
            {
                Message m = new Message(Message.Type.alert, "No puedes poner ese día como inicio porque es posterior a la fecha de fin");
                m.ShowDialog();
                ignore = true;
                DiaInicio.SelectedDate = anterior;
                ignore = false;
                return;
            }
        }

        private void DiaFinChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ignore) { return; }

            DateTime dia = DiaFin.SelectedDate.GetValueOrDefault();
            DateTime anterior = calendario.ObtenDiaFin();
            calendario.PonDiaFin(dia);


            Cronogramador.Calendario.Completitud completitud = calendario.CompruebaCompleta();
            if (completitud == Cronogramador.Calendario.Completitud.festivoFueraCalendario)
            {
                Message m = new Message(Message.Type.alert, "No puedes poner ese día como fin porque quedarían festivos fuera de las fechas de inicio y fin del curso");
                m.ShowDialog();
                calendario.PonDiaFin(anterior);
                ignore = true;
                DiaFin.SelectedDate = anterior;
                ignore = false;
                return;
            }
            else if (completitud == Cronogramador.Calendario.Completitud.fechaInicioPosteriorAFin)
            {
                Message m = new Message(Message.Type.alert, "No puedes poner ese día como fin porque es anterior a la fecha de inicio");
                m.ShowDialog();
                ignore = true;
                DiaFin.SelectedDate = anterior;
                ignore = false;
                return;
            }
        }

        private void QuitarFestivo_Click(object sender, RoutedEventArgs e)
        {
            DateTime dia = FestivoAnyadir.SelectedDate.GetValueOrDefault();
            if (!calendario.EsFestivo(dia))
            {
                Message m = new Message(Message.Type.alert, "No tienes ese festivo en la lista");
                m.ShowDialog();
                return;
            }

            calendario.EliminaFestivo(dia);

            ActualizaDias();

            FestivoAnyadir.SelectedDate = dia.AddDays(1);
        }

        private void ListaFestivos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ListaFestivos.SelectedValue != null)
            {
                FestivoQuitar.SelectedDate = DateTime.Parse(ListaFestivos.SelectedValue.ToString());
            }
        }
    }
}
