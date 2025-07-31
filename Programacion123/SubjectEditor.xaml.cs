using System;
using System.Collections.Generic;
using System.Data;
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
        StrongReferenceFieldController<CommonText, CommonTextEditor> evaluationIntroductionController;

        StrongReferenceFieldController<CommonText, CommonTextEditor > blocksIntroductionController;
        StrongReferencesBoxController<Block, BlockEditor> blocksController;
        
        DataTable dataTableResultsWeight;
        DataTable dataTableActivitiesWeight;

        DataTable dataTableActivitiesSchedule;


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
            _subject.Save(_parentStorageId);

            entity = _subject;
            parentStorageId = _parentStorageId;

            var configTemplate = WeakReferenceFieldConfiguration<SubjectTemplate>.CreateForTextBox(TextTemplate)
                                               .WithStorageId(entity.Template?.StorageId)
                                               .WithPick(ButtonTemplatePick)
                                               .WithFormat(EntityFormatContent.title)
                                               .WithPickerTitle("Selecciona una plantilla")
                                               .WithBlocker(Blocker);

            subjectTemplateController = new(configTemplate);

            subjectTemplateController.Changed += SubjectTemplateController_Changed;

            var configCalendar = WeakReferenceFieldConfiguration<Calendar>.CreateForTextBox(TextCalendar)
                                               .WithStorageId(entity.Calendar?.StorageId)
                                               .WithPick(ButtonCalendarPick)
                                               .WithFormat(EntityFormatContent.title)
                                               .WithPickerTitle("Selecciona un calendario")
                                               .WithBlocker(Blocker);

            calendarController = new(configCalendar);

            calendarController.Changed += CalendarController_Changed;

            var configWeekSchedule = WeakReferenceFieldConfiguration<WeekSchedule>.CreateForTextBox(TextWeekSchedule)
                                               .WithStorageId(entity.WeekSchedule?.StorageId)
                                               .WithPick(ButtonWeekSchedulePick)
                                               .WithFormat(EntityFormatContent.title)
                                               .WithPickerTitle("Selecciona un horario")
                                               .WithBlocker(Blocker);

            weekScheduleController = new(configWeekSchedule);

            weekScheduleController.Changed += WeekScheduleController_Changed;

            var configMetodologiesIntroduction = StrongReferenceFieldConfiguration<CommonText>.CreateForTextBox(TextMetodologyIntroduction)
                                               .WithStorageId(entity.MetodologiesIntroduction.StorageId)
                                               .WithParentStorageId(entity.StorageId)
                                               .WithFormat(EntityFormatContent.description)
                                               .WithNew(ButtonMetodologiesIntroductionNew)
                                               .WithEdit(ButtonMetodologiesIntroductionEdit)
                                               .WithReplaceConfirmQuestion("Esto sustituirá la introducción anterior por una nueva. ¿Estás seguro/a?")
                                               .WithTitleEditable(false)
                                               .WithEditorTitle("Introducción a las metodologías")
                                               .WithBlocker(Blocker);

            metodologiesIntroductionController = new(configMetodologiesIntroduction);

            metodologiesIntroductionController.Changed += MetodologiesIntroductionController_Changed;

            var configMetodologies = StrongReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxMetodologies)
                                                        .WithParentStorageId(entity.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(entity.Metodologies.ToList()))
                                                        .WithFormat(EntityFormatContent.title, EntityFormatIndex.number)
                                                        .WithNew(ButtonMetodologyNew)
                                                        .WithEdit(ButtonMetodologyEdit)
                                                        .WithDelete(ButtonMetodologyDelete)
                                                        .WithUpDown(ButtonMetodologyUp, ButtonMetodologyDown)
                                                        .WithDeleteConfirmQuestion("Esto eliminará permanentemente la metodología seleccionada. ¿Estás seguro/a?")
                                                        .WithEditorTitle("Metodología")
                                                        .WithBlocker(Blocker);

            metodologiesController = new(configMetodologies);

            metodologiesController.Changed += MetodologiesController_Changed;

            var configResourcesIntroduction = StrongReferenceFieldConfiguration<CommonText>.CreateForTextBox(TextResourcesIntroduction)
                                               .WithStorageId(entity.ResourcesIntroduction.StorageId)
                                               .WithParentStorageId(entity.StorageId)
                                               .WithFormat(EntityFormatContent.description)
                                               .WithNew(ButtonResourcesIntroductionNew)
                                               .WithEdit(ButtonResourcesIntroductionEdit)
                                               .WithReplaceConfirmQuestion("Esto sustituirá la introducción anterior por una nueva. ¿Estás seguro/a?")
                                               .WithTitleEditable(false)
                                               .WithEditorTitle("Introducción a los recursos")
                                               .WithBlocker(Blocker);

            resourcesIntroductionController = new(configResourcesIntroduction);

            resourcesIntroductionController.Changed += ResourcesIntroductionController_Changed;

            var configSpaceResources = StrongReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxSpaceResources)
                                                        .WithParentStorageId(entity.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(entity.SpaceResources.ToList()))
                                                        .WithFormat(EntityFormatContent.title)
                                                        .WithNew(ButtonSpaceResourceNew)
                                                        .WithEdit(ButtonSpaceResourceEdit)
                                                        .WithDelete(ButtonSpaceResourceDelete)
                                                        .WithUpDown(ButtonSpaceResourceUp, ButtonSpaceResourceDown)
                                                        .WithDeleteConfirmQuestion("Esto eliminará permanentemente el espacio seleccionado. ¿Estás seguro/a?")
                                                        .WithEditorTitle("Espacio")
                                                        .WithBlocker(Blocker);

            spaceResourcesController = new(configSpaceResources);

            spaceResourcesController.Changed += SpaceResourcesController_Changed;

            var configMaterialResources = StrongReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxMaterialResources)
                                                        .WithParentStorageId(entity.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(entity.MaterialResources.ToList()))
                                                        .WithFormat(EntityFormatContent.title)
                                                        .WithNew(ButtonMaterialResourceNew)
                                                        .WithEdit(ButtonMaterialResourceEdit)
                                                        .WithDelete(ButtonMaterialResourceDelete)
                                                        .WithUpDown(ButtonMaterialResourceUp, ButtonMaterialResourceDown)
                                                        .WithDeleteConfirmQuestion("Esto eliminará permanentemente el material seleccionado. ¿Estás seguro/a?")
                                                        .WithEditorTitle("Material")
                                                        .WithBlocker(Blocker);

            materialResourcesController = new(configMaterialResources);

            materialResourcesController.Changed += MaterialResourcesController_Changed;

            var configEvaluationInstrumentTypesIntroduction = StrongReferenceFieldConfiguration<CommonText>.CreateForTextBox(TextEvaluationInstrumentTypesIntroduction)
                                               .WithStorageId(entity.EvaluationInstrumentTypesIntroduction.StorageId)
                                               .WithParentStorageId(entity.StorageId)
                                               .WithFormat(EntityFormatContent.description)
                                               .WithNew(ButtonEvaluationInstrumentTypesIntroductionNew)
                                               .WithEdit(ButtonEvaluationInstrumentTypesIntroductionEdit)
                                               .WithReplaceConfirmQuestion("Esto sustituirá la introducción anterior por una nueva. ¿Estás seguro/a?")
                                               .WithTitleEditable(false)
                                               .WithEditorTitle("Introducción a los instrumentos de evaluación")
                                               .WithBlocker(Blocker);

            evaluationInstrumentTypesIntroductionController = new(configEvaluationInstrumentTypesIntroduction);

            evaluationInstrumentTypesIntroductionController.Changed += EvaluationInstrumentTypesIntroductionController_Changed;

            var configEvaluationInstrumentTypes = StrongReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxEvaluationInstrumentTypes)
                                                        .WithParentStorageId(entity.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(entity.EvaluationInstrumentsTypes.ToList()))
                                                        .WithFormat(EntityFormatContent.title)
                                                        .WithNew(ButtonEvaluationInstrumentTypeNew)
                                                        .WithEdit(ButtonEvaluationInstrumentTypeEdit)
                                                        .WithDelete(ButtonEvaluationInstrumentTypeDelete)
                                                        .WithUpDown(ButtonEvaluationInstrumentTypeUp, ButtonEvaluationInstrumentTypeDown)
                                                        .WithDeleteConfirmQuestion("Esto eliminará el tipo de instrumento de evaluación seleccionado. ¿Estás seguro/a?")
                                                        .WithEditorTitle("Tipo de instrumento de evaluación")
                                                        .WithBlocker(Blocker);

            evaluationInstrumentTypesController = new(configEvaluationInstrumentTypes);

            evaluationInstrumentTypesController.Changed += EvaluationInstrumentTypesController_Changed;

            var configBlocksIntroduction = StrongReferenceFieldConfiguration<CommonText>.CreateForTextBox(TextBlocksIntroduction)
                                               .WithStorageId(entity.BlocksIntroduction.StorageId)
                                               .WithParentStorageId(entity.StorageId)
                                               .WithFormat(EntityFormatContent.description)
                                               .WithNew(ButtonBlocksIntroductionNew)
                                               .WithEdit(ButtonBlocksIntroductionEdit)
                                               .WithReplaceConfirmQuestion("Esto sustituirá la introducción anterior por una nueva. ¿Estás seguro/a?")
                                               .WithTitleEditable(false)
                                               .WithEditorTitle("Introducción a los bloques")
                                               .WithBlocker(Blocker);


            blocksIntroductionController = new(configBlocksIntroduction);

            blocksIntroductionController.Changed += BlocksIntroductionController_Changed;

            var configBlocks = StrongReferencesBoxConfiguration<Block>.CreateForList(ListBoxBlocks)
                                                        .WithParentStorageId(entity.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<Block>(entity.Blocks.ToList()))
                                                        .WithFormat(EntityFormatContent.title, EntityFormatIndex.number)
                                                        .WithNew(ButtonBlockNew)
                                                        .WithEdit(ButtonBlockEdit)
                                                        .WithDelete(ButtonBlockDelete)
                                                        .WithUpDown(ButtonBlockUp, ButtonBlockDown)
                                                        .WithDeleteConfirmQuestion("Esto eliminará permanentemente el bloque seleccionado junto con todas las actividades definidas en él. ¿Estás seguro/a?")
                                                        .WithEditorTitle("Bloque")
                                                        .WithBlocker(Blocker);

            blocksController = new(configBlocks);

            blocksController.Changed += BlocksController_Changed;

            var configEvaluationIntroduction = StrongReferenceFieldConfiguration<CommonText>.CreateForTextBox(TextEvaluationIntroduction)
                                               .WithStorageId(entity.EvaluationIntroduction.StorageId)
                                               .WithParentStorageId(entity.StorageId)
                                               .WithFormat(EntityFormatContent.description)
                                               .WithNew(ButtonEvaluationIntroductionNew)
                                               .WithEdit(ButtonEvaluationIntroductionEdit)
                                               .WithReplaceConfirmQuestion("Esto sustituirá la introducción anterior por una nueva. ¿Estás seguro/a?")
                                               .WithTitleEditable(false)
                                               .WithEditorTitle("Introducción a la evaluación")
                                               .WithBlocker(Blocker);

            evaluationIntroductionController = new(configEvaluationIntroduction);

            evaluationIntroductionController.Changed += EvaluationIntroductionController_Changed;

            TextTitle.Text = entity.Title;

            TextTitle.TextChanged += TextTitle_TextChanged;

            dataTableResultsWeight = new DataTable();
            
            DataGridLearningResultsWeight.ItemsSource = dataTableResultsWeight.DefaultView;
            DataGridLearningResultsWeight.CanUserAddRows = false;
            DataGridLearningResultsWeight.CanUserDeleteRows = false;
            DataGridLearningResultsWeight.CanUserReorderColumns = false;
            DataGridLearningResultsWeight.CanUserSortColumns = false;
            DataGridLearningResultsWeight.CanUserResizeColumns = false;
            DataGridLearningResultsWeight.CanUserResizeRows = false;

            dataTableActivitiesWeight = new DataTable();

            DataGridActivitiesWeight.ItemsSource = dataTableActivitiesWeight.DefaultView;
            DataGridActivitiesWeight.CanUserAddRows = false;
            DataGridActivitiesWeight.CanUserDeleteRows = false;
            DataGridActivitiesWeight.CanUserReorderColumns = false;
            DataGridActivitiesWeight.CanUserSortColumns = false;
            DataGridActivitiesWeight.CanUserResizeColumns = false;
            DataGridActivitiesWeight.CanUserResizeRows = false;

            dataTableActivitiesSchedule = new DataTable();

            DataGridActivitiesSchedule.ItemsSource = dataTableActivitiesSchedule.DefaultView;
            DataGridActivitiesSchedule.FrozenColumnCount = 1;
            DataGridActivitiesSchedule.CanUserAddRows = false;
            DataGridActivitiesSchedule.CanUserDeleteRows = false;
            DataGridActivitiesSchedule.CanUserReorderColumns = false;
            DataGridActivitiesSchedule.CanUserSortColumns = false;
            DataGridActivitiesSchedule.CanUserResizeColumns = false;
            DataGridActivitiesSchedule.CanUserResizeRows = false;

            UpdateEntityTemplateReferences();

            dataTableResultsWeight.RowChanged += DataTableResultsWeight_RowChanged;
            dataTableActivitiesWeight.RowChanged += DataTableActivitiesWeight_RowChanged;
            dataTableActivitiesSchedule.RowChanged += DataTableActivitiesSchedule_RowChanged;

            UpdateActivityWeightsUIFromEntity();
            UpdateWeightsUIFromEntity();
            UpdateScheduleUIFromEntity();


            Validate();

        }


        private void TextTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateEntity();
            Validate();
        }

        private void EvaluationIntroductionController_Changed(StrongReferenceFieldController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity();
            Validate();
        }

        private void DataTableActivitiesWeight_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            UpdateEntity();
            Validate();
        }

        private void DataTableResultsWeight_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            UpdateEntity();
            Validate();
        }

        private void DataTableActivitiesSchedule_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            UpdateEntity();
            UpdateScheduleUIFromEntity(true);
            Validate();
        }


        void UpdateWeightsUIFromEntity()
        {
            dataTableResultsWeight.Clear();
            dataTableResultsWeight.Rows.Clear();
            dataTableResultsWeight.Columns.Clear();

            if(entity.Template != null)
            {
                List< KeyValuePair<LearningResult, float> > resultsWeightsList = entity.LearningResultsWeights.ToList();
                List<LearningResult> resultsList = entity.Template.LearningResults.ToList();
                int resultsCount = resultsList.Count;
                for(int i = 0; i < resultsCount; i++)
                { dataTableResultsWeight.Columns.Add(String.Format("RA{0}", i + 1), typeof(float)); }

                DataRow row = dataTableResultsWeight.NewRow();
                for(int i = 0; i < resultsCount; i++)
                {
                    int weightIndex;
                    float weight;                    
                    weightIndex = resultsWeightsList.FindIndex(r => r.Key.StorageId == resultsList[i].StorageId);
                    if(weightIndex >= 0) { weight = resultsWeightsList[weightIndex].Value; }
                    else { weight = 0; }

                    row[String.Format("RA{0}", i + 1)] = weight;
                }

                dataTableResultsWeight.RowChanged -= DataTableResultsWeight_RowChanged;
                dataTableResultsWeight.Rows.Add(row);
                dataTableResultsWeight.RowChanged += DataTableResultsWeight_RowChanged;
            }

            DataGridLearningResultsWeight.ItemsSource = null;
            DataGridLearningResultsWeight.ItemsSource = dataTableResultsWeight.DefaultView;
        }

        void UpdateActivityWeightsUIFromEntity()
        {
            dataTableActivitiesWeight.Clear();
            dataTableActivitiesWeight.Rows.Clear();
            dataTableActivitiesWeight.Columns.Clear();

            if(entity.Template != null)
            {
                dataTableActivitiesWeight.Columns.Add("Actividad", typeof(string));

                List<LearningResult> results = entity.Template.LearningResults.ToList();
                for(int i = 0; i < results.Count; i++)
                { dataTableActivitiesWeight.Columns.Add(String.Format("RA{0}", i + 1), typeof(float)); }

                for(int b = 0; b < entity.Blocks.Count; b++)
                {
                    Block block = entity.Blocks[b];

                    int evaluableActivityIndex = 0;
    
                    for(int a = 0; a < block.Activities.Count; a++)
                    {
                        Activity activity = block.Activities[a];
                        //activity = Storage.LoadOrCreateEntity<Activity>(activity.StorageId, block.StorageId);

                        if(activity.IsEvaluable)
                        {
                            DataRow row = dataTableActivitiesWeight.NewRow();
                            row["Actividad"] = String.Format("B{0:00}-A{1:00}", b + 1, evaluableActivityIndex + 1);

                            List<KeyValuePair<LearningResult, float>> resultsWeightsList = activity.LearningResultsWeights.ToList();

                            for(int i = 0; i < results.Count; i++)
                            { 
                                int weightIndex = resultsWeightsList.FindIndex(r => r.Key.StorageId == results[i].StorageId);
                                float weight = weightIndex >= 0 ? resultsWeightsList[weightIndex].Value : 0;
                                row[String.Format("RA{0}", i + 1)] = weight;
                            }

                            dataTableActivitiesWeight.RowChanged -= DataTableActivitiesWeight_RowChanged;
                            dataTableActivitiesWeight.Rows.Add(row);
                            dataTableActivitiesWeight.RowChanged += DataTableActivitiesWeight_RowChanged;

                            evaluableActivityIndex ++;
                        }
                    }
                }
            }            

            DataGridActivitiesWeight.ItemsSource = null;
            DataGridActivitiesWeight.ItemsSource = dataTableActivitiesWeight.DefaultView;
        }

        void UpdateScheduleUIFromEntity(bool dontChangeItemSource = false)
        {
            dataTableActivitiesSchedule.Clear();
            dataTableActivitiesSchedule.Rows.Clear();
            dataTableActivitiesSchedule.Columns.Clear();

            List<Activity> activities = new();

            List<ActivitySchedule>? scheduledActivities = null;

            if (entity.CanScheduleActivities())
            {
                scheduledActivities = entity.ScheduleActivities();
            }

            Dictionary<string, int> activityStorageIdToBlockIndex = new();
            Dictionary<string, int> activityStorageIdToActivityIndex = new();

            int bIndex = 0;
            int aIndex = 0;
            entity.Blocks.ToList().ForEach(
            b =>
            {
                b.Activities.ToList().ForEach(
                a => 
                {
                    if(a.IsEvaluable)
                    {
                        activityStorageIdToBlockIndex.Add(a.StorageId, bIndex);
                        activityStorageIdToActivityIndex.Add(a.StorageId, aIndex);
                        aIndex++;
                    }

                    activities.Add(a);
                });
                aIndex = 0;
                bIndex++;
            });

            DataColumn column = new DataColumn("Actividad", typeof(string));
            column.ReadOnly = true;
            dataTableActivitiesSchedule.Columns.Add(column);

            column = new DataColumn("Inicio", typeof(string));
            column.ReadOnly = true;
            dataTableActivitiesSchedule.Columns.Add(column);

            column = new DataColumn("Fin", typeof(string));
            column.ReadOnly = true;
            dataTableActivitiesSchedule.Columns.Add(column);

            column = new DataColumn("Horas", typeof(float));
            dataTableActivitiesSchedule.Columns.Add(column);

            column = new DataColumn("Sesiones", typeof(float));
            column.ReadOnly = true;
            dataTableActivitiesSchedule.Columns.Add(column);

            int activityIndex = 0;

            foreach (Activity a in activities)
            {
                DataRow row = dataTableActivitiesSchedule.NewRow();

                row["Actividad"] =  a.IsEvaluable ?
                                        String.Format("B{0:00}-A{1:00}",
                                        activityStorageIdToBlockIndex[a.StorageId] + 1,
                                        activityStorageIdToActivityIndex[a.StorageId] + 1) :
                                        a.Title.Substring(0, Math.Min(a.Title.Length, 20)) + 
                                        (a.Title.Length > 20 ? "..." : "");

                row["Horas"] = a.Duration;

                ActivitySchedule? schedule = null;
                if (scheduledActivities != null)
                {
                    if (activityIndex < scheduledActivities.Count) { schedule = scheduledActivities[activityIndex]; }
                }

                if(schedule.HasValue)
                {
                    row["Inicio"] = Utils.FormatStartDayHour(schedule.Value.start.day, schedule.Value.start.hour, entity.WeekSchedule);
                    row["Fin"] = Utils.FormatEndDayHour(schedule.Value.end.day, schedule.Value.end.hour, entity.WeekSchedule);

                    float count = 0;
                    for (DateTime d = schedule.Value.start.day; d <= schedule.Value.end.day; d = d.AddDays(1))
                    {
                        if (Utils.IsSchoolDay(d, entity.Calendar, entity.WeekSchedule)) { count++; }
                    }

                    row["Sesiones"] = count;
                }
                else
                {
                    row["Inicio"] = "<no planificable>";
                    row["Fin"] = "<no planificable>";
                    row["Sesiones"] = 0;
                }

                dataTableActivitiesSchedule.RowChanged -= DataTableActivitiesSchedule_RowChanged;
                dataTableActivitiesSchedule.Rows.Add(row);
                dataTableActivitiesSchedule.RowChanged += DataTableActivitiesSchedule_RowChanged;

                activityIndex ++;
            }

            // Fixes Index out of range exception in ShowDialog, that maybe occurs because the ItemSource reset is done two times in the same event
            if(!dontChangeItemSource)
            {
                DataGridActivitiesSchedule.ItemsSource = null;
                DataGridActivitiesSchedule.ItemsSource = dataTableActivitiesSchedule.DefaultView;
            }

        }

        private void BlocksIntroductionController_Changed(StrongReferenceFieldController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity();
            Validate();
        }

        private void EvaluationInstrumentTypesController_Changed(StrongReferencesBoxController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity();
            Validate();
        }

        private void EvaluationInstrumentTypesIntroductionController_Changed(StrongReferenceFieldController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity();
            Validate();
        }

        private void MaterialResourcesController_Changed(StrongReferencesBoxController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity();
            Validate();

        }

        private void SpaceResourcesController_Changed(StrongReferencesBoxController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity();
            Validate();
        }

        private void ResourcesIntroductionController_Changed(StrongReferenceFieldController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity();
            Validate();
        }

        private void MetodologiesController_Changed(StrongReferencesBoxController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity();
            Validate();
        }

        private void MetodologiesIntroductionController_Changed(StrongReferenceFieldController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity();
            Validate();
        }

        private void WeekScheduleController_Changed(WeakReferenceFieldController<WeekSchedule, EntityPicker<WeekSchedule> > controller)
        {
            UpdateEntity();
            UpdateScheduleUIFromEntity();
            Validate();
        }

        private void CalendarController_Changed(WeakReferenceFieldController<Calendar, EntityPicker<Calendar>> controller)
        {
            UpdateEntity();
            UpdateScheduleUIFromEntity();
            Validate();
        }

        private void SubjectTemplateController_Changed(WeakReferenceFieldController<SubjectTemplate, EntityPicker<SubjectTemplate>> controller)
        {
            UpdateEntity();
            Validate();
            UpdateWeightsUIFromEntity();
            UpdateActivityWeightsUIFromEntity();
        }

        void BlocksController_Changed(StrongReferencesBoxController<Block, BlockEditor> controller)
        {
            // Reload blocks because activities may have changed
            entity.Blocks.Set(Storage.LoadOrCreateEntities<Block>(blocksController.StorageIds, entity.StorageId));

            UpdateActivityWeightsUIFromEntity();
            UpdateScheduleUIFromEntity();
            UpdateEntity();
            Validate();

        }

        private void UpdateEntity()
        {            
            entity.Title = TextTitle.Text;

            entity.Template = subjectTemplateController.GetEntity();
            entity.Calendar = calendarController.GetEntity();
            entity.WeekSchedule = weekScheduleController.GetEntity();

            entity.MetodologiesIntroduction = Storage.LoadOrCreateEntity<CommonText>(metodologiesIntroductionController.StorageId, entity.StorageId);
            entity.Metodologies.Set(Storage.LoadOrCreateEntities<CommonText>(metodologiesController.StorageIds, entity.StorageId));

            entity.ResourcesIntroduction = Storage.LoadOrCreateEntity<CommonText>(resourcesIntroductionController.StorageId, entity.StorageId);
            entity.SpaceResources.Set(Storage.LoadOrCreateEntities<CommonText>(spaceResourcesController.StorageIds, entity.StorageId));
            entity.MaterialResources.Set(Storage.LoadOrCreateEntities<CommonText>(materialResourcesController.StorageIds, entity.StorageId));

            entity.EvaluationInstrumentTypesIntroduction = Storage.LoadOrCreateEntity<CommonText>(evaluationInstrumentTypesIntroductionController.StorageId, entity.StorageId);
            entity.EvaluationInstrumentsTypes.Set(Storage.LoadOrCreateEntities<CommonText>(evaluationInstrumentTypesController.StorageIds, entity.StorageId));

            entity.BlocksIntroduction = Storage.LoadOrCreateEntity<CommonText>(blocksIntroductionController.StorageId, entity.StorageId);
            entity.Blocks.Set(Storage.LoadOrCreateEntities<Block>(blocksController.StorageIds, entity.StorageId));

            entity.EvaluationIntroduction = Storage.LoadOrCreateEntity<CommonText>(evaluationIntroductionController.StorageId, entity.StorageId);

            entity.LearningResultsWeights.Clear();
            if(entity.Template != null)
            {
                int columnIndex = 0;
                int count = Math.Min(dataTableResultsWeight.Columns.Count, entity.Template.LearningResults.Count);
                for(int i = 0; i < count; i++)
                {
                    DataColumn c = dataTableResultsWeight.Columns[i];
                    LearningResult r = entity.Template.LearningResults[columnIndex];
                    entity.LearningResultsWeights.Add(r, (float)dataTableResultsWeight.Rows[0][c.ColumnName]);
                    columnIndex++;
                }
            }
            else
            {
                entity.LearningResultsWeights.Clear();
            }

            int evaluableActivityIndex = 0;
            List<Block> blocksList = entity.Blocks.ToList();
            foreach(Block b in blocksList)
            {
                List<Activity> activitiesList = b.Activities.ToList();
                foreach(Activity a in activitiesList)
                {
                    if(a.IsEvaluable)
                    {
                        if(entity.Template != null)
                        {
                            a.LearningResultsWeights.Clear();
                            List<LearningResult> resultList = entity.Template.LearningResults.ToList();
                            int columnCount = Math.Min(dataTableActivitiesWeight.Columns.Count - 1, resultList.Count);
                            for(int i = 0; i < columnCount; i++)
                            {
                                string columnName = dataTableActivitiesWeight.Columns[i + 1].ColumnName;
                                float weight;
                                if(dataTableActivitiesWeight.Rows.Count > evaluableActivityIndex)
                                { weight = (float)dataTableActivitiesWeight.Rows[evaluableActivityIndex][columnName]; }
                                else { weight = 0; }

                                a.LearningResultsWeights.Add(resultList[i], weight);
                            }
                        }
                        else
                        {
                            a.LearningResultsWeights.Clear();
                        }

                        evaluableActivityIndex++;
                    }
                    else
                    {
                        a.LearningResultsWeights.Clear();
                    }


                }
            }

            int activityScheduleIndex = 0;

            foreach (Block b in blocksList)
            {
                List<Activity> activitiesList = b.Activities.ToList();
                foreach (Activity a in activitiesList)
                {
                    if (activityScheduleIndex < dataTableActivitiesSchedule.Rows.Count)
                    {
                        DataRow row = dataTableActivitiesSchedule.Rows[activityScheduleIndex];

                        float h = (int)((float)row["Horas"] / 0.25f) *0.25f;
                        if (h <= 0) { h = 0.25f; }

                        a.Duration = h;
                    }

                    activityScheduleIndex++;
                }
            }

            UpdateEntityTemplateReferences();

            // Not needed as UpdateEntityTemplateReferences already does that
            //entity.Save(parentStorageId);
        }

        void UpdateEntityTemplateReferences()
        {
            if(entity.Template == null)
            {
                entity.LearningResultsWeights.Clear();

                List<Block> blocksList = entity.Blocks.ToList();

                foreach(Block b in blocksList)
                {
                    List<Activity> activitiesList = b.Activities.ToList();

                    foreach(Activity a in activitiesList)
                    {
                        a.LearningResultsWeights.Clear();
                    }
                }
            }
            else
            {
                List<KeyValuePair<LearningResult, float>> previousWeights = entity.LearningResultsWeights.ToList();

                List<LearningResult> resultsList = entity.Template.LearningResults.ToList();
                entity.LearningResultsWeights.Clear();
                foreach(LearningResult r in resultsList)
                {
                    int weightIndex = previousWeights.FindIndex(p => p.Key.StorageId == r.StorageId);
                    float weight = weightIndex >= 0 ? previousWeights[weightIndex].Value : 0;
                    entity.LearningResultsWeights.Add(r, weight);
                }

                List<Block> blocksList = entity.Blocks.ToList();

                foreach(Block b in blocksList)
                {
                    List<Activity> activitiesList = b.Activities.ToList();

                    foreach(Activity a in activitiesList)
                    {
                        List<KeyValuePair<LearningResult, float>> previousActivityWeights = a.LearningResultsWeights.ToList();
                        a.LearningResultsWeights.Clear();
                        foreach(LearningResult r in resultsList)
                        {
                            int weightIndex = previousActivityWeights.FindIndex(p => p.Key.StorageId == r.StorageId);
                            float weight = weightIndex >= 0 ? previousActivityWeights[weightIndex].Value : 0;
                            a.LearningResultsWeights.Add(r, weight);
                        }

                    }
                }
            }

            entity.Save(parentStorageId);
        }

        void Validate()
        {
            ValidationResult validation = entity.Validate();

            string colorResource = (validation.code == ValidationCode.success ? "ColorValid" : "ColorInvalid");
            BorderValidation.Background = new SolidColorBrush((Color)Application.Current.Resources[colorResource]);
            TextValidation.Text = validation.ToString();

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
