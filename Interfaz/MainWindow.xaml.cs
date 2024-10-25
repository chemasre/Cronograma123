using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CronogramaMe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Cronogramador.Cronograma cronograma;
        Cronogramador.Calendario calendario;
        Cronogramador.Asignatura asignatura;

        public MainWindow()
        {
            InitializeComponent();

            calendario = new Cronogramador.Calendario();

            DateTime now = DateTime.Now;
            int d = now.Day;
            int m = now.Month;
            int a = now.Year;
            DateTime dia = new DateTime(a, m, d);

            calendario.PonDiaInicio(dia);
            calendario.PonDiaFin(dia);
            asignatura = new Cronogramador.Asignatura();
        }

        private void Calendario_Click(object sender, RoutedEventArgs e)
        {
            Calendario c = new Calendario(calendario);
            c.ShowDialog();
        }

        private void Horario_Click(object sender, RoutedEventArgs e)
        {
            Horario h = new Horario(asignatura);
            h.ShowDialog();
        }

        private void Unidades_Click(object sender, RoutedEventArgs e)
        {
            Unidades u = new Unidades(asignatura);
            u.ShowDialog();
        }
    }
}