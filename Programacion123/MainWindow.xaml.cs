using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Programacion123
{
    public interface IEntityEditor<T>
    {
        void SetEntityTitleEditable(bool editable);
        void SetEditorTitle(string title);
        void SetEntity(T entity, string? parentStorageId = null);
        T GetEntity();
    }

    public interface IEntityPicker<T>
    {
        void SetPickerTitle(string title);
        void SetEntity(T? entity, string? parentStorageId = null);
        T? GetEntity();
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
        EntityBoxController<SubjectTemplate, SubjectTemplateEditor> subjectTemplatesController;
        EntityBoxController<Subject, SubjectEditor> subjectsController;

        public MainWindow()
        {
            InitializeComponent();

            Storage.Init();

            var configWeeks = EntityBoxConfiguration<WeekSchedule>.CreateForCombo(ComboWeekSchedules)
                                                   .WithStorageIds(Storage.GetStorageIds<WeekSchedule>(Storage.LoadEntities<WeekSchedule>()))
                                                   .WithNew(ButtonWeekScheduleNew)
                                                   .WithEdit(ButtonWeekScheduleEdit)
                                                   .WithDelete(ButtonWeekScheduleDelete)
                                                   .WithBlocker(Blocker);

            var configCalendars = EntityBoxConfiguration<Calendar>.CreateForCombo(ComboBoxCalendars)
                                                   .WithStorageIds(Storage.GetStorageIds<Calendar>(Storage.LoadEntities<Calendar>()))
                                                   .WithNew(ButtonCalendarNew)
                                                   .WithEdit(ButtonCalendarEdit)
                                                   .WithDelete(ButtonCalendarDelete)
                                                   .WithBlocker(Blocker);

            var configTemplates = EntityBoxConfiguration<SubjectTemplate>.CreateForCombo(ComboSubjectTemplates)
                                                   .WithStorageIds(Storage.GetStorageIds<SubjectTemplate>(Storage.LoadEntities<SubjectTemplate>()))
                                                   .WithNew(ButtonSubjectTemplateNew)
                                                   .WithEdit(ButtonSubjectTemplateEdit)
                                                   .WithDelete(ButtonSubjectTemplateDelete)
                                                   .WithBlocker(Blocker);

            weekSchedulesController = new (configWeeks);
            calendarsController = new (configCalendars);
            subjectTemplatesController = new (configTemplates);

            var configSubjects = EntityBoxConfiguration<Subject>.CreateForCombo(ComboSubjects)
                                                   .WithStorageIds(Storage.GetStorageIds<Subject>(Storage.LoadEntities<Subject>()))
                                                   .WithEntityInitializer(
                                                            (Subject s) =>
                                                            {
                                                                s.Template = subjectTemplatesController.GetSelectedEntity();
                                                                s.Calendar = calendarsController.GetSelectedEntity();
                                                                s.WeekSchedule = weekSchedulesController.GetSelectedEntity();
                                                            })
                                                   .WithNew(ButtonSubjectNew)
                                                   .WithEdit(ButtonSubjectEdit)
                                                   .WithDelete(ButtonSubjectDelete)
                                                   .WithBlocker(Blocker);



            subjectsController = new (configSubjects);
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