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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Calendario_Click(object sender, RoutedEventArgs e)
        {
            Calendario c = new Calendario();
            c.ShowDialog();
        }

        private void Horario_Click(object sender, RoutedEventArgs e)
        {
            Horario h = new Horario();
            h.ShowDialog();
        }

        private void Unidades_Click(object sender, RoutedEventArgs e)
        {
            Unidades u = new Unidades();
            u.ShowDialog();
        }
    }
}