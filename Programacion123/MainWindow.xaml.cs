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

namespace Programacion123
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WeekSchedule weekSchedule;

        public MainWindow()
        {
            InitializeComponent();

            weekSchedule = new WeekSchedule();
        }


        private void TestWeekScheduleEditor_Click(object sender, RoutedEventArgs e)
        {
            WeekScheduleEditor editor = new(weekSchedule);
            editor.ShowDialog();
        }
    }
}