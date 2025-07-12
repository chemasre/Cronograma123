using System.Windows;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace Programacion123
{
    /// <summary>
    /// Lógica de interacción para SubjectTemplateEditor.xaml
    /// </summary>
    public partial class SubjectTemplateEditor : Window, EntityEditor<SubjectTemplate>
    {
        SubjectTemplate entity;
        string? parentStorageId;
        EntityFieldController<CommonText, CommonTextEditor> generalObjectivesIntroductionController;
        EntityBoxController<CommonText, CommonTextEditor> generalObjectivesController;
        EntityFieldController<CommonText, CommonTextEditor> generalCompetencesIntroductionController;
        EntityBoxController<CommonText, CommonTextEditor> generalCompetencesController;

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

            var configObjectivesIntroduction = EntityFieldConfiguration.CreateForTextBox(TextGeneralObjectivesIntroduction)
                                               .WithStorageId(entity.GeneralObjectivesIntroduction.StorageId)
                                               .WithParentStorageId(entity.StorageId)
                                               .WithNew(ButtonGeneralObjectivesIntroductionNew)
                                               .WithEdit(ButtonGeneralObjectivesIntroductionEdit)
                                               .WithTitleEditable(false)
                                               .WithEditorTitle("Introducción a los objetivos generales")
                                               .WithBlocker(Blocker);

            generalObjectivesIntroductionController = new(configObjectivesIntroduction);

            var configObjectives = EntityBoxConfiguration.CreateForList(ListBoxGeneralObjectives)
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

            var configCompetencesIntroduction = EntityFieldConfiguration.CreateForTextBox(TextGeneralCompetencesIntroduction)
                                               .WithStorageId(entity.GeneralCompetencesIntroduction.StorageId)
                                               .WithParentStorageId(entity.StorageId)
                                               .WithNew(ButtonGeneralCompetencesIntroductionNew)
                                               .WithEdit(ButtonGeneralCompetencesIntroductionEdit)
                                               .WithTitleEditable(false)
                                               .WithEditorTitle("Introducción a las competencias generales")
                                               .WithBlocker(Blocker);

            generalCompetencesIntroductionController = new(configCompetencesIntroduction);

            var configCompetences = EntityBoxConfiguration.CreateForList(ListBoxGeneralCompetences)
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

            TextTitle.Text = _subjectTemplate.Title;
        }

        private void UpdateEntity()
        {
            entity.Title = TextTitle.Text;
            entity.GeneralObjectives.Set(Storage.LoadEntities<CommonText>(generalObjectivesController.StorageIds, entity.StorageId));
            entity.GeneralCompetences.Set(Storage.LoadEntities<CommonText>(generalCompetencesController.StorageIds, entity.StorageId));
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
