using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Microsoft.Windows.Themes;
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

    public class Constants
    {
        public const string appName = "Programabara";
        public const string contactName = "Chema";
        public const string contactEmail = "chema.sre@gmail.com";
        public const string version = "0.8.0";
        public const string configFileName = "Config.json";
        public const float  restartWaitTime = 2.0f;

        public const string helpUrl = "http://youtube.com";

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

            List<DocumentStyle> styles = Storage.LoadAllEntities<DocumentStyle>();
            Debug.Assert(styles.Count <= 1, "More than one style found");

            if(styles.Count == 0) { style = new(); style.Save(); }
            else { style = styles[0]; }

            InitUI();

            LaunchFirstRunDialogs();

        }

        void LaunchFirstRunDialogs()
        {
            Dispatcher.BeginInvoke(
                async () =>
                {
                    if(configuration.FirstRun)
                    {
                        AboutDialog about = new();

                        Blocker.Visibility = Visibility.Visible;
                        about.ShowDialog();
                        Blocker.Visibility = Visibility.Hidden;
                
                        bool wordFound = false;

                        LongTaskDialog longTask = new();

                        longTask.Init("Buscando Word en el equipo");

                        Blocker.Visibility = Visibility.Visible;
                        longTask.Show();

                        await Task.Run(() => 
                        {
                            Microsoft.Office.Interop.Word.Application app = new();
                            if(app == null) { wordFound = false; }
                            else { app.Quit(); wordFound = true; }

                        });

                        longTask.Close();
                        Blocker.Visibility = Visibility.Hidden;

                        if(!wordFound)
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
                                    if(b) { OpenUrl(Constants.helpUrl); }
                                    else
                                    {
                                        ConfirmDialog checkLater = new();

                                        checkLater.Init(ConfirmIconType.info, "Ver más tarde", "Si quieres ver el tutorial más adelante, " +
                                            "pulsa el botón Ayuda en la ventana principal de la aplicación.",
                                            ConfirmChooseType.acceptOnly, (b)=> { });

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

            if(File.Exists(Constants.configFileName))
            {
                string text = File.ReadAllText(Constants.configFileName);
                Configuration? loaded = JsonSerializer.Deserialize<Configuration>(text);
                if(loaded != null) { configuration = loaded; }
            }

        }

        void ResetConfiguration()
        {
            configuration = new Configuration(); 

            if(File.Exists(Constants.configFileName))
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

        private void Window_MouseDown_1(object sender, MouseButtonEventArgs e)
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
                (b) => { if(b) { OpenUrl(Constants.helpUrl); } });    
            
            question.ShowDialog();

            Blocker.Visibility = Visibility.Hidden;        }

        void OpenUrl(string url)
        {
            ProcessStartInfo info = new ();
            info.FileName = url;
            info.UseShellExecute = true;
            Process.Start (info);
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            Blocker.Visibility = Visibility.Visible;

            ConfirmDialog confirm = new();
            confirm.Init(ConfirmIconType.warning, "Confirmación",
                        "Esto eliminará TODOS los datos y ajustes guardados y reiniciará " +
                        "la aplicación ¿estás seguro/a?",
                         ConfirmChooseType.acceptAndCancel,
                async (e) =>
                {
                    if(e)
                    {
                        ResetConfiguration();
                        Storage.Reset();

                        LongTaskDialog longTask = new();
                        
                        Hide();

                        longTask.Init("Reiniciando la aplicación");

                        longTask.Show();
                        await Task.Run(() => { Thread.Sleep((int)(Constants.restartWaitTime * 1000)); });
                        longTask.Close();

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

        private void ButtonImport_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new();
            openFileDialog.Title = "Elige archivo para cargar";
            openFileDialog.Filter = "Ficheros zip (*.zip)|*.zip|Todos los ficheros (*.*)|*.*";

            Blocker.Visibility = Visibility.Visible;

            if(openFileDialog.ShowDialog().GetValueOrDefault())
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

                            if(accepted)
                            {
                                List<string> storageIds = new();
                                storageIds.AddRange(exportDialog.GradeTemplatesStorageIds);
                                storageIds.AddRange(exportDialog.SubjectTemplatesStorageIds);
                                storageIds.AddRange(exportDialog.CalendarsStorageIds);
                                storageIds.AddRange(exportDialog.WeekSchedulesStorageIds);
                                storageIds.AddRange(exportDialog.SubjectsStorageIds);

                                if(exportDialog.CheckBoxIncludeDocumentStyle.IsChecked.GetValueOrDefault())
                                {
                                    replaceStyle = true;
                                    replaceStyleId = exportDialog.DocumentStyleStorageId;
                                    storageIds.Add(exportDialog.DocumentStyleStorageId);
                                }

                                Storage.Archive_CopyStorageIdsToBase(storageIds);


                            }

                            Storage.Archive_Close();

                            Blocker.Visibility = Visibility.Hidden;

                            if(accepted)
                            {
                                if(replaceStyle)
                                {
                                    if(replaceStyleId != style.StorageId)
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

            }
            else
            {
                Blocker.Visibility = Visibility.Hidden;
            }
        }

        private void ButtonExport_Click(object sender, RoutedEventArgs e)
        {
            Blocker.Visibility = Visibility.Visible;

            ExportImportDialog dialog = new();

            Func<bool, ExportImportDialog, bool> closeAction = 
            (accepted, exportDialog) =>
            {
                bool cancelClose = false;

                if(accepted)
                {
                    SaveFileDialog saveFileDialog = new();
                    saveFileDialog.Title = "Elige archivo para guardar";
                    saveFileDialog.Filter = "Ficheros zip (*.zip)|*.zip|Todos los ficheros (*.*)|*.*";

                    exportDialog.Blocker.Visibility = Visibility.Visible;

                    if(saveFileDialog.ShowDialog().GetValueOrDefault())
                    {
                        List<string> storageIds = new();
                        storageIds.AddRange(dialog.GradeTemplatesStorageIds);
                        storageIds.AddRange(dialog.SubjectTemplatesStorageIds);
                        storageIds.AddRange(dialog.CalendarsStorageIds);
                        storageIds.AddRange(dialog.WeekSchedulesStorageIds);
                        storageIds.AddRange(dialog.SubjectsStorageIds);

                        if(exportDialog.CheckBoxIncludeDocumentStyle.IsChecked.GetValueOrDefault())
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

                if(!cancelClose)
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
            if(subject != null)
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
    }
}