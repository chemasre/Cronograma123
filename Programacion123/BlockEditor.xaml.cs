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
    /// Lógica de interacción para BlockEditor.xaml
    /// </summary>
    public partial class BlockEditor : Window, IEntityEditor<Block>
    {
        string? parentStorageId;
        Block entity;

        StrongReferencesBoxController<Activity, ActivityEditor> activitiesController;

        public BlockEditor()
        {
            InitializeComponent();
        }

        public void InitEditor(Block _entity, string? _parentStorageId = null)
        {
            _entity.Save(_parentStorageId);

            parentStorageId = _parentStorageId;
            entity = _entity;

            var configActivities = StrongReferencesBoxConfiguration<Activity>.CreateForList(ListBoxPoints)
                                                        .WithParentStorageId(_entity.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<Activity>(_entity.Activities.ToList()))
                                                        .WithFormat(EntityFormatContent.title, EntityFormatIndex.number)
                                                        .WithNew(ButtonPointNew)
                                                        .WithEdit(ButtonPointEdit)
                                                        .WithDelete(ButtonPointDelete)
                                                        .WithUpDown(ButtonPointUp, ButtonPointDown)
                                                        .WithEditorTitle("Actividad")
                                                        .WithBlocker(Blocker);

            activitiesController = new(configActivities);
            activitiesController.StorageIdsChanged += ActivitiesController_StorageIdsChanged;


            TextTitle.Text = _entity.Title;

            TextBoxDescription.Text = _entity.Description;

            TextTitle.TextChanged += TextTitle_TextChanged;
            TextBoxDescription.TextChanged += TextBoxDescription_TextChanged;            

            Validate();

        }

        private void ActivitiesController_StorageIdsChanged(StrongReferencesBoxController<Activity, ActivityEditor> controller, List<string> storageIdList)
        {
            UpdateEntity();
        }

        public Block GetEntity()
        {
            return entity;
        }

        void UpdateEntity()
        {
            entity.Title = TextTitle.Text.Trim();
            entity.Description = TextBoxDescription.Text;
            //entity.Description = TextBoxDescription.Document.ToString().Trim();

            entity.Activities.Set(Storage.LoadOrCreateEntities<Activity>(activitiesController.StorageIds, entity.StorageId));

            entity.Save(parentStorageId);
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

            TextTitle.TextChanged -= TextTitle_TextChanged;
            TextBoxDescription.TextChanged -= TextBoxDescription_TextChanged;
            activitiesController.StorageIdsChanged -= ActivitiesController_StorageIdsChanged;

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
