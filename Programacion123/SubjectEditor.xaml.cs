using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Programacion123
{
    /// <summary>
    /// Lógica de interacción para SubjectEditor.xaml
    /// </summary>
    public partial class SubjectEditor : Window, IEntityEditor<Subject>
    {
        Subject entity;
        string? parentStorageId;

        EntityFieldController<SubjectTemplate, SubjectTemplateEditor, EntityPicker<SubjectTemplate>> subjectTemplateController;
        EntityFieldController<Calendar, CalendarEditor, EntityPicker<Calendar>> calendarController;
        EntityFieldController<WeekSchedule, WeekScheduleEditor, EntityPicker<WeekSchedule>> weekScheduleController;

        EntityFieldController<CommonText, CommonTextEditor, EntityPicker<CommonText> > metodologiesIntroductionController;
        EntityBoxController<CommonText, CommonTextEditor> metodologiesController;
        EntityFieldController<CommonText, CommonTextEditor, EntityPicker<CommonText> > resourcesIntroductionController;
        EntityBoxController<CommonText, CommonTextEditor> spaceResourcesController;
        EntityBoxController<CommonText, CommonTextEditor> materialResourcesController;
        EntityFieldController<CommonText, CommonTextEditor, EntityPicker<CommonText> > evaluationInstrumentTypesIntroductionController;
        EntityBoxController<CommonText, CommonTextEditor> evaluationInstrumentTypesController;

        public SubjectEditor()
        {
            InitializeComponent();


        }

        public Subject GetEntity()
        {
            return entity;
        }

        public void SetEntity(Subject _subject, string? _parentStorageId)
        {
            entity = _subject;
            parentStorageId = _parentStorageId;

            var configTemplate = EntityFieldConfiguration<SubjectTemplate>.CreateForTextBox(TextTemplate)
                                               .WithStorageId(entity.Template?.StorageId, true)
                                               .WithPick(ButtonTemplatePick)
                                               .WithFieldDisplayType(EntityFieldDisplayType.title)
                                               .WithPickerTitle("Selecciona una plantilla")
                                               .WithBlocker(Blocker);

            subjectTemplateController = new(configTemplate);

            var configCalendar = EntityFieldConfiguration<Calendar>.CreateForTextBox(TextCalendar)
                                               .WithStorageId(entity.Calendar?.StorageId, true)
                                               .WithPick(ButtonCalendarPick)
                                               .WithFieldDisplayType(EntityFieldDisplayType.title)
                                               .WithPickerTitle("Selecciona un calendario")
                                               .WithBlocker(Blocker);

            calendarController = new(configCalendar);

            var configWeekSchedule = EntityFieldConfiguration<WeekSchedule>.CreateForTextBox(TextWeekSchedule)
                                               .WithStorageId(entity.WeekSchedule?.StorageId, true)
                                               .WithPick(ButtonWeekSchedulePick)
                                               .WithFieldDisplayType(EntityFieldDisplayType.title)
                                               .WithPickerTitle("Selecciona un horario")
                                               .WithBlocker(Blocker);

            weekScheduleController = new(configWeekSchedule);

            var configMetodologiesIntroduction = EntityFieldConfiguration<CommonText>.CreateForTextBox(TextMetodologyIntroduction)
                                               .WithStorageId(entity.MetodologiesIntroduction.StorageId)
                                               .WithParentStorageId(entity.StorageId)
                                               .WithNew(ButtonMetodologiesIntroductionNew)
                                               .WithEdit(ButtonMetodologiesIntroductionEdit)
                                               .WithTitleEditable(false)
                                               .WithEditorTitle("Introducción a las metodologías")
                                               .WithBlocker(Blocker);

            metodologiesIntroductionController = new(configMetodologiesIntroduction);

            var configMetodologies = EntityBoxConfiguration<CommonText>.CreateForList(ListBoxMetodologies)
                                                        .WithParentStorageId(entity.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(entity.Metodologies.ToList()))
                                                        .WithPrefix(EntityBoxItemsPrefix.none)
                                                        .WithNew(ButtonMetodologyNew)
                                                        .WithEdit(ButtonMetodologyEdit)
                                                        .WithDelete(ButtonMetodologyDelete)
                                                        .WithUpDown(ButtonMetodologyUp, ButtonMetodologyDown)
                                                        .WithEditorTitle("Metodología")
                                                        .WithBlocker(Blocker);

            metodologiesController = new(configMetodologies);

            var configResourcesIntroduction = EntityFieldConfiguration<CommonText>.CreateForTextBox(TextResourcesIntroduction)
                                               .WithStorageId(entity.ResourcesIntroduction.StorageId)
                                               .WithParentStorageId(entity.StorageId)
                                               .WithNew(ButtonResourcesIntroductionNew)
                                               .WithEdit(ButtonResourcesIntroductionEdit)
                                               .WithTitleEditable(false)
                                               .WithEditorTitle("Introducción a los recursos")
                                               .WithBlocker(Blocker);

            resourcesIntroductionController = new(configResourcesIntroduction);

            var configSpaceResources = EntityBoxConfiguration<CommonText>.CreateForList(ListBoxSpaceResources)
                                                        .WithParentStorageId(entity.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(entity.SpaceResources.ToList()))
                                                        .WithPrefix(EntityBoxItemsPrefix.none)
                                                        .WithNew(ButtonSpaceResourceNew)
                                                        .WithEdit(ButtonSpaceResourceEdit)
                                                        .WithDelete(ButtonSpaceResourceDelete)
                                                        .WithUpDown(ButtonSpaceResourceUp, ButtonSpaceResourceDown)
                                                        .WithEditorTitle("Espacio")
                                                        .WithBlocker(Blocker);

            spaceResourcesController = new(configSpaceResources);

            var configMaterialResources = EntityBoxConfiguration<CommonText>.CreateForList(ListBoxMaterialResources)
                                                        .WithParentStorageId(entity.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(entity.MaterialResources.ToList()))
                                                        .WithPrefix(EntityBoxItemsPrefix.none)
                                                        .WithNew(ButtonMaterialResourceNew)
                                                        .WithEdit(ButtonMaterialResourceEdit)
                                                        .WithDelete(ButtonMaterialResourceDelete)
                                                        .WithUpDown(ButtonMaterialResourceUp, ButtonMaterialResourceDown)
                                                        .WithEditorTitle("Material")
                                                        .WithBlocker(Blocker);

            materialResourcesController = new(configMaterialResources);

            var configEvaluationInstrumentTypesIntroduction = EntityFieldConfiguration<CommonText>.CreateForTextBox(TextEvaluationInstrumentTypesIntroduction)
                                               .WithStorageId(entity.EvaluationInstrumentTypesIntroduction.StorageId)
                                               .WithParentStorageId(entity.StorageId)
                                               .WithNew(ButtonEvaluationInstrumentTypesIntroductionNew)
                                               .WithEdit(ButtonEvaluationInstrumentTypesIntroductionEdit)
                                               .WithTitleEditable(false)
                                               .WithEditorTitle("Introducción a los instrumentos de evaluación")
                                               .WithBlocker(Blocker);

            evaluationInstrumentTypesIntroductionController = new(configEvaluationInstrumentTypesIntroduction);

            var configEvaluationInstrumentTypes = EntityBoxConfiguration<CommonText>.CreateForList(ListBoxEvaluationInstrumentTypes)
                                                        .WithParentStorageId(entity.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(entity.EvaluationInstrumentsTypes.ToList()))
                                                        .WithPrefix(EntityBoxItemsPrefix.none)
                                                        .WithNew(ButtonEvaluationInstrumentTypeNew)
                                                        .WithEdit(ButtonEvaluationInstrumentTypeEdit)
                                                        .WithDelete(ButtonEvaluationInstrumentTypeDelete)
                                                        .WithUpDown(ButtonEvaluationInstrumentTypeUp, ButtonEvaluationInstrumentTypeDown)
                                                        .WithEditorTitle("Tipo de instrumento de evaluación")
                                                        .WithBlocker(Blocker);

            evaluationInstrumentTypesController = new(configEvaluationInstrumentTypes);

            TextTitle.Text = entity.Title;

        }

        private void UpdateEntity()
        {
            entity.Title = TextTitle.Text;

            entity.Template = subjectTemplateController.GetEntity();
            entity.Calendar = calendarController.GetEntity();
            entity.WeekSchedule = weekScheduleController.GetEntity();

            entity.MetodologiesIntroduction = Storage.LoadOrCreateEntity<CommonText>(metodologiesIntroductionController.StorageId, entity.StorageId);
            entity.Metodologies.Set(Storage.LoadEntities<CommonText>(metodologiesController.StorageIds, entity.StorageId));

            entity.ResourcesIntroduction = Storage.LoadOrCreateEntity<CommonText>(resourcesIntroductionController.StorageId, entity.StorageId);
            entity.SpaceResources.Set(Storage.LoadEntities<CommonText>(spaceResourcesController.StorageIds, entity.StorageId));
            entity.MaterialResources.Set(Storage.LoadEntities<CommonText>(materialResourcesController.StorageIds, entity.StorageId));

            entity.EvaluationInstrumentTypesIntroduction = Storage.LoadOrCreateEntity<CommonText>(evaluationInstrumentTypesIntroductionController.StorageId, entity.StorageId);
            entity.EvaluationInstrumentsTypes.Set(Storage.LoadEntities<CommonText>(evaluationInstrumentTypesController.StorageIds, entity.StorageId));
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            UpdateEntity();
            entity.Save(parentStorageId);

            Close();
        }

        public void SetEntityTitleEditable(bool editable)
        {
            TextTitle.IsReadOnly = !editable;
            TextTitle.IsReadOnlyCaretVisible = false;
        }

        public void SetEditorTitle(string title)
        {
            TextEditorTitle.Content = title;
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
