using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Programacion123
{
    public struct StrongReferencesBoxConfiguration<TEntity>
    {
        public string? parentStorageId;
        public List<string> storageIds;
        public Action<TEntity>? entityInitializer;
        public ComboBox? comboBox;            
        public ListBox? listBox;
        public EntityFormatContent formatContent;
        public EntityFormatIndex formatIndex;
        public Func<TEntity, int, string>? formatter;
        public Button? buttonNew;
        public Button? buttonEdit;
        public Button? buttonDelete;
        public Button? buttonUp;
        public Button? buttonDown;
        public bool? titleEditable;
        public string? editorTitle;
        public UIElement? blocker;

        public static StrongReferencesBoxConfiguration<TEntity> CreateForCombo(ComboBox _combo) { StrongReferencesBoxConfiguration<TEntity> c = new(); c.comboBox = _combo; c.storageIds = new(); return c; }
        public static StrongReferencesBoxConfiguration<TEntity> CreateForList(ListBox _list) { StrongReferencesBoxConfiguration<TEntity> c = new(); c.listBox = _list; c.storageIds = new(); return c; }
        public StrongReferencesBoxConfiguration<TEntity> WithStorageIds(List<string> _storageIds) { storageIds.AddRange(_storageIds); return this; }
        public StrongReferencesBoxConfiguration<TEntity> WithEntityInitializer(Action<TEntity> _entityInitializer) { entityInitializer = _entityInitializer; return this; }
        public StrongReferencesBoxConfiguration<TEntity> WithFormat(EntityFormatContent _formatContent, EntityFormatIndex _formatIndex = EntityFormatIndex.none) { formatContent = _formatContent; formatIndex = _formatIndex; return this; }
        public StrongReferencesBoxConfiguration<TEntity> WithFormatter(Func<TEntity, int, string> _formatter) { formatter = _formatter; return this; }
        public StrongReferencesBoxConfiguration<TEntity> WithNew(Button _buttonNew) { buttonNew = _buttonNew; return this; }
        public StrongReferencesBoxConfiguration<TEntity> WithEdit(Button _buttonEdit) { buttonEdit = _buttonEdit; return this; }
        public StrongReferencesBoxConfiguration<TEntity> WithDelete(Button _buttonDelete) { buttonDelete = _buttonDelete; return this; }
        public StrongReferencesBoxConfiguration<TEntity> WithUpDown(Button _buttonUp, Button _buttonDown) { buttonUp = _buttonUp; buttonDown = _buttonDown; return this; }
        public StrongReferencesBoxConfiguration<TEntity> WithParentStorageId(string _parentStorageId) { parentStorageId = _parentStorageId; return this; }
        public StrongReferencesBoxConfiguration<TEntity> WithTitleEditable(bool _titleEditable) { titleEditable = _titleEditable; return this; }
        public StrongReferencesBoxConfiguration<TEntity> WithEditorTitle(string _editorTitle) { editorTitle = _editorTitle; return this; }
        public StrongReferencesBoxConfiguration<TEntity> WithBlocker(UIElement? _blocker) { blocker = _blocker; return this; }
    }

    public class StrongReferencesBoxController<TEntity, TEditor> where TEntity: Entity, new()
                                                        where TEditor: Window, IEntityEditor<TEntity>, new()
    {
        public delegate void OnChanged(StrongReferencesBoxController<TEntity, TEditor> controller);

        public event OnChanged Changed;
        
        public List<string> StorageIds { get { return storageIds; } }

        List<string> storageIds;
        string? parentStorageId;
        bool? storageIdsAreWeak;
        Action<TEntity>? entityInitializer;
        EntityFormatContent formatContent;
        EntityFormatIndex formatIndex;
        Func<TEntity, int, string>? formatter;
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

        public StrongReferencesBoxController(StrongReferencesBoxConfiguration<TEntity> configuration)
        {
            formatContent = configuration.formatContent;
            parentStorageId = configuration.parentStorageId;
            entityInitializer = configuration.entityInitializer;
            comboBox = configuration.comboBox;
            listBox = configuration.listBox;
            formatIndex = configuration.formatIndex;
            formatContent = configuration.formatContent;
            formatter = configuration.formatter;
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
            if(buttonUp != null)
            {
                buttonUp.Click += ButtonUp_Click; buttonUp.ToolTip = "Mover arriba en la lista";
                buttonDown.Click += ButtonDown_Click; buttonDown.ToolTip = "Mover abajo en la lista";
            }

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

                Changed?.Invoke(this);
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

                Changed?.Invoke(this);
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
                Changed?.Invoke(this);
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
                editor.InitEditor(entity, parentStorageId);
                editor.Closed += OnDialogClosed;
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
            editor.InitEditor(entity, parentStorageId);
            storageIds.Add(entity.StorageId);
            Changed?.Invoke(this);
            editor.Closed += OnDialogClosed;
            editor.ShowDialog();
        }

        void UpdateListOrCombo()
        {
            List<TEntity> entities;

            entities = Storage.LoadOrCreateEntities<TEntity>(storageIds, parentStorageId);

            storageIds.Clear();

            if(comboBox != null)
            {
                comboBox.Items.Clear();

                int index = 0;
                entities.ForEach(
                    (e) =>
                    {   string formatted;                        
                        if(formatter != null) { formatted = formatter.Invoke(e, index); }
                        else { formatted = Utils.FormatEntity<TEntity>(e, index, formatContent, formatIndex); }
                        comboBox.Items.Add(formatted);
                        storageIds.Add(e.StorageId);
                        index ++;
                    });
                if(comboBox.Items.Count > 0) { comboBox.SelectedIndex = 0;  }
            }
            else
            {
                listBox.Items.Clear();

                int index = 0;
                entities.ForEach(
                    (e) =>
                    {   string formatted;
                        if(formatter != null) { formatted = formatter.Invoke(e, index); }
                        else { formatted = Utils.FormatEntity<TEntity>(e, index, formatContent, formatIndex); }
                        listBox.Items.Add(formatted);
                        storageIds.Add(e.StorageId);
                        index ++;
                    });
                if(listBox.Items.Count > 0) { listBox.SelectedIndex = 0;  }
            }


        }

        void OnDialogClosed(object? sender, EventArgs e)
        {
            UpdateListOrCombo();
            if(blocker != null) { blocker.Visibility = Visibility.Hidden; }

            string storageId = editor.GetEntity().StorageId;
            SelectStorageId(storageId);
            Changed?.Invoke(this);
            editor.Closed -= OnDialogClosed;

        }

        void SelectStorageId(string storageId)
        {
            int index = storageIds.FindIndex(e => e == storageId);

            if(comboBox != null) { comboBox.SelectedIndex = index; }
            else { listBox.SelectedIndex = index; }

        }

    }

}
