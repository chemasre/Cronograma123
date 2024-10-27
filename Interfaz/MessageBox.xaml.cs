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
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBox : Window
    {
        public enum Type
        {
            alert,
            info,
            error,
            question
        }

        public bool result;

        public MessageBox(Type type, string message)
        {
            InitializeComponent();

            Texto.Text = message;

            Alerta.Visibility = type == Type.alert ? Visibility.Visible : Visibility.Hidden;
            Info.Visibility = type == Type.info ? Visibility.Visible : Visibility.Hidden;
            Error.Visibility = type == Type.error ? Visibility.Visible : Visibility.Hidden;
            Pregunta.Visibility = type == Type.question ? Visibility.Visible : Visibility.Hidden;
            Si.Visibility = type == Type.question ? Visibility.Visible : Visibility.Hidden;
            No.Visibility = type == Type.question ? Visibility.Visible : Visibility.Hidden;
            Aceptar.Visibility = type != Type.question ? Visibility.Visible : Visibility.Hidden;

            if (type == Type.alert) { Title = "Alerta"; }
            else if (type == Type.info) { Title = "Información"; }
            else if (type == Type.error) { Title = "Error"; }
            else // type == Type.question
            { Title = "Pregunta"; }
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Si_Click(object sender, RoutedEventArgs e)
        {
            result = true;
            Close();

        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            result = false;
            Close();
        }
    }
}
