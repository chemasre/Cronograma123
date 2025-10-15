using System.Windows;
using System.Windows.Input;

namespace Programacion123
{
    /// <summary>
    /// Lógica de interacción para ExportImportDialog.xaml
    /// </summary>
    public partial class ExportImportDialog : Window
    {
        public bool Result { get { return result; } }

        bool result;

        Func<bool, ExportImportDialog, bool>? closeAction;

        WeakReferencesBoxController<GradeTemplate, EntityPicker<GradeTemplate>> gradeTemplatesController;
        WeakReferencesBoxController<SubjectTemplate, EntityPicker<SubjectTemplate>> subjectTemplatesController;
        WeakReferencesBoxController<Calendar, EntityPicker<Calendar>> calendarsController;
        WeakReferencesBoxController<WeekSchedule, EntityPicker<WeekSchedule>> weekSchedulesController;
        WeakReferencesBoxController<Subject, EntityPicker<Subject>> subjectsController;

        string documentStyleStorageId;

        public List<string> GradeTemplatesStorageIds { get { return gradeTemplatesController.StorageIds; } }
        public List<string> SubjectTemplatesStorageIds { get { return subjectTemplatesController.StorageIds; } }
        public List<string> WeekSchedulesStorageIds { get { return weekSchedulesController.StorageIds; } }
        public List<string> CalendarsStorageIds { get { return calendarsController.StorageIds; } }
        public List<string> SubjectsStorageIds { get { return subjectsController.StorageIds; } }
        public string? DocumentStyleStorageId { get { return documentStyleStorageId; } }

        string previousStorageBasePath;

        public ExportImportDialog()
        {
            InitializeComponent();
        }

        public struct ExportImportDialogConfiguration
        {
            public bool isExport;
            public List<string> gradeTemplateStorageIds;
            public List<string> subjectTemplatesStorageIds;
            public List<string> calendarsStorageIds;
            public List<string> weekSchedulesStorageIds;
            public List<string> subjectsStorageIds;
            public bool includeDocumentStyle;
            public string? documentStyleStorageId;
            public Func<bool, ExportImportDialog, bool>? closeAction;

        }

        public void Init(ExportImportDialogConfiguration config)
        {
            var configGradeTemplates = WeakReferencesBoxConfiguration<GradeTemplate>.CreateForList(ListBoxGradeTemplates)
                                                        .WithStorageIds(config.gradeTemplateStorageIds)
                                                        .WithFormat(EntityFormatContent.Title)
                                                        .WithPick(ButtonGradeTemplateReferenceAdd, ButtonGradeTemplateReferenceRemove)
                                                        .WithPickerTitle("Plantillas de ciclo")
                                                        .WithBlocker(Blocker)
                                                        .WithDialogsOwner(this);


            gradeTemplatesController = new(configGradeTemplates);

            var configSubjectTemplates = WeakReferencesBoxConfiguration<SubjectTemplate>.CreateForList(ListBoxSubjectTemplates)
                                                        .WithStorageIds(config.subjectTemplatesStorageIds)
                                                        .WithFormat(EntityFormatContent.Title)
                                                        .WithPick(ButtonSubjectTemplateReferenceAdd, ButtonSubjectTemplateReferenceRemove)
                                                        .WithPickerTitle("Plantillas de módulo")
                                                        .WithBlocker(Blocker)
                                                        .WithDialogsOwner(this);


            subjectTemplatesController = new(configSubjectTemplates);

            var configCalendars = WeakReferencesBoxConfiguration<Calendar>.CreateForList(ListBoxCalendars)
                                                        .WithStorageIds(config.calendarsStorageIds)
                                                        .WithFormat(EntityFormatContent.Title)
                                                        .WithPick(ButtonCalendarReferenceAdd, ButtonCalendarReferenceRemove)
                                                        .WithPickerTitle("Calendarios")
                                                        .WithBlocker(Blocker)
                                                        .WithDialogsOwner(this);


            calendarsController = new(configCalendars);

            var configWeekSchedules = WeakReferencesBoxConfiguration<WeekSchedule>.CreateForList(ListBoxWeekSchedules)
                                                        .WithStorageIds(config.weekSchedulesStorageIds)
                                                        .WithFormat(EntityFormatContent.Title)
                                                        .WithPick(ButtonWeekScheduleReferenceAdd, ButtonWeekScheduleReferenceRemove)
                                                        .WithPickerTitle("Horarios")
                                                        .WithBlocker(Blocker)
                                                        .WithDialogsOwner(this);


            weekSchedulesController = new(configWeekSchedules);

            var configSubjects = WeakReferencesBoxConfiguration<Subject>.CreateForList(ListBoxSubjects)
                                                        .WithStorageIds(config.subjectsStorageIds)
                                                        .WithFormat(EntityFormatContent.Title)
                                                        .WithPick(ButtonSubjectReferenceAdd, ButtonSubjectReferenceRemove)
                                                        .WithPickerTitle("Programaciones")
                                                        .WithBlocker(Blocker)
                                                        .WithDialogsOwner(this);


            subjectsController = new(configSubjects);

            documentStyleStorageId = config.documentStyleStorageId;
            CheckBoxIncludeDocumentStyle.IsChecked = config.includeDocumentStyle;

            if (!config.isExport && !config.includeDocumentStyle) { CheckBoxIncludeDocumentStyle.IsEnabled = false; }


            if (config.isExport)
            {
                TextTitle.Text = "Exportar elementos";
                LabelAccept.Content = "Exportar";
            }
            else
            {
                TextTitle.Text = "Importar elementos";
                LabelAccept.Content = "Importar";
            }

            ButtonClose.ToolTip = "Cerrar";

            closeAction = config.closeAction;


        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            result = false;
            bool? cancel = closeAction?.Invoke(false, this);
            if (!cancel.GetValueOrDefault()) { Close(); }
        }

        private void ButtonAccept_Click(object sender, RoutedEventArgs e)
        {
            result = true;
            bool? cancel = closeAction?.Invoke(true, this);

            if (!cancel.GetValueOrDefault()) { Close(); }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            result = false;
            bool? cancel = closeAction?.Invoke(false, this);

            if (!cancel.GetValueOrDefault()) { Close(); }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
