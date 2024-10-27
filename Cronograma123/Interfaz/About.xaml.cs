using System;
using System.Collections.Generic;
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

namespace CronogramaMe.Interfaz
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();

            Owner = Application.Current.MainWindow;

            Texto.Text = MainWindow.projectName + " v" + MainWindow.projectVersion + "\n";
            Texto.Text += "por " + MainWindow.companyName + " (" + MainWindow.projectYear + ")\n";
            Texto.Text += "------------------------------------\n";
            Texto.Text += "Distribuido bajo licencia " + MainWindow.projectLicense + "\n";
            Texto.Text += MainWindow.projectUrl;


        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
