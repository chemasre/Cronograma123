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
    /// Lógica de interacción para LearningResultEditor.xaml
    /// </summary>
    public partial class LearningResultEditor : Window, IEntityEditor<LearningResult>
    {
        string? parentStorageId;
        LearningResult entity;

        StrongReferencesBoxController<CommonText, CommonTextEditor> criteriasController;

        public LearningResultEditor()
        {
            InitializeComponent();
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
                    return String.Format("RA{0}.{1}: {2}", resultIndex + 1, i + 1, e.Title);
                };

            var configCriterias = StrongReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxCriterias)
                                                        .WithParentStorageId(_entity.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_entity.Criterias.ToList()))
                                                        .WithFormatter(formatter)
                                                        .WithNew(ButtonCriteriaNew)
                                                        .WithEdit(ButtonCriteriaEdit)
                                                        .WithDelete(ButtonCriteriaDelete)
                                                        .WithUpDown(ButtonCriteriaUp, ButtonCriteriaDown)
                                                        .WithEditorTitle("Criterio")
                                                        .WithBlocker(Blocker);

            criteriasController = new(configCriterias);

            criteriasController.Changed += CriteriasController_Changed;

            TextTitle.Text = _entity.Title;

            TextBoxDescription.Text = _entity.Description;

            TextTitle.TextChanged += TextTitle_TextChanged;
            TextBoxDescription.TextChanged += TextBoxDescription_TextChanged;

            Validate();

        }

        private void CriteriasController_Changed(StrongReferencesBoxController<CommonText, CommonTextEditor> controller)
        {
            UpdateEntity();
        }

        public LearningResult GetEntity()
        {
            return entity;
        }

        void UpdateEntity()
        {
            entity.Title = TextTitle.Text.Trim();
            entity.Description = TextBoxDescription.Text;
            //entity.Description = TextBoxDescription.Document.ToString().Trim();

            entity.Criterias.Set(Storage.LoadOrCreateEntities<CommonText>(criteriasController.StorageIds, entity.StorageId));

            entity.Save(parentStorageId);
        }

        void Validate()
        {
            Entity.ValidationResult validation = entity.Validate();

            if (validation.code == Entity.ValidationCode.success)
            {
                BorderValidation.Background = new SolidColorBrush((Color)Application.Current.Resources["ColorValid"]);
                TextValidation.Text = "No se han detectado problemas";
            }
            else
            {
                BorderValidation.Background = new SolidColorBrush((Color)Application.Current.Resources["ColorInvalid"]);

                if (validation.code == Entity.ValidationCode.entityTitleEmpty)
                {                    
                    TextValidation.Text = "Tienes que escribir un título";
                }
                else // validation.code == Entity.ValidationCode.descriptionEmpty
                {
                    TextValidation.Text = "La descripción no puede estar vacía";
                }
            }
            
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
