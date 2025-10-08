using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
                    if (subject.Template.Value != null)
                    {
                        List<LearningResult> results = subject.Template.Value.LearningResults.ToList();
                        foreach (LearningResult r in results) { a.LearningResultsWeights.Add(r, 0); }
                    }

                    if (subject.Calendar.Value != null)
                    {
                        a.StartDate.Value = subject.Calendar.Value.StartDay.Value;
                    }
                    else
                    {
                        DateTime now = DateTime.Now;
                        a.StartDate.Value = new DateTime(now.Year, now.Month, now.Day);
                    }
                };

            var configActivities = StrongReferencesBoxConfiguration<Activity>.CreateForList(ListBoxPoints)
                                                        .WithEntityInitializer(activityInitializer)
                                                        .WithParentStorageId(_entity.StorageId)
                                                        .WithStorageIds(Storage.GetStorageIds<Activity>(_entity.Activities.ToList()))
                                                        .WithFormat(EntityFormatContent.Title, EntityFormatIndex.Number)
                                                        .WithNew(ButtonPointNew)
                                                        .WithEdit(ButtonPointEdit)
                                                        .WithDelete(ButtonPointDelete)
                                                        .WithDeleteConfirmQuestion("Esto eliminará  permanentemente la actividad seleccionada. ¿Estás seguro/a?")
                                                        .WithUpDown(ButtonPointUp, ButtonPointDown)
                                                        .WithEditorTitle("Actividad")
                                                        .WithBlocker(Blocker);

            activitiesController = new(configActivities);
            activitiesController.Changed += ActivitiesController_Changed;


            TextTitle.Text = _entity.Title.Value;

            TextBoxDescription.Text = _entity.Description.Value;

            ButtonClose.ToolTip = "Cerrar";

            TextTitle.TextChanged += TextTitle_TextChanged;
            TextBoxDescription.TextChanged += TextBoxDescription_TextChanged;

            Validate();

        }

        private void ActivitiesController_Changed(StrongReferencesBoxController<Activity, ActivityEditor> controller)
        {
            UpdateEntity();
            Validate(true);
        }

        public Block GetEntity()
        {
            return entity;
        }

        void UpdateEntity()
        {
            entity.Title.Value = TextTitle.Text.Trim();
            entity.Description.Value = TextBoxDescription.Text;
            //entity.Description = TextBoxDescription.Document.ToString().Trim();

            entity.Activities.Set(Storage.LoadOrCreateEntities<Activity>(activitiesController.StorageIds, entity.StorageId));

            entity.Save(parentStorageId);
        }

        void Validate(bool force = false)
        {
            if(force) { entity.Invalidate(); }

            ValidationResult validation = entity.Validate();

            string colorResource = (validation.code == ValidationCode.success ? "ColorValid" : "ColorInvalid");
            BorderValidation.Background = new SolidColorBrush((Color)Application.Current.Resources[colorResource]);
            TextValidation.Text = validation.ToString();

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
