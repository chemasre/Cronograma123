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

    public struct StrongReferenceFieldConfiguration<TEntity>
    {
        public string? parentStorageId;
        public string? storageId;
        public TextBox? textBox;
        public Button? buttonNew;
        public Button? buttonEdit;
        public Button? buttonPick;
        public EntityFieldDisplayType fieldDisplayType;
        public bool? titleEditable;
        public string? editorTitle;
        public UIElement? blocker;

        public static StrongReferenceFieldConfiguration<TEntity> CreateForTextBox(TextBox _textBox) { StrongReferenceFieldConfiguration<TEntity> c = new(); c.textBox = _textBox; return c; }
        public StrongReferenceFieldConfiguration<TEntity> WithStorageId(string _storageId) { storageId = _storageId; return this; }
        public StrongReferenceFieldConfiguration<TEntity> WithNew(Button _buttonNew) { buttonNew = _buttonNew; return this; }
        public StrongReferenceFieldConfiguration<TEntity> WithEdit(Button _buttonEdit) { buttonEdit = _buttonEdit; return this; }
        public StrongReferenceFieldConfiguration<TEntity> WithParentStorageId(string _parentStorageId) { parentStorageId = _parentStorageId; return this; }
        public StrongReferenceFieldConfiguration<TEntity> WithFieldDisplayType(EntityFieldDisplayType _fieldDisplayType) { fieldDisplayType = _fieldDisplayType; return this; }
        public StrongReferenceFieldConfiguration<TEntity> WithTitleEditable(bool _titleEditable) { titleEditable = _titleEditable; return this; }
        public StrongReferenceFieldConfiguration<TEntity> WithEditorTitle(string _editorTitle) { editorTitle = _editorTitle; return this; }
        public StrongReferenceFieldConfiguration<TEntity> WithBlocker(UIElement? _blocker) { blocker = _blocker; return this; }
    }

    public class StrongReferenceFieldController<TEntity, TEditor> where TEntity : Entity, new()
                                                    where TEditor : Window, IEntityEditor<TEntity>, new()
    {
        public delegate void OnStorageIdChanged(StrongReferenceFieldController<TEntity, TEditor> controller, string storageId);

        public event OnStorageIdChanged StorageIdChanged;

        public string? StorageId { get { return storageId; } }

        TextBox textBox;
        string? parentStorageId;
        string? storageId;
        Button? buttonNew;
        Button? buttonEdit;
        EntityFieldDisplayType fieldDisplayType;
        bool? titleEditable;
        string? editorTitle;
        UIElement? blocker;

        TEditor? editor;

        public StrongReferenceFieldController(StrongReferenceFieldConfiguration<TEntity> configuration)
        {
            storageId = configuration.storageId;
            parentStorageId = configuration.parentStorageId;

            textBox = configuration.textBox;
            fieldDisplayType = configuration.fieldDisplayType;
            
            buttonNew = configuration.buttonNew;
            buttonEdit = configuration.buttonEdit;

            titleEditable = configuration.titleEditable;
            editorTitle = configuration.editorTitle;

            blocker = configuration.blocker;

            if(buttonNew != null) { buttonNew.Click += ButtonNew_Click; buttonNew.ToolTip = "Crear"; }
            if(buttonEdit != null)  { buttonEdit.Click += ButtonEdit_Click; buttonEdit.ToolTip = "Modificar"; }

            UpdateField();
        }

        public TEntity? GetEntity()
        {
            if(storageId == null) { return null; }
            else { return Storage.LoadOrCreateEntity<TEntity>(storageId, parentStorageId); }
        }

        void UpdateField()
        {
            TEntity entity = Storage.LoadOrCreateEntity<TEntity>(storageId, parentStorageId);

            string trimmed = (fieldDisplayType == EntityFieldDisplayType.description ? entity.Description : entity.Title).Trim();
            textBox.Text = trimmed.Substring(0, Math.Min(100, trimmed.Length)) + "...";
        }

        void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var entity = Storage.LoadOrCreateEntity<TEntity>(storageId, parentStorageId);
            editor = new TEditor();
            if(titleEditable != null) { editor.SetEntityTitleEditable(titleEditable.Value); }
            if(editorTitle != null) { editor.SetEditorTitle(editorTitle); }
            if(blocker != null) { blocker.Visibility = Visibility.Visible; }
            editor.InitEditor(entity, parentStorageId);
            editor.Closed += OnDialogClosed;

            editor.ShowDialog();            
        }

        void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            var entity = new TEntity();
            editor = new TEditor();
            if(titleEditable != null) { editor.SetEntityTitleEditable(titleEditable.Value); }
            if(editorTitle != null) { editor.SetEditorTitle(editorTitle); }
            if(blocker != null) { blocker.Visibility = Visibility.Visible; }
            editor.InitEditor(entity, parentStorageId);
            editor.Closed += OnDialogClosed;

            editor.ShowDialog();            
        }


        void OnDialogClosed(object? sender, EventArgs e)
        {
            if(blocker != null) { blocker.Visibility = Visibility.Hidden; }

            if(storageId != null)
            {
                TEntity previous = Storage.LoadOrCreateEntity<TEntity>(storageId, parentStorageId);
                previous.Delete(parentStorageId);
                storageId = null;
            }

            storageId = editor.GetEntity().StorageId;
            StorageIdChanged?.Invoke(this, storageId);
            editor.Closed -= OnDialogClosed;

            UpdateField();


        }
    }

}
