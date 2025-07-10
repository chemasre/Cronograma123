using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Programacion123
{
    public interface EntityEditor<T>
    {
        void SetTitleEditable(bool editable);
        void SetEntity(T entity, string? parentStorageId = null);
        T GetEntity();
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //WeekScheduleEditor weekScheduleEditor;
        //List<string> weekSchedulesStorageIds;

        EntityBoxController<WeekSchedule, WeekScheduleEditor> weekSchedulesController;
        EntityBoxController<Calendar, CalendarEditor> calendarsController;
        EntityBoxController<SubjectTemplate, SubjectTemplateEditor> subjectTemplateController;

        public MainWindow()
        {
            InitializeComponent();

            var configWeeks = EntityBoxConfiguration.CreateForCombo(ComboWeekSchedules)
                                                   .WithStorageIds(Storage.GetStorageIds<WeekSchedule>(Storage.LoadEntities<WeekSchedule>()))
                                                   .WithNew(ButtonWeekScheduleNew)
                                                   .WithEdit(ButtonWeekScheduleEdit)
                                                   .WithDelete(ButtonWeekScheduleDelete);

            var configCalendars = EntityBoxConfiguration.CreateForCombo(ComboBoxCalendars)
                                                   .WithStorageIds(Storage.GetStorageIds<Calendar>(Storage.LoadEntities<Calendar>()))
                                                   .WithNew(ButtonCalendarNew)
                                                   .WithEdit(ButtonCalendarEdit)
                                                   .WithDelete(ButtonCalendarDelete);

            var configTemplates = EntityBoxConfiguration.CreateForCombo(ComboSubjectTemplates)
                                                   .WithStorageIds(Storage.GetStorageIds<SubjectTemplate>(Storage.LoadEntities<SubjectTemplate>()))
                                                   .WithNew(ButtonSubjectTemplateNew)
                                                   .WithEdit(ButtonSubjectTemplateEdit)
                                                   .WithDelete(ButtonSubjectTemplateDelete);

            weekSchedulesController = new (configWeeks);
            calendarsController = new (configCalendars);
            subjectTemplateController = new (configTemplates);
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
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