using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using static Programacion123.ExportImportDialog;

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
        void SetFormat(EntityFormatContent formatContent, EntityFormatIndex formatIndex = EntityFormatIndex.None);
        void SetFormatter(Func<T, int, string>? formatter);
        void SetSinglePickerEntities(T? selectedEntity, List<T> entities);
        void SetMultiPickerEntities(List<T> selectedEntities, List<T> pickableEntities);
        T? GetPickedEntity();
        List<T> GetPickedEntities();
        bool GetWasCancelled();
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
        StrongReferencesBoxController<GradeTemplate, GradeTemplateEditor> gradeTemplatesController;
        StrongReferencesBoxController<Subject, SubjectEditor> subjectsController;

        public MainWindow()
        {
            InitializeComponent();

            Settings.Init();
            Storage.Init();

            InitUI();
        }

        void InitUI()
        {
            var configGradeTemplates = StrongReferencesBoxConfiguration<GradeTemplate>.CreateForCombo(ComboGradeTemplates)
                                                   .WithStorageIds(Storage.GetStorageIds<GradeTemplate>(Storage.LoadAllEntities<GradeTemplate>()))
                                                   .WithNew(ButtonGradeTemplateNew)
                                                   .WithEdit(ButtonGradeTemplateEdit)
                                                   .WithDelete(ButtonGradeTemplateDelete)
                                                   .WithDeleteConfirmQuestion("Esto eliminará permanentemente la plantilla de ciclo seleccionada junto con los elementos curriculares definidos en ella. ¿Estás seguro/a?")
                                                   .WithBlocker(Blocker);

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

            var configSubjectTemplates = StrongReferencesBoxConfiguration<SubjectTemplate>.CreateForCombo(ComboSubjectTemplates)
                                                   .WithStorageIds(Storage.GetStorageIds<SubjectTemplate>(Storage.LoadAllEntities<SubjectTemplate>()))
                                                   .WithEntityInitializer(
                                                            (SubjectTemplate t) =>
                                                            {
                                                                t.GradeTemplate = gradeTemplatesController.GetSelectedEntity();
                                                            })
                                                   .WithNew(ButtonSubjectTemplateNew)
                                                   .WithEdit(ButtonSubjectTemplateEdit)
                                                   .WithDelete(ButtonSubjectTemplateDelete)
                                                   .WithDeleteConfirmQuestion("Esto eliminará permanentemente la plantilla de módulo seleccionada junto con los elementos curriculares definidos en ella. ¿Estás seguro/a?")
                                                   .WithBlocker(Blocker);

            weekSchedulesController = new (configWeeks);
            calendarsController = new (configCalendars);
            subjectTemplatesController = new(configSubjectTemplates);
            gradeTemplatesController = new(configGradeTemplates);

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
                                                   .WithDeleteConfirmQuestion("Esto eliminará permanentemente el módulo seleccionada junto con los bloques y otros elementos definidos en ella. ¿Estás seguro/a?")
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
            confirm.Init(ConfirmIconType.warning, "Confirmación", "Esto eliminará TODOS los datos y ajustes guardados, devolviendo la aplicación a su estado inicial ¿estás seguro/a?",
                         ConfirmChooseType.acceptAndCancel,
                (e) =>
                {
                    if(e)
                    {
                        Storage.Reset();
                        Settings.Reset();
                        RestartUI();
                    }

                });

            confirm.ShowDialog();

        }

        private void ButtonImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Title = "Elige archivo para cargar";
            openFileDialog.Filter = "Ficheros zip (*.zip)|*.zip|Todos los ficheros (*.*)|*.*";

            if(openFileDialog.ShowDialog().GetValueOrDefault())
            {
                Storage.Archive_Open(openFileDialog.FileName);
                Settings.Archive_Open(openFileDialog.FileName);

                ExportImportDialogConfiguration config = new()
                {
                    isExport = false,
                    gradeTemplateStorageIds = Storage.GetStorageIds<GradeTemplate>(Storage.LoadAllEntities<GradeTemplate>()),
                    subjectTemplatesStorageIds = Storage.GetStorageIds<SubjectTemplate>(Storage.LoadAllEntities<SubjectTemplate>()),
                    calendarsStorageIds = Storage.GetStorageIds<Calendar>(Storage.LoadAllEntities<Calendar>()),
                    weekSchedulesStorageIds = Storage.GetStorageIds<WeekSchedule>(Storage.LoadAllEntities<WeekSchedule>()),
                    subjectsStorageIds = Storage.GetStorageIds<Subject>(Storage.LoadAllEntities<Subject>()),
                    includeSettings = Settings.ExistSettings<HTMLGeneratorSettings>(HTMLGenerator.SettingsId),
                    closeAction =
                        (accepted, exportDialog) =>
                        {
                            if(accepted)
                            {
                                List<string> storageIds = new();
                                storageIds.AddRange(exportDialog.GradeTemplatesStorageIds);
                                storageIds.AddRange(exportDialog.SubjectTemplatesStorageIds);
                                storageIds.AddRange(exportDialog.CalendarsStorageIds);
                                storageIds.AddRange(exportDialog.WeekSchedulesStorageIds);
                                storageIds.AddRange(exportDialog.SubjectsStorageIds);

                                Storage.Archive_CopyStorageIdsToBase(storageIds);

                                if(exportDialog.CheckboxSettings.IsChecked.GetValueOrDefault())
                                {
                                    Settings.Archive_CopyToBase();
                                }
                            }

                            Storage.Archive_Close();
                            Settings.Archive_Close();

                            if(accepted)
                            {
                                RestartUI();
                            }

                        }

                };

                ExportImportDialog dialog = new();

                dialog.Init(config);

                dialog.ShowDialog();

            }
        }

        private void ButtonExport_Click(object sender, RoutedEventArgs e)
        {
            ExportImportDialog dialog = new();

            Action<bool, ExportImportDialog> closeAction = 
            (accepted, exportDialog) =>
            {
                SaveFileDialog saveFileDialog = new();
                saveFileDialog.Title = "Elige archivo para guardar";
                saveFileDialog.Filter = "Ficheros zip (*.zip)|*.zip|Todos los ficheros (*.*)|*.*";

                if(saveFileDialog.ShowDialog().GetValueOrDefault())
                {
                    List<string> storageIds = new();
                    storageIds.AddRange(dialog.GradeTemplatesStorageIds);
                    storageIds.AddRange(dialog.SubjectTemplatesStorageIds);
                    storageIds.AddRange(dialog.CalendarsStorageIds);
                    storageIds.AddRange(dialog.WeekSchedulesStorageIds);
                    storageIds.AddRange(dialog.SubjectsStorageIds);
                    Storage.Archive_Create(storageIds, saveFileDialog.FileName);

                    if(exportDialog.CheckboxSettings.IsChecked.GetValueOrDefault())
                    {
                        Settings.Archive_Add(saveFileDialog.FileName);
                    }
                }				
            };

            ExportImportDialogConfiguration config = new()
            {
                isExport = true,
                gradeTemplateStorageIds = Storage.GetStorageIds<GradeTemplate>(Storage.LoadAllEntities<GradeTemplate>()),
                subjectTemplatesStorageIds = Storage.GetStorageIds<SubjectTemplate>(Storage.LoadAllEntities<SubjectTemplate>()),
                calendarsStorageIds = Storage.GetStorageIds<Calendar>(Storage.LoadAllEntities<Calendar>()),
                weekSchedulesStorageIds = Storage.GetStorageIds<WeekSchedule>(Storage.LoadAllEntities<WeekSchedule>()),
                subjectsStorageIds = Storage.GetStorageIds<Subject>(Storage.LoadAllEntities<Subject>()),
                includeSettings = true,
                closeAction = closeAction
            };

            dialog.Init(config);

            dialog.ShowDialog();
        }

        void RestartUI()
        {
            weekSchedulesController.Finish();
            calendarsController.Finish();
            subjectTemplatesController.Finish();
            gradeTemplatesController.Finish();
            subjectsController.Finish();

            InitUI();
        }

        private void ButtonGenerateDocument_Click(object sender, RoutedEventArgs e)
        {
            Subject? subject = subjectsController.GetSelectedEntity();
            if(subject != null)
            {
                HTMLGeneratorDialog generatorDialog = new();

                generatorDialog.Init(subject);
                generatorDialog.ShowDialog();

            }
            else
            {
                ConfirmDialog dialog = new();
                dialog.Init(ConfirmIconType.warning, "Aviso", "No se puede generar el documento porque no se ha seleccionado una programación", ConfirmChooseType.acceptOnly, (b) => Blocker.Visibility = Visibility.Hidden);
                Blocker.Visibility = Visibility.Visible;
                dialog.ShowDialog();
            }
        }

    }
}