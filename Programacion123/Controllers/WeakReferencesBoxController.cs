using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Programacion123
{
    public struct WeakReferencesBoxConfiguration<TEntity>
    {
        public EntityBoxItemsPrefix itemsPrefix;
        public string? parentStorageId;
        public List<string> storageIds;
        public ListBox? listBox;
        public Button? buttonUp;
        public Button? buttonDown;
        public Button? buttonPickAdd;
        public Button? buttonPickRemove;
        public string? pickerTitle;
        public Func<List<string>>? pickListQuery;
        public UIElement? blocker;

        public static WeakReferencesBoxConfiguration<TEntity> CreateForList(ListBox _list) { WeakReferencesBoxConfiguration<TEntity> c = new(); c.listBox = _list; c.storageIds = new(); return c; }
        public WeakReferencesBoxConfiguration<TEntity> WithStorageIds(List<string> _storageIds) { storageIds.AddRange(_storageIds); return this; }
        public WeakReferencesBoxConfiguration<TEntity> WithPrefix(EntityBoxItemsPrefix _prefix) { itemsPrefix = _prefix; return this; }
        public WeakReferencesBoxConfiguration<TEntity> WithUpDown(Button _buttonUp, Button _buttonDown) { buttonUp = _buttonUp; buttonDown = _buttonDown; return this; }
        public WeakReferencesBoxConfiguration<TEntity> WithPick(Button _buttonPickAdd, Button _buttonPickRemove) { buttonPickAdd = _buttonPickAdd; buttonPickRemove = _buttonPickRemove; return this; }
        public WeakReferencesBoxConfiguration<TEntity> WithParentStorageId(string _parentStorageId) { parentStorageId = _parentStorageId; return this; }
        public WeakReferencesBoxConfiguration<TEntity> WithPickListQuery(Func<List<string>>? _pickListQuery) { pickListQuery = _pickListQuery; return this; }
        public WeakReferencesBoxConfiguration<TEntity> WithPickerTitle(string title) { pickerTitle = title; return this; }
        public WeakReferencesBoxConfiguration<TEntity> WithBlocker(UIElement? _blocker) { blocker = _blocker; return this; }
    }

    public class WeakReferencesBoxController<TEntity, TPicker> where TEntity: Entity, new()
                                                        where TPicker : Window, IEntityPicker<TEntity>, new()
    {
        public delegate void OnStorageIdsChanged(WeakReferencesBoxController<TEntity, TPicker> controller, List<string> storageIdList);

        public event OnStorageIdsChanged StorageIdsChanged;
        
        public List<string> StorageIds { get { return storageIds; } }

        List<string> storageIds;
        string? parentStorageId;
        EntityBoxItemsPrefix itemsPrefix;
        ListBox? listBox;
        Button? buttonUp;
        Button? buttonDown;
        Button? buttonPickAdd;
        Button? buttonPickRemove;
        string? pickerTitle;
        public Func<List<string>>? pickListQuery;
        UIElement? blocker;
        TPicker picker;

        public WeakReferencesBoxController(WeakReferencesBoxConfiguration<TEntity> configuration)
        {
            itemsPrefix = configuration.itemsPrefix;
            parentStorageId = configuration.parentStorageId;
            listBox = configuration.listBox;
            buttonUp = configuration.buttonUp;
            buttonDown = configuration.buttonDown;
            buttonPickAdd = configuration.buttonPickAdd;
            buttonPickRemove = configuration.buttonPickRemove;
            storageIds = new List<string>(configuration.storageIds);
            pickerTitle = configuration.pickerTitle;
            pickListQuery = configuration.pickListQuery;
            blocker = configuration.blocker;

            if(buttonUp != null)
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
                StorageIdsChanged?.Invoke(this, storageIds);
                UpdateList();
            }

        }

        private void ButtonPickAdd_Click(object sender, RoutedEventArgs e)
        {
            picker = new TPicker();
            if(pickerTitle != null) { picker.SetPickerTitle(pickerTitle); }
            if(blocker != null) { blocker.Visibility = Visibility.Visible; }

            List<string> storageIdList;
            List<TEntity> entityList;

            // Important difference that should be normalized in future versions
            // When picking without a query, all entities must share the same parent
            // When picking from a generated list, we are assuming the storage id list can contain elements from different parents

            if(pickListQuery == null)
            {
                storageIdList = Storage.GetStorageIds<TEntity>(Storage.LoadAllEntities<TEntity>(parentStorageId));
                entityList = Storage.LoadEntitiesFromStorageIdList<TEntity>(storageIdList, parentStorageId);
            }
            else
            {
                storageIdList = pickListQuery.Invoke();
                foreach(string s in storageIds) { storageIdList.Remove(s); }
                entityList = Storage.FindAndLoadEntities<TEntity>(storageIdList);
            }
             
            picker.InitMultiPicker(new List<TEntity>(), entityList);
            picker.Closed += OnDialogClosed;

            picker.ShowDialog();
        }

        public TEntity? GetSelectedEntity()
        {
            int selectedIndex = listBox.SelectedIndex;

            if(selectedIndex < 0) { return null; }
            else { return Storage.LoadOrCreateEntity<TEntity>(storageIds[selectedIndex], parentStorageId); }
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

                StorageIdsChanged?.Invoke(this, storageIds);
                UpdateList();

                SelectStorageId(previousSelectedStorageId);
            }


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

                StorageIdsChanged?.Invoke(this, storageIds);
                UpdateList();

                SelectStorageId(previousSelectedStorageId);
            }
        }

        void UpdateList()
        {
            List<TEntity> entities;

            if(pickListQuery != null)
            {
                entities = Storage.FindAndLoadEntities<TEntity>(storageIds);
            }
            else
            {
                entities = Storage.LoadEntitiesFromStorageIdList<TEntity>(storageIds, parentStorageId);
            }

            storageIds.Clear();

            listBox.Items.Clear();

            int index = 0;
            entities.ForEach((e) => { listBox.Items.Add(GetPrefix(index) + e.Title); storageIds.Add(e.StorageId); index ++; });
            if(listBox.Items.Count > 0) { listBox.SelectedIndex = 0;  }

        }

        void OnDialogClosed(object? sender, EventArgs e)
        {
            List<TEntity> selected = picker.GetPickedEntities();
            HashSet<string> set = new(Storage.GetStorageIds<TEntity>(selected));
            foreach(TEntity entity in selected) { set.Add(entity.StorageId); }
            storageIds = set.ToList<string>();
            StorageIdsChanged?.Invoke(this, storageIds);

            UpdateList();
            if(blocker != null) { blocker.Visibility = Visibility.Hidden; }

            picker.Closed -= OnDialogClosed;

        }

        void SelectStorageId(string storageId)
        {
            int index = storageIds.FindIndex(e => e == storageId);
            
            listBox.SelectedIndex = index;

        }

        string GetPrefix(int index)
        {
            if(itemsPrefix == EntityBoxItemsPrefix.none) { return ""; }
            else if(itemsPrefix == EntityBoxItemsPrefix.number) { return (index + 1).ToString() + ".- "; }
            else // itemsPrefix == ItemsPrefix.character
            { return System.Text.Encoding.ASCII.GetString(new byte[] { (byte)(65 + index) }).ToLower() + ". "; }
        }

    }

}
