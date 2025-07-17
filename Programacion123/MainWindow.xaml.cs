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
        void InitEditor(T entity, string? _parentStorageId);
        T GetEntity();
    }

    public interface IEntityPicker<T>
    {
        void SetPickerTitle(string title);
        void InitSinglePicker(T? selectedEntity, List<T> entities);
        void InitMultiPicker(List<T> selectedEntities, List<T> entities);
        T? GetPickedEntity();
        List<T> GetPickedEntities();
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //WeekScheduleEditor weekScheduleEditor;
        //List<string> weekSchedulesStorageIds;

        StrongReferencesBoxController<WeekSchedule, WeekScheduleEditor> weekSchedulesController;
        StrongReferencesBoxController<Calendar, CalendarEditor> calendarsController;
        StrongReferencesBoxController<SubjectTemplate, SubjectTemplateEditor> subjectTemplatesController;
        StrongReferencesBoxController<Subject, SubjectEditor> subjectsController;

        public MainWindow()
        {
            InitializeComponent();

            Storage.Init();

            var configWeeks = StrongReferencesBoxConfiguration<WeekSchedule>.CreateForCombo(ComboWeekSchedules)
                                                   .WithStorageIds(Storage.GetStorageIds<WeekSchedule>(Storage.LoadAllEntities<WeekSchedule>()))
                                                   .WithNew(ButtonWeekScheduleNew)
                                                   .WithEdit(ButtonWeekScheduleEdit)
                                                   .WithDelete(ButtonWeekScheduleDelete)
                                                   .WithBlocker(Blocker);

            var configCalendars = StrongReferencesBoxConfiguration<Calendar>.CreateForCombo(ComboBoxCalendars)
                                                   .WithStorageIds(Storage.GetStorageIds<Calendar>(Storage.LoadAllEntities<Calendar>()))
                                                   .WithNew(ButtonCalendarNew)
                                                   .WithEdit(ButtonCalendarEdit)
                                                   .WithDelete(ButtonCalendarDelete)
                                                   .WithBlocker(Blocker);

            var configTemplates = StrongReferencesBoxConfiguration<SubjectTemplate>.CreateForCombo(ComboSubjectTemplates)
                                                   .WithStorageIds(Storage.GetStorageIds<SubjectTemplate>(Storage.LoadAllEntities<SubjectTemplate>()))
                                                   .WithNew(ButtonSubjectTemplateNew)
                                                   .WithEdit(ButtonSubjectTemplateEdit)
                                                   .WithDelete(ButtonSubjectTemplateDelete)
                                                   .WithBlocker(Blocker);

            weekSchedulesController = new (configWeeks);
            calendarsController = new (configCalendars);
            subjectTemplatesController = new (configTemplates);

            var configSubjects = StrongReferencesBoxConfiguration<Subject>.CreateForCombo(ComboSubjects)
                                                   .WithStorageIds(Storage.GetStorageIds<Subject>(Storage.LoadAllEntities<Subject>()))
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