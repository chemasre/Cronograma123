using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Programacion123
{
    /// <summary>
    /// Lógica de interacción para ActivityEditor.xaml
    /// </summary>
    public partial class ActivityEditor : Window, IEntityEditor<Activity>
    {
        Activity entity;
        string? parentStorageId;

        WeakReferenceFieldController<CommonText, EntityPicker<CommonText> > metodologyController;
        WeakReferencesBoxController<CommonText, EntityPicker<CommonText> > contentPointsController;
        WeakReferencesBoxController<CommonText, EntityPicker<CommonText> > keyCompetencesController;
        WeakReferencesBoxController<CommonText, EntityPicker<CommonText> > spaceResourcesController;
        WeakReferencesBoxController<CommonText, EntityPicker<CommonText> > materialResourcesController;
        WeakReferenceFieldController<CommonText, EntityPicker<CommonText> > evaluationInstrumentController;
        WeakReferencesBoxController<CommonText, EntityPicker<CommonText> > criteriasController;


        string subjectStorageId;
        string blockStorageId;
        Subject? subject;
        Block? block;

        DataTable dataTableResultsWeight;

        public ActivityEditor()
        {
            InitializeComponent();
        }

        void ButtonClose_Click(object sender, RoutedEventArgs e)
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

        void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        public void UpdateEntity()
        {
            entity.Title = TextTitle.Text;
            entity.Description = TextBoxDescription.Text;

            entity.StartType = (ActivityStartType)ComboStartType.SelectedIndex;
            entity.StartDate = DateStartDate.SelectedDate.Value;
            entity.StartDayOfWeek = (DayOfWeek)(ComboStartWeekDay.SelectedIndex + 1);

            entity.Duration = ComboDuration.SelectedIndex + ComboDurationFraction.SelectedIndex * 0.25f;

            entity.NoActivitiesBefore = CheckboxNoActivitiesBefore.IsChecked.GetValueOrDefault();
            entity.NoActivitiesAfter = CheckboxNoActivitiesAfter.IsChecked.GetValueOrDefault();


            entity.Metodology = metodologyController.GetEntity();

            entity.ContentPoints.Set(contentPointsController.GetSelectedEntities());
            entity.KeyCompetences.Set(keyCompetencesController.GetSelectedEntities());

            entity.SpaceResources.Set(spaceResourcesController.GetSelectedEntities());
            entity.MaterialResources.Set(materialResourcesController.GetSelectedEntities());

            entity.IsEvaluable = CheckboxIsEvaluable.IsChecked.GetValueOrDefault();

            entity.EvaluationInstrumentType = evaluationInstrumentController.GetEntity();
            entity.Criterias.Set(criteriasController.GetSelectedEntities());
            entity.LearningResultsWeights.Clear();

            if (subject.Template != null)
            {
                int resultIndex = 0;
                List<LearningResult> results = subject.Template.LearningResults.ToList();
                foreach (DataColumn c in dataTableResultsWeight.Columns)
                {
                    entity.LearningResultsWeights.Add(results[resultIndex], (float)dataTableResultsWeight.Rows[0][c.ColumnName]);
                    resultIndex++;
                }
            }

            entity.Save(parentStorageId);

            subject = new Subject();
            subject.LoadOrCreate(subjectStorageId);
            block = new Block();
            block.LoadOrCreate(blockStorageId, subjectStorageId);

        }

        void Validate()
        {
            ValidationResult validation = entity.Validate();

            string colorResource = (validation.code == ValidationCode.success ? "ColorValid" : "ColorInvalid");
            BorderValidation.Background = new SolidColorBrush((Color)Application.Current.Resources[colorResource]);
            TextValidation.Text = validation.ToString();

        }


        public void InitEditor(Activity _entity, string? _parentStorageId)
        {
            _entity.Save(_parentStorageId);

            entity = _entity;

            parentStorageId = _parentStorageId;

            block = new();

            blockStorageId = _parentStorageId;
            subjectStorageId = Storage.FindParentStorageId(blockStorageId, block.StorageClassId);

            subject = new Subject();
            subject.LoadOrCreate(subjectStorageId);
            block.LoadOrCreate(blockStorageId, subjectStorageId);

            TextBlock.Text = block.Title;
            TextBlock.Background = new SolidColorBrush((Color)Application.Current.Resources["ColorLocked"]);

            var configMetodology = WeakReferenceFieldConfiguration<CommonText>.CreateForTextBox(TextMetodology)
                                               .WithStorageId(entity.Metodology?.StorageId)
                                               .WithParentStorageId(subject.StorageId)
                                               .WithPick(ButtonMetodologyPick)
                                               .WithPickerTitle("Elige una metodología")
                                               .WithFormat(EntityFormatContent.Title)
                                               .WithPickListQuery(() => Storage.GetStorageIds<CommonText>(subject.Metodologies.ToList()) )
                                               .WithBlocker(Blocker);

            metodologyController = new(configMetodology);

            Func<List<string>> pickContentPointsQuery =
                () =>
                {
                    List<string> contentPoints = new();
                    if(subject.Template != null)
                    {
                        List<Content> contentList = subject.Template.Contents.ToList();

                        foreach(Content c in contentList)
                        {
                            List<CommonText> pointsList = c.Points.ToList();

                            foreach(CommonText p in pointsList)
                            {
                                contentPoints.Add(p.StorageId);
                            }
                        }
                    }

                    return contentPoints;
                };

            Func<CommonText, int, string> contentsFormatter =
                (e, i) =>
                {
                    bool canFormat;                    
                    SubjectTemplate? template = null;
                    List<Content>? contents = null;
                    string? contentStorageId = null;
                    int pointIndex = -1;
                    int contentIndex = -1;

                    canFormat = (subject.Template != null);
                    if(canFormat)
                    {
                        template = subject.Template;
                    }
                    if(canFormat)
                    {   contents = template.Contents.ToList();
                        contentStorageId = Storage.FindParentStorageId(e.StorageId, e.StorageClassId);
                        canFormat = (contentStorageId != null);
                    }
                    if(canFormat)
                    {   contentIndex = contents.FindIndex(c => c.StorageId == contentStorageId);
                        canFormat = (contentIndex >= 0);
                    }
                    if(canFormat)
                    {   pointIndex = contents[contentIndex].Points.ToList().FindIndex(p => p.StorageId == e.StorageId);
                        canFormat = (pointIndex >= 0);
                    }
                    
                    if(canFormat)
                    {
                        return String.Format("{0}.{1}: {2}", contentIndex + 1, pointIndex + 1, e.Description);
                    }
                    else
                    {
                        return "<no se encuentra la referencia>";    
                    }
                };

            Func<List<string>> pickKeyCompetenceQuery =
                () =>
                {
                    List<string> keyCompetences = new();
                    if(subject.Template != null)
                    {
                        if(subject.Template.GradeTemplate != null)
                        {
                            List<CommonText> competencesList = subject.Template.GradeTemplate.KeyCapacities.ToList();

                            foreach(CommonText c in competencesList)
                            {
                                keyCompetences.Add(c.StorageId);
                            }
                        }
                    }

                    return keyCompetences;
                };

            Func<CommonText, int, string> keyCompetenceFormatter =
                (c, i) =>
                {
                    bool canFormat;                    
                    SubjectTemplate? template = null;
                    GradeTemplate? gradeTemplate = null;
                    int capacityIndex = -1;

                    canFormat = (subject.Template != null);
                    if(canFormat)
                    {
                        template = subject.Template;
                        canFormat = subject.Template.GradeTemplate != null;
                    }
                    if(canFormat)
                    {
                        gradeTemplate = template.GradeTemplate;
                        canFormat = (gradeTemplate != null);
                    }
                    if(canFormat)
                    {   
                        capacityIndex = gradeTemplate.KeyCapacities.ToList().FindIndex(k => k.StorageId == c.StorageId);
                        canFormat = (capacityIndex >= 0);
                    }
                    
                    if(canFormat)
                    {
                        return String.Format("{0}: {1}", capacityIndex + 1, c.Title);
                    }
                    else
                    {
                        return "<no se encuentra la referencia>";    
                    }
                };

            var configContents = WeakReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxContents)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_entity.ContentPoints.ToList()))
                                                        .WithFormatter(contentsFormatter)
                                                        .WithPick(ButtonContentReferenceAdd, ButtonContentReferenceRemove)
                                                        .WithPickListQuery(pickContentPointsQuery)
                                                        .WithPickerTitle("Puntos de contenido")
                                                        .WithBlocker(Blocker);

            contentPointsController = new(configContents);

            var configKeyCompetences = WeakReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxKeyCompetences)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_entity.KeyCompetences.ToList()))
                                                        .WithFormatter(keyCompetenceFormatter)
                                                        .WithPick(ButtonKeyCompetenceReferenceAdd, ButtonKeyCompetenceReferenceRemove)
                                                        .WithPickListQuery(pickKeyCompetenceQuery)
                                                        .WithPickerTitle("Competencias clave")
                                                        .WithBlocker(Blocker);

            keyCompetencesController = new(configKeyCompetences);

            var configSpaceResources = WeakReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxSpaceResources)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_entity.SpaceResources.ToList()))
                                                        .WithFormat(EntityFormatContent.Title)
                                                        .WithPick(ButtonSpaceResourceReferenceAdd, ButtonSpaceResourceReferenceRemove)
                                                        .WithPickListQuery(() => Storage.GetStorageIds<CommonText>(subject.SpaceResources.ToList()))
                                                        .WithPickerTitle("Espacios")
                                                        .WithBlocker(Blocker);

            spaceResourcesController = new(configSpaceResources);

            var configMaterialResources = WeakReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxMaterialResources)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_entity.MaterialResources.ToList()))
                                                        .WithFormat(EntityFormatContent.Title)
                                                        .WithPick(ButtonMaterialResourceReferenceAdd, ButtonMaterialResourceReferenceRemove)
                                                        .WithPickListQuery(() => Storage.GetStorageIds<CommonText>(subject.MaterialResources.ToList()))
                                                        .WithPickerTitle("Recursos materiales")
                                                        .WithBlocker(Blocker);

            materialResourcesController = new(configMaterialResources);


            var configEvaluationInstrument = WeakReferenceFieldConfiguration<CommonText>.CreateForTextBox(TextEvaluationInstrument)
                                               .WithStorageId(entity.EvaluationInstrumentType?.StorageId)
                                               .WithParentStorageId(subject.StorageId)
                                               .WithPick(ButtonEvaluationInstrumentPick)
                                               .WithPickerTitle("Elige un instrumento de evaluación")
                                               .WithFormat(EntityFormatContent.Title)
                                               .WithPickListQuery(() => Storage.GetStorageIds<CommonText>(subject.EvaluationInstrumentsTypes.ToList()) )
                                               .WithBlocker(Blocker);

            evaluationInstrumentController = new(configEvaluationInstrument);

            Func<List<string>> pickCriteriasQuery =
                () =>
                {
                    List<string> criterias = new();
                    if(subject.Template != null)
                    {
                        List<LearningResult> resultList = subject.Template.LearningResults.ToList();

                        foreach(LearningResult r in resultList)
                        {
                            List<CommonText> criteriasList = r.Criterias.ToList();

                            foreach(CommonText c in criteriasList)
                            {
                                criterias.Add(c.StorageId);
                            }
                        }
                    }

                    return criterias;
                };

            Func<CommonText, int, string> criteriaFormatter =
                (e, i) =>
                {
                    bool canFormat;                    
                    SubjectTemplate? template = null;
                    List<LearningResult>? results = null;
                    string? resultStorageId = null;
                    int criteriaIndex = -1;
                    int resultIndex = -1;

                    canFormat = (subject.Template != null);
                    if(canFormat)
                    {
                        template = subject.Template;
                    }
                    if(canFormat)
                    {   results = template.LearningResults.ToList();
                        resultStorageId = Storage.FindParentStorageId(e.StorageId, e.StorageClassId);
                        canFormat = (resultStorageId != null);
                    }
                    if(canFormat)
                    {   resultIndex = results.FindIndex(c => c.StorageId == resultStorageId);
                        canFormat = (resultIndex >= 0);
                    }
                    if(canFormat)
                    {   criteriaIndex = results[resultIndex].Criterias.ToList().FindIndex(c => c.StorageId == e.StorageId);
                        canFormat = (criteriaIndex >= 0);
                    }
                    
                    if(canFormat)
                    {
                        return String.Format("RA{0}.{1}: {2}", resultIndex + 1, criteriaIndex + 1, e.Description);
                    }
                    else
                    {
                        return "<no se encuentra la referencia>";    
                    }
                };

            var configCriterias = WeakReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxCriterias)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_entity.Criterias.ToList()))
                                                        .WithFormatter(criteriaFormatter)
                                                        .WithPick(ButtonCriteriaReferenceAdd, ButtonCriteriaReferenceRemove)
                                                        .WithPickListQuery(pickCriteriasQuery)
                                                        .WithPickerTitle("Criterios")
                                                        .WithBlocker(Blocker);

            criteriasController = new(configCriterias);

            for (int i = 0; i < 100; i++) { ComboDuration.Items.Add(i.ToString()); }
            ComboDurationFraction.Items.Add(".0");
            ComboDurationFraction.Items.Add(".25");
            ComboDurationFraction.Items.Add(".5");
            ComboDurationFraction.Items.Add(".75");

            for (int i = 0; i < 5; i++) { ComboStartWeekDay.Items.Add(Utils.WeekdayToText(Utils.IndexToWeekday(i + 1)));  }

            TextTitle.Text = entity.Title;
            TextBoxDescription.Text = entity.Description;
            ComboStartType.SelectedIndex = (int)entity.StartType;
            DateStartDate.SelectedDate = entity.StartDate;
            ComboStartWeekDay.SelectedIndex = (int)(entity.StartDayOfWeek - 1);
            ComboDuration.SelectedIndex = (int)entity.Duration;
            ComboDurationFraction.SelectedIndex = (int)((entity.Duration - MathF.Floor(entity.Duration)) / 0.25f);
            CheckboxNoActivitiesBefore.IsChecked = entity.NoActivitiesBefore;
            CheckboxNoActivitiesAfter.IsChecked = entity.NoActivitiesAfter;
            CheckboxIsEvaluable.IsChecked = entity.IsEvaluable;

            TextActivityCode.Background = new SolidColorBrush((Color)Application.Current.Resources["ColorLocked"]);
            TextActivityCode.IsReadOnly = true;

            TextScheduledStartDay.Background = new SolidColorBrush((Color)Application.Current.Resources["ColorLocked"]);
            TextScheduledStartDay.IsReadOnly = true;

            TextScheduledEndDay.Background = new SolidColorBrush((Color)Application.Current.Resources["ColorLocked"]);
            TextScheduledEndDay.IsReadOnly = true;

            TextScheduledSessions.Background = new SolidColorBrush((Color)Application.Current.Resources["ColorLocked"]);
            TextScheduledSessions.IsReadOnly = true;

            dataTableResultsWeight = new DataTable();
            
            DataGridLearningResultsWeight.ItemsSource = dataTableResultsWeight.DefaultView;
            DataGridLearningResultsWeight.CanUserAddRows = false;
            DataGridLearningResultsWeight.CanUserDeleteRows = false;
            DataGridLearningResultsWeight.CanUserReorderColumns = false;
            DataGridLearningResultsWeight.CanUserSortColumns = false;
            DataGridLearningResultsWeight.CanUserResizeColumns = false;
            DataGridLearningResultsWeight.CanUserResizeRows = false;

            TextTitle.TextChanged += TextTitle_TextChanged;
            TextBoxDescription.TextChanged += TextBoxDescription_TextChanged;

            ComboStartType.SelectionChanged += ComboStartType_SelectionChanged;
            ComboStartWeekDay.SelectionChanged += ComboStartWeekDay_SelectionChanged;
            DateStartDate.SelectedDateChanged += DateStartDate_SelectedDateChanged;

            ComboDuration.SelectionChanged += ComboDuration_SelectionChanged;
            ComboDurationFraction.SelectionChanged += ComboDurationFraction_SelectionChanged; ;
            CheckboxNoActivitiesBefore.Checked += CheckboxNoActivitiesBefore_Checked;
            CheckboxNoActivitiesBefore.Unchecked += CheckboxNoActivitiesBefore_Unchecked;
            CheckboxNoActivitiesAfter.Checked += CheckboxNoActivitiesAfter_Checked;
            CheckboxNoActivitiesAfter.Unchecked += CheckboxNoActivitiesAfter_Unchecked;

            metodologyController.Changed += MetodologyController_Changed;
            contentPointsController.Changed += ContentPointsController_Changed;
            keyCompetencesController.Changed += KeyCompetencesController_Changed;
            spaceResourcesController.Changed += SpaceResourcesController_Changed;
            materialResourcesController.Changed += MaterialResourcesController_Changed;
            evaluationInstrumentController.Changed += EvaluationInstrumentController_Changed;
            criteriasController.Changed += CriteriasController_Changed;

            CheckboxIsEvaluable.Checked += CheckboxIsEvaluable_CheckedChanged;
            CheckboxIsEvaluable.Unchecked += CheckboxIsEvaluable_CheckedChanged;

            dataTableResultsWeight.RowChanged += DataTableResultsWeight_RowChanged;

            UpdateStartTypeUI();
            UpdateActivityCodeUI();
            UpdateActivityScheduleUI();
            UpdateEvaluableUI();
            UpdateResultsWeightTableUI();

            Validate();

        }


        private void UpdateEvaluableUI()
        {
            Visibility visibility = entity.IsEvaluable ? Visibility.Visible : Visibility.Hidden;
            LabelActivityCode.Visibility = visibility;
            TextActivityCode.Visibility = visibility;
            LabelEvaluationInstrument.Visibility = visibility;
            TextEvaluationInstrument.Visibility = visibility;
            ButtonEvaluationInstrumentPick.Visibility = visibility;
            LabelCriterias.Visibility = visibility;
            ButtonCriteriaReferenceAdd.Visibility = visibility;
            ButtonCriteriaReferenceRemove.Visibility = visibility;
            ListBoxCriterias.Visibility = visibility;
            LabelLearningResultsWeight.Visibility = visibility;
            DataGridLearningResultsWeight.Visibility = visibility;

        }

        private void DateStartDate_SelectedDateChanged(object? sender, SelectionChangedEventArgs e)
        {
            UpdateEntity();
            Validate();
            UpdateActivityScheduleUI();
        }

        private void ComboStartWeekDay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateEntity();
            Validate();
            UpdateActivityScheduleUI();
        }

        private void ComboStartType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateEntity();
            Validate();

            UpdateStartTypeUI();
            UpdateActivityScheduleUI();

        }

        private void CheckboxNoActivitiesAfter_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateEntity();
            Validate();
            UpdateActivityScheduleUI();
        }

        private void CheckboxNoActivitiesAfter_Checked(object sender, RoutedEventArgs e)
        {
            UpdateEntity();
            Validate();
            UpdateActivityScheduleUI();
        }

        void CombosDurationApplyLimits()
        {
            if (ComboDuration.SelectedIndex == 0 && ComboDurationFraction.SelectedIndex == 0)
            {
                ComboDuration.SelectionChanged -= ComboDuration_SelectionChanged;
                ComboDurationFraction.SelectionChanged -= ComboDurationFraction_SelectionChanged;

                ComboDuration.SelectedIndex = 0;
                ComboDurationFraction.SelectedIndex = 1;

                ComboDuration.SelectionChanged += ComboDuration_SelectionChanged;
                ComboDurationFraction.SelectionChanged += ComboDurationFraction_SelectionChanged;
            }

        }

        private void ComboDurationFraction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CombosDurationApplyLimits();

            UpdateEntity();
            Validate();
            UpdateActivityScheduleUI();
        }

        private void ComboDuration_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CombosDurationApplyLimits();
               
            UpdateEntity();
            Validate();
            UpdateActivityScheduleUI();
        }

        private void CheckboxNoActivitiesBefore_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateEntity();
            Validate();
            UpdateActivityScheduleUI();
        }

        private void CheckboxNoActivitiesBefore_Checked(object sender, RoutedEventArgs e)
        {
            UpdateEntity();
            Validate();
            UpdateActivityScheduleUI();
        }

        private void TextTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateEntity();
            Validate();
        }

        private void TextBoxDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateEntity();
            Validate();
        }

        private void DataTableResultsWeight_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            UpdateEntity();
            Validate();
        }

        void SpaceResourcesController_Changed(WeakReferencesBoxController<CommonText, EntityPicker<CommonText>> controller)
        {
            UpdateEntity();
            Validate();
        }

        void MaterialResourcesController_Changed(WeakReferencesBoxController<CommonText, EntityPicker<CommonText>> controller)
        {
            UpdateEntity();
            Validate();
        }

        void CriteriasController_Changed(WeakReferencesBoxController<CommonText, EntityPicker<CommonText>> controller)
        {
            UpdateEntity();
            Validate();
            UpdateResultsWeightTableUI();
        }

        void EvaluationInstrumentController_Changed(WeakReferenceFieldController<CommonText, EntityPicker<CommonText>> controller)
        {
            UpdateEntity();
            Validate();
        }

        void MetodologyController_Changed(WeakReferenceFieldController<CommonText, EntityPicker<CommonText>> controller)
        {
            UpdateEntity();
            Validate();
        }

        void ContentPointsController_Changed(WeakReferencesBoxController<CommonText, EntityPicker<CommonText> > controller)
        {
            UpdateEntity();
            Validate();
        }

        private void KeyCompetencesController_Changed(WeakReferencesBoxController<CommonText, EntityPicker<CommonText>> controller)
        {
            UpdateEntity();
            Validate();
        }

        void CheckboxIsEvaluable_CheckedChanged(object sender, RoutedEventArgs e)
        {
            UpdateActivityCodeUI();
            UpdateEntity();
            Validate();
            UpdateEvaluableUI();
            UpdateResultsWeightTableUI();
        }

        void UpdateActivityCodeUI()
        {
            if(CheckboxIsEvaluable.IsChecked.GetValueOrDefault())
            {
                int activityIndex = block.Activities.ToList().FindIndex((a => a.StorageId == entity.StorageId));
                int blockIndex = subject.Blocks.ToList().FindIndex((b) => b.StorageId == block.StorageId);
                TextActivityCode.Text = String.Format("B{0:00}-A{1:00}", blockIndex + 1, activityIndex + 1);
            }
            else
            {
                TextActivityCode.Text = "(actividad no evaluable)";
            }
        }

        void UpdateStartTypeUI()
        {
            ActivityStartType startType = (ActivityStartType)ComboStartType.SelectedIndex;
            ComboStartWeekDay.Visibility = (startType == ActivityStartType.DayOfWeek ? Visibility.Visible : Visibility.Hidden);
            DateStartDate.Visibility = (startType == ActivityStartType.Date ? Visibility.Visible : Visibility.Hidden);

        }

        void UpdateActivityScheduleUI()
        {
            bool cannotSchedule = false;

            if(subject.CanScheduleActivities())
            {
                List<ActivitySchedule> schedules = subject.ScheduleActivities();
                int scheduleIndex = schedules.FindIndex(s => s.activity.StorageId == entity.StorageId);
                
                if(scheduleIndex >= 0)
                {
                    ActivitySchedule schedule = schedules[scheduleIndex];

                    TextScheduledStartDay.Text = Utils.FormatStartDayHour(schedule.start.day, schedule.start.hour, subject.WeekSchedule);
                    TextScheduledEndDay.Text = Utils.FormatStartDayHour(schedule.end.day, schedule.end.hour, subject.WeekSchedule);

                    int count = 0;
                    for(DateTime d = schedule.start.day; d <= schedule.end.day; d = d.AddDays(1))
                    {
                        if(Utils.IsSchoolDay(d, subject.Calendar, subject.WeekSchedule)) { count++; }
                    }

                    TextScheduledSessions.Text = count.ToString();
                }
                else
                {
                    cannotSchedule = true;
                }
            }
            else
            {
                cannotSchedule = true;
            }

            if(cannotSchedule)
            {
                TextScheduledStartDay.Text = "<no planificable>";
                TextScheduledEndDay.Text = "<no planificable>";
                TextScheduledSessions.Text = "<n/p>";
            }
        }

        void UpdateResultsWeightTableUI()
        {
            dataTableResultsWeight.Clear();
            dataTableResultsWeight.Rows.Clear();
            dataTableResultsWeight.Columns.Clear();

            List<LearningResult> learningResultList = subject.Template.LearningResults.ToList();
            for (int i = 0; i < learningResultList.Count; i++)
            {
                string columnName = String.Format("RA{0}", i + 1);
                dataTableResultsWeight.Columns.Add(columnName, typeof(float));
            }

            List<KeyValuePair<LearningResult, float>> learningResultsWeightList = entity.LearningResultsWeights.ToList();

            DataRow row = dataTableResultsWeight.NewRow();

            for (int i = 0; i < learningResultList.Count; i++)
            {
                string columnName = String.Format("RA{0}", i + 1);
                row[columnName] = learningResultsWeightList[i].Value;
            }

            dataTableResultsWeight.Rows.Add(row);
        }

        public Activity GetEntity()
        {
            return entity;
        }

        public HashSet<int> GetReferencedResults(List<CommonText> criteriasList, List<LearningResult> resultsList)
        {
            HashSet<int> referencedResults = new();
            for(int i = 0; i < resultsList.Count; i ++)
            {
                bool done = false;
                List<CommonText> resultCriteriaList = resultsList[i].Criterias.ToList();
                int j = 0;
                while(j < resultCriteriaList.Count && !done)
                {
                    if(criteriasList.Find(r => r.StorageId == resultCriteriaList[j].StorageId) != null)
                    {
                        referencedResults.Add(i);
                        done = true;
                    }
                    else
                    {
                        j++;
                    }
                }
                    
            }

            return referencedResults;
        }
    }
}
