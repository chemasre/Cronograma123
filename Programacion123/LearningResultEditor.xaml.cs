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

        EntityBoxController<CommonText, CommonTextEditor> criteriasController;

        public LearningResultEditor()
        {
            InitializeComponent();
        }

        public void SetEntity(LearningResult _entity, string? _parentStorageId = null)
        {
            parentStorageId = _parentStorageId;
            entity = _entity;

            var configCriterias = EntityBoxConfiguration<CommonText>.CreateForList(ListBoxCriterias)
                                                        .WithParentStorageId(_entity.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_entity.Criterias.ToList()))
                                                        .WithPrefix(EntityBoxItemsPrefix.number)
                                                        .WithNew(ButtonCriteriaNew)
                                                        .WithEdit(ButtonCriteriaEdit)
                                                        .WithDelete(ButtonCriteriaDelete)
                                                        .WithUpDown(ButtonCriteriaUp, ButtonCriteriaDown)
                                                        .WithEditorTitle("Criterio")
                                                        .WithBlocker(Blocker);

            criteriasController = new(configCriterias);


            TextTitle.Text = _entity.Title;

            TextBoxDescription.Text = _entity.Description;

            TextTitle.TextChanged += TextTitle_TextChanged;
            TextBoxDescription.TextChanged += TextBoxDescription_TextChanged;

            Validate();

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

            entity.Criterias.Set(Storage.LoadEntities<CommonText>(criteriasController.StorageIds, entity.StorageId));

        }

        void Validate()
        {
            Entity.ValidationResult validation = entity.Validate();

            if (validation == Entity.ValidationResult.success)
            {
                BorderValidation.Background = new SolidColorBrush((Color)Application.Current.Resources["ColorValid"]);
                TextValidation.Text = "No se han detectado problemas";
            }
            else
            {
                BorderValidation.Background = new SolidColorBrush((Color)Application.Current.Resources["ColorInvalid"]);

                if (validation == Entity.ValidationResult.titleEmpty)
                {                    
                    TextValidation.Text = "Tienes que escribir un título";
                }
                else // validation == Entity.ValidationResult.descriptionEmpty
                {
                    TextValidation.Text = "La descripción no puede estar vacía";
                }
            }
            
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            UpdateEntity();
            entity.Save(parentStorageId);

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
