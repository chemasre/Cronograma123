using System;
using System.Collections.Generic;
using System.Data;
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

namespace Programacion123
{
    /// <summary>
    /// Lógica de interacción para WeekScheduleEditor.xaml
    /// </summary>
    public partial class WeekScheduleEditor : Window
    {
        WeekSchedule weekSchedule;

        DataTable dataTable;


        public WeekScheduleEditor(WeekSchedule _weekSchedule)
        {
            InitializeComponent();

            dataTable = new DataTable();
            DataColumn column = dataTable.Columns.Add("Día", typeof(string));
            column.ReadOnly = true;
            column = dataTable.Columns.Add("Horas", typeof(int));

            WeekDataGrid.ItemsSource = dataTable.DefaultView;
            WeekDataGrid.CanUserAddRows = false;
            WeekDataGrid.CanUserDeleteRows = false;
            WeekDataGrid.CanUserReorderColumns = false;
            WeekDataGrid.CanUserSortColumns = false;
            WeekDataGrid.CanUserResizeColumns = false;
            WeekDataGrid.CanUserResizeRows = false;

            DataRow row = dataTable.NewRow();
            row["Día"] = "Lunes";
            row["Horas"] = 0;
            dataTable.Rows.Add(row);

            row = dataTable.NewRow();
            row["Día"] = "Martes";
            row["Horas"] = 0;
            dataTable.Rows.Add(row);

            row = dataTable.NewRow();
            row["Día"] = "Miércoles";
            row["Horas"] = 0;
            dataTable.Rows.Add(row);

            row = dataTable.NewRow();
            row["Día"] = "Jueves";
            row["Horas"] = 0;
            dataTable.Rows.Add(row);

            row = dataTable.NewRow();
            row["Día"] = "Viernes";
            row["Horas"] = 0;
            dataTable.Rows.Add(row);

            weekSchedule = _weekSchedule;

            if(_weekSchedule.HoursPerWeekDay.ContainsKey(DayOfWeek.Monday)) { dataTable.Rows[0]["Horas"] = _weekSchedule.HoursPerWeekDay[DayOfWeek.Monday]; }
            if(_weekSchedule.HoursPerWeekDay.ContainsKey(DayOfWeek.Tuesday)) { dataTable.Rows[1]["Horas"] = _weekSchedule.HoursPerWeekDay[DayOfWeek.Tuesday]; }
            if(_weekSchedule.HoursPerWeekDay.ContainsKey(DayOfWeek.Wednesday)) { dataTable.Rows[2]["Horas"] = _weekSchedule.HoursPerWeekDay[DayOfWeek.Wednesday]; }
            if(_weekSchedule.HoursPerWeekDay.ContainsKey(DayOfWeek.Thursday)) { dataTable.Rows[3]["Horas"] = _weekSchedule.HoursPerWeekDay[DayOfWeek.Thursday]; }
            if(_weekSchedule.HoursPerWeekDay.ContainsKey(DayOfWeek.Friday)) { dataTable.Rows[4]["Horas"] = _weekSchedule.HoursPerWeekDay[DayOfWeek.Friday]; }
        }

        private void WeekDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if(WeekDataGrid.SelectedCells.Count == 1)
            {
                if(WeekDataGrid.SelectedCells[0].Column.IsReadOnly)
                {
                    WeekDataGrid.SelectedCells.Clear();
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            weekSchedule.HoursPerWeekDay.Clear();
            weekSchedule.HoursPerWeekDay.Add(DayOfWeek.Monday, (int)dataTable.Rows[0]["Horas"]);
            weekSchedule.HoursPerWeekDay.Add(DayOfWeek.Tuesday, (int)dataTable.Rows[1]["Horas"]);
            weekSchedule.HoursPerWeekDay.Add(DayOfWeek.Wednesday, (int)dataTable.Rows[2]["Horas"]);
            weekSchedule.HoursPerWeekDay.Add(DayOfWeek.Thursday, (int)dataTable.Rows[3]["Horas"]);
            weekSchedule.HoursPerWeekDay.Add(DayOfWeek.Friday, (int)dataTable.Rows[4]["Horas"]);

            Entity.ValidationResult validation = weekSchedule.Validate();

            if(validation == Entity.ValidationResult.oneHourMinimum)
            {
                MessageBox.Show("Tienes que introducir como mínimo una hora en alguno de los días", "No se puede guardar", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                e.Cancel = true;
            }
            else
            {
                weekSchedule.Save();
            }
            
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
             if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
