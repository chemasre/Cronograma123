using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using static Programacion123.ExportImportDialog;

namespace Programacion123
{
    public interface IEntityEditor<T>
    {
        void SetEntityTitleEditable(bool editable);
        void SetEditorTitle(string title);
        void InitEditor(T entity, string? _parentStorageId);
        Task InitEditorAsync(T entity, string? _parentStorageId);
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
        public static MainWindow Instance { get { return instance; } }
        static MainWindow instance;

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

            Utils.LogInit();

            Loaded += MainWindow_Loaded;

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            instance = this;

            EventManager.RegisterClassHandler(typeof(Window), Keyboard.PreviewKeyDownEvent, new KeyEventHandler(AnyWindow_PreviewKeyDown));

            InitUI();
            LaunchFirstRunDialogs();

        }

        private void AnyWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.F11)
            {
                if(Switches.debugLogEnabled)
                {
                    if(!LogPanel.Instance.IsVisible) { LogPanel.Instance.Show(); }
                    else { LogPanel.Instance.Hide(); }
                    e.Handled = true;
                    
                }
            }
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
                        about.Owner = this;
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
                            wordCheckFailed.Owner = this;

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
                            wordCheckSuccess.Owner = this;

                            wordCheckSuccess.Init(ConfirmIconType.info, "Word encontrado",
                                        "¡Felicidades! Se ha encontrado una versión compatible de Microsoft Word en el equipo, " +
                                        "por lo que podrás generar las programaciones en formato Word además de HTML.",
                                        ConfirmChooseType.acceptOnly, (b) => { });

                            Blocker.Visibility = Visibility.Visible;
                            wordCheckSuccess.ShowDialog();
                            Blocker.Visibility = Visibility.Hidden;
                        }

                        ConfirmDialog tutorialQuestion = new ConfirmDialog();
                        tutorialQuestion.Owner = this;

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
                                        checkLater.Owner = this;

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
                                                   .WithBlocker(Blocker)
                                                   .WithDialogsOwner(this);

            var configWeeks = StrongReferencesBoxConfiguration<WeekSchedule>.CreateForCombo(ComboWeekSchedules)
                                                   .WithStorageIds(Storage.GetStorageIds<WeekSchedule>(Storage.LoadAllEntities<WeekSchedule>()))
                                                   .WithNew(ButtonWeekScheduleNew)
                                                   .WithEdit(ButtonWeekScheduleEdit)
                                                   .WithDelete(ButtonWeekScheduleDelete)
                                                   .WithDeleteConfirmQuestion("Esto eliminará permanentemente el horario seleccionado. ¿Estás seguro/a?")
                                                   .WithBlocker(Blocker)
                                                   .WithDialogsOwner(this);

            var configCalendars = StrongReferencesBoxConfiguration<Calendar>.CreateForCombo(ComboBoxCalendars)
                                                   .WithStorageIds(Storage.GetStorageIds<Calendar>(Storage.LoadAllEntities<Calendar>()))
                                                   .WithNew(ButtonCalendarNew)
                                                   .WithEdit(ButtonCalendarEdit)
                                                   .WithDelete(ButtonCalendarDelete)
                                                   .WithDeleteConfirmQuestion("Esto eliminará permanentemente el calendario seleccionado. ¿Estás seguro/a?")
                                                   .WithBlocker(Blocker)
                                                   .WithDialogsOwner(this);


            var configSubjectTemplates = StrongReferencesBoxConfiguration<SubjectTemplate>.CreateForCombo(ComboSubjectTemplates)
                                                   .WithStorageIds(Storage.GetStorageIds<SubjectTemplate>(Storage.LoadAllEntities<SubjectTemplate>()))
                                                   .WithEntityInitializer(
                                                            (SubjectTemplate t) =>
                                                            {
                                                                t.GradeTemplate.Value = gradeTemplatesController.GetSelectedEntity();
                                                            })
                                                   .WithNew(ButtonSubjectTemplateNew)
                                                   .WithEdit(ButtonSubjectTemplateEdit)
                                                   .WithDelete(ButtonSubjectTemplateDelete)
                                                   .WithDeleteConfirmQuestion("Esto eliminará permanentemente la plantilla de módulo seleccionada junto con los elementos curriculares definidos en ella. ¿Estás seguro/a?")
                                                   .WithBlocker(Blocker)
                                                   .WithDialogsOwner(this);

            weekSchedulesController = new(configWeeks);
            calendarsController = new(configCalendars);
            subjectTemplatesController = new(configSubjectTemplates);
            gradeTemplatesController = new(configGradeTemplates);

            var configSubjects = StrongReferencesBoxConfiguration<Subject>.CreateForCombo(ComboSubjects)
                                                   .WithStorageIds(Storage.GetStorageIds<Subject>(Storage.LoadAllEntities<Subject>()))
                                                   .WithEntityInitializer(
                                                            (Subject s) =>
                                                            {
                                                                s.Template.Value = subjectTemplatesController.GetSelectedEntity();
                                                                s.Calendar.Value = calendarsController.GetSelectedEntity();
                                                                s.WeekSchedule.Value = weekSchedulesController.GetSelectedEntity();
                                                            })
                                                   .WithNew(ButtonSubjectNew)
                                                   .WithEdit(ButtonSubjectEdit)
                                                   .WithDelete(ButtonSubjectDelete)
                                                   .WithDeleteConfirmQuestion("Esto eliminará permanentemente el módulo seleccionada junto con los bloques y otros elementos definidos en ella. ¿Estás seguro/a?")
                                                   .WithBlocker(Blocker)
                                                   .WithAsyncEditorInit(true)
                                                   .WithDialogsOwner(this);

            subjectsController = new(configSubjects);

            ButtonClose.ToolTip = "Salir";
            ButtonExport.ToolTip = "Exportar";
            ButtonImport.ToolTip = "Importar";
            ButtonGenerateDocument.ToolTip = "Generar";
            ButtonHelp.ToolTip = "Ver ayuda";
            ButtonAbout.ToolTip = "Ver información acerca de la aplicación";
            ButtonReset.ToolTip = "Borrar todos los datos";

        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            configuration.FirstRun = false;

            SaveConfiguration();

            Utils.LogFinish();

            if(Switches.debugLogEnabled) { LogPanel.Instance.Close(); }

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
            question.Owner = this;

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
            confirm.Owner = this;
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

                        await RunInformativeTask("Reiniciando la aplicación", Constants.resetTaskMinDuration);

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
                dialog.Owner = this;

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
            dialog.Owner = this;

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

        async private void ButtonGenerateDocument_Click(object sender, RoutedEventArgs e)
        {
            Blocker.Visibility = Visibility.Visible;

            Subject? subject = subjectsController.GetSelectedEntity();
            if (subject != null)
            {
                HTMLGeneratorDialog generatorDialog = new();
                generatorDialog.Owner = this;
                await generatorDialog.InitAsync(subject, style, (b) => { Blocker.Visibility = Visibility.Hidden; });
                generatorDialog.ShowDialog();

            }
            else
            {
                ConfirmDialog dialog = new();
                dialog.Owner = this;
                dialog.Init(ConfirmIconType.warning, "Aviso", "No se puede generar el documento porque no se ha seleccionado una programación", ConfirmChooseType.acceptOnly, (b) => Blocker.Visibility = Visibility.Hidden);
                dialog.ShowDialog();
            }
        }

        private void ButtonAbout_Click(object sender, RoutedEventArgs e)
        {
            Blocker.Visibility = Visibility.Visible;

            AboutDialog dialog = new();
            dialog.Owner = this;
            dialog.ShowDialog();

            Blocker.Visibility = Visibility.Hidden;
        }

        private async Task RunLongTaskAsync(string title, Action action)
        {
            LongTaskDialog longTask = new();
            longTask.Owner = this;
            longTask.Init(title);

            Blocker.Visibility = Visibility.Visible;
            longTask.Show();

            await Task.Run(action);

            longTask.Close();
            Blocker.Visibility = Visibility.Hidden;
        }

        private async Task RunInformativeTask(string title, float duration = Constants.setupTaskMinDuration)
        {
            await RunLongTaskAsync(title, () => { Thread.Sleep((int)(duration * 1000)); });
        }

        private void ShowMessageDialog(string title, string text)
        {
            Blocker.Visibility = Visibility.Visible;
            ConfirmDialog confirm = new();
            confirm.Owner = this;
            confirm.Init(ConfirmIconType.info, title,text,
                        ConfirmChooseType.acceptOnly, (b)=>{ });
            confirm.ShowDialog();
            Blocker.Visibility = Visibility.Hidden;
        }
    }
}