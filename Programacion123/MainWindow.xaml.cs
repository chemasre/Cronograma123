using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using static Programacion123.ExportImportDialog;

namespace Programacion123
{
    public struct UpdateFlags
    {
        public const uint entityFlagGradeTemplate   = 1 << (16 + 0);
        public const uint entityFlagSubjectTemplate = 1 << (16 + 1);
        public const uint entityFlagLearningResult  = 1 << (16 + 2);
        public const uint entityFlagContent         = 1 << (16 + 3);
        public const uint entityFlagCommonText      = 1 << (16 + 4);
        public const uint entityFlagCalendar        = 1 << (16 + 5);
        public const uint entityFlagWeekSchedule    = 1 << (16 + 6);
        public const uint entityFlagSubject         = 1 << (16 + 7);
        public const uint entityFlagBlock           = 1 << (16 + 8);
        public const uint entityFlagActivity        = 1 << (16 + 9);
        public const uint entityFlagDocumentStyle   = 1 << (16 + 10);

        public const uint propertyFlagTitle       = 1 << 0;
        public const uint propertyFlagDescription = 1 << 1;

        public const uint propertyFlagGradeTemplateGradeType          = 1 << 2;
        public const uint propertyFlagGradeTemplateGradeName          = 1 << 3;
        public const uint propertyFlagGradeTemplateGradeFamilyName    = 1 << 4;
        public const uint propertyFlagGradeTemplateGeneralObjectives  = 1 << 5;
        public const uint propertyFlagGradeTemplateGeneralCompetences = 1 << 6;
        public const uint propertyFlagGradeTemplateKeyCapacities      = 1 << 7;
        public const uint propertyFlagGradeTemplateCommonTexts        = 1 << 8;

        public const uint updateFlagSubjectTemplateGradeTemplate        = 1 << 2;
        public const uint updateFlagSubjectTemplateSubjectName          = 1 << 3;
        public const uint updateFlagSubjectTemplateSubjectCode          = 1 << 4;
        public const uint updateFlagSubjectTemplateGradeClassroomHours  = 1 << 5;
        public const uint updateFlagSubjectTemplateGradeCompanyHours    = 1 << 6;
        public const uint updateFlagSubjectTemplateGeneralObjectives    = 1 << 7;
        public const uint updateFlagSubjectTemplateGeneralCompetences   = 1 << 8;
        public const uint updateFlagSubjectTemplateLearningResults      = 1 << 9;
        public const uint updateFlagSubjectTemplateContents             = 1 << 10;

        public const uint updateFlagCalendarStartDay    = 1 << 2;
        public const uint updateFlagCalendarEndDay      = 1 << 3;
        public const uint updateFlagCalendarFreeDays    = 1 << 4;

        public const uint updateFlagContentPoints = 1 << 2;

        public const uint updateFlagDocumentStyleLogo               = 1 << 2;
        public const uint updateFlagDocumentStyleCover              = 1 << 3;
        public const uint updateFlagDocumentStyleSize               = 1 << 4;
        public const uint updateFlagDocumentStyleMargins            = 1 << 5;
        public const uint updateFlagDocumentStyleCoverElementStyles = 1 << 6;
        public const uint updateFlagDocumentStyleTextElementStyles  = 1 << 7;
        public const uint updateFlagDocumentStyleTableElementStyles = 1 << 8;

        public const uint updateFlagLearningResultCriterias = 1 << 2;

        public const uint updateFlagSubjectTemplate                  = 1 << 2;
        public const uint updateFlagSubjectCalendar                  = 1 << 3;
        public const uint updateFlagSubjectWeekSchedule              = 1 << 4;
        public const uint updateFlagSubjectMetodologies              = 1 << 5;
        public const uint updateFlagSubjectSpaceResources            = 1 << 6;
        public const uint updateFlagSubjectMaterialResources         = 1 << 7;
        public const uint updateFlagSubjectEvaluationInstrumentTypes = 1 << 8;

        public const uint updateFlagBlockActivities = 1 << 2;


        uint flags;

        public static UpdateFlags Create()
        {
            UpdateFlags result = new UpdateFlags();
            result.flags = 0;

            return result;
        }

        public static UpdateFlags Create(uint entityFlags, uint propertyFlags)
        {
            UpdateFlags result = Create();
            result.flags = entityFlags | propertyFlags;
            return result;
        }

        public UpdateFlags WithEntity(uint entityFlag)
        {
            UpdateFlags result = Create();
            result.flags = (flags & 0xFFFF) | entityFlag;
            return result;
        }

        public UpdateFlags WithProperty(uint propertyFlag)
        {
            UpdateFlags result = Create();
            result.flags = flags | propertyFlag;
            return result;
        }

        public bool IsEntity(uint entityFlag)
        {
            return (flags & entityFlag) != 0;
        }

        public bool HasProperty(uint propertyFlag)
        {
            return (flags & propertyFlag) != 0;
        }
    }


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
        StrongReferencesBoxController<WeekSchedule, WeekScheduleEditor> weekSchedulesController;
        StrongReferencesBoxController<Calendar, CalendarEditor> calendarsController;
        StrongReferencesBoxController<SubjectTemplate, SubjectTemplateEditor> subjectTemplatesController;
        StrongReferencesBoxController<GradeTemplate, GradeTemplateEditor> gradeTemplatesController;
        StrongReferencesBoxController<Subject, SubjectEditor> subjectsController;

        DocumentStyle style;

        Configuration configuration;

        public MainWindow()
        {
            InitializeComponent();

            string title = Constants.appName;
            Title = title;
            LabelTitle.Content = title;


            InitConfiguration();

            Storage.Init();

            CreatDefaultStyleIfNotPresent();

            InitUI();

            LaunchFirstRunDialogs();

        }

        private void CreatDefaultStyleIfNotPresent()
        {
            List<DocumentStyle> styles = Storage.LoadAllEntities<DocumentStyle>();
            Debug.Assert(styles.Count <= 1, "More than one style found");

            if (styles.Count == 0) { style = new(); style.Save(); }
            else { style = styles[0]; }

        }

        void LaunchFirstRunDialogs()
        {
            Dispatcher.BeginInvoke(
                async () =>
                {
                    if (configuration.FirstRun)
                    {
                        AboutDialog about = new();

                        Blocker.Visibility = Visibility.Visible;
                        about.ShowDialog();
                        Blocker.Visibility = Visibility.Hidden;

                        bool wordFound = false;

                        await RunLongTaskAsync("Buscando Word en el equipo", () =>
                        {
                            Microsoft.Office.Interop.Word.Application app = new();
                            if (app == null) { wordFound = false; }
                            else { app.Quit(); wordFound = true; }

                        });

                        if (!wordFound)
                        {
                            ConfirmDialog wordCheckFailed = new ConfirmDialog();

                            wordCheckFailed.Init(ConfirmIconType.warning, "Word no encontrado",
                                        "¡Vaya! No se ha podido encontrar una versión compatible de Microsoft Word en el equipo, " +
                                        "pero no te preocupes. Podrás generar las programaciones en formato HTML.",
                                        ConfirmChooseType.acceptOnly, (b) => { });

                            Blocker.Visibility = Visibility.Visible;
                            wordCheckFailed.ShowDialog();
                            Blocker.Visibility = Visibility.Hidden;

                        }
                        else
                        {
                            ConfirmDialog wordCheckSuccess = new ConfirmDialog();

                            wordCheckSuccess.Init(ConfirmIconType.info, "Word encontrado",
                                        "¡Felicidades! Se ha encontrado una versión compatible de Microsoft Word en el equipo, " +
                                        "por lo que podrás generar las programaciones en formato Word además de HTML.",
                                        ConfirmChooseType.acceptOnly, (b) => { });

                            Blocker.Visibility = Visibility.Visible;
                            wordCheckSuccess.ShowDialog();
                            Blocker.Visibility = Visibility.Hidden;
                        }

                        ConfirmDialog tutorialQuestion = new ConfirmDialog();

                        tutorialQuestion.Init(ConfirmIconType.question, "Ver tutorial",
                                "Parece que es la primera vez que arrancas la aplicación ¿quieres ver el tutorial?\n" +
                                "(se abrirá en tu navegador por defecto).",
                                ConfirmChooseType.yesAndNo,
                                (b) =>
                                {
                                    if (b) { Utils.OpenUrl(Constants.helpUrl); }
                                    else
                                    {
                                        ConfirmDialog checkLater = new();

                                        checkLater.Init(ConfirmIconType.info, "Ver más tarde", "Si quieres ver el tutorial más adelante, " +
                                            "pulsa el botón Ayuda en la ventana principal de la aplicación.",
                                            ConfirmChooseType.acceptOnly, (b) => { });

                                        checkLater.ShowDialog();
                                    }
                                });


                        Blocker.Visibility = Visibility.Visible;
                        tutorialQuestion.ShowDialog();
                        Blocker.Visibility = Visibility.Hidden;

                    }
                }
            );

        }

        void InitConfiguration()
        {
            configuration = new Configuration();

            if (File.Exists(Constants.configFileName))
            {
                string text = File.ReadAllText(Constants.configFileName);
                Configuration? loaded = JsonSerializer.Deserialize<Configuration>(text);
                if (loaded != null) { configuration = loaded; }
            }

        }

        void ResetConfiguration()
        {
            configuration = new Configuration();

            if (File.Exists(Constants.configFileName))
            {
                File.Delete(Constants.configFileName);
            }
        }

        void SaveConfiguration()
        {
            string text = JsonSerializer.Serialize<Configuration>(configuration);
            File.WriteAllText(Constants.configFileName, text);
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

            weekSchedulesController = new(configWeeks);
            calendarsController = new(configCalendars);
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



            subjectsController = new(configSubjects);

            ButtonClose.ToolTip = "Salir";
            ButtonExport.ToolTip = "Exportar";
            ButtonImport.ToolTip = "Importar";
            ButtonGenerateDocument.ToolTip = "Generar";
            ButtonHelp.ToolTip = "Ver ayuda";
            ButtonAbout.ToolTip = "Ver información acerca de la aplicación";
            ButtonReset.ToolTip = "Borrar todos los datos";

            Topmost = false;

        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            configuration.FirstRun = false;

            SaveConfiguration();

            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            Blocker.Visibility = Visibility.Visible;

            ConfirmDialog question = new();

            question.Init(ConfirmIconType.info,
                "Abrir navegador",
                "Esto abrirá tu navegador por defecto y te dirigirá al tutorial de la aplicación",
                ConfirmChooseType.acceptAndCancel,
                (b) => { if (b) { Utils.OpenUrl(Constants.helpUrl); } });

            question.ShowDialog();

            Blocker.Visibility = Visibility.Hidden;
        }

        async private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            Blocker.Visibility = Visibility.Visible;

            ConfirmDialog confirm = new();
            confirm.Init(ConfirmIconType.warning, "Confirmación",
                        "Esto eliminará TODOS los elementos y ajustes guardados y reiniciará " +
                        "la aplicación ¿estás seguro/a?",
                         ConfirmChooseType.acceptAndCancel,
                async (e) =>
                {
                    if (e)
                    {
                        Hide();

                        await RunInformativeTask("Eliminando elementos y ajustes");

                        ResetConfiguration();
                        Storage.Reset();

                        CreatDefaultStyleIfNotPresent();

                        await RunInformativeTask("Reiniciando la aplicación", Constants.restartWaitTime);

                        RestartUI();

                        Show();

                        Blocker.Visibility = Visibility.Hidden;
                        LaunchFirstRunDialogs();
                    }
                    else
                    {
                        Blocker.Visibility = Visibility.Hidden;
                    }


                });

            confirm.ShowDialog();

        }

        async private void ButtonImport_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new();
            openFileDialog.Title = "Elige archivo para cargar";
            openFileDialog.Filter = "Ficheros zip (*.zip)|*.zip|Todos los ficheros (*.*)|*.*";

            Blocker.Visibility = Visibility.Visible;

            if (openFileDialog.ShowDialog().GetValueOrDefault())
            {

                Storage.Archive_Open(openFileDialog.FileName);

                List<DocumentStyle> styles = Storage.LoadAllEntities<DocumentStyle>();
                bool hasStyle = (styles.Count > 0);

                ExportImportDialogConfiguration config = new()
                {
                    isExport = false,
                    gradeTemplateStorageIds = Storage.GetStorageIds<GradeTemplate>(Storage.LoadAllEntities<GradeTemplate>()),
                    subjectTemplatesStorageIds = Storage.GetStorageIds<SubjectTemplate>(Storage.LoadAllEntities<SubjectTemplate>()),
                    calendarsStorageIds = Storage.GetStorageIds<Calendar>(Storage.LoadAllEntities<Calendar>()),
                    weekSchedulesStorageIds = Storage.GetStorageIds<WeekSchedule>(Storage.LoadAllEntities<WeekSchedule>()),
                    subjectsStorageIds = Storage.GetStorageIds<Subject>(Storage.LoadAllEntities<Subject>()),
                    documentStyleStorageId = (styles.Count > 0 ? styles[0].StorageId : null),
                    includeDocumentStyle = hasStyle,
                    closeAction =
                        (accepted, exportDialog) =>
                        {
                            bool replaceStyle = false;
                            string replaceStyleId = "";

                            if (accepted)
                            {
                                List<string> storageIds = new();
                                storageIds.AddRange(exportDialog.GradeTemplatesStorageIds);
                                storageIds.AddRange(exportDialog.SubjectTemplatesStorageIds);
                                storageIds.AddRange(exportDialog.CalendarsStorageIds);
                                storageIds.AddRange(exportDialog.WeekSchedulesStorageIds);
                                storageIds.AddRange(exportDialog.SubjectsStorageIds);

                                if (exportDialog.CheckBoxIncludeDocumentStyle.IsChecked.GetValueOrDefault())
                                {
                                    replaceStyle = true;
                                    replaceStyleId = exportDialog.DocumentStyleStorageId;
                                    storageIds.Add(exportDialog.DocumentStyleStorageId);
                                }

                                Storage.Archive_CopyStorageIdsToBase(storageIds);

                            }

                            Storage.Archive_Close();

                            Blocker.Visibility = Visibility.Hidden;

                            if (accepted)
                            {
                                if (replaceStyle)
                                {
                                    if (replaceStyleId != style.StorageId)
                                    {
                                        style.Delete();
                                    }

                                    style.LoadOrCreate(replaceStyleId);

                                }

                                RestartUI();
                            }

                            return false;
                        }

                };

                ExportImportDialog dialog = new();

                dialog.Init(config);

                dialog.ShowDialog();

                if(dialog.Result)
                {
                    await RunInformativeTask("Importando elementos del zip");
                    ShowMessageDialog("Importación completada", "Se han importado los elementos del zip seleccionados.");
                }

            }
            else
            {
                Blocker.Visibility = Visibility.Hidden;
            }
        }

        async void ButtonExport_Click(object sender, RoutedEventArgs e)
        {
            Blocker.Visibility = Visibility.Visible;

            ExportImportDialog dialog = new();

            Func<bool, ExportImportDialog, bool> closeAction =
            (accepted, exportDialog) =>
            {
                bool cancelClose = false;

                if (accepted)
                {
                    SaveFileDialog saveFileDialog = new();
                    saveFileDialog.Title = "Elige archivo para guardar";
                    saveFileDialog.Filter = "Ficheros zip (*.zip)|*.zip|Todos los ficheros (*.*)|*.*";

                    exportDialog.Blocker.Visibility = Visibility.Visible;

                    if (saveFileDialog.ShowDialog().GetValueOrDefault())
                    {
                        List<string> storageIds = new();
                        storageIds.AddRange(dialog.GradeTemplatesStorageIds);
                        storageIds.AddRange(dialog.SubjectTemplatesStorageIds);
                        storageIds.AddRange(dialog.CalendarsStorageIds);
                        storageIds.AddRange(dialog.WeekSchedulesStorageIds);
                        storageIds.AddRange(dialog.SubjectsStorageIds);

                        if (exportDialog.CheckBoxIncludeDocumentStyle.IsChecked.GetValueOrDefault())
                        {
                            storageIds.Add(dialog.DocumentStyleStorageId);
                        }

                        Storage.Archive_Create(storageIds, saveFileDialog.FileName);


                    }
                    else
                    {
                        cancelClose = true;
                    }

                    exportDialog.Blocker.Visibility = Visibility.Hidden;
                }

                if (!cancelClose)
                {
                    Blocker.Visibility = Visibility.Hidden;
                }

                return cancelClose;
            };

            ExportImportDialogConfiguration config = new()
            {
                isExport = true,
                gradeTemplateStorageIds = Storage.GetStorageIds<GradeTemplate>(Storage.LoadAllEntities<GradeTemplate>()),
                subjectTemplatesStorageIds = Storage.GetStorageIds<SubjectTemplate>(Storage.LoadAllEntities<SubjectTemplate>()),
                calendarsStorageIds = Storage.GetStorageIds<Calendar>(Storage.LoadAllEntities<Calendar>()),
                weekSchedulesStorageIds = Storage.GetStorageIds<WeekSchedule>(Storage.LoadAllEntities<WeekSchedule>()),
                subjectsStorageIds = Storage.GetStorageIds<Subject>(Storage.LoadAllEntities<Subject>()),
                includeDocumentStyle = true,
                documentStyleStorageId = style.StorageId,
                closeAction = closeAction
            };

            dialog.Init(config);

            dialog.ShowDialog();

            if(dialog.Result)
            {
                await RunInformativeTask("Exportando al zip");
                ShowMessageDialog("Exportación completada", "Los elementos seleccionados se han guardado en el zip");
            }
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
            Blocker.Visibility = Visibility.Visible;

            Subject? subject = subjectsController.GetSelectedEntity();
            if (subject != null)
            {
                HTMLGeneratorDialog generatorDialog = new();

                generatorDialog.Init(subject, style, (b) => { Blocker.Visibility = Visibility.Hidden; });
                generatorDialog.ShowDialog();

            }
            else
            {
                ConfirmDialog dialog = new();
                dialog.Init(ConfirmIconType.warning, "Aviso", "No se puede generar el documento porque no se ha seleccionado una programación", ConfirmChooseType.acceptOnly, (b) => Blocker.Visibility = Visibility.Hidden);
                dialog.ShowDialog();
            }
        }

        private void ButtonAbout_Click(object sender, RoutedEventArgs e)
        {
            Blocker.Visibility = Visibility.Visible;

            AboutDialog dialog = new();

            dialog.ShowDialog();

            Blocker.Visibility = Visibility.Hidden;
        }

        private async Task RunLongTaskAsync(string title, Action action)
        {
            LongTaskDialog longTask = new();

            longTask.Init(title);

            Blocker.Visibility = Visibility.Visible;
            longTask.Show();

            await Task.Run(action);

            longTask.Close();
            Blocker.Visibility = Visibility.Hidden;
        }

        private async Task RunInformativeTask(string title, float duration = Constants.informativeTaskWaitTime)
        {
            await RunLongTaskAsync(title, () => { Thread.Sleep((int)(duration * 1000)); });
        }

        private void ShowMessageDialog(string title, string text)
        {
            Blocker.Visibility = Visibility.Visible;
            ConfirmDialog confirm = new();
            confirm.Init(ConfirmIconType.info, title,text,
                        ConfirmChooseType.acceptOnly, (b)=>{ });
            confirm.ShowDialog();
            Blocker.Visibility = Visibility.Hidden;
        }
    }
}