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

        WeakReferenceFieldController<SubjectTemplate, EntityPicker<SubjectTemplate>> subjectTemplateController;
        WeakReferenceFieldController<Calendar, EntityPicker<Calendar>> calendarController;
        WeakReferenceFieldController<WeekSchedule, EntityPicker<WeekSchedule>> weekScheduleController;

        StrongReferenceFieldController<CommonText, CommonTextEditor > metodologiesIntroductionController;
        StrongReferencesBoxController<CommonText, CommonTextEditor> metodologiesController;
        StrongReferenceFieldController<CommonText, CommonTextEditor > resourcesIntroductionController;
        StrongReferencesBoxController<CommonText, CommonTextEditor> spaceResourcesController;
        StrongReferencesBoxController<CommonText, CommonTextEditor > materialResourcesController;
        StrongReferenceFieldController<CommonText, CommonTextEditor> evaluationInstrumentTypesIntroductionController;
        StrongReferencesBoxController<CommonText, CommonTextEditor> evaluationInstrumentTypesController;

        StrongReferenceFieldController<CommonText, CommonTextEditor > blocksIntroductionController;
        StrongReferencesBoxController<Block, BlockEditor> blocksController;


        public SubjectEditor()
        {
            InitializeComponent();


        }

        public Subject GetEntity()
        {
            return entity;
        }

        public void InitEditor(Subject _subject, string? _parentStorageId)
        {
            entity = _subject;
            parentStorageId = _parentStorageId;

            var configTemplate = WeakReferenceFieldConfiguration<SubjectTemplate>.CreateForTextBox(TextTemplate)
                                               .WithStorageId(entity.Template?.StorageId)
                                               .WithPick(ButtonTemplatePick)
                                               .WithFieldDisplayType(EntityFieldDisplayType.title)
                                               .WithPickerTitle("Selecciona una plantilla")
                                               .WithBlocker(Blocker);

            subjectTemplateController = new(configTemplate);

            subjectTemplateController.StorageIdChanged += SubjectTemplateController_StorageIdChanged;

            var configCalendar = WeakReferenceFieldConfiguration<Calendar>.CreateForTextBox(TextCalendar)
                                               .WithStorageId(entity.Calendar?.StorageId)
                                               .WithPick(ButtonCalendarPick)
                                               .WithFieldDisplayType(EntityFieldDisplayType.title)
                                               .WithPickerTitle("Selecciona un calendario")
                                               .WithBlocker(Blocker);

            calendarController = new(configCalendar);

            calendarController.StorageIdChanged += CalendarController_StorageIdChanged;

            var configWeekSchedule = WeakReferenceFieldConfiguration<WeekSchedule>.CreateForTextBox(TextWeekSchedule)
                                               .WithStorageId(entity.WeekSchedule?.StorageId)
                                               .WithPick(ButtonWeekSchedulePick)
                                               .WithFieldDisplayType(EntityFieldDisplayType.title)
                                               .WithPickerTitle("Selecciona un horario")
                                               .WithBlocker(Blocker);

            weekScheduleController = new(configWeekSchedule);

            weekScheduleController.StorageIdChanged += WeekScheduleController_StorageIdChanged;

            var configMetodologiesIntroduction = StrongReferenceFieldConfiguration<CommonText>.CreateForTextBox(TextMetodologyIntroduction)
                                               .WithStorageId(entity.MetodologiesIntroduction.StorageId)
                                               .WithParentStorageId(entity.StorageId)
                                               .WithNew(ButtonMetodologiesIntroductionNew)
                                               .WithEdit(ButtonMetodologiesIntroductionEdit)
                                               .WithTitleEditable(false)
                                               .WithEditorTitle("Introducción a las metodologías")
                                               .WithBlocker(Blocker);

            metodologiesIntroductionController = new(configMetodologiesIntroduction);

            metodologiesIntroductionController.StorageIdChanged += MetodologiesIntroductionController_StorageIdChanged;

            var configMetodologies = StrongReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxMetodologies)
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

            metodologiesController.StorageIdsChanged += MetodologiesController_StorageIdsChanged;

            var configResourcesIntroduction = StrongReferenceFieldConfiguration<CommonText>.CreateForTextBox(TextResourcesIntroduction)
                                               .WithStorageId(entity.ResourcesIntroduction.StorageId)
                                               .WithParentStorageId(entity.StorageId)
                                               .WithNew(ButtonResourcesIntroductionNew)
                                               .WithEdit(ButtonResourcesIntroductionEdit)
                                               .WithTitleEditable(false)
                                               .WithEditorTitle("Introducción a los recursos")
                                               .WithBlocker(Blocker);

            resourcesIntroductionController = new(configResourcesIntroduction);

            resourcesIntroductionController.StorageIdChanged += ResourcesIntroductionController_StorageIdChanged;

            var configSpaceResources = StrongReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxSpaceResources)
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

            spaceResourcesController.StorageIdsChanged += SpaceResourcesController_StorageIdsChanged;

            var configMaterialResources = StrongReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxMaterialResources)
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

            materialResourcesController.StorageIdsChanged += MaterialResourcesController_StorageIdsChanged;

            var configEvaluationInstrumentTypesIntroduction = StrongReferenceFieldConfiguration<CommonText>.CreateForTextBox(TextEvaluationInstrumentTypesIntroduction)
                                               .WithStorageId(entity.EvaluationInstrumentTypesIntroduction.StorageId)
                                               .WithParentStorageId(entity.StorageId)
                                               .WithNew(ButtonEvaluationInstrumentTypesIntroductionNew)
                                               .WithEdit(ButtonEvaluationInstrumentTypesIntroductionEdit)
                                               .WithTitleEditable(false)
                                               .WithEditorTitle("Introducción a los instrumentos de evaluación")
                                               .WithBlocker(Blocker);

            evaluationInstrumentTypesIntroductionController = new(configEvaluationInstrumentTypesIntroduction);

            evaluationInstrumentTypesIntroductionController.StorageIdChanged += EvaluationInstrumentTypesIntroductionController_StorageIdChanged;

            var configEvaluationInstrumentTypes = StrongReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxEvaluationInstrumentTypes)
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

            evaluationInstrumentTypesController.StorageIdsChanged += EvaluationInstrumentTypesController_StorageIdsChanged;

            var configBlocksIntroduction = StrongReferenceFieldConfiguration<CommonText>.CreateForTextBox(TextBlocksIntroduction)
                                               .WithStorageId(entity.BlocksIntroduction.StorageId)
                                               .WithParentStorageId(entity.StorageId)
                                               .WithNew(ButtonBlocksIntroductionNew)
                                               .WithEdit(ButtonBlocksIntroductionEdit)
                                               .WithTitleEditable(false)
                                               .WithEditorTitle("Introducción a los bloques")
                                               .WithBlocker(Blocker);


            blocksIntroductionController = new(configBlocksIntroduction);

            blocksIntroductionController.StorageIdChanged += BlocksIntroductionController_StorageIdChanged;

            var configBlocks = StrongReferencesBoxConfiguration<Block>.CreateForList(ListBoxBlocks)
                                                        .WithParentStorageId(entity.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<Block>(entity.Blocks.ToList()))
                                                        .WithPrefix(EntityBoxItemsPrefix.number)
                                                        .WithNew(ButtonBlockNew)
                                                        .WithEdit(ButtonBlockEdit)
                                                        .WithDelete(ButtonBlockDelete)
                                                        .WithUpDown(ButtonBlockUp, ButtonBlockDown)
                                                        .WithEditorTitle("Bloque")
                                                        .WithBlocker(Blocker);

            blocksController = new(configBlocks);

            blocksController.StorageIdsChanged += BlocksController_StorageIdsChanged;

            TextTitle.Text = entity.Title;

        }

        private void BlocksIntroductionController_StorageIdChanged(StrongReferenceFieldController<CommonText, CommonTextEditor> controller, string storageId)
        {
            UpdateEntity();
        }

        private void EvaluationInstrumentTypesController_StorageIdsChanged(StrongReferencesBoxController<CommonText, CommonTextEditor> controller, List<string> storageIdList)
        {
            UpdateEntity();
        }

        private void EvaluationInstrumentTypesIntroductionController_StorageIdChanged(StrongReferenceFieldController<CommonText, CommonTextEditor> controller, string storageId)
        {
            UpdateEntity();
        }

        private void MaterialResourcesController_StorageIdsChanged(StrongReferencesBoxController<CommonText, CommonTextEditor> controller, List<string> storageIdList)
        {
            UpdateEntity();
        }

        private void SpaceResourcesController_StorageIdsChanged(StrongReferencesBoxController<CommonText, CommonTextEditor> controller, List<string> storageIdList)
        {
            UpdateEntity();
        }

        private void ResourcesIntroductionController_StorageIdChanged(StrongReferenceFieldController<CommonText, CommonTextEditor> controller, string storageId)
        {
            UpdateEntity();
        }

        private void MetodologiesController_StorageIdsChanged(StrongReferencesBoxController<CommonText, CommonTextEditor> controller, List<string> storageIdList)
        {
            UpdateEntity();
        }

        private void MetodologiesIntroductionController_StorageIdChanged(StrongReferenceFieldController<CommonText, CommonTextEditor> controller, string storageId)
        {
            UpdateEntity();
        }

        private void WeekScheduleController_StorageIdChanged(WeakReferenceFieldController<WeekSchedule, EntityPicker<WeekSchedule> > controller, string storageId)
        {
            UpdateEntity();
        }

        private void CalendarController_StorageIdChanged(WeakReferenceFieldController<Calendar, EntityPicker<Calendar>> controller, string storageId)
        {
            UpdateEntity();
        }

        private void SubjectTemplateController_StorageIdChanged(WeakReferenceFieldController<SubjectTemplate, EntityPicker<SubjectTemplate>> controller, string storageId)
        {
            UpdateEntity();
        }

        void BlocksController_StorageIdsChanged(StrongReferencesBoxController<Block, BlockEditor> controller, List<string> storageIdList)
        {
            UpdateEntity();
        }

        private void UpdateEntity()
        {
            entity.Title = TextTitle.Text;

            entity.Template = subjectTemplateController.GetEntity();
            entity.Calendar = calendarController.GetEntity();
            entity.WeekSchedule = weekScheduleController.GetEntity();

            entity.MetodologiesIntroduction = Storage.LoadOrCreateEntity<CommonText>(metodologiesIntroductionController.StorageId, entity.StorageId);
            entity.Metodologies.Set(Storage.LoadEntitiesFromStorageIdList<CommonText>(metodologiesController.StorageIds, entity.StorageId));

            entity.ResourcesIntroduction = Storage.LoadOrCreateEntity<CommonText>(resourcesIntroductionController.StorageId, entity.StorageId);
            entity.SpaceResources.Set(Storage.LoadEntitiesFromStorageIdList<CommonText>(spaceResourcesController.StorageIds, entity.StorageId));
            entity.MaterialResources.Set(Storage.LoadEntitiesFromStorageIdList<CommonText>(materialResourcesController.StorageIds, entity.StorageId));

            entity.EvaluationInstrumentTypesIntroduction = Storage.LoadOrCreateEntity<CommonText>(evaluationInstrumentTypesIntroductionController.StorageId, entity.StorageId);
            entity.EvaluationInstrumentsTypes.Set(Storage.LoadEntitiesFromStorageIdList<CommonText>(evaluationInstrumentTypesController.StorageIds, entity.StorageId));

            entity.BlocksIntroduction = Storage.LoadOrCreateEntity<CommonText>(blocksIntroductionController.StorageId, entity.StorageId);
            entity.Blocks.Set(Storage.LoadEntitiesFromStorageIdList<Block>(blocksController.StorageIds, entity.StorageId));

            entity.Save(parentStorageId);
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            UpdateEntity();

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
