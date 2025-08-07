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

        WeakReferenceFieldController<GradeTemplate, EntityPicker<GradeTemplate>> gradeTemplateController;
        WeakReferencesBoxController<CommonText, EntityPicker<CommonText> > generalObjectivesController;
        WeakReferencesBoxController<CommonText, EntityPicker<CommonText>> generalCompetencesController;

        StrongReferencesBoxController<LearningResult, LearningResultEditor> learningResultsController;
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

            var configGradeTemplate = WeakReferenceFieldConfiguration<GradeTemplate>.CreateForTextBox(TextGradeTemplate)
                                               .WithStorageId(entity.GradeTemplate?.StorageId)
                                               .WithPick(ButtonGradeTemplatePick)
                                               .WithFormat(EntityFormatContent.Title)
                                               .WithPickerTitle("Selecciona una plantilla de ciclo")
                                               .WithBlocker(Blocker);

            gradeTemplateController = new(configGradeTemplate);

            gradeTemplateController.Changed += GradeTemplateController_Changed;

            Func<List<string>> pickObjectivesQuery =
            () =>
            {
                List<string> objectivesStorageIds = new();
                if (entity.GradeTemplate != null)
                {
                    List<CommonText> objectivesList = entity.GradeTemplate.GeneralObjectives.ToList();
                    objectivesStorageIds = Storage.GetStorageIds<CommonText>(objectivesList);

                }

                return objectivesStorageIds;
            };

            Func<CommonText, int, string> objectivesFormatter =
                (e, i) =>
                {
                    bool canFormat;
                    GradeTemplate? gradeTemplate = null;
                    List<CommonText>? objectives = null;
                    int objectiveIndex = -1;

                    canFormat = (entity.GradeTemplate != null);
                    if (canFormat)
                    {
                        gradeTemplate = entity.GradeTemplate;
                    }
                    if (canFormat)
                    {
                        objectives = gradeTemplate.GeneralObjectives.ToList();
                        objectiveIndex = objectives.FindIndex(o => o.StorageId == e.StorageId);
                        canFormat = (objectiveIndex >= 0);
                    }

                    if (canFormat)
                    {
                        return String.Format("{0}: {1}", Utils.FormatLetterPrefixLowercase(objectiveIndex), e.Description);
                    }
                    else
                    {
                        return "<no se encuentra la referencia>";
                    }
                };



            var configObjectives = WeakReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxGeneralObjectives)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_subjectTemplate.GeneralObjectives.ToList()))
                                                        .WithFormatter(objectivesFormatter)
                                                        .WithPick(ButtonGeneralObjectiveReferenceAdd, ButtonGeneralObjectiveReferenceRemove)
                                                        .WithPickListQuery(pickObjectivesQuery)
                                                        .WithPickerTitle("Objetivos generales")
                                                        .WithBlocker(Blocker);

            generalObjectivesController = new(configObjectives);

            generalObjectivesController.Changed += GeneralObjectivesController_Changed;

            Func<List<string>> pickCompetencesQuery =
            () =>
            {
                List<string> competencesStorageIds = new();
                if (entity.GradeTemplate != null)
                {
                    List<CommonText> competencesList = entity.GradeTemplate.GeneralCompetences.ToList();
                    competencesStorageIds = Storage.GetStorageIds<CommonText>(competencesList);

                }

                return competencesStorageIds;
            };

            Func<CommonText, int, string> competencesFormatter =
                (e, i) =>
                {
                    bool canFormat;
                    GradeTemplate? gradeTemplate = null;
                    List<CommonText>? competences = null;
                    int competenceIndex = -1;

                    canFormat = (entity.GradeTemplate != null);
                    if (canFormat)
                    {
                        gradeTemplate = entity.GradeTemplate;
                    }
                    if (canFormat)
                    {
                        competences = gradeTemplate.GeneralCompetences.ToList();
                        competenceIndex = competences.FindIndex(c => c.StorageId == e.StorageId);
                        canFormat = (competenceIndex >= 0);
                    }

                    if (canFormat)
                    {
                        return String.Format("{0}: {1}", Utils.FormatLetterPrefixLowercase(competenceIndex), e.Description);
                    }
                    else
                    {
                        return "<no se encuentra la referencia>";
                    }
                };



            var configCompetences = WeakReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxGeneralCompetences)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_subjectTemplate.GeneralCompetences.ToList()))
                                                        .WithFormatter(competencesFormatter)
                                                        .WithPick(ButtonGeneralCompetenceReferenceAdd, ButtonGeneralCompetenceReferenceRemove)
                                                        .WithPickListQuery(pickCompetencesQuery)
                                                        .WithPickerTitle("Competencias generales")
                                                        .WithBlocker(Blocker);

            generalCompetencesController = new(configCompetences);

            generalCompetencesController.Changed += GeneralCompetencesController_Changed;

            Func<List<string>> pickKeyCapacitiesQuery =
            () =>
            {
                List<string> keyCapacitiesStorageIds = new();
                if (entity.GradeTemplate != null)
                {
                    List<CommonText> capacitiesList = entity.GradeTemplate.KeyCapacities.ToList();
                    keyCapacitiesStorageIds = Storage.GetStorageIds<CommonText>(capacitiesList);

                }

                return keyCapacitiesStorageIds;
            };

            Func<CommonText, int, string> keyCapacitiesFormatter =
                (e, i) =>
                {
                    bool canFormat;
                    GradeTemplate? gradeTemplate = null;
                    List<CommonText>? capacities = null;
                    int capacityIndex = -1;

                    canFormat = (entity.GradeTemplate != null);
                    if (canFormat)
                    {
                        gradeTemplate = entity.GradeTemplate;
                    }
                    if (canFormat)
                    {
                        capacities = gradeTemplate.KeyCapacities.ToList();
                        capacityIndex = capacities.FindIndex(c => c.StorageId == e.StorageId);
                        canFormat = (capacityIndex >= 0);
                    }

                    if (canFormat)
                    {
                        return String.Format("{0}: {1}", Utils.FormatLetterPrefixLowercase(capacityIndex), e.Title);
                    }
                    else
                    {
                        return "<no se encuentra la referencia>";
                    }
                };



            var configLearningResults = StrongReferencesBoxConfiguration<LearningResult>.CreateForList(ListBoxLearningResults)
                                                        .WithParentStorageId(_subjectTemplate.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<LearningResult>(_subjectTemplate.LearningResults.ToList()))
                                                        .WithFormatter((e, i) => String.Format("RA{0}: {1}", i + 1, e.Description))
                                                        .WithNew(ButtonLearningResultsNew)
                                                        .WithEdit(ButtonLearningResultsEdit)
                                                        .WithDelete(ButtonLearningResultsDelete)
                                                        .WithUpDown(ButtonLearningResultsUp, ButtonLearningResultsDown)
                                                        .WithDeleteConfirmQuestion("Esto eliminará permanentemente el resultado de aprendizaje seleccionado junto con los criterios definidos en él. ¿Estás seguro/a?")
                                                        .WithEditorTitle("Resultado de aprendizaje")
                                                        .WithBlocker(Blocker);

            learningResultsController = new(configLearningResults);

            learningResultsController.Changed += LearningResultsController_Changed;

            var configContents = StrongReferencesBoxConfiguration<Content>.CreateForList(ListBoxContents)
                                                        .WithParentStorageId(_subjectTemplate.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<Content>(_subjectTemplate.Contents.ToList()))
                                                        .WithFormat(EntityFormatContent.Description, EntityFormatIndex.Number)
                                                        .WithNew(ButtonContentsNew)
                                                        .WithEdit(ButtonContentsEdit)
                                                        .WithDelete(ButtonContentsDelete)
                                                        .WithUpDown(ButtonContentsUp, ButtonContentsDown)
                                                        .WithDeleteConfirmQuestion("Esto eliminará permanentemente el contenido seleccionado junto con los puntos definidos en él. ¿Estás seguro/a?")
                                                        .WithEditorTitle("Contenido")
                                                        .WithBlocker(Blocker);

            contentsController = new(configContents);

            contentsController.Changed += ContentsController_Changed;

            TextTitle.Text = _subjectTemplate.Title;

            TextSubjectName.Text = _subjectTemplate.SubjectName;
            TextSubjectCode.Text = _subjectTemplate.SubjectCode;
            TextGradeClassroomHours.Text = _subjectTemplate.GradeClassroomHours.ToString();
            TextGradeCompanyHours.Text = _subjectTemplate.GradeCompanyHours.ToString();


            TextTitle.TextChanged += TextTitle_TextChanged;
            TextSubjectName.TextChanged += TextSubjectName_TextChanged;
            TextSubjectCode.TextChanged += TextSubjectCode_TextChanged;
            TextGradeClassroomHours.TextChanged += TextGradeClassroomHours_TextChanged;
            TextGradeCompanyHours.TextChanged += TextGradeCompanyHours_TextChanged;

            Validate();

        }

        private void GradeTemplateController_Changed(WeakReferenceFieldController<GradeTemplate, EntityPicker<GradeTemplate>> controller)
        {
            UpdateEntity();
            Validate();
        }

        private void KeyCapacitiesController_Changed(WeakReferencesBoxController<CommonText, EntityPicker<CommonText> > controller)
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

        private void LearningResultsController_Changed(StrongReferencesBoxController<LearningResult, LearningResultEditor> controller)
        {
            UpdateEntity();
            Validate();
        }

        private void GeneralCompetencesController_Changed(WeakReferencesBoxController<CommonText, EntityPicker<CommonText>>  controller)
        {
            UpdateEntity();
            Validate();
        }

        private void GeneralObjectivesController_Changed(WeakReferencesBoxController<CommonText, EntityPicker<CommonText> > controller)
        {
            UpdateEntity();
            Validate();
        }


        private void UpdateEntity()
        {
            entity.Title = TextTitle.Text;

            entity.GradeTemplate = gradeTemplateController.GetEntity();

            entity.GeneralObjectives.Set(generalObjectivesController.GetSelectedEntities());
            entity.GeneralCompetences.Set(generalCompetencesController.GetSelectedEntities());

            entity.LearningResults.Set(Storage.LoadOrCreateEntities<LearningResult>(learningResultsController.StorageIds, entity.StorageId));
            entity.Contents.Set(Storage.LoadOrCreateEntities<Content>(contentsController.StorageIds, entity.StorageId));

            entity.SubjectName = TextSubjectName.Text;
            entity.SubjectCode = TextSubjectCode.Text;

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
