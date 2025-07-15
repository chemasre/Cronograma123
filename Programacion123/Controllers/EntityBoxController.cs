using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Programacion123
{
    public struct EntityBoxConfiguration<TEntity>
    {
        public EntityBoxItemsPrefix itemsPrefix;
        public string? parentStorageId;
        public List<string> storageIds;
        public Action<TEntity>? entityInitializer;
        public ComboBox? comboBox;            
        public ListBox? listBox;
        public Button? buttonNew;
        public Button? buttonEdit;
        public Button? buttonDelete;
        public Button? buttonUp;
        public Button? buttonDown;
        public bool? titleEditable;
        public string? editorTitle;
        public UIElement? blocker;

        public static EntityBoxConfiguration<TEntity> CreateForCombo(ComboBox _combo) { EntityBoxConfiguration<TEntity> c = new(); c.comboBox = _combo; c.storageIds = new(); return c; }
        public static EntityBoxConfiguration<TEntity> CreateForList(ListBox _list) { EntityBoxConfiguration<TEntity> c = new(); c.listBox = _list; c.storageIds = new(); return c; }
        public EntityBoxConfiguration<TEntity> WithStorageIds(List<string> _storageIds) { storageIds.AddRange(_storageIds); return this; }
        public EntityBoxConfiguration<TEntity> WithEntityInitializer(Action<TEntity> _entityInitializer) { entityInitializer = _entityInitializer; return this; }
        public EntityBoxConfiguration<TEntity> WithPrefix(EntityBoxItemsPrefix _prefix) { itemsPrefix = _prefix; return this; }
        public EntityBoxConfiguration<TEntity> WithNew(Button _buttonNew) { buttonNew = _buttonNew; return this; }
        public EntityBoxConfiguration<TEntity> WithEdit(Button _buttonEdit) { buttonEdit = _buttonEdit; return this; }
        public EntityBoxConfiguration<TEntity> WithDelete(Button _buttonDelete) { buttonDelete = _buttonDelete; return this; }
        public EntityBoxConfiguration<TEntity> WithUpDown(Button _buttonUp, Button _buttonDown) { buttonUp = _buttonUp; buttonDown = _buttonDown; return this; }
        public EntityBoxConfiguration<TEntity> WithParentStorageId(string _parentStorageId) { parentStorageId = _parentStorageId; return this; }
        public EntityBoxConfiguration<TEntity> WithTitleEditable(bool _titleEditable) { titleEditable = _titleEditable; return this; }
        public EntityBoxConfiguration<TEntity> WithEditorTitle(string _editorTitle) { editorTitle = _editorTitle; return this; }
        public EntityBoxConfiguration<TEntity> WithBlocker(UIElement? _blocker) { blocker = _blocker; return this; }
    }

    public enum EntityBoxItemsPrefix
    {
        none,
        number,
        character
    }

    public class EntityBoxController<TEntity, TEditor> where TEntity: Entity, new()
                                                        where TEditor: Window, IEntityEditor<TEntity>, new()
    {
        public List<string> StorageIds { get { return storageIds; } }

        List<string> storageIds;
        string? parentStorageId;
        Action<TEntity>? entityInitializer;
        EntityBoxItemsPrefix itemsPrefix;
        ComboBox? comboBox;
        ListBox? listBox;
        Button? buttonNew;
        Button? buttonEdit;
        Button? buttonDelete;
        Button? buttonUp;
        Button? buttonDown;
        bool? titleEditable;
        string? editorTitle;
        UIElement? blocker;
        TEditor editor;

        public EntityBoxController(EntityBoxConfiguration<TEntity> configuration)
        {
            itemsPrefix = configuration.itemsPrefix;
            parentStorageId = configuration.parentStorageId;
            entityInitializer = configuration.entityInitializer;
            comboBox = configuration.comboBox;
            listBox = configuration.listBox;
            buttonNew = configuration.buttonNew;
            buttonEdit = configuration.buttonEdit;
            buttonDelete = configuration.buttonDelete;
            buttonUp = configuration.buttonUp;
            buttonDown = configuration.buttonDown;
            storageIds = new List<string>(configuration.storageIds);
            titleEditable = configuration.titleEditable;
            editorTitle= configuration.editorTitle;
            blocker = configuration.blocker;

            if(buttonNew != null) { buttonNew.Click += ButtonNew_Click; buttonNew.ToolTip = "Crear"; }
            if(buttonEdit != null) { buttonEdit.Click += ButtonEdit_Click; buttonEdit.ToolTip = "Modificar"; }
            if(buttonDelete != null) { buttonDelete.Click += ButtonDelete_Click; buttonDelete.ToolTip = "Eliminar"; }
            if(buttonUp != null) { buttonUp.Click += ButtonUp_Click; buttonUp.ToolTip = "Mover arriba en la lista"; }
            if(buttonDown != null) { buttonDown.Click += ButtonDown_Click; buttonDown.ToolTip = "Mover abajo en la lista"; }

            UpdateListOrCombo();
        }

        public TEntity? GetSelectedEntity()
        {
            int selectedIndex;

            if (comboBox != null) { selectedIndex = comboBox.SelectedIndex; }
            else { selectedIndex = listBox.SelectedIndex; }

            if(selectedIndex < 0) { return null; }
            else { return Storage.LoadOrCreateEntity<TEntity>(storageIds[selectedIndex], parentStorageId); }
        }

        void ButtonDown_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex;

            if (comboBox != null) { selectedIndex = comboBox.SelectedIndex; }
            else { selectedIndex = listBox.SelectedIndex; }

            if(selectedIndex < storageIds.Count - 1)
            {
                string previousSelectedStorageId = storageIds[selectedIndex];

                string s;
                s = storageIds[selectedIndex + 1];
                storageIds[selectedIndex + 1] = storageIds[selectedIndex];
                storageIds[selectedIndex] = s;

                UpdateListOrCombo();

                SelectStorageId(previousSelectedStorageId);
            }


        }

        void ButtonUp_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex;

            if (comboBox != null) { selectedIndex = comboBox.SelectedIndex; }
            else { selectedIndex = listBox.SelectedIndex; }

            if (selectedIndex > 0)
            {
                string previousSelectedStorageId = storageIds[selectedIndex];

                string s;
                s = storageIds[selectedIndex - 1];
                storageIds[selectedIndex - 1] = storageIds[selectedIndex];
                storageIds[selectedIndex] = s;

                UpdateListOrCombo();

                SelectStorageId(previousSelectedStorageId);
            }
        }

        void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            int index = -61;

            if(comboBox != null)
            {
                index = comboBox.SelectedIndex;
            }
            else
            {
                index = listBox.SelectedIndex;
            }

            if(index >= 0)
            {
                string? previousStorageId = index > 0 ? storageIds[index - 1] : null;

                Entity entity = Storage.LoadOrCreateEntity<TEntity>(storageIds[index], parentStorageId);
                entity.Delete(parentStorageId);
                storageIds.RemoveAt(index);
                UpdateListOrCombo();

                if(previousStorageId != null)
                {
                    SelectStorageId(previousStorageId);
                }
            }

        }

        void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            bool openEditor = false;
            int index = -1;
            
            if(comboBox != null)
            {
                if(comboBox.SelectedIndex >= 0)
                {
                    openEditor = true;
                    index = comboBox.SelectedIndex;
                }
            }
            else
            {
                if(listBox.SelectedIndex >= 0)
                {
                    openEditor = true;
                    index = listBox.SelectedIndex;
                }
            }

            if(openEditor)
            {
                var entity = Storage.LoadOrCreateEntity<TEntity>(storageIds[index], parentStorageId);

                editor = new TEditor();
                if(titleEditable != null) { editor.SetEntityTitleEditable(titleEditable.Value); }
                if(editorTitle != null) { editor.SetEditorTitle(editorTitle); }
                if(blocker != null) { blocker.Visibility = Visibility.Visible; }
                editor.SetEntity(entity, parentStorageId);
                editor.Closed += OnEditorClosed;
                editor.ShowDialog();
            }

        }

        void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            TEntity entity = new();
            if(entityInitializer != null) { entityInitializer.Invoke(entity); }
            editor = new TEditor();
            if(titleEditable != null) { editor.SetEntityTitleEditable(titleEditable.Value); }
            if(editorTitle != null) { editor.SetEditorTitle(editorTitle); }
            if(blocker != null) { blocker.Visibility = Visibility.Visible; }
            editor.SetEntity(entity, parentStorageId);
            storageIds.Add(entity.StorageId);
            editor.Closed += OnEditorClosed;
            editor.ShowDialog();
        }

        void UpdateListOrCombo()
        {
            List<TEntity> entities;

            entities = Storage.LoadEntitiesFromList<TEntity>(storageIds, parentStorageId);

            storageIds.Clear();

            if(comboBox != null)
            {
                comboBox.Items.Clear();

                int index = 0;
                entities.ForEach((e) => { comboBox.Items.Add(GetPrefix(index) + e.Title); storageIds.Add(e.StorageId); index ++; });
                if(comboBox.Items.Count > 0) { comboBox.SelectedIndex = 0;  }
            }
            else
            {
                listBox.Items.Clear();

                int index = 0;
                entities.ForEach((e) => { listBox.Items.Add(GetPrefix(index) + e.Title); storageIds.Add(e.StorageId); index ++; });
                if(listBox.Items.Count > 0) { listBox.SelectedIndex = 0;  }
            }


        }

        void OnEditorClosed(object? sender, EventArgs e)
        {
            UpdateListOrCombo();
            if(blocker != null) { blocker.Visibility = Visibility.Hidden; }
            SelectStorageId(editor.GetEntity().StorageId);
            editor.Closed -= OnEditorClosed;
        }

        void SelectStorageId(string storageId)
        {
            int index = storageIds.FindIndex(e => e == storageId);

            if(comboBox != null) { comboBox.SelectedIndex = index; }
            else { listBox.SelectedIndex = index; }

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
