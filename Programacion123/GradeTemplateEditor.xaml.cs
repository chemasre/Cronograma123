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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Programacion123
{
    /// <summary>
    /// Lógica de interacción para GradeTemplateEditor.xaml
    /// </summary>
    public partial class GradeTemplateEditor : Window, IEntityEditor<GradeTemplate>
    {
        GradeTemplate entity;
        string? parentStorageId;
        StrongReferenceFieldController<CommonText, CommonTextEditor> generalObjectivesIntroductionController;
        StrongReferencesBoxController<CommonText, CommonTextEditor> generalObjectivesController;
        StrongReferenceFieldController<CommonText, CommonTextEditor> generalCompetencesIntroductionController;
        StrongReferencesBoxController<CommonText, CommonTextEditor> generalCompetencesController;
        StrongReferenceFieldController<CommonText, CommonTextEditor> keyCapacitiesIntroductionController;
        StrongReferencesBoxController<CommonText, CommonTextEditor> keyCapacitiesController;
        StrongReferenceFieldController<CommonText, CommonTextEditor> learningResultsIntroductionController;
        StrongReferencesBoxController<LearningResult, LearningResultEditor> learningResultsController;
        StrongReferenceFieldController<CommonText, CommonTextEditor> contentsIntroductionController;
        StrongReferencesBoxController<Content, ContentEditor> contentsController;

        public GradeTemplateEditor()
        {
            InitializeComponent();

        }

        public GradeTemplate GetEntity()
        {
            return entity;
        }

        public void InitEditor(GradeTemplate _gradeTemplate, string? _parentStorageId)
        {
            _gradeTemplate.Save(_parentStorageId);

            entity = _gradeTemplate;
            parentStorageId = _parentStorageId;


            var configObjectivesIntroduction = StrongReferenceFieldConfiguration<CommonText>.CreateForTextBox(TextGeneralObjectivesIntroduction)
                                               .WithStorageId(entity.GeneralObjectivesIntroduction.StorageId)
                                               .WithParentStorageId(entity.StorageId)
                                               .WithFormat(EntityFormatContent.description)
                                               .WithNew(ButtonGeneralObjectivesIntroductionNew)
                                               .WithEdit(ButtonGeneralObjectivesIntroductionEdit)
                                               .WithReplaceConfirmQuestion("Esto sustituirá la introducción anterior por una nueva. ¿Estás seguro/a?")
                                               .WithTitleEditable(false)
                                               .WithEditorTitle("Introducción a los objetivos generales")
                                               .WithBlocker(Blocker);

            generalObjectivesIntroductionController = new(configObjectivesIntroduction);

            generalObjectivesIntroductionController.Changed += GeneralObjectivesIntroductionController_Changed;

            var configObjectives = StrongReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxGeneralObjectives)
                                                        .WithParentStorageId(_gradeTemplate.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_gradeTemplate.GeneralObjectives.ToList()))
                                                        .WithFormat(EntityFormatContent.description, EntityFormatIndex.character)
                                                        .WithTitleEditable(false)
                                                        .WithNew(ButtonGeneralObjectiveNew)
                                                        .WithEdit(ButtonGeneralObjectiveEdit)
                                                        .WithDelete(ButtonGeneralObjectiveDelete)
                                                        .WithUpDown(ButtonGeneralObjectiveUp, ButtonGeneralObjectiveDown)
                                                        .WithDeleteConfirmQuestion("Esto eliminará permanentemente el objetivo seleccionado. ¿Estás seguro/a?")
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
                                               .WithReplaceConfirmQuestion("Esto sustituirá la introducción anterior por una nueva. ¿Estás seguro/a?")
                                               .WithTitleEditable(false)
                                               .WithEditorTitle("Introducción a las competencias generales")
                                               .WithBlocker(Blocker);

            generalCompetencesIntroductionController = new(configCompetencesIntroduction);

            generalCompetencesIntroductionController.Changed += GeneralCompetencesIntroductionController_Changed;

            var configCompetences = StrongReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxGeneralCompetences)
                                                        .WithParentStorageId(_gradeTemplate.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_gradeTemplate.GeneralCompetences.ToList()))
                                                        .WithFormat(EntityFormatContent.description, EntityFormatIndex.character)
                                                        .WithTitleEditable(false)
                                                        .WithNew(ButtonGeneralCompetenceNew)
                                                        .WithEdit(ButtonGeneralCompetenceEdit)
                                                        .WithDelete(ButtonGeneralCompetenceDelete)
                                                        .WithUpDown(ButtonGeneralCompetenceUp, ButtonGeneralCompetenceDown)
                                                        .WithDeleteConfirmQuestion("Esto eliminará permanentemente la competencia general seleccionada. ¿Estás seguro/a?")
                                                        .WithEditorTitle("Competencias profesionales, personales y sociales")
                                                        .WithBlocker(Blocker);

            generalCompetencesController = new(configCompetences);

            generalCompetencesController.Changed += GeneralCompetencesController_Changed;

            var configKeyCapacitiesIntroduction = StrongReferenceFieldConfiguration<CommonText>.CreateForTextBox(TextKeyCapacitiesIntroduction)
                                               .WithStorageId(entity.KeyCapacitiesIntroduction.StorageId)
                                               .WithParentStorageId(entity.StorageId)
                                               .WithFormat(EntityFormatContent.description)
                                               .WithNew(ButtonKeyCapacitiesIntroductionNew)
                                               .WithEdit(ButtonKeyCapacitiesIntroductionEdit)
                                               .WithReplaceConfirmQuestion("Esto sustituirá la introducción anterior por una nueva. ¿Estás seguro/a?")
                                               .WithTitleEditable(false)
                                               .WithEditorTitle("Introducción a las competencias clave")
                                               .WithBlocker(Blocker);

            keyCapacitiesIntroductionController = new(configKeyCapacitiesIntroduction);

            keyCapacitiesIntroductionController.Changed += KeyCapacitiesIntroductionController_Changed;

            var configKeyCapacities = StrongReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxKeyCapacities)
                                                        .WithParentStorageId(_gradeTemplate.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_gradeTemplate.KeyCapacities.ToList()))
                                                        .WithFormat(EntityFormatContent.title, EntityFormatIndex.number)
                                                        .WithNew(ButtonKeyCapacitiesNew)
                                                        .WithEdit(ButtonKeyCapacitiesEdit)
                                                        .WithDelete(ButtonKeyCapacitiesDelete)
                                                        .WithUpDown(ButtonKeyCapacitiesUp, ButtonKeyCapacitiesDown)
                                                        .WithDeleteConfirmQuestion("Esto eliminará permanentemente la capacidad clave seleccionada. ¿Estás seguro/a?")
                                                        .WithEditorTitle("Capacidad clave")
                                                        .WithBlocker(Blocker);

            keyCapacitiesController = new(configKeyCapacities);

            keyCapacitiesController.Changed += KeyCapacitiesController_Changed;

            TextTitle.Text = _gradeTemplate.Title;

            ComboType.SelectedIndex = (int)_gradeTemplate.GradeType;
            TextName.Text = _gradeTemplate.GradeName;
            TextFamilyName.Text = _gradeTemplate.GradeFamilyName;

            TextTitle.TextChanged += TextTitle_TextChanged;
            TextName.TextChanged += TextName_TextChanged;
            TextFamilyName.TextChanged += TextFamilyName_TextChanged;

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

        private void TextFamilyName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateEntity();
            Validate();
        }

        private void TextName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
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

            entity.GradeName = TextName.Text;
            entity.GradeType = (GradeType)(ComboType.SelectedIndex >= 0 ? ComboType.SelectedIndex : 0);
            entity.GradeFamilyName = TextFamilyName.Text;

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
