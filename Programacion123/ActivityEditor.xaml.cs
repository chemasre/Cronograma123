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
        WeakReferencesBoxController<CommonText, EntityPicker<CommonText> > spaceResourcesController;
        WeakReferencesBoxController<CommonText, EntityPicker<CommonText> > materialResourcesController;
        WeakReferenceFieldController<CommonText, EntityPicker<CommonText> > evaluationInstrumentController;
        WeakReferencesBoxController<CommonText, EntityPicker<CommonText> > criteriasController;


        Subject? subject;
        Block? block;


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

            int hours;
            if(Int32.TryParse(TextHours.Text, out hours))  { entity.Hours = hours; }
            else { entity.Hours = 0; }
            

            entity.Metodology = metodologyController.GetEntity();

            entity.ContentPoints.Set(contentPointsController.GetSelectedEntities());

            entity.SpaceResources.Set(spaceResourcesController.GetSelectedEntities());
            entity.MaterialResources.Set(materialResourcesController.GetSelectedEntities());

            entity.IsEvaluable = CheckboxIsEvaluable.IsChecked.GetValueOrDefault();

            entity.EvaluationInstrumentType = evaluationInstrumentController.GetEntity();

            entity.Criterias.Set(criteriasController.GetSelectedEntities());


            entity.Save(parentStorageId);
        }

        public void InitEditor(Activity _entity, string? _parentStorageId)
        {
            _entity.Save(_parentStorageId);

            entity = _entity;

            parentStorageId = _parentStorageId;

            block = new();

            string blockStorageId = _parentStorageId;
            string subjectStorageId = Storage.FindParentStorageId(blockStorageId, block.StorageClassId);

            subject = new Subject();
            subject.LoadOrCreate(subjectStorageId);
            block.LoadOrCreate(blockStorageId, subjectStorageId);

            TextBlock.Text = block.Title;
            TextBlock.Background = Brushes.LightPink;

            var configMetodology = WeakReferenceFieldConfiguration<CommonText>.CreateForTextBox(TextMetodology)
                                               .WithStorageId(entity.Metodology?.StorageId)
                                               .WithParentStorageId(subject.StorageId)
                                               .WithPick(ButtonMetodologyPick)
                                               .WithPickerTitle("Elige una metodología")
                                               .WithFormat(EntityFormatContent.title)
                                               .WithPickListQuery(() => Storage.GetStorageIds<CommonText>(subject.Metodologies.ToList()) )
                                               .WithBlocker(Blocker);

            metodologyController = new(configMetodology);
            metodologyController.StorageIdChanged += MetodologyController_StorageIdChanged;

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
                        return String.Format("{0}.{1}: {2}", contentIndex + 1, pointIndex + 1, e.Title);
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
            contentPointsController.StorageIdsChanged += ContentPointsController_StorageIdsChanged;

            var configSpaceResources = WeakReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxSpaceResources)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_entity.SpaceResources.ToList()))
                                                        .WithFormat(EntityFormatContent.title)
                                                        .WithPick(ButtonSpaceResourceReferenceAdd, ButtonSpaceResourceReferenceRemove)
                                                        .WithPickListQuery(() => Storage.GetStorageIds<CommonText>(subject.SpaceResources.ToList()))
                                                        .WithPickerTitle("Espacios")
                                                        .WithBlocker(Blocker);

            spaceResourcesController = new(configSpaceResources);
            spaceResourcesController.StorageIdsChanged += SpaceResourcesController_StorageIdsChanged;

            var configMaterialResources = WeakReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxMaterialResources)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_entity.MaterialResources.ToList()))
                                                        .WithFormat(EntityFormatContent.title)
                                                        .WithPick(ButtonMaterialResourceReferenceAdd, ButtonMaterialResourceReferenceRemove)
                                                        .WithPickListQuery(() => Storage.GetStorageIds<CommonText>(subject.MaterialResources.ToList()))
                                                        .WithPickerTitle("Recursos materiales")
                                                        .WithBlocker(Blocker);

            materialResourcesController = new(configMaterialResources);
            materialResourcesController.StorageIdsChanged += MaterialResourcesController_StorageIdsChanged;


            var configEvaluationInstrument = WeakReferenceFieldConfiguration<CommonText>.CreateForTextBox(TextEvaluationInstrument)
                                               .WithStorageId(entity.EvaluationInstrumentType?.StorageId)
                                               .WithParentStorageId(subject.StorageId)
                                               .WithPick(ButtonEvaluationInstrumentPick)
                                               .WithPickerTitle("Elige un instrumento de evaluación")
                                               .WithFormat(EntityFormatContent.title)
                                               .WithPickListQuery(() => Storage.GetStorageIds<CommonText>(subject.EvaluationInstrumentsTypes.ToList()) )
                                               .WithBlocker(Blocker);

            evaluationInstrumentController = new(configEvaluationInstrument);
            evaluationInstrumentController.StorageIdChanged += EvaluationInstrumentController_StorageIdChanged;

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
                        return String.Format("RA{0}.{1}: {2}", resultIndex + 1, criteriaIndex + 1, e.Title);
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
            criteriasController.StorageIdsChanged += CriteriasController_StorageIdsChanged;


            TextTitle.Text = entity.Title;
            TextBoxDescription.Text = entity.Description;
            TextHours.Text = entity.Hours.ToString();
            CheckboxIsEvaluable.IsChecked = entity.IsEvaluable;

            CheckboxIsEvaluable.Checked += CheckboxIsEvaluable_CheckedChanged;
            CheckboxIsEvaluable.Unchecked += CheckboxIsEvaluable_CheckedChanged;

            UpdateActivityCodeUI();

        }

        void SpaceResourcesController_StorageIdsChanged(WeakReferencesBoxController<CommonText, EntityPicker<CommonText>> controller, List<string> storageIdList)
        {
            UpdateEntity();
        }

        void MaterialResourcesController_StorageIdsChanged(WeakReferencesBoxController<CommonText, EntityPicker<CommonText>> controller, List<string> storageIdList)
        {
            UpdateEntity();
        }

        void CriteriasController_StorageIdsChanged(WeakReferencesBoxController<CommonText, EntityPicker<CommonText>> controller, List<string> storageIdList)
        {
            UpdateEntity();
        }

        void EvaluationInstrumentController_StorageIdChanged(WeakReferenceFieldController<CommonText, EntityPicker<CommonText>> controller, string storageId)
        {
            UpdateEntity();
        }

        void MetodologyController_StorageIdChanged(WeakReferenceFieldController<CommonText, EntityPicker<CommonText>> controller, string storageId)
        {
            UpdateEntity();
        }

        void ContentPointsController_StorageIdsChanged(WeakReferencesBoxController<CommonText, EntityPicker<CommonText> > controller, List<string> storageIdList)
        {
            UpdateEntity();
        }

        void CheckboxIsEvaluable_CheckedChanged(object sender, RoutedEventArgs e)
        {
            UpdateActivityCodeUI();
            UpdateEntity();
        }

        void UpdateActivityCodeUI()
        {
            if(CheckboxIsEvaluable.IsChecked.GetValueOrDefault())
            {
                int activityIndex = block.Activities.ToList().FindIndex((a => a.StorageId == entity.StorageId));
                int blockIndex = subject.Blocks.ToList().FindIndex((b) => b.StorageId == block.StorageId);
                TextActivityCode.Text = String.Format("B{0:00}-A{1:00}", blockIndex + 1, activityIndex + 1);
                TextActivityCode.Background = Brushes.LightPink;
            }
            else
            {
                TextActivityCode.Text = "";
                TextActivityCode.Background = Brushes.LightGray;
            }
        }

        public Activity GetEntity()
        {
            return entity;
        }

        private void TextHours_TextChanged(object sender, TextChangedEventArgs e)
        {
            int h;
            if(!Int32.TryParse(TextHours.Text, out h)) { TextHours.Text = ""; }

            UpdateEntity();
        }
    }
}
