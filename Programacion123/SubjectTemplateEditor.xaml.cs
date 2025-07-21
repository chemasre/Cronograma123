using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Programacion123
{
    /// <summary>
    /// Lógica de interacción para SubjectTemplateEditor.xaml
    /// </summary>
    public partial class SubjectTemplateEditor : Window, IEntityEditor<SubjectTemplate>
    {
        SubjectTemplate entity;
        string? parentStorageId;
        StrongReferenceFieldController<CommonText, CommonTextEditor> generalObjectivesIntroductionController;
        StrongReferencesBoxController<CommonText, CommonTextEditor > generalObjectivesController;
        StrongReferenceFieldController<CommonText, CommonTextEditor> generalCompetencesIntroductionController;
        StrongReferencesBoxController<CommonText, CommonTextEditor> generalCompetencesController;
        StrongReferenceFieldController<CommonText, CommonTextEditor> learningResultsIntroductionController;
        StrongReferencesBoxController<LearningResult, LearningResultEditor> learningResultsController;
        StrongReferenceFieldController<CommonText, CommonTextEditor> contentsIntroductionController;
        StrongReferencesBoxController<Content, ContentEditor> contentsController;

        public SubjectTemplateEditor()
        {
            InitializeComponent();

        }

        public SubjectTemplate GetEntity()
        {
            return entity;
        }

        public void InitEditor(SubjectTemplate _subjectTemplate, string? _parentStorageId)
        {
            _subjectTemplate.Save(_parentStorageId);

            entity = _subjectTemplate;
            parentStorageId = _parentStorageId;

            var configObjectivesIntroduction = StrongReferenceFieldConfiguration<CommonText>.CreateForTextBox(TextGeneralObjectivesIntroduction)
                                               .WithStorageId(entity.GeneralObjectivesIntroduction.StorageId)
                                               .WithParentStorageId(entity.StorageId)
                                               .WithNew(ButtonGeneralObjectivesIntroductionNew)
                                               .WithEdit(ButtonGeneralObjectivesIntroductionEdit)
                                               .WithTitleEditable(false)
                                               .WithEditorTitle("Introducción a los objetivos generales")
                                               .WithBlocker(Blocker);

            generalObjectivesIntroductionController = new(configObjectivesIntroduction);

            generalObjectivesIntroductionController.Changed += GeneralObjectivesIntroductionController_Changed;

            var configObjectives = StrongReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxGeneralObjectives)
                                                        .WithParentStorageId(_subjectTemplate.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_subjectTemplate.GeneralObjectives.ToList()))
                                                        .WithFormat(EntityFormatContent.title, EntityFormatIndex.character)
                                                        .WithNew(ButtonGeneralObjectiveNew)
                                                        .WithEdit(ButtonGeneralObjectiveEdit)
                                                        .WithDelete(ButtonGeneralObjectiveDelete)
                                                        .WithUpDown(ButtonGeneralObjectiveUp, ButtonGeneralObjectiveDown)
                                                        .WithEditorTitle("Objetivo general")
                                                        .WithBlocker(Blocker);

            generalObjectivesController = new(configObjectives);

            generalObjectivesController.Changed += GeneralObjectivesController_Changed;

            var configCompetencesIntroduction = StrongReferenceFieldConfiguration<CommonText>.CreateForTextBox(TextGeneralCompetencesIntroduction)
                                               .WithStorageId(entity.GeneralCompetencesIntroduction.StorageId)
                                               .WithParentStorageId(entity.StorageId)
                                               .WithNew(ButtonGeneralCompetencesIntroductionNew)
                                               .WithEdit(ButtonGeneralCompetencesIntroductionEdit)
                                               .WithTitleEditable(false)
                                               .WithEditorTitle("Introducción a las competencias generales")
                                               .WithBlocker(Blocker);

            generalCompetencesIntroductionController = new(configCompetencesIntroduction);

            generalCompetencesIntroductionController.Changed += GeneralCompetencesIntroductionController_Changed;

            var configCompetences = StrongReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxGeneralCompetences)
                                                        .WithParentStorageId(_subjectTemplate.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_subjectTemplate.GeneralCompetences.ToList()))
                                                        .WithFormat(EntityFormatContent.title, EntityFormatIndex.character)
                                                        .WithNew(ButtonGeneralCompetenceNew)
                                                        .WithEdit(ButtonGeneralCompetenceEdit)
                                                        .WithDelete(ButtonGeneralCompetenceDelete)
                                                        .WithUpDown(ButtonGeneralCompetenceUp, ButtonGeneralCompetenceDown)
                                                        .WithEditorTitle("Competencia general")
                                                        .WithBlocker(Blocker);

            generalCompetencesController = new(configCompetences);

            generalCompetencesController.Changed += GeneralCompetencesController_Changed;

            var configLearningResultsIntroduction = StrongReferenceFieldConfiguration<CommonText>.CreateForTextBox(TextLearningResultsIntroduction)
                                               .WithStorageId(entity.LearningResultsIntroduction.StorageId)
                                               .WithParentStorageId(entity.StorageId)
                                               .WithNew(ButtonLearningResultsIntroductionNew)
                                               .WithEdit(ButtonLearningResultsIntroductionEdit)
                                               .WithTitleEditable(false)
                                               .WithEditorTitle("Introducción a los resultados de aprendizaje")
                                               .WithBlocker(Blocker);

            learningResultsIntroductionController = new(configLearningResultsIntroduction);

            learningResultsIntroductionController.Changed += LearningResultsIntroductionController_Changed;

            var configLearningResults = StrongReferencesBoxConfiguration<LearningResult>.CreateForList(ListBoxLearningResults)
                                                        .WithParentStorageId(_subjectTemplate.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<LearningResult>(_subjectTemplate.LearningResults.ToList()))
                                                        .WithFormatter((e, i) => String.Format("RA{0}: {1}", i + 1, e.Title))
                                                        .WithNew(ButtonLearningResultsNew)
                                                        .WithEdit(ButtonLearningResultsEdit)
                                                        .WithDelete(ButtonLearningResultsDelete)
                                                        .WithUpDown(ButtonLearningResultsUp, ButtonLearningResultsDown)
                                                        .WithEditorTitle("Resultado de aprendizaje")
                                                        .WithBlocker(Blocker);

            learningResultsController = new(configLearningResults);

            learningResultsController.Changed += LearningResultsController_Changed;

            var configContentsIntroduction = StrongReferenceFieldConfiguration<CommonText>.CreateForTextBox(TextContentsIntroduction)
                                               .WithStorageId(entity.ContentsIntroduction.StorageId)
                                               .WithParentStorageId(entity.StorageId)
                                               .WithNew(ButtonContentsIntroductionNew)
                                               .WithEdit(ButtonContentsIntroductionEdit)
                                               .WithTitleEditable(false)
                                               .WithEditorTitle("Introducción a los contenidos")
                                               .WithBlocker(Blocker);

            contentsIntroductionController = new(configContentsIntroduction);

            contentsIntroductionController.Changed += ContentsIntroductionController_Changed;

            var configContents = StrongReferencesBoxConfiguration<Content>.CreateForList(ListBoxContents)
                                                        .WithParentStorageId(_subjectTemplate.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<Content>(_subjectTemplate.Contents.ToList()))
                                                        .WithFormat(EntityFormatContent.title, EntityFormatIndex.number)
                                                        .WithNew(ButtonContentsNew)
                                                        .WithEdit(ButtonContentsEdit)
                                                        .WithDelete(ButtonContentsDelete)
                                                        .WithUpDown(ButtonContentsUp, ButtonContentsDown)
                                                        .WithEditorTitle("Contenido")
                                                        .WithBlocker(Blocker);

            contentsController = new(configContents);

            contentsController.Changed += ContentsController_Changed;

            TextTitle.Text = _subjectTemplate.Title;

            TextSubjectName.Text = _subjectTemplate.SubjectName;
            TextSubjectCode.Text = _subjectTemplate.SubjectCode;
            ComboGradeType.SelectedIndex = (int)_subjectTemplate.GradeType;
            TextGradeName.Text = _subjectTemplate.GradeName;
            TextGradeFamilyName.Text = _subjectTemplate.GradeFamilyName;
            TextGradeClassroomHours.Text = _subjectTemplate.GradeClassroomHours.ToString();
            TextGradeCompanyHours.Text = _subjectTemplate.GradeCompanyHours.ToString();


            Validate();

        }

        void Validate()
        {
            Entity.ValidationResult validation = entity.Validate();

            if (validation.code == Entity.ValidationCode.success)
            {
                BorderValidation.Background = new SolidColorBrush((Color)Application.Current.Resources["ColorValid"]);
                TextValidation.Text = "No se han detectado problemas";
            }
            else
            {
                BorderValidation.Background = new SolidColorBrush((Color)Application.Current.Resources["ColorInvalid"]);
            }
        }

        private void ContentsController_Changed(StrongReferencesBoxController<Content, ContentEditor> controller)
        {
            UpdateEntity();
        }

        private void ContentsIntroductionController_Changed(StrongReferenceFieldController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity();
        }

        private void LearningResultsController_Changed(StrongReferencesBoxController<LearningResult, LearningResultEditor> controller)
        {
            UpdateEntity();
        }

        private void LearningResultsIntroductionController_Changed(StrongReferenceFieldController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity();
        }

        private void GeneralCompetencesController_Changed(StrongReferencesBoxController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity();
        }

        private void GeneralCompetencesIntroductionController_Changed(StrongReferenceFieldController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity();
        }

        private void GeneralObjectivesController_Changed(StrongReferencesBoxController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity();
        }

        private void GeneralObjectivesIntroductionController_Changed(StrongReferenceFieldController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity();
        }

        private void UpdateEntity()
        {
            entity.Title = TextTitle.Text;
            entity.GeneralObjectivesIntroduction = Storage.LoadOrCreateEntity<CommonText>(generalObjectivesIntroductionController.StorageId, entity.StorageId);
            entity.GeneralObjectives.Set(Storage.LoadOrCreateEntities<CommonText>(generalObjectivesController.StorageIds, entity.StorageId));
            entity.GeneralCompetencesIntroduction = Storage.LoadOrCreateEntity<CommonText>(generalCompetencesIntroductionController.StorageId, entity.StorageId);
            entity.GeneralCompetences.Set(Storage.LoadOrCreateEntities<CommonText>(generalCompetencesController.StorageIds, entity.StorageId));

            entity.LearningResultsIntroduction = Storage.LoadOrCreateEntity<CommonText>(learningResultsIntroductionController.StorageId, entity.StorageId);
            entity.LearningResults.Set(Storage.LoadOrCreateEntities<LearningResult>(learningResultsController.StorageIds, entity.StorageId));
            entity.ContentsIntroduction = Storage.LoadOrCreateEntity<CommonText>(contentsIntroductionController.StorageId, entity.StorageId);
            entity.Contents.Set(Storage.LoadOrCreateEntities<Content>(contentsController.StorageIds, entity.StorageId));

            entity.SubjectName = TextSubjectName.Text;
            entity.SubjectCode = TextSubjectCode.Text;
            entity.GradeName = TextGradeName.Text;
            entity.GradeType = (GradeType)(ComboGradeType.SelectedIndex >= 0 ? ComboGradeType.SelectedIndex : 0);
            entity.GradeName = TextGradeName.Text;
            entity.GradeFamilyName = TextGradeFamilyName.Text;

            int number;
            entity.GradeClassroomHours = Int32.TryParse(TextGradeClassroomHours.Text, out number) ? number : 0;
            entity.GradeCompanyHours= Int32.TryParse(TextGradeCompanyHours.Text, out number) ? number : 0;

            entity.Save(parentStorageId);
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            UpdateEntity();
            // entity.Save(parentStorageId);

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
