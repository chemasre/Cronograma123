using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Programacion123
{
    /// <summary>
    /// Lógica de interacción para ContentEditor.xaml
    /// </summary>
    public partial class ContentEditor : Window, IEntityEditor<Content>
    {
        string? parentStorageId;
        Content entity;

        StrongReferencesBoxController<CommonText, CommonTextEditor> pointsController;

        //const uint flagUpdateTitle          = 1 << 0;
        const uint flagUpdateDescription    = 1 << 1;
        const uint flagUpdatePoints         = 1 << 2;

        const uint flagUpdateAll = ~0U;

        public ContentEditor()
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


        public void InitEditor(Content _entity, string? _parentStorageId = null)
        {
            _entity.Save(_parentStorageId);

            parentStorageId = _parentStorageId;
            entity = _entity;

            SubjectTemplate template = new();
            template = Storage.FindEntity<SubjectTemplate>(Storage.FindParentStorageId(_entity.StorageId, _entity.StorageClassId), null);

            Func<CommonText, int, string> formatter =
                (e, i) =>
                {
                    string contentStorageId = Storage.FindParentStorageId(e.StorageId, e.StorageClassId);
                    int contentIndex = template.Contents.ToList().FindIndex(c => c.StorageId == contentStorageId);
                    return String.Format("{0}.{1}: {2}", contentIndex + 1, i + 1, e.Description.Value);
                };

            var configPoints = StrongReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxPoints)
                                                        .WithParentStorageId(_entity.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_entity.Points.ToList()))
                                                        .WithFormatter(formatter)
                                                        .WithTitleEditable(false)
                                                        .WithNew(ButtonPointNew)
                                                        .WithEdit(ButtonPointEdit)
                                                        .WithDelete(ButtonPointDelete)
                                                        .WithUpDown(ButtonPointUp, ButtonPointDown)
                                                        .WithDeleteConfirmQuestion("Esto eliminará la actividad seleccionada. ¿Estás seguro/a?")
                                                        .WithEditorTitle("Punto de contenido")
                                                        .WithBlocker(Blocker)
                                                        .WithDialogsOwner(this);

            pointsController = new(configPoints);

            pointsController.Changed += PointsController_Changed;

            TextBoxDescription.Text = _entity.Description.Value;

            TextBoxDescription.TextChanged += TextBoxDescription_TextChanged;

            ButtonClose.ToolTip = "Cerrar";


            Validate(true);

        }

        private void PointsController_Changed(StrongReferencesBoxController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity(flagUpdatePoints);
            Validate();
        }

        public Content GetEntity()
        {
            return entity;
        }

        void UpdateEntity(uint flags)
        {
            if(Flags.Test(flags, flagUpdateDescription)) { entity.Description.Value = TextBoxDescription.Text; Utils.Log("Updated", "description"); }

            //entity.Description = TextBoxDescription.Document.ToString().Trim();

            if(Flags.Test(flags, flagUpdatePoints)) { entity.Points.Set(Storage.LoadOrCreateEntities<CommonText>(pointsController.StorageIds, entity.StorageId)); Utils.Log("Updated", "contentPoints"); }

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

        public Task InitEditorAsync(Content entity, string? _parentStorageId)
        {
            throw new NotImplementedException();
        }
    }
}
