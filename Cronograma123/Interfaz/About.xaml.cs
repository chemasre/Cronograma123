using System.Windows;

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
