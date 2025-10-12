using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Programacion123
{
    /// <summary>
    /// Lógica de interacción para GradeTemplateEditor.xaml
    /// </summary>
    public partial class GradeTemplateEditor : Window, IEntityEditor<GradeTemplate>
    {
        GradeTemplate entity;
        string? parentStorageId;
        StrongReferencesBoxController<CommonText, CommonTextEditor> generalObjectivesController;
        StrongReferencesBoxController<CommonText, CommonTextEditor> generalCompetencesController;
        StrongReferencesBoxController<CommonText, CommonTextEditor> keyCapacitiesController;
        StrongReferencesBoxController<CommonText, CommonTextEditor> commonTextsController;

        const uint flagUpdateTitle       = 1 << 0;
        const uint flagUpdateType        = 1 << 1;
        const uint flagUpdateName        = 1 << 2;
        const uint flagUpdateFamilyName  = 1 << 3;
        const uint flagUpdateObjectives  = 1 << 4;
        const uint flagUpdateCompetences = 1 << 5;
        const uint flagUpdateCapacities  = 1 << 6;
        const uint flagUpdateCommonTexts = 1 << 7;

        const uint flagUpdateAll = ~0U;

        public GradeTemplateEditor()
        {
            InitializeComponent();

        }

        public GradeTemplate GetEntity()
        {
            return entity;
        }

        async public Task InitEditorAsync(GradeTemplate _gradeTemplate, string? _parentStorageId)
        {
            _gradeTemplate.Save(_parentStorageId);

            entity = _gradeTemplate;
            parentStorageId = _parentStorageId;

            var configObjectives = StrongReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxGeneralObjectives)
                                                        .WithParentStorageId(_gradeTemplate.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_gradeTemplate.GeneralObjectives.ToList()))
                                                        .WithFormat(EntityFormatContent.Description, EntityFormatIndex.Character)
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

            var configCompetences = StrongReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxGeneralCompetences)
                                                        .WithParentStorageId(_gradeTemplate.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_gradeTemplate.GeneralCompetences.ToList()))
                                                        .WithFormat(EntityFormatContent.Description, EntityFormatIndex.Character)
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

            var configKeyCapacities = StrongReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxKeyCapacities)
                                                        .WithParentStorageId(_gradeTemplate.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_gradeTemplate.KeyCapacities.ToList()))
                                                        .WithFormat(EntityFormatContent.Title, EntityFormatIndex.Number)
                                                        .WithNew(ButtonKeyCapacitiesNew)
                                                        .WithEdit(ButtonKeyCapacitiesEdit)
                                                        .WithDelete(ButtonKeyCapacitiesDelete)
                                                        .WithUpDown(ButtonKeyCapacitiesUp, ButtonKeyCapacitiesDown)
                                                        .WithDeleteConfirmQuestion("Esto eliminará permanentemente la capacidad clave seleccionada. ¿Estás seguro/a?")
                                                        .WithEditorTitle("Capacidad clave")
                                                        .WithBlocker(Blocker);

            keyCapacitiesController = new(configKeyCapacities);

            keyCapacitiesController.Changed += KeyCapacitiesController_Changed;

            List<string> commonTextsIds = new();
            foreach (CommonTextId id in Enum.GetValues<CommonTextId>()) { commonTextsIds.Add(entity.CommonTexts[id].StorageId); }

            ListBoxCommonTexts.Background = new SolidColorBrush((Color)Application.Current.Resources["ColorLocked"]);

            var configCommonTexts = StrongReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxCommonTexts)
                                                        .WithParentStorageId(_gradeTemplate.StorageId)
                                                        .WithStorageIds(commonTextsIds)
                                                        .WithFormat(EntityFormatContent.Title, EntityFormatIndex.Number)
                                                        .WithTitleEditable(false)
                                                        .WithEdit(ButtonCommonTextsEdit)
                                                        .WithEditorTitle("Texto común")
                                                        .WithBlocker(Blocker);

            commonTextsController = new(configCommonTexts);

            commonTextsController.Changed += CommonTextsController_Changed;

            TextTitle.Text = _gradeTemplate.Title.Value;

            ComboType.SelectedIndex = (int)_gradeTemplate.GradeType.Value;
            TextName.Text = _gradeTemplate.GradeName.Value;
            TextFamilyName.Text = _gradeTemplate.GradeFamilyName.Value;

            ButtonClose.ToolTip = "Cerrar";

            TextTitle.TextChanged += TextTitle_TextChanged;
            TextName.TextChanged += TextName_TextChanged;
            TextFamilyName.TextChanged += TextFamilyName_TextChanged;

            Validate(true);

        }

        private void CommonTextsController_Changed(StrongReferencesBoxController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity(flagUpdateCommonTexts);
            Validate();
        }

        private void KeyCapacitiesController_Changed(StrongReferencesBoxController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity(flagUpdateCapacities);
            Validate();
        }

        private void TextFamilyName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateEntity(flagUpdateFamilyName);
            Validate();
        }

        private void TextName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateEntity(flagUpdateName);
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

        private void GeneralCompetencesController_Changed(StrongReferencesBoxController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity(flagUpdateCompetences);
            Validate();
        }


        private void GeneralObjectivesController_Changed(StrongReferencesBoxController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity(flagUpdateObjectives);
            Validate();
        }


        private void UpdateEntity(uint flags)
        {
            if(Flags.Test(flags, flagUpdateTitle)) { entity.Title.Value = TextTitle.Text; }
            if(Flags.Test(flags, flagUpdateObjectives)) { entity.GeneralObjectives.Set(Storage.LoadOrCreateEntities<CommonText>(generalObjectivesController.StorageIds, entity.StorageId)); }
            if(Flags.Test(flags, flagUpdateCompetences)) { entity.GeneralCompetences.Set(Storage.LoadOrCreateEntities<CommonText>(generalCompetencesController.StorageIds, entity.StorageId)); }
            if(Flags.Test(flags, flagUpdateCapacities)) { entity.KeyCapacities.Set(Storage.LoadOrCreateEntities<CommonText>(keyCapacitiesController.StorageIds, entity.StorageId)); }

            if(Flags.Test(flags, flagUpdateCommonTexts))
            {
                for (int i = 0; i < commonTextsController.StorageIds.Count; i++)
                { entity.CommonTexts.Set((CommonTextId)i, Storage.LoadOrCreateEntity<CommonText>(commonTextsController.StorageIds[i], entity.StorageId)); }
            }

            if(Flags.Test(flags, flagUpdateName)) { entity.GradeName.Value = TextName.Text; }
            if(Flags.Test(flags, flagUpdateType)) { entity.GradeType.Value = (GradeType)(ComboType.SelectedIndex >= 0 ? ComboType.SelectedIndex : 0); }
            if(Flags.Test(flags, flagUpdateFamilyName)) { entity.GradeFamilyName.Value = TextFamilyName.Text; }

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
    }

}
