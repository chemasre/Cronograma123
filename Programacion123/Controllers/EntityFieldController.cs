using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Programacion123
{
    public enum EntityFieldDisplayType
    {
        description,
        title
    }

    public struct EntityFieldConfiguration<TEntity>
    {
        public string? parentStorageId;
        public string? storageId;
        public bool? storageIdIsWeak;
        public TextBox? textBox;
        public Button? buttonNew;
        public Button? buttonEdit;
        public Button? buttonPick;
        public EntityFieldDisplayType fieldDisplayType;
        public bool? titleEditable;
        public string? editorTitle;
        public string? pickerTitle;
        public UIElement? blocker;

        public static EntityFieldConfiguration<TEntity> CreateForTextBox(TextBox _textBox) { EntityFieldConfiguration<TEntity> c = new(); c.textBox = _textBox; return c; }
        public EntityFieldConfiguration<TEntity> WithStorageId(string _storageId, bool _isWeak = false) { storageId = _storageId; storageIdIsWeak = _isWeak; return this; }
        public EntityFieldConfiguration<TEntity> WithNew(Button _buttonNew) { buttonNew = _buttonNew; return this; }
        public EntityFieldConfiguration<TEntity> WithEdit(Button _buttonEdit) { buttonEdit = _buttonEdit; return this; }
        public EntityFieldConfiguration<TEntity> WithParentStorageId(string _parentStorageId) { parentStorageId = _parentStorageId; return this; }
        public EntityFieldConfiguration<TEntity> WithFieldDisplayType(EntityFieldDisplayType _fieldDisplayType) { fieldDisplayType = _fieldDisplayType; return this; }
        public EntityFieldConfiguration<TEntity> WithTitleEditable(bool _titleEditable) { titleEditable = _titleEditable; return this; }
        public EntityFieldConfiguration<TEntity> WithEditorTitle(string _editorTitle) { editorTitle = _editorTitle; return this; }
        public EntityFieldConfiguration<TEntity> WithPickerTitle(string _pickerTitle) { pickerTitle = _pickerTitle; return this; }
        public EntityFieldConfiguration<TEntity> WithBlocker(UIElement? _blocker) { blocker = _blocker; return this; }
        public EntityFieldConfiguration<TEntity> WithPick(Button _buttonPick) { buttonPick = _buttonPick; return this; }
    }

    public class EntityFieldController<TEntity, TEditor, TPicker> where TEntity : Entity, new()
                                                    where TEditor : Window, IEntityEditor<TEntity>, new()
                                                    where TPicker : Window, IEntityPicker<TEntity>, new()
    {
        public enum State
        {
            idle,
            waitingForNew,
            waitingForEdit,
            waitingForPick
        };

        public string? StorageId { get { return storageId; } }

        TextBox textBox;
        string? parentStorageId;
        string? storageId;
        bool storageIdIsWeak;
        Button? buttonNew;
        Button? buttonEdit;
        Button? buttonPick;
        EntityFieldDisplayType fieldDisplayType;
        bool? titleEditable;
        string? editorTitle;
        string? pickerTitle;
        UIElement? blocker;

        TEditor? editor;
        TPicker? picker;

        State state;

        public EntityFieldController(EntityFieldConfiguration<TEntity> configuration)
        {
            storageId = configuration.storageId;
            parentStorageId = configuration.parentStorageId;
            storageIdIsWeak = configuration.storageIdIsWeak.HasValue ? configuration.storageIdIsWeak.Value : false;

            textBox = configuration.textBox;
            fieldDisplayType = configuration.fieldDisplayType;
            
            buttonNew = configuration.buttonNew;
            buttonEdit = configuration.buttonEdit;
            buttonPick = configuration.buttonPick;

            titleEditable = configuration.titleEditable;
            editorTitle = configuration.editorTitle;
            pickerTitle = configuration.pickerTitle;

            blocker = configuration.blocker;

            if(buttonNew != null) { buttonNew.Click += ButtonNew_Click; buttonNew.ToolTip = "Crear"; }
            if(buttonEdit != null)  { buttonEdit.Click += ButtonEdit_Click; buttonEdit.ToolTip = "Modificar"; }
            if(buttonPick != null) { buttonPick.Click += ButtonPick_Click; buttonPick.ToolTip = "Elegir"; }

            UpdateField();

            state = State.idle;
        }

        public TEntity? GetEntity()
        {
            if(storageId == null) { return null; }
            else { return Storage.LoadOrCreateEntity<TEntity>(storageId, parentStorageId); }
        }

        void UpdateField()
        {
            if(storageId == null)
            {
                textBox.Text = "(nada seleccionado)";
            }
            else if(storageIdIsWeak && !Storage.ExistsEntity<TEntity>(storageId, parentStorageId))
            {
                textBox.Text = "(no encontrado)";
            }
            else
            {
                TEntity entity = Storage.LoadOrCreateEntity<TEntity>(storageId, parentStorageId);

                string trimmed = (fieldDisplayType == EntityFieldDisplayType.description ? entity.Description : entity.Title).Trim();
                textBox.Text = trimmed.Substring(0, Math.Min(100, trimmed.Length)) + "...";
            }
        }

        void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var entity = Storage.LoadOrCreateEntity<TEntity>(storageId, parentStorageId);
            editor = new TEditor();
            if(titleEditable != null) { editor.SetEntityTitleEditable(titleEditable.Value); }
            if(editorTitle != null) { editor.SetEditorTitle(editorTitle); }
            if(blocker != null) { blocker.Visibility = Visibility.Visible; }
            editor.SetEntity(entity, parentStorageId);
            editor.Closed += OnDialogClosed;

            state = State.waitingForEdit;
            editor.ShowDialog();            
        }

        void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            var entity = new TEntity();
            editor = new TEditor();
            if(titleEditable != null) { editor.SetEntityTitleEditable(titleEditable.Value); }
            if(editorTitle != null) { editor.SetEditorTitle(editorTitle); }
            if(blocker != null) { blocker.Visibility = Visibility.Visible; }
            editor.SetEntity(entity, parentStorageId);
            editor.Closed += OnDialogClosed;

            state = State.waitingForNew;
            editor.ShowDialog();            
        }


        void ButtonPick_Click(object sender, RoutedEventArgs e)
        {
            var entity = Storage.LoadOrCreateEntity<TEntity>(storageId, parentStorageId);

            picker = new TPicker();
            if(pickerTitle != null) { picker.SetPickerTitle(pickerTitle); }
            if(blocker != null) { blocker.Visibility = Visibility.Visible; }
            picker.SetEntity(entity, parentStorageId);
            picker.Closed += OnDialogClosed;

            state = State.waitingForPick;
            picker.ShowDialog();     
        }

        void OnDialogClosed(object? sender, EventArgs e)
        {
            if(blocker != null) { blocker.Visibility = Visibility.Hidden; }

            if(state == State.waitingForNew || state == State.waitingForPick)
            {
                if(!storageIdIsWeak && storageId != null)
                {
                    TEntity previous = Storage.LoadOrCreateEntity<TEntity>(storageId, parentStorageId);
                    previous.Delete(parentStorageId);
                    storageId = null;
                }
            }


            if(state == State.waitingForNew || state == State.waitingForEdit)
            {
                storageId = editor.GetEntity().StorageId;
                editor.Closed -= OnDialogClosed;
            }
            else
            {
                storageId = picker.GetEntity().StorageId;
                picker.Closed -= OnDialogClosed;
            }

            UpdateField();

        }
    }

}
