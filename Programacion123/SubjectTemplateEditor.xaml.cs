using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
        WeakReferencesBoxController<CommonText, EntityPicker<CommonText>> generalObjectivesController;
        WeakReferencesBoxController<CommonText, EntityPicker<CommonText>> generalCompetencesController;

        StrongReferencesBoxController<LearningResult, LearningResultEditor> learningResultsController;
        StrongReferencesBoxController<Content, ContentEditor> contentsController;

        const uint flagUpdateTitle              = 1 << 0;
        const uint flagUpdateGradeTemplate      = 1 << 1;
        const uint flagUpdateObjectives         = 1 << 2;
        const uint flagUpdateCompetences        = 1 << 3;
        const uint flagUpdateLearningResults    = 1 << 4;
        const uint flagUpdateContents           = 1 << 5;
        const uint flagUpdateSubjectName        = 1 << 6;
        const uint flagUpdateSubjectCode        = 1 << 7;
        const uint flagUpdateGradeClassroomHours = 1 << 8;
        const uint flagUpdateGradeCompanyHours  = 1 << 9;

        const uint flagUpdateAll = ~0U;

        public SubjectTemplateEditor()
        {
            InitializeComponent();

            if (Switches.featureChristmasThemeEnabled)
            {
                if (Utils.IsChristmas()) { ChristmasThemeApply(); }
            }

        }

        void ChristmasThemeApply()
        {
            ValidatorCapy.Source = new BitmapImage(new Uri("pack://application:,,,/Images/ValidatorCapyBig_Winter.png"));
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
                                               .WithStorageId(entity.GradeTemplate.Value?.StorageId)
                                               .WithPick(ButtonGradeTemplatePick)
                                               .WithFormat(EntityFormatContent.Title)
                                               .WithPickerTitle("Selecciona una plantilla de ciclo")
                                               .WithBlocker(Blocker)
                                               .WithDialogsOwner(this);


            gradeTemplateController = new(configGradeTemplate);

            gradeTemplateController.Changed += GradeTemplateController_Changed;

            Func<List<string>> pickObjectivesQuery =
            () =>
            {
                List<string> objectivesStorageIds = new();
                if (entity.GradeTemplate.Value != null)
                {
                    List<CommonText> objectivesList = entity.GradeTemplate.Value.GeneralObjectives.ToList();
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

                    canFormat = (entity.GradeTemplate.Value != null);
                    if (canFormat)
                    {
                        gradeTemplate = entity.GradeTemplate.Value;
                    }
                    if (canFormat)
                    {
                        objectives = gradeTemplate.GeneralObjectives.ToList();
                        objectiveIndex = objectives.FindIndex(o => o.StorageId == e.StorageId);
                        canFormat = (objectiveIndex >= 0);
                    }

                    if (canFormat)
                    {
                        return String.Format("{0}: {1}", Utils.FormatLetterPrefixLowercase(objectiveIndex), e.Description.Value);
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
                                                        .WithBlocker(Blocker)
                                                        .WithDialogsOwner(this);


            generalObjectivesController = new(configObjectives);

            generalObjectivesController.Changed += GeneralObjectivesController_Changed;

            Func<List<string>> pickCompetencesQuery =
            () =>
            {
                List<string> competencesStorageIds = new();
                if (entity.GradeTemplate.Value != null)
                {
                    List<CommonText> competencesList = entity.GradeTemplate.Value.GeneralCompetences.ToList();
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

                    canFormat = (entity.GradeTemplate.Value != null);
                    if (canFormat)
                    {
                        gradeTemplate = entity.GradeTemplate.Value;
                    }
                    if (canFormat)
                    {
                        competences = gradeTemplate.GeneralCompetences.ToList();
                        competenceIndex = competences.FindIndex(c => c.StorageId == e.StorageId);
                        canFormat = (competenceIndex >= 0);
                    }

                    if (canFormat)
                    {
                        return String.Format("{0}: {1}", Utils.FormatLetterPrefixLowercase(competenceIndex), e.Description.Value);
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
                                                        .WithBlocker(Blocker)
                                                        .WithDialogsOwner(this);


            generalCompetencesController = new(configCompetences);

            generalCompetencesController.Changed += GeneralCompetencesController_Changed;

            Func<List<string>> pickKeyCapacitiesQuery =
            () =>
            {
                List<string> keyCapacitiesStorageIds = new();
                if (entity.GradeTemplate.Value != null)
                {
                    List<CommonText> capacitiesList = entity.GradeTemplate.Value.KeyCapacities.ToList();
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

                    canFormat = (entity.GradeTemplate.Value != null);
                    if (canFormat)
                    {
                        gradeTemplate = entity.GradeTemplate.Value;
                    }
                    if (canFormat)
                    {
                        capacities = gradeTemplate.KeyCapacities.ToList();
                        capacityIndex = capacities.FindIndex(c => c.StorageId == e.StorageId);
                        canFormat = (capacityIndex >= 0);
                    }

                    if (canFormat)
                    {
                        return String.Format("{0}: {1}", Utils.FormatLetterPrefixLowercase(capacityIndex), e.Title.Value);
                    }
                    else
                    {
                        return "<no se encuentra la referencia>";
                    }
                };



            var configLearningResults = StrongReferencesBoxConfiguration<LearningResult>.CreateForList(ListBoxLearningResults)
                                                        .WithParentStorageId(_subjectTemplate.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<LearningResult>(_subjectTemplate.LearningResults.ToList()))
                                                        .WithFormatter((e, i) => String.Format("RA{0}: {1}", i + 1, e.Description.Value))
                                                        .WithNew(ButtonLearningResultsNew)
                                                        .WithEdit(ButtonLearningResultsEdit)
                                                        .WithDelete(ButtonLearningResultsDelete)
                                                        .WithUpDown(ButtonLearningResultsUp, ButtonLearningResultsDown)
                                                        .WithDeleteConfirmQuestion("Esto eliminará permanentemente el resultado de aprendizaje seleccionado junto con los criterios definidos en él. ¿Estás seguro/a?")
                                                        .WithEditorTitle("Resultado de aprendizaje")
                                                        .WithBlocker(Blocker)
                                                        .WithDialogsOwner(this);

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
                                                        .WithBlocker(Blocker)
                                                        .WithDialogsOwner(this);

            contentsController = new(configContents);

            contentsController.Changed += ContentsController_Changed;

            TextTitle.Text = _subjectTemplate.Title.Value;

            TextSubjectName.Text = _subjectTemplate.SubjectName.Value;
            TextSubjectCode.Text = _subjectTemplate.SubjectCode.Value;
            TextGradeClassroomHours.Text = _subjectTemplate.GradeClassroomHours.Value.ToString();
            TextGradeCompanyHours.Text = _subjectTemplate.GradeCompanyHours.Value.ToString();

            ButtonClose.ToolTip = "Cerrar";

            TextTitle.TextChanged += TextTitle_TextChanged;
            TextSubjectName.TextChanged += TextSubjectName_TextChanged;
            TextSubjectCode.TextChanged += TextSubjectCode_TextChanged;
            TextGradeClassroomHours.TextChanged += TextGradeClassroomHours_TextChanged;
            TextGradeCompanyHours.TextChanged += TextGradeCompanyHours_TextChanged;

            Validate(true);

        }

        private void GradeTemplateController_Changed(WeakReferenceFieldController<GradeTemplate, EntityPicker<GradeTemplate>> controller)
        {
            generalObjectivesController.Clear();    
            generalCompetencesController.Clear();    
            UpdateEntity(flagUpdateGradeTemplate);
            Validate();
        }

        private void TextSubjectCode_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateEntity(flagUpdateSubjectCode);
            Validate();
        }

        private void TextGradeCompanyHours_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            int result;
            if (!Int32.TryParse(TextGradeCompanyHours.Text, out result)) { TextGradeCompanyHours.Text = ""; }

            UpdateEntity(flagUpdateGradeCompanyHours);
            Validate();
        }

        private void TextGradeClassroomHours_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            int result;
            if (!Int32.TryParse(TextGradeClassroomHours.Text, out result)) { TextGradeClassroomHours.Text = ""; }

            UpdateEntity(flagUpdateGradeClassroomHours);
            Validate();
        }

        private void TextSubjectName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateEntity(flagUpdateSubjectName);
            Validate();
        }

        private void TextTitle_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateEntity(flagUpdateTitle);
            Validate();
        }

        void Validate(bool force = false)
        {
            ValidationResult validation = entity.Validate(force);

            string colorResource = (validation.code == ValidationCode.success ? "ColorValid" : "ColorInvalid");
            BorderValidation.Background = new SolidColorBrush((Color)Application.Current.Resources[colorResource]);
            TextValidation.Text = validation.ToString();

        }

        private void ContentsController_Changed(StrongReferencesBoxController<Content, ContentEditor> controller)
        {
            UpdateEntity(flagUpdateContents);
            Validate();
        }

        private void LearningResultsController_Changed(StrongReferencesBoxController<LearningResult, LearningResultEditor> controller)
        {
            UpdateEntity(flagUpdateLearningResults);
            Validate();
        }

        private void GeneralCompetencesController_Changed(WeakReferencesBoxController<CommonText, EntityPicker<CommonText>> controller)
        {
            UpdateEntity(flagUpdateCompetences);
            Validate();
        }

        private void GeneralObjectivesController_Changed(WeakReferencesBoxController<CommonText, EntityPicker<CommonText>> controller)
        {
            UpdateEntity(flagUpdateObjectives);
            Validate();
        }


        private void UpdateEntity(uint flags)
        {
            if(Flags.Test(flags, flagUpdateTitle))
            {
                entity.Title.Value = TextTitle.Text;
                Utils.Log("Updated", "title");
            }

            if(Flags.Test(flags, flagUpdateGradeTemplate))
            {
                entity.GradeTemplate.Value = gradeTemplateController.GetEntity();
                Utils.Log("Updated", "gradeTemplate");
            }

            if(Flags.Test(flags, flagUpdateObjectives | flagUpdateGradeTemplate))
            {
                entity.GeneralObjectives.Set(generalObjectivesController.GetSelectedEntities());
                Utils.Log("Updated", "generalObjectives");
            }

            if(Flags.Test(flags, flagUpdateCompetences | flagUpdateGradeTemplate))
            {
                entity.GeneralCompetences.Set(generalCompetencesController.GetSelectedEntities());
                Utils.Log("Updated", "generalCompetences");
            }

            if(Flags.Test(flags, flagUpdateLearningResults))
            {
                entity.LearningResults.Set(Storage.LoadOrCreateEntities<LearningResult>(learningResultsController.StorageIds, entity.StorageId));
                Utils.Log("Updated", "learningResults");
            }

            if(Flags.Test(flags, flagUpdateContents))
            {
                entity.Contents.Set(Storage.LoadOrCreateEntities<Content>(contentsController.StorageIds, entity.StorageId));
                Utils.Log("Updated", "contents");
            }

            if(Flags.Test(flags, flagUpdateSubjectName))
            {
                entity.SubjectName.Value = TextSubjectName.Text;
                Utils.Log("Updated", "subjectName");
            }

            if(Flags.Test(flags, flagUpdateSubjectCode))
            {
                entity.SubjectCode.Value = TextSubjectCode.Text;
                Utils.Log("Updated", "subjectCode");
            }

            if(Flags.Test(flags, flagUpdateGradeClassroomHours))
            {
                int number;
                entity.GradeClassroomHours.Value = Int32.TryParse(TextGradeClassroomHours.Text, out number) ? number : 0;
                Utils.Log("Updated", "gradeClassRoomHours");

            }

            if(Flags.Test(flags, flagUpdateGradeCompanyHours))
            {
                int number;
                entity.GradeCompanyHours.Value = Int32.TryParse(TextGradeCompanyHours.Text, out number) ? number : 0;
                Utils.Log("Updated", "gradeCompanyHours");
            }

            entity.Save(parentStorageId);
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {

            UpdateEntity(flagUpdateAll);
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

        public Task InitEditorAsync(SubjectTemplate entity, string? _parentStorageId)
        {
            throw new NotImplementedException();
        }
    }
}
