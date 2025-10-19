using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Programacion123
{
    /// <summary>
    /// Lógica de interacción para LearningResultEditor.xaml
    /// </summary>
    public partial class LearningResultEditor : Window, IEntityEditor<LearningResult>
    {
        string? parentStorageId;
        LearningResult entity;

        StrongReferencesBoxController<CommonText, CommonTextEditor> criteriasController;

        const uint flagUpdateDescription = 1 << 1;
        const uint flagUpdateCriterias = 1 << 1;

        const uint flagUpdateAll = ~0U;

        public LearningResultEditor()
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


        public void InitEditor(LearningResult _entity, string? _parentStorageId = null)
        {
            _entity.Save(_parentStorageId);

            parentStorageId = _parentStorageId;
            entity = _entity;

            SubjectTemplate template = new();
            template = Storage.FindEntity<SubjectTemplate>(Storage.FindParentStorageId(_entity.StorageId, _entity.StorageClassId), null);

            Func<CommonText, int, string> formatter =
                (e, i) =>
                {
                    string resultStorageId = Storage.FindParentStorageId(e.StorageId, e.StorageClassId);
                    int resultIndex = template.LearningResults.ToList().FindIndex(r => r.StorageId == resultStorageId);
                    return String.Format("RA{0}.{1}: {2}", resultIndex + 1, i + 1, e.Description.Value);
                };

            var configCriterias = StrongReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxCriterias)
                                                        .WithParentStorageId(_entity.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_entity.Criterias.ToList()))
                                                        .WithFormatter(formatter)
                                                        .WithTitleEditable(false)
                                                        .WithNew(ButtonCriteriaNew)
                                                        .WithEdit(ButtonCriteriaEdit)
                                                        .WithDelete(ButtonCriteriaDelete)
                                                        .WithDeleteConfirmQuestion("Esto eliminará permanentemente el criterio seleccionado. ¿Estás seguro/a?")
                                                        .WithUpDown(ButtonCriteriaUp, ButtonCriteriaDown)
                                                        .WithEditorTitle("Criterio")
                                                        .WithBlocker(Blocker)
                                                        .WithDialogsOwner(this);

            criteriasController = new(configCriterias);

            criteriasController.Changed += CriteriasController_Changed;

            TextBoxDescription.Text = _entity.Description.Value;

            TextBoxDescription.TextChanged += TextBoxDescription_TextChanged;

            ButtonClose.ToolTip = "Cerrar";

            Validate(true);

        }

        private void CriteriasController_Changed(StrongReferencesBoxController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity(flagUpdateCriterias);
            Validate();
        }

        public LearningResult GetEntity()
        {
            return entity;
        }

        void UpdateEntity(uint flags)
        {
            if(Flags.Test(flags, flagUpdateDescription))
            {
                entity.Description.Value = TextBoxDescription.Text;
                //entity.Description = TextBoxDescription.Document.ToString().Trim();
                Utils.Log("Updated", "description");
            }

            if(Flags.Test(flags, flagUpdateCriterias))
            {
                entity.Criterias.Set(Storage.LoadOrCreateEntities<CommonText>(criteriasController.StorageIds, entity.StorageId));
                Utils.Log("Updated", "criterias");
            }

            entity.Save(parentStorageId);
        }

        void Validate(bool force = false)
        {
            ValidationResult validation = entity.Validate(force);

            string colorResource = (validation.code == ValidationCode.success ? "ColorValid" : "ColorInvalid");
            BorderValidation.Background = new SolidColorBrush((Color)Application.Current.Resources[colorResource]);
            TextValidation.Text = validation.ToString();

        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            UpdateEntity(flagUpdateAll);
            //entity.Save(parentStorageId);

            TextBoxDescription.TextChanged -= TextBoxDescription_TextChanged;

            Close();

        }


        private void TextBoxDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateEntity(flagUpdateDescription);
            Validate();
        }

        public void SetEntityTitleEditable(bool editable)
        {
            // Nothing to do
        }

        public void SetEditorTitle(string title)
        {
            TextEditorTitle.Text = title;
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        public Task InitEditorAsync(LearningResult entity, string? _parentStorageId)
        {
            throw new NotImplementedException();
        }
    }
}
