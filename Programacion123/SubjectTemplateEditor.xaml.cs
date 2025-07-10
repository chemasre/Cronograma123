using System.Windows;

namespace Programacion123
{
    /// <summary>
    /// Lógica de interacción para SubjectTemplateEditor.xaml
    /// </summary>
    public partial class SubjectTemplateEditor : Window, EntityEditor<SubjectTemplate>
    {
        SubjectTemplate entity;
        string? parentStorageId;
        EntityBoxController<CommonText, CommonTextEditor> generalObjectivesController;
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

            var configObjectives = EntityBoxConfiguration.CreateForList(ListBoxGeneralObjectives)
                                                        .WithParentStorageId(_subjectTemplate.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_subjectTemplate.GeneralObjectives.ToList()))
                                                        .WithPrefix(EntityBoxItemsPrefix.character)
                                                        .WithNew(ButtonGeneralObjectiveNew)
                                                        .WithEdit(ButtonGeneralObjectiveEdit)
                                                        .WithDelete(ButtonGeneralObjectiveDelete)
                                                        .WithUpDown(ButtonGeneralObjectiveUp, ButtonGeneralObjectiveDown);

            generalObjectivesController = new(configObjectives);

            var configCompetences = EntityBoxConfiguration.CreateForList(ListBoxGeneralCompetences)
                                                        .WithParentStorageId(_subjectTemplate.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_subjectTemplate.GeneralCompetences.ToList()))
                                                        .WithPrefix(EntityBoxItemsPrefix.character)
                                                        .WithNew(ButtonGeneralCompetenceNew)
                                                        .WithEdit(ButtonGeneralCompetenceEdit)
                                                        .WithDelete(ButtonGeneralCompetenceDelete)
                                                        .WithUpDown(ButtonGeneralCompetenceUp, ButtonGeneralCompetenceDown);

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
    }
}
