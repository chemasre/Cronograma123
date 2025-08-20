using System.Windows;
using System.Windows.Controls;

namespace Programacion123
{
    public struct WeakReferencesBoxConfiguration<TEntity>
    {
        public EntityFormatContent formatContent;
        public EntityFormatIndex formatIndex;
        public Func<TEntity, int, string>? formatter;
        public string? parentStorageId;
        public List<string> storageIds;
        public ListBox? listBox;
        public Button? buttonUp;
        public Button? buttonDown;
        public Button? buttonPickAdd;
        public Button? buttonPickRemove;
        public string? pickerTitle;
        public Func<List<string>>? pickListQuery;
        public List<string>? pickList;
        public UIElement? blocker;

        public static WeakReferencesBoxConfiguration<TEntity> CreateForList(ListBox _list) { WeakReferencesBoxConfiguration<TEntity> c = new(); c.listBox = _list; c.storageIds = new(); return c; }
        public WeakReferencesBoxConfiguration<TEntity> WithStorageIds(List<string> _storageIds) { storageIds.AddRange(_storageIds); return this; }
        public WeakReferencesBoxConfiguration<TEntity> WithFormat(EntityFormatContent _formatContent, EntityFormatIndex _formatIndex = EntityFormatIndex.None) { formatContent = _formatContent; formatIndex = _formatIndex; return this; }
        public WeakReferencesBoxConfiguration<TEntity> WithFormatter(Func<TEntity, int, string> _formatter) { formatter = _formatter; return this; }
        public WeakReferencesBoxConfiguration<TEntity> WithUpDown(Button _buttonUp, Button _buttonDown) { buttonUp = _buttonUp; buttonDown = _buttonDown; return this; }
        public WeakReferencesBoxConfiguration<TEntity> WithPick(Button _buttonPickAdd, Button _buttonPickRemove) { buttonPickAdd = _buttonPickAdd; buttonPickRemove = _buttonPickRemove; return this; }
        public WeakReferencesBoxConfiguration<TEntity> WithParentStorageId(string _parentStorageId) { parentStorageId = _parentStorageId; return this; }
        public WeakReferencesBoxConfiguration<TEntity> WithPickListQuery(Func<List<string>>? _pickListQuery) { pickListQuery = _pickListQuery; return this; }
        public WeakReferencesBoxConfiguration<TEntity> WithPickerTitle(string title) { pickerTitle = title; return this; }
        public WeakReferencesBoxConfiguration<TEntity> WithBlocker(UIElement? _blocker) { blocker = _blocker; return this; }
        public WeakReferencesBoxConfiguration<TEntity> WithPickList(List<string> _pickList) { pickList = _pickList; return this; }
    }

    public class WeakReferencesBoxController<TEntity, TPicker> where TEntity: Entity, new()
                                                        where TPicker : Window, IEntityPicker<TEntity>, new()
    {
        public delegate void OnChanged(WeakReferencesBoxController<TEntity, TPicker> controller);

        public event OnChanged Changed;
        
        public List<string> StorageIds { get { return storageIds; } }

        List<string> storageIds;
        string? parentStorageId;
        EntityFormatContent formatContent;
        EntityFormatIndex formatIndex;
        Func<TEntity, int, string>? formatter;
        ListBox? listBox;
        Button? buttonUp;
        Button? buttonDown;
        Button? buttonPickAdd;
        Button? buttonPickRemove;
        string? pickerTitle;
        Func<List<string>>? pickListQuery;
        List<string>? pickList;
        UIElement? blocker;
        TPicker picker;

        public WeakReferencesBoxController(WeakReferencesBoxConfiguration<TEntity> configuration)
        {
            formatContent = configuration.formatContent;
            formatIndex = configuration.formatIndex;
            formatter = configuration.formatter;
            parentStorageId = configuration.parentStorageId;
            listBox = configuration.listBox;
            buttonUp = configuration.buttonUp;
            buttonDown = configuration.buttonDown;
            buttonPickAdd = configuration.buttonPickAdd;
            buttonPickRemove = configuration.buttonPickRemove;
            storageIds = new List<string>(configuration.storageIds);
            pickerTitle = configuration.pickerTitle;
            pickListQuery = configuration.pickListQuery;
            pickList = configuration.pickList;
            blocker = configuration.blocker;

            if (buttonUp != null)
            {
                buttonUp.Click += ButtonUp_Click; buttonUp.ToolTip = "Mover arriba en la lista";
                buttonDown.Click += ButtonDown_Click; buttonDown.ToolTip = "Mover abajo en la lista";
            }
            if(buttonPickAdd != null)
            {
                buttonPickAdd.Click += ButtonPickAdd_Click; ; buttonPickAdd.ToolTip = "Añadir referencia";
                buttonPickRemove.Click += ButtonPickRemove_Click; ; buttonPickRemove.ToolTip = "Quitar referencia";
            }

            UpdateList();

        }

        private void ButtonPickRemove_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = listBox.SelectedIndex;

            if(selectedIndex >= 0)
            {
                storageIds.RemoveAt(selectedIndex);
                Changed?.Invoke(this);
                UpdateList();
            }

        }

        private void ButtonPickAdd_Click(object sender, RoutedEventArgs e)
        {
            picker = new TPicker();
            if(pickerTitle != null) { picker.SetPickerTitle(pickerTitle); }
            if(formatter != null) { picker.SetFormatter(formatter); }
            picker.SetFormat(formatContent, formatIndex);
            if(blocker != null) { blocker.Visibility = Visibility.Visible; }

            List<TEntity> pickableEntities = GetPickableEntities();
            foreach(string s in storageIds) { pickableEntities.RemoveAll(e => e.StorageId == s); }
             
            picker.SetMultiPickerEntities(new List<TEntity>(), pickableEntities);
            picker.Closed += OnDialogClosed;

            picker.ShowDialog();
        }

        public List<TEntity> GetSelectedEntities()
        {
            return Storage.FindEntities<TEntity>(storageIds);
        }

        void ButtonDown_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = listBox.SelectedIndex;

            if(selectedIndex < storageIds.Count - 1)
            {
                string previousSelectedStorageId = storageIds[selectedIndex];

                string s;
                s = storageIds[selectedIndex + 1];
                storageIds[selectedIndex + 1] = storageIds[selectedIndex];
                storageIds[selectedIndex] = s;

                Changed?.Invoke(this);
                UpdateList();

                SelectStorageId(previousSelectedStorageId);
            }


        }

        List<TEntity> GetPickableEntities()
        {
            List<string> pickableStorageIds;
            List<TEntity> pickableEntities;

            // Important difference that should be normalized in future versions
            // When picking without a query or list, all entities must share the same parent
            // When picking with a query or a list, we are assuming the storage id list can contain elements from different parents

            if(pickListQuery == null)
            {
                pickableStorageIds = Storage.GetStorageIds<TEntity>(Storage.LoadAllEntities<TEntity>(parentStorageId));
                pickableEntities = Storage.LoadOrCreateEntities<TEntity>(pickableStorageIds, parentStorageId);
            }
            else if(pickList != null)
            {
                pickableStorageIds = pickList;
                pickableEntities = Storage.FindEntities<TEntity>(pickableStorageIds);
            }
            else
            {
                pickableStorageIds = pickListQuery.Invoke();
                pickableEntities = Storage.FindEntities<TEntity>(pickableStorageIds);
            }

            return pickableEntities;

        }

        void ButtonUp_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = listBox.SelectedIndex;

            if (selectedIndex > 0)
            {
                string previousSelectedStorageId = storageIds[selectedIndex];

                string s;
                s = storageIds[selectedIndex - 1];
                storageIds[selectedIndex - 1] = storageIds[selectedIndex];
                storageIds[selectedIndex] = s;

                Changed?.Invoke(this);
                UpdateList();

                SelectStorageId(previousSelectedStorageId);
            }
        }

        void UpdateList()
        {
            List<TEntity> entities;

            if(pickListQuery != null)
            {
                entities = Storage.FindEntities<TEntity>(storageIds);
            }
            else
            {
                entities = Storage.LoadOrCreateEntities<TEntity>(storageIds, parentStorageId);
            }

            storageIds.Clear();

            listBox.Items.Clear();

            int index = 0;
            entities.ForEach(
                (e) =>
                {   string formattedEntity;
                    if(formatter != null) { formattedEntity = formatter.Invoke(e, index); }
                    else { formattedEntity = Utils.FormatEntity(e, index, formatContent, formatIndex); }
                    listBox.Items.Add(formattedEntity);
                    storageIds.Add(e.StorageId);
                    index ++;
                });

            if(listBox.Items.Count > 0) { listBox.SelectedIndex = 0;  }

        }

        void OnDialogClosed(object? sender, EventArgs e)
        {
            if(!picker.GetWasCancelled())
            {
                List<TEntity> selected = picker.GetPickedEntities();
                foreach(TEntity entity in selected) { storageIds.Add(entity.StorageId); }

                List<TEntity> pickableEntities = GetPickableEntities();
                storageIds.Sort(
                    (string s1, string s2) =>
                    {
                        int index1 = pickableEntities.FindIndex((e) => e.StorageId == s1);
                        int index2 = pickableEntities.FindIndex((e) => e.StorageId == s2);
                        return index1.CompareTo(index2);
                    });

                Changed?.Invoke(this);

                UpdateList();
            }

            if(blocker != null) { blocker.Visibility = Visibility.Hidden; }

            picker.Closed -= OnDialogClosed;

        }

        void SelectStorageId(string storageId)
        {
            int index = storageIds.FindIndex(e => e == storageId);
            
            listBox.SelectedIndex = index;

        }

        public void Finish()
        {
            if (buttonUp != null)
            {
                buttonUp.Click -= ButtonUp_Click;
                buttonDown.Click -= ButtonDown_Click;
            }

            if(buttonPickAdd != null)
            {
                buttonPickAdd.Click -= ButtonPickAdd_Click;
                buttonPickRemove.Click -= ButtonPickRemove_Click;
            }
        }

    }

}
