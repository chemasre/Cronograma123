using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Security.Policy;
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
        void SetFormat(EntityFormatContent formatContent, EntityFormatIndex formatIndex = EntityFormatIndex.none);
        void SetFormatter(Func<T, int, string>? formatter);
        void SetSinglePickerEntities(T? selectedEntity, List<T> entities);
        void SetMultiPickerEntities(List<T> selectedEntities, List<T> pickableEntities);
        T? GetPickedEntity();
        List<T> GetPickedEntities();
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string homeUrl = "http://sinestesiagamedesign.es/teaching";
        const string helpUrl = "http://youtube.com";

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

            InitUI();
        }

        void InitUI()
        {
            var configWeeks = StrongReferencesBoxConfiguration<WeekSchedule>.CreateForCombo(ComboWeekSchedules)
                                                   .WithStorageIds(Storage.GetStorageIds<WeekSchedule>(Storage.LoadAllEntities<WeekSchedule>()))
                                                   .WithNew(ButtonWeekScheduleNew)
                                                   .WithEdit(ButtonWeekScheduleEdit)
                                                   .WithDelete(ButtonWeekScheduleDelete)
                                                   .WithDeleteConfirmQuestion("Esto eliminará permanentemente el horario seleccionado. ¿Estás seguro/a?")
                                                   .WithBlocker(Blocker);

            var configCalendars = StrongReferencesBoxConfiguration<Calendar>.CreateForCombo(ComboBoxCalendars)
                                                   .WithStorageIds(Storage.GetStorageIds<Calendar>(Storage.LoadAllEntities<Calendar>()))
                                                   .WithNew(ButtonCalendarNew)
                                                   .WithEdit(ButtonCalendarEdit)
                                                   .WithDelete(ButtonCalendarDelete)
                                                   .WithDeleteConfirmQuestion("Esto eliminará permanentemente el calendario seleccionado. ¿Estás seguro/a?")
                                                   .WithBlocker(Blocker);

            var configTemplates = StrongReferencesBoxConfiguration<SubjectTemplate>.CreateForCombo(ComboSubjectTemplates)
                                                   .WithStorageIds(Storage.GetStorageIds<SubjectTemplate>(Storage.LoadAllEntities<SubjectTemplate>()))
                                                   .WithNew(ButtonSubjectTemplateNew)
                                                   .WithEdit(ButtonSubjectTemplateEdit)
                                                   .WithDelete(ButtonSubjectTemplateDelete)
                                                   .WithDeleteConfirmQuestion("Esto eliminará permanentemente la plantilla seleccionada junto con los elementos curriculares definidos en ella. ¿Estás seguro/a?")
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
                                                   .WithDeleteConfirmQuestion("Esto eliminará permanentemente la asignatura seleccionada junto con los bloques y otros elementos definidos en ella. ¿Estás seguro/a?")
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

        private void ButtonHome_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl(homeUrl);
        }

        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl(helpUrl);
        }

        void OpenUrl(string url)
        {
            ProcessStartInfo info = new ();
            info.FileName = url;
            info.UseShellExecute = true;
            Process.Start (info);
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            ConfirmDialog confirm = new();
            confirm.Init("Confirmación", "Esto eliminará TODOS los datos guardados y devolverá la aplicación a su estado inicial ¿estás seguro/a?",
                (e) =>
                {
                    if(e)
                    {
                        Storage.Reset();
                        InitUI();
                    }

                });

            confirm.ShowDialog();

        }
    }
}