using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Programacion123
{
    /// <summary>
    /// Lógica de interacción para AboutDialog.xaml
    /// </summary>
    public partial class AboutDialog : Window
    {
        public AboutDialog()
        {
            InitializeComponent();

            TextTitle.Text = "Acerca de " + Constants.appName;
            LabelVersion.Content = "v" + Constants.version;
        }

        private void ButtonAccept_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }

        }

        async private void IconOtherApp1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            await HandleIconClickAsync("a la página principal de TurtleSandbox", Constants.otherApp1Url);
        }

        async private void IconOtherApp2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            await HandleIconClickAsync("a la página principal de MiniBoy Color", Constants.otherApp2Url);
        }

        async private void IconApp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            await HandleIconClickAsync("a la página principal de la aplicación", Constants.projectsUrl);
        }

        async private void IconLicense_MouseDown(object sender, MouseButtonEventArgs e)
        {
            await HandleIconClickAsync("al texto completo de la licencia", Constants.licenseUrl);
        }

        async private Task HandleIconClickAsync(string destination, string url)
        {
            Blocker.Visibility = Visibility.Visible;

            await Dispatcher.BeginInvoke(
                () =>
                {
                    ConfirmDialog question = new();
                    question.Owner = this;

                    question.Init(ConfirmIconType.info,
                        "Abrir navegador",
                        "Esto abrirá tu navegador por defecto y te dirigirá " + destination,
                        ConfirmChooseType.acceptAndCancel,
                        (b) => { if (b) { Utils.OpenUrl(url); } });

                    question.ShowDialog();

                    Blocker.Visibility = Visibility.Hidden;

                });
        }
    }
}
