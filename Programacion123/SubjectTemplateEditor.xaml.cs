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
        StrongReferenceFieldController<CommonText, CommonTextEditor> keyCapacitiesIntroductionController;
        StrongReferencesBoxController<CommonText, CommonTextEditor> keyCapacitiesController;
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
                                               .WithFormat(EntityFormatContent.description)
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
                                               .WithFormat(EntityFormatContent.description)
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

            var configKeyCapacitiesIntroduction = StrongReferenceFieldConfiguration<CommonText>.CreateForTextBox(TextKeyCapacitiesIntroduction)
                                               .WithStorageId(entity.KeyCapacitiesIntroduction.StorageId)
                                               .WithParentStorageId(entity.StorageId)
                                               .WithFormat(EntityFormatContent.description)
                                               .WithNew(ButtonKeyCapacitiesIntroductionNew)
                                               .WithEdit(ButtonKeyCapacitiesIntroductionEdit)
                                               .WithTitleEditable(false)
                                               .WithEditorTitle("Introducción a las capacidades clave")
                                               .WithBlocker(Blocker);

            keyCapacitiesIntroductionController = new(configKeyCapacitiesIntroduction);

            keyCapacitiesIntroductionController.Changed += KeyCapacitiesIntroductionController_Changed;

            var configKeyCapacities = StrongReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxKeyCapacities)
                                                        .WithParentStorageId(_subjectTemplate.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_subjectTemplate.KeyCapacities.ToList()))
                                                        .WithFormat(EntityFormatContent.title, EntityFormatIndex.character)
                                                        .WithNew(ButtonKeyCapacitiesNew)
                                                        .WithEdit(ButtonKeyCapacitiesEdit)
                                                        .WithDelete(ButtonKeyCapacitiesDelete)
                                                        .WithUpDown(ButtonKeyCapacitiesUp, ButtonKeyCapacitiesDown)
                                                        .WithEditorTitle("Capacidad clave")
                                                        .WithBlocker(Blocker);

            keyCapacitiesController = new(configKeyCapacities);

            keyCapacitiesController.Changed += KeyCapacitiesController_Changed;

            var configLearningResultsIntroduction = StrongReferenceFieldConfiguration<CommonText>.CreateForTextBox(TextLearningResultsIntroduction)
                                               .WithStorageId(entity.LearningResultsIntroduction.StorageId)
                                               .WithParentStorageId(entity.StorageId)
                                               .WithFormat(EntityFormatContent.description)
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
                                               .WithFormat(EntityFormatContent.description)
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


            TextTitle.TextChanged += TextTitle_TextChanged;
            TextSubjectName.TextChanged += TextSubjectName_TextChanged;
            TextSubjectCode.TextChanged += TextSubjectCode_TextChanged;
            TextGradeName.TextChanged += TextGradeName_TextChanged;
            TextGradeFamilyName.TextChanged += TextGradeFamilyName_TextChanged;
            TextGradeClassroomHours.TextChanged += TextGradeClassroomHours_TextChanged;
            TextGradeCompanyHours.TextChanged += TextGradeCompanyHours_TextChanged;

            Validate();

        }

        private void KeyCapacitiesController_Changed(StrongReferencesBoxController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity();
            Validate();
        }

        private void KeyCapacitiesIntroductionController_Changed(StrongReferenceFieldController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity();
            Validate();
        }

        private void TextSubjectCode_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateEntity();
            Validate();
        }

        private void TextGradeCompanyHours_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            int result;
            if (!Int32.TryParse(TextGradeCompanyHours.Text, out result)) { TextGradeCompanyHours.Text = ""; }

            UpdateEntity();
            Validate();
        }

        private void TextGradeClassroomHours_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            int result;
            if (!Int32.TryParse(TextGradeClassroomHours.Text, out result)) { TextGradeClassroomHours.Text = ""; }

            UpdateEntity();
            Validate();
        }

        private void TextGradeFamilyName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateEntity();
            Validate();
        }

        private void TextGradeName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateEntity();
            Validate();
        }

        private void TextSubjectName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateEntity();
            Validate();
        }

        private void TextTitle_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateEntity();
            Validate();
        }

        void Validate()
        {
            ValidationResult validation = entity.Validate();

            string colorResource = (validation.code == ValidationCode.success ? "ColorValid" : "ColorInvalid");
            BorderValidation.Background = new SolidColorBrush((Color)Application.Current.Resources[colorResource]);
            TextValidation.Text = validation.ToString();

        }

        private void ContentsController_Changed(StrongReferencesBoxController<Content, ContentEditor> controller)
        {
            UpdateEntity();
            Validate();
        }

        private void ContentsIntroductionController_Changed(StrongReferenceFieldController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity();
            Validate();
        }

        private void LearningResultsController_Changed(StrongReferencesBoxController<LearningResult, LearningResultEditor> controller)
        {
            UpdateEntity();
            Validate();
        }

        private void LearningResultsIntroductionController_Changed(StrongReferenceFieldController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity();
            Validate();
        }

        private void GeneralCompetencesController_Changed(StrongReferencesBoxController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity();
            Validate();
        }

        private void GeneralCompetencesIntroductionController_Changed(StrongReferenceFieldController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity();
            Validate();
        }

        private void GeneralObjectivesController_Changed(StrongReferencesBoxController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity();
            Validate();
        }

        private void GeneralObjectivesIntroductionController_Changed(StrongReferenceFieldController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity();
            Validate();
        }

        private void UpdateEntity()
        {
            entity.Title = TextTitle.Text;
            entity.GeneralObjectivesIntroduction = Storage.LoadOrCreateEntity<CommonText>(generalObjectivesIntroductionController.StorageId, entity.StorageId);
            entity.GeneralObjectives.Set(Storage.LoadOrCreateEntities<CommonText>(generalObjectivesController.StorageIds, entity.StorageId));
            entity.GeneralCompetencesIntroduction = Storage.LoadOrCreateEntity<CommonText>(generalCompetencesIntroductionController.StorageId, entity.StorageId);
            entity.GeneralCompetences.Set(Storage.LoadOrCreateEntities<CommonText>(generalCompetencesController.StorageIds, entity.StorageId));
            entity.KeyCapacitiesIntroduction = Storage.LoadOrCreateEntity<CommonText>(keyCapacitiesIntroductionController.StorageId, entity.StorageId);
            entity.KeyCapacities.Set(Storage.LoadOrCreateEntities<CommonText>(keyCapacitiesController.StorageIds, entity.StorageId));

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
