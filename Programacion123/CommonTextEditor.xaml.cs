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

            TextTitle.Text = _entity.Title;

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
                TextBoxDescription.Text = _entity.Description;

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
                NoTitleTextBoxDescription.Text = _entity.Description;

                NoTitleTextBoxDescription.TextChanged += NoTitleTextBoxDescription_TextChanged;
            }


            Validate();

        }

        private void NoTitleTextBoxDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateEntity();
            Validate();
        }

        public CommonText GetEntity()
        {
            return entity;
        }

        void UpdateEntity()
        {
            entity.Title = TextTitle.Text.Trim();
            entity.Description = (titleEditable ? TextBoxDescription.Text : NoTitleTextBoxDescription.Text).Trim();
            //entity.Description = TextBoxDescription.Document.ToString().Trim();

            entity.Save(parentStorageId);

        }

        void Validate()
        {
            ValidationResult validation = entity.Validate();

            string colorResource = (validation.code == ValidationCode.success ? "ColorValid" : "ColorInvalid");
            BorderValidation.Background = new SolidColorBrush((Color)Application.Current.Resources[colorResource]);
            TextValidation.Text = validation.ToString();

        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            UpdateEntity();
            //entity.Save(parentStorageId);

            if(titleEditable)
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
            UpdateEntity();
            Validate();
        }

        private void TextBoxDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateEntity();
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
    }
}
