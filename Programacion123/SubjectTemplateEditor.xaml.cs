using System.Windows;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace Programacion123
{
    /// <summary>
    /// Lógica de interacción para SubjectTemplateEditor.xaml
    /// </summary>
    public partial class SubjectTemplateEditor : Window, IEntityEditor<SubjectTemplate>
    {
        SubjectTemplate entity;
        string? parentStorageId;
        EntityFieldController<CommonText, CommonTextEditor, EntityPicker<CommonText> > generalObjectivesIntroductionController;
        EntityBoxController<CommonText, CommonTextEditor> generalObjectivesController;
        EntityFieldController<CommonText, CommonTextEditor, EntityPicker<CommonText>> generalCompetencesIntroductionController;
        EntityBoxController<CommonText, CommonTextEditor> generalCompetencesController;
        EntityFieldController<CommonText, CommonTextEditor, EntityPicker<CommonText>> learningResultsIntroductionController;
        EntityBoxController<LearningResult, LearningResultEditor> learningResultsController;
        EntityFieldController<CommonText, CommonTextEditor, EntityPicker<CommonText>> contentsIntroductionController;
        EntityBoxController<Content, ContentEditor> contentsController;

        public SubjectTemplateEditor()
        {
            InitializeComponent();

        }

        public SubjectTemplate GetEntity()
        {
            return entity;
        }

        public void SetEntity(SubjectTemplate _subjectTemplate, string? _parentStorageId)
        {
            entity = _subjectTemplate;
            parentStorageId = _parentStorageId;

            var configObjectivesIntroduction = EntityFieldConfiguration<CommonText>.CreateForTextBox(TextGeneralObjectivesIntroduction)
                                               .WithStorageId(entity.GeneralObjectivesIntroduction.StorageId)
                                               .WithParentStorageId(entity.StorageId)
                                               .WithNew(ButtonGeneralObjectivesIntroductionNew)
                                               .WithEdit(ButtonGeneralObjectivesIntroductionEdit)
                                               .WithTitleEditable(false)
                                               .WithEditorTitle("Introducción a los objetivos generales")
                                               .WithBlocker(Blocker);

            generalObjectivesIntroductionController = new(configObjectivesIntroduction);

            var configObjectives = EntityBoxConfiguration<CommonText>.CreateForList(ListBoxGeneralObjectives)
                                                        .WithParentStorageId(_subjectTemplate.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_subjectTemplate.GeneralObjectives.ToList()))
                                                        .WithPrefix(EntityBoxItemsPrefix.character)
                                                        .WithNew(ButtonGeneralObjectiveNew)
                                                        .WithEdit(ButtonGeneralObjectiveEdit)
                                                        .WithDelete(ButtonGeneralObjectiveDelete)
                                                        .WithUpDown(ButtonGeneralObjectiveUp, ButtonGeneralObjectiveDown)
                                                        .WithEditorTitle("Objetivo general")
                                                        .WithBlocker(Blocker);

            generalObjectivesController = new(configObjectives);

            var configCompetencesIntroduction = EntityFieldConfiguration<CommonText>.CreateForTextBox(TextGeneralCompetencesIntroduction)
                                               .WithStorageId(entity.GeneralCompetencesIntroduction.StorageId)
                                               .WithParentStorageId(entity.StorageId)
                                               .WithNew(ButtonGeneralCompetencesIntroductionNew)
                                               .WithEdit(ButtonGeneralCompetencesIntroductionEdit)
                                               .WithTitleEditable(false)
                                               .WithEditorTitle("Introducción a las competencias generales")
                                               .WithBlocker(Blocker);

            generalCompetencesIntroductionController = new(configCompetencesIntroduction);

            var configCompetences = EntityBoxConfiguration<CommonText>.CreateForList(ListBoxGeneralCompetences)
                                                        .WithParentStorageId(_subjectTemplate.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_subjectTemplate.GeneralCompetences.ToList()))
                                                        .WithPrefix(EntityBoxItemsPrefix.character)
                                                        .WithNew(ButtonGeneralCompetenceNew)
                                                        .WithEdit(ButtonGeneralCompetenceEdit)
                                                        .WithDelete(ButtonGeneralCompetenceDelete)
                                                        .WithUpDown(ButtonGeneralCompetenceUp, ButtonGeneralCompetenceDown)
                                                        .WithEditorTitle("Competencia general")
                                                        .WithBlocker(Blocker);

            generalCompetencesController = new(configCompetences);

            var configLearningResultsIntroduction = EntityFieldConfiguration<CommonText>.CreateForTextBox(TextLearningResultsIntroduction)
                                               .WithStorageId(entity.LearningResultsIntroduction.StorageId)
                                               .WithParentStorageId(entity.StorageId)
                                               .WithNew(ButtonLearningResultsIntroductionNew)
                                               .WithEdit(ButtonLearningResultsIntroductionEdit)
                                               .WithTitleEditable(false)
                                               .WithEditorTitle("Introducción a los resultados de aprendizaje")
                                               .WithBlocker(Blocker);

            learningResultsIntroductionController = new(configLearningResultsIntroduction);

            var configLearningResults = EntityBoxConfiguration<LearningResult>.CreateForList(ListBoxLearningResults)
                                                        .WithParentStorageId(_subjectTemplate.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<LearningResult>(_subjectTemplate.LearningResults.ToList()))
                                                        .WithPrefix(EntityBoxItemsPrefix.number)
                                                        .WithNew(ButtonLearningResultsNew)
                                                        .WithEdit(ButtonLearningResultsEdit)
                                                        .WithDelete(ButtonLearningResultsDelete)
                                                        .WithUpDown(ButtonLearningResultsUp, ButtonLearningResultsDown)
                                                        .WithEditorTitle("Resultado de aprendizaje")
                                                        .WithBlocker(Blocker);

            learningResultsController = new(configLearningResults);

            var configContentsIntroduction = EntityFieldConfiguration<CommonText>.CreateForTextBox(TextContentsIntroduction)
                                               .WithStorageId(entity.ContentsIntroduction.StorageId)
                                               .WithParentStorageId(entity.StorageId)
                                               .WithNew(ButtonContentsIntroductionNew)
                                               .WithEdit(ButtonContentsIntroductionEdit)
                                               .WithTitleEditable(false)
                                               .WithEditorTitle("Introducción a los contenidos")
                                               .WithBlocker(Blocker);

            contentsIntroductionController = new(configContentsIntroduction);

            var configContents = EntityBoxConfiguration<Content>.CreateForList(ListBoxContents)
                                                        .WithParentStorageId(_subjectTemplate.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<Content>(_subjectTemplate.Contents.ToList()))
                                                        .WithPrefix(EntityBoxItemsPrefix.number)
                                                        .WithNew(ButtonContentsNew)
                                                        .WithEdit(ButtonContentsEdit)
                                                        .WithDelete(ButtonContentsDelete)
                                                        .WithUpDown(ButtonContentsUp, ButtonContentsDown)
                                                        .WithEditorTitle("Contenido")
                                                        .WithBlocker(Blocker);

            contentsController = new(configContents);

            TextTitle.Text = _subjectTemplate.Title;

            TextSubjectName.Text = _subjectTemplate.SubjectName;
            TextSubjectCode.Text = _subjectTemplate.SubjectCode;
            ComboGradeType.SelectedIndex = (int)_subjectTemplate.GradeType;
            TextGradeName.Text = _subjectTemplate.GradeName;
            TextGradeFamilyName.Text = _subjectTemplate.GradeFamilyName;
            TextGradeClassroomHours.Text = _subjectTemplate.GradeClassroomHours.ToString();
            TextGradeCompanyHours.Text = _subjectTemplate.GradeCompanyHours.ToString();
        }

        private void UpdateEntity()
        {
            entity.Title = TextTitle.Text;
            entity.GeneralObjectivesIntroduction = Storage.LoadOrCreateEntity<CommonText>(generalObjectivesIntroductionController.StorageId, entity.StorageId);
            entity.GeneralObjectives.Set(Storage.LoadEntitiesFromList<CommonText>(generalObjectivesController.StorageIds, entity.StorageId));
            entity.GeneralCompetencesIntroduction = Storage.LoadOrCreateEntity<CommonText>(generalCompetencesIntroductionController.StorageId, entity.StorageId);
            entity.GeneralCompetences.Set(Storage.LoadEntitiesFromList<CommonText>(generalCompetencesController.StorageIds, entity.StorageId));

            entity.LearningResultsIntroduction = Storage.LoadOrCreateEntity<CommonText>(learningResultsIntroductionController.StorageId, entity.StorageId);
            entity.LearningResults.Set(Storage.LoadEntitiesFromList<LearningResult>(learningResultsController.StorageIds, entity.StorageId));
            entity.ContentsIntroduction = Storage.LoadOrCreateEntity<CommonText>(contentsIntroductionController.StorageId, entity.StorageId);
            entity.Contents.Set(Storage.LoadEntitiesFromList<Content>(contentsController.StorageIds, entity.StorageId));

            entity.SubjectName = TextSubjectName.Text;
            entity.SubjectCode = TextSubjectCode.Text;
            entity.GradeName = TextGradeName.Text;
            entity.GradeType = (GradeType)(ComboGradeType.SelectedIndex >= 0 ? ComboGradeType.SelectedIndex : 0);
            entity.GradeName = TextGradeName.Text;
            entity.GradeFamilyName = TextGradeFamilyName.Text;

            int number;
            entity.GradeClassroomHours = Int32.TryParse(TextGradeClassroomHours.Text, out number) ? number : 0;
            entity.GradeCompanyHours= Int32.TryParse(TextGradeCompanyHours.Text, out number) ? number : 0;
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
