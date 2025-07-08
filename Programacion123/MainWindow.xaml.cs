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
    public interface EntityEditor<T>
    {
        void SetEntity(T entity);
        T GetEntity();
    }


    public class EntityComboController<TEntity, TEditor> where TEntity: Entity, new()
                                                        where TEditor: Window, EntityEditor<TEntity>, new()
    {
        List<string> storageIds;
        ComboBox comboBox;
        Button buttonNew;
        Button buttonEdit;
        Button buttonDelete;
        TEditor editor;

        public EntityComboController(ComboBox _combo, Button _new, Button _edit, Button _delete)
        {
            comboBox = _combo;
            buttonNew = _new;
            buttonEdit = _edit;
            buttonDelete = _delete;
            storageIds = new List<string>();

            buttonNew.Click += ButtonNew_Click;
            buttonEdit.Click += ButtonEdit_Click;
            buttonDelete.Click += ButtonDelete_Click;

            UpdateComboBox();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox.SelectedIndex >= 0)
            {
                int index = comboBox.SelectedIndex;
                string? previousStorageId = index > 0 ? storageIds[index - 1] : null;

                Storage.DeleteData(storageIds[index], new TEntity().StorageClassId);
                UpdateComboBox();

                if(previousStorageId != null)
                {
                    SelectStorageId(previousStorageId);
                }
            }            
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            if(comboBox.SelectedIndex >= 0)
            {
                var entity = Storage.LoadEntity<TEntity>(storageIds[comboBox.SelectedIndex]);
                editor = new TEditor();
                editor.SetEntity(entity);
                editor.Closed += OnEditorClosed;
                editor.ShowDialog();

            }
        }

        private void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            editor = new TEditor();
            editor.SetEntity(new TEntity());
            editor.Closed += OnEditorClosed;
            editor.ShowDialog();
        }

        void UpdateComboBox()
        {
            storageIds.Clear();
            comboBox.Items.Clear();
            var entities = Storage.LoadEntities<TEntity>();
            entities.ForEach((e) => { comboBox.Items.Add(e.Title); storageIds.Add(e.StorageId); });

            if(comboBox.Items.Count > 0) { comboBox.SelectedIndex = 0;  }

        }

        void OnEditorClosed(object? sender, EventArgs e)
        {
            UpdateComboBox();
            SelectStorageId(editor.GetEntity().StorageId);
            editor.Closed -= OnEditorClosed;
        }

        void SelectStorageId(string storageId)
        {
            comboBox.SelectedIndex = storageIds.FindIndex(e => e == storageId);
        }

    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //WeekScheduleEditor weekScheduleEditor;
        //List<string> weekSchedulesStorageIds;

        EntityComboController<WeekSchedule, WeekScheduleEditor> weekSchedulesController;
        EntityComboController<Calendar, CalendarEditor> calendarsController;

        public MainWindow()
        {
            InitializeComponent();

            weekSchedulesController = new (ComboWeekSchedules, ButtonWeekScheduleNew, ButtonWeekScheduleEdit, ButtonWeekScheduleDelete);
            calendarsController = new (ComboBoxCalendars, ButtonCalendarNew, ButtonCalendarEdit, ButtonCalendarDelete);

            //weekSchedulesStorageIds = new List<string>();

            //UpdateWeekSchedulesCombo();
        }

        //private void WeekScheduleEditor_Closed(object? sender, EventArgs e)
        //{
        //    UpdateWeekSchedulesCombo();
        //    SelectWeekScheduleByStorageId(weekScheduleEditor.WeekSchedule.StorageId);
        //    weekScheduleEditor.Closed -= WeekScheduleEditor_Closed;
        //}

        //private void SelectWeekScheduleByStorageId(string storageId)
        //{
        //    ComboWeekSchedules.SelectedIndex = weekSchedulesStorageIds.FindIndex(e => e == storageId);
        //}

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

        //private void ButtonWeekScheduleNew_Click(object sender, RoutedEventArgs e)
        //{
        //    weekScheduleEditor = new();
        //    weekScheduleEditor.SetEntity(new WeekSchedule());
        //    weekScheduleEditor.Closed += WeekScheduleEditor_Closed;
        //    weekScheduleEditor.ShowDialog();
        //}

        //private void ButtonWeekScheduleDelete_Click(object sender, RoutedEventArgs e)
        //{
        //    if (ComboWeekSchedules.SelectedIndex >= 0)
        //    {
        //        int index = ComboWeekSchedules.SelectedIndex;
        //        string? previousStorageId = index > 0 ? weekSchedulesStorageIds[index - 1] : null;

        //        Storage.DeleteData(weekSchedulesStorageIds[index], new WeekSchedule().StorageClassId);
        //        UpdateWeekSchedulesCombo();

        //        if(previousStorageId != null)
        //        {
        //            SelectWeekScheduleByStorageId(previousStorageId);
        //        }
        //    }
        //}

        //private void UpdateWeekSchedulesCombo()
        //{
        //    weekSchedulesStorageIds.Clear();
        //    ComboWeekSchedules.Items.Clear();
        //    var weekSchedules = Storage.LoadEntities<WeekSchedule>();
        //    weekSchedules.ForEach((e) => { ComboWeekSchedules.Items.Add(e.Title); weekSchedulesStorageIds.Add(e.StorageId); });

        //    if(ComboWeekSchedules.Items.Count > 0) { ComboWeekSchedules.SelectedIndex = 0;  }

        //}

        //private void ButtonWeekScheduleEdit_Click(object sender, RoutedEventArgs e)
        //{
        //    if(ComboWeekSchedules.SelectedIndex >= 0)
        //    {
        //        var weekSchedule = Storage.LoadEntity<WeekSchedule>(weekSchedulesStorageIds[ComboWeekSchedules.SelectedIndex]);
        //        weekScheduleEditor = new();
        //        weekScheduleEditor.SetEntity(weekSchedule);
        //        weekScheduleEditor.Closed += WeekScheduleEditor_Closed;
        //        weekScheduleEditor.ShowDialog();

        //    }
        //}

        private void Window_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}