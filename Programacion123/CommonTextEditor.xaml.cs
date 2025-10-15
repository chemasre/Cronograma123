using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Programacion123
{
    /// <summary>
    /// Lógica de interacción para CommonTextEditor.xaml
    /// </summary>
    public partial class CommonTextEditor : Window, IEntityEditor<CommonText>
    {
        string? parentStorageId;
        CommonText entity;

        bool titleEditable;

        const uint flagUpdateTitle          = 1 << 0;
        const uint flagUpdateDescription    = 1 << 1;
        
        const uint flagUpdateAll = ~0U;

        public CommonTextEditor()
        {
            InitializeComponent();

            titleEditable = true;
        }

        public void InitEditor(CommonText _entity, string? _parentStorageId = null)
        {
            _entity.Save(_parentStorageId);

            parentStorageId = _parentStorageId;
            entity = _entity;

            TextTitle.Text = _entity.Title.Value;

            ButtonClose.ToolTip = "Cerrar";

            if (titleEditable)
            {
                LabelTitle.Visibility = Visibility.Visible;
                LabelDescription.Visibility = Visibility.Visible;
                BorderDescriptionBase.Visibility = Visibility.Visible;
                TextTitle.Visibility = Visibility.Visible;
                TextBoxDescription.Visibility = Visibility.Visible;
                NoTitleBorderDescriptionBase.Visibility = Visibility.Hidden;
                NoTitleTextBoxDescription.Visibility = Visibility.Hidden;
                TextBoxDescription.Text = _entity.Description.Value;

                TextTitle.TextChanged += TextTitle_TextChanged;
                TextBoxDescription.TextChanged += TextBoxDescription_TextChanged;
            }
            else
            {
                LabelTitle.Visibility = Visibility.Hidden;
                LabelDescription.Visibility = Visibility.Hidden;
                BorderDescriptionBase.Visibility = Visibility.Hidden;
                TextTitle.Visibility = Visibility.Hidden;
                TextBoxDescription.Visibility = Visibility.Hidden;
                NoTitleBorderDescriptionBase.Visibility = Visibility.Visible;
                NoTitleTextBoxDescription.Visibility = Visibility.Visible;
                NoTitleTextBoxDescription.Text = _entity.Description.Value;

                NoTitleTextBoxDescription.TextChanged += NoTitleTextBoxDescription_TextChanged;
            }

            Validate(true);

        }

        private void NoTitleTextBoxDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateEntity(flagUpdateDescription);
            Validate();
        }

        public CommonText GetEntity()
        {
            return entity;
        }

        void UpdateEntity(uint flags)
        {
            if(Flags.Test(flags, flagUpdateTitle)) { entity.Title.Value = TextTitle.Text.Trim(); Utils.Log("Updated", "title"); }
            if(Flags.Test(flags, flagUpdateDescription)) { entity.Description.Value = (titleEditable ? TextBoxDescription.Text : NoTitleTextBoxDescription.Text).Trim(); Utils.Log("Updated", "description"); }
            
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

            if (titleEditable)
            {
                TextTitle.TextChanged -= TextTitle_TextChanged;
                TextBoxDescription.TextChanged -= TextBoxDescription_TextChanged;
            }
            else
            {
                NoTitleTextBoxDescription.TextChanged -= NoTitleTextBoxDescription_TextChanged;
            }

            Close();

        }

        private void TextTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateEntity(flagUpdateTitle);
            Validate();
        }

        private void TextBoxDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateEntity(flagUpdateDescription);
            Validate();
        }

        public void SetEntityTitleEditable(bool editable)
        {
            titleEditable = editable;
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

        Task IEntityEditor<CommonText>.InitEditorAsync(CommonText entity, string? _parentStorageId)
        {
            throw new NotImplementedException();
        }
    }
}
