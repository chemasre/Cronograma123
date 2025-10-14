using System.Windows;
using System.Windows.Input;

namespace Programacion123
{
    /// <summary>
    /// Lógica de interacción para LongTaskDialog.xaml
    /// </summary>
    public partial class LongTaskDialog : Window
    {
        public string Name { get { return LabelTitle.Text; } set { LabelTitle.Text = value; } }
        public float Value { get { return Bar.IsIndeterminate ? 0 : (float)Bar.Value; } set { Bar.IsIndeterminate = (value < 0); Bar.Value = value >= 0 ? value : 0; } }

        public LongTaskDialog()
        {
            InitializeComponent();
        }

        public void Init(string taskName)
        {
            LabelTitle.Text = taskName;
            Bar.Value = 0;
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
