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
    /// Lógica de interacción para ContentEditor.xaml
    /// </summary>
    public partial class ContentEditor : Window, IEntityEditor<Content>
    {
        string? parentStorageId;
        Content entity;

        StrongReferencesBoxController<CommonText, CommonTextEditor > pointsController;

        public ContentEditor()
        {
            InitializeComponent();
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
                    return String.Format("{0}.{1}: {2}", contentIndex + 1, i + 1, e.Title);
                };

            var configPoints = StrongReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxPoints)
                                                        .WithParentStorageId(_entity.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_entity.Points.ToList()))
                                                        .WithFormatter(formatter)
                                                        .WithNew(ButtonPointNew)
                                                        .WithEdit(ButtonPointEdit)
                                                        .WithDelete(ButtonPointDelete)
                                                        .WithUpDown(ButtonPointUp, ButtonPointDown)
                                                        .WithEditorTitle("Punto de contenido")
                                                        .WithBlocker(Blocker);

            pointsController = new(configPoints);

            pointsController.Changed += PointsController_Changed;

            TextTitle.Text = _entity.Title;

            TextBoxDescription.Text = _entity.Description;

            TextTitle.TextChanged += TextTitle_TextChanged;
            TextBoxDescription.TextChanged += TextBoxDescription_TextChanged;

            Validate();

        }

        private void PointsController_Changed(StrongReferencesBoxController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity();
            Validate();
        }

        public Content GetEntity()
        {
            return entity;
        }

        void UpdateEntity()
        {
            entity.Title = TextTitle.Text.Trim();
            entity.Description = TextBoxDescription.Text;
            //entity.Description = TextBoxDescription.Document.ToString().Trim();

            entity.Points.Set(Storage.LoadOrCreateEntities<CommonText>(pointsController.StorageIds, entity.StorageId));

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

            TextTitle.TextChanged -= TextTitle_TextChanged;
            TextBoxDescription.TextChanged -= TextBoxDescription_TextChanged;

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
            TextTitle.IsReadOnly = !editable;
            TextTitle.IsReadOnlyCaretVisible = false;
            TextTitle.Background = Brushes.LightGray;
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
