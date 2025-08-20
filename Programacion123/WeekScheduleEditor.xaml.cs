using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Programacion123
{
    /// <summary>
    /// Lógica de interacción para WeekScheduleEditor.xaml
    /// </summary>
    public partial class WeekScheduleEditor : Window, IEntityEditor<WeekSchedule>
    {
        public WeekSchedule WeekSchedule { get { return entity; }  }

        DataTable dataTable;
        WeekSchedule entity;
        public string? parentStorageId;

        public WeekScheduleEditor()
        {
            InitializeComponent();
        }

        private void DataTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            UpdateEntity();
            Validate();
        }

        private void UpdateEntity()
        {
            entity.Title = TextTitle.Text.Trim();

            entity.HoursPerWeekDay.Clear();
            entity.HoursPerWeekDay.Add(DayOfWeek.Monday, (int)dataTable.Rows[0]["Horas"]);
            entity.HoursPerWeekDay.Add(DayOfWeek.Tuesday, (int)dataTable.Rows[1]["Horas"]);
            entity.HoursPerWeekDay.Add(DayOfWeek.Wednesday, (int)dataTable.Rows[2]["Horas"]);
            entity.HoursPerWeekDay.Add(DayOfWeek.Thursday, (int)dataTable.Rows[3]["Horas"]);
            entity.HoursPerWeekDay.Add(DayOfWeek.Friday, (int)dataTable.Rows[4]["Horas"]);

            entity.Save(parentStorageId);
        }

        void Validate()
        {
            ValidationResult validation = entity.Validate();

            string colorResource = (validation.code == ValidationCode.success ? "ColorValid" : "ColorInvalid");
            BorderValidation.Background = new SolidColorBrush((Color)Application.Current.Resources[colorResource]);
            TextValidation.Text = validation.ToString();

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

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            UpdateEntity();
            // entity.Save(parentStorageId);
            TextTitle.TextChanged -= TextTitle_TextChanged;
            dataTable.RowChanged -= DataTable_RowChanged;
            Close();
        }

        private void TextTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateEntity();
            Validate();
        }

        public void InitEditor(WeekSchedule _weekSchedule, string? _parentStorageId)
        {
            _weekSchedule.Save(_parentStorageId);

            parentStorageId = _parentStorageId;

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

            entity = _weekSchedule;

            TextTitle.Text = entity.Title;

            if(_weekSchedule.HoursPerWeekDay.ContainsKey(DayOfWeek.Monday)) { dataTable.Rows[0]["Horas"] = _weekSchedule.HoursPerWeekDay[DayOfWeek.Monday]; }
            if(_weekSchedule.HoursPerWeekDay.ContainsKey(DayOfWeek.Tuesday)) { dataTable.Rows[1]["Horas"] = _weekSchedule.HoursPerWeekDay[DayOfWeek.Tuesday]; }
            if(_weekSchedule.HoursPerWeekDay.ContainsKey(DayOfWeek.Wednesday)) { dataTable.Rows[2]["Horas"] = _weekSchedule.HoursPerWeekDay[DayOfWeek.Wednesday]; }
            if(_weekSchedule.HoursPerWeekDay.ContainsKey(DayOfWeek.Thursday)) { dataTable.Rows[3]["Horas"] = _weekSchedule.HoursPerWeekDay[DayOfWeek.Thursday]; }
            if(_weekSchedule.HoursPerWeekDay.ContainsKey(DayOfWeek.Friday)) { dataTable.Rows[4]["Horas"] = _weekSchedule.HoursPerWeekDay[DayOfWeek.Friday]; }

            dataTable.RowChanged += DataTable_RowChanged;
            TextTitle.TextChanged += TextTitle_TextChanged;

            Validate();
        }

        public WeekSchedule GetEntity()
        {
            return entity;
        }

        public void SetEntityTitleEditable(bool editable)
        {
            TextTitle.IsReadOnly = !editable;
            TextTitle.IsReadOnlyCaretVisible = false;
        }

        public void SetEditorTitle(string title)
        {
            TextEditorTitle.Content = title;
        }

    }
}
