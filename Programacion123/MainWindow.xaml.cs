using System.Printing;
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
        List<string> comboWeekSchedulesStorageIds;

        public MainWindow()
        {
            InitializeComponent();

            comboWeekSchedulesStorageIds = new List<string>();

            UpdateWeekSchedulesCombo();
        }


        private void TestWeekScheduleEditor_Click(object sender, RoutedEventArgs e)
        {
            WeekScheduleEditor editor = new(new WeekSchedule());
            editor.ShowDialog();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonWeekScheduleNew_Click(object sender, RoutedEventArgs e)
        {
            WeekSchedule weekSchedule = new();
            WeekScheduleEditor editor = new(weekSchedule);
            editor.ShowDialog();
        }

        private void ButtonWeekScheduleDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ComboWeekSchedules.SelectedIndex >= 0)
            {
                Storage.DeleteData(comboWeekSchedulesStorageIds[ComboWeekSchedules.SelectedIndex], new WeekSchedule().StorageClassId);
                UpdateWeekSchedulesCombo();
            }
        }

        private void UpdateWeekSchedulesCombo()
        {
            comboWeekSchedulesStorageIds.Clear();
            ComboWeekSchedules.Items.Clear();
            var weekSchedules = Storage.LoadEntities<WeekSchedule>();
            weekSchedules.ForEach((e) => { ComboWeekSchedules.Items.Add(e.Title); comboWeekSchedulesStorageIds.Add(e.StorageId); });

            if(ComboWeekSchedules.Items.Count > 0) { ComboWeekSchedules.SelectedIndex = 0;  }

        }

        private void ButtonWeekScheduleEdit_Click(object sender, RoutedEventArgs e)
        {
            if(ComboWeekSchedules.SelectedIndex >= 0)
            {
                var weekSchedule = Storage.LoadEntity<WeekSchedule>(comboWeekSchedulesStorageIds[ComboWeekSchedules.SelectedIndex]);
                WeekScheduleEditor editor = new(weekSchedule);
                editor.ShowDialog();
            }
        }

        private void Window_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}