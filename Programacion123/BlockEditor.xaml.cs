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

            Action<Activity> activityInitializer =
                (Activity a) =>
                {
                    Subject subject = Storage.FindEntity<Subject>(Storage.FindParentStorageId(entity.StorageId, entity.StorageClassId), null);
                    if(subject.Template != null)
                    {
                        List<LearningResult> results = subject.Template.LearningResults.ToList();
                        foreach(LearningResult r in results) { a.LearningResultsWeights.Add(r, 0); }
                    }
                };

            var configActivities = StrongReferencesBoxConfiguration<Activity>.CreateForList(ListBoxPoints)
                                                        .WithEntityInitializer(activityInitializer)
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
            activitiesController.Changed += ActivitiesController_Changed;


            TextTitle.Text = _entity.Title;

            TextBoxDescription.Text = _entity.Description;

            TextTitle.TextChanged += TextTitle_TextChanged;
            TextBoxDescription.TextChanged += TextBoxDescription_TextChanged;            

            Validate();

        }

        private void ActivitiesController_Changed(StrongReferencesBoxController<Activity, ActivityEditor> controller)
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
                else // validation.code == Entity.ValidationResult.descriptionEmpty
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
            activitiesController.Changed -= ActivitiesController_Changed;

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
