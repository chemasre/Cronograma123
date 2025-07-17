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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Programacion123
{
    /// <summary>
    /// Lógica de interacción para ActivityEditor.xaml
    /// </summary>
    public partial class ActivityEditor : Window, IEntityEditor<Activity>
    {
        Activity entity;
        string? parentStorageId;

        WeakReferenceFieldController<CommonText, EntityPicker<CommonText> > metodologyController;
        WeakReferencesBoxController<CommonText, EntityPicker<CommonText> > contentPointsController;

        Subject? subject;
        Block? block;


        public ActivityEditor()
        {
            InitializeComponent();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            UpdateEntity();

            Close();
        }

        public void SetEntityTitleEditable(bool editable)
        {
            TextTitle.IsReadOnly = !editable;
            TextTitle.IsReadOnlyCaretVisible = false;
        }

        public void SetEditorTitle(string title)
        {
            TextEditorTitle.Content = title;
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        public void UpdateEntity()
        {
            entity.Title = TextTitle.Text;
            entity.Description = TextBoxDescription.Text;

            entity.Metodology = metodologyController.GetEntity();

            entity.ContentPoints.Set(Storage.FindAndLoadEntities<CommonText>(contentPointsController.StorageIds));

            entity.IsEvaluable = CheckboxIsEvaluable.IsChecked.GetValueOrDefault();

            entity.Save(parentStorageId);
        }

        public void InitEditor(Activity _entity, string? _parentStorageId)
        {
            entity = _entity;

            parentStorageId = _parentStorageId;

            block = new();

            string blockStorageId = _parentStorageId;
            string subjectStorageId = Storage.FindParentStorageId(blockStorageId, block.StorageClassId);

            subject = new Subject();
            subject.LoadOrCreate(subjectStorageId);
            block.LoadOrCreate(blockStorageId, subjectStorageId);

            TextBlock.Text = block.Title;
            TextBlock.Background = Brushes.LightPink;

            var configMetodology = WeakReferenceFieldConfiguration<CommonText>.CreateForTextBox(TextMetodology)
                                               .WithStorageId(entity.Metodology?.StorageId)
                                               .WithParentStorageId(subject.StorageId)
                                               .WithPick(ButtonMetodologyPick)
                                               .WithPickerTitle("Elige una metodología")
                                               .WithFieldDisplayType(EntityFieldDisplayType.title)
                                               .WithPickListQuery(() => Storage.GetStorageIds<CommonText>(subject.Metodologies.ToList()) )
                                               .WithBlocker(Blocker);

            metodologyController = new(configMetodology);

            Func<List<string>> pickContentPointsQuery =
                () =>
                {
                    List<string> contentPoints = new();
                    if(subject.Template != null)
                    {
                        List<Content> contentList = subject.Template.Contents.ToList();

                        foreach(Content c in contentList)
                        {
                            List<CommonText> pointsList = c.Points.ToList();

                            foreach(CommonText p in pointsList)
                            {
                                contentPoints.Add(p.StorageId);
                            }
                        }
                    }

                    return contentPoints;
                };

            var configContents = WeakReferencesBoxConfiguration<CommonText>.CreateForList(ListBoxContents)
                                                        .WithStorageIds(Storage.GetStorageIds<CommonText>(_entity.ContentPoints.ToList()))
                                                        .WithPrefix(EntityBoxItemsPrefix.number)
                                                        .WithPick(ButtonContentReferenceAdd, ButtonContentReferenceRemove)
                                                        .WithPickListQuery(pickContentPointsQuery)
                                                        .WithPickerTitle("Puntos de contenido")
                                                        .WithBlocker(Blocker);

            contentPointsController = new(configContents);
            contentPointsController.StorageIdsChanged += ContentPointsController_StorageIdsChanged;

            TextTitle.Text = entity.Title;
            TextBoxDescription.Text = entity.Description;
            CheckboxIsEvaluable.IsChecked = entity.IsEvaluable;

            CheckboxIsEvaluable.Checked += CheckboxIsEvaluable_CheckedChanged;
            CheckboxIsEvaluable.Unchecked += CheckboxIsEvaluable_CheckedChanged;

            UpdateActivityCodeUI();

        }

        private void ContentPointsController_StorageIdsChanged(WeakReferencesBoxController<CommonText, EntityPicker<CommonText> > controller, List<string> storageIdList)
        {
            UpdateEntity();
        }

        void CheckboxIsEvaluable_CheckedChanged(object sender, RoutedEventArgs e)
        {
            UpdateActivityCodeUI();
            UpdateEntity();
        }

        void UpdateActivityCodeUI()
        {
            if(CheckboxIsEvaluable.IsChecked.GetValueOrDefault())
            {
                int activityIndex = block.Activities.ToList().FindIndex((a => a.StorageId == entity.StorageId));
                int blockIndex = subject.Blocks.ToList().FindIndex((b) => b.StorageId == block.StorageId);
                TextActivityCode.Text = String.Format("B{0:00}-A{1:00}", blockIndex + 1, activityIndex + 1);
                TextActivityCode.Background = Brushes.LightPink;
            }
            else
            {
                TextActivityCode.Text = "";
                TextActivityCode.Background = Brushes.LightGray;
            }
        }

        public Activity GetEntity()
        {
            return entity;
        }

    }
}
