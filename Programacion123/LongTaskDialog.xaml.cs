using System.Windows;
using System.Windows.Input;

namespace Programacion123
{
    /// <summary>
    /// Lógica de interacción para LongTaskDialog.xaml
    /// </summary>
    public partial class LongTaskDialog : Window
    {
        public LongTaskDialog()
        {
            InitializeComponent();
        }

        public void Init(string taskName)
        {
            LabelTitle.Text = taskName;
        }

        void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
