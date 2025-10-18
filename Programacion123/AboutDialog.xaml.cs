using System.Windows;
using System.Windows.Input;

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

        private void IconOtherApp1_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void IconOtherApp2_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void IconApp_MouseDown(object sender, MouseButtonEventArgs e)
        {

        private void ButtonVisitWeb_Click(object sender, RoutedEventArgs e)
        {
            ConfirmDialog question = new();
            question.Owner = this;

            question.Init(ConfirmIconType.info,
                "Abrir navegador",
                "Esto abrirá tu navegador por defecto y te dirigirá a la página web de la aplicación",
                (b) => { if (b) { Utils.OpenUrl(Constants.homeUrl); } });

            question.ShowDialog();
        }

        private void ButtonSendEmail_Click(object sender, RoutedEventArgs e)
        {
            ConfirmDialog question = new();
            question.Owner = this;

            question.Init(ConfirmIconType.info,
                "Enviar correo",
                "Esto abrirá tu aplicación de correo por defecto para que puedas enviarme un mail",
                ConfirmChooseType.acceptAndCancel,
                (b) => { if (b) { Utils.OpenUrl(Constants.sendMailUrl); } });

            question.ShowDialog();
        }

        //private void ButtonHome_Click(object sender, RoutedEventArgs e)
        //{
        //    Blocker.Visibility = Visibility.Visible;

        //    ConfirmDialog question = new();

        //    question.Init(ConfirmIconType.info,
        //        "Abrir navegador",
        //        "Esto abrirá tu navegador por defecto y te dirigirá a la página principal de la aplicación",
        //        ConfirmChooseType.acceptAndCancel,
        //        (b) => { if (b) { Utils.OpenUrl(Constants.homeUrl); } });

        //    question.ShowDialog();

        //    Blocker.Visibility = Visibility.Hidden;            
        //}

    }
}
