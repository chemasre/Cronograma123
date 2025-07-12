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
    public struct EntityFieldConfiguration
    {
        public string? parentStorageId;
        public string? storageId;
        public TextBox? textBox;
        public Button? buttonNew;
        public Button? buttonEdit;
        public bool? titleEditable;
        public string? editorTitle;
        public UIElement? blocker;

        public static EntityFieldConfiguration CreateForTextBox(TextBox _textBox) { EntityFieldConfiguration c = new(); c.textBox = _textBox; return c; }
        public EntityFieldConfiguration WithStorageId(string _storageId) { storageId = _storageId; return this; }
        public EntityFieldConfiguration WithNew(Button _buttonNew) { buttonNew = _buttonNew; return this; }
        public EntityFieldConfiguration WithEdit(Button _buttonEdit) { buttonEdit = _buttonEdit; return this; }
        public EntityFieldConfiguration WithParentStorageId(string _parentStorageId) { parentStorageId = _parentStorageId; return this; }
        public EntityFieldConfiguration WithTitleEditable(bool _titleEditable) { titleEditable = _titleEditable; return this; }
        public EntityFieldConfiguration WithEditorTitle(string _editorTitle) { editorTitle = _editorTitle; return this; }
        public EntityFieldConfiguration WithBlocker(UIElement? _blocker) { blocker = _blocker; return this; }

    }

    public class EntityFieldController<TEntity, TEditor> where TEntity : Entity, new()
                                                    where TEditor : Window, EntityEditor<TEntity>, new()
    {
        public string StorageId { get { return storageId; } }

        TextBox textBox;
        string? parentStorageId;
        string storageId;
        Button? buttonNew;
        Button? buttonEdit;
        bool? titleEditable;
        string? editorTitle;
        UIElement? blocker;

        TEditor? editor;

        public EntityFieldController(EntityFieldConfiguration configuration)
        {
            storageId = configuration.storageId;
            parentStorageId = configuration.parentStorageId;

            textBox = configuration.textBox;
            
            buttonNew = configuration.buttonNew;
            buttonEdit = configuration.buttonEdit;

            titleEditable = configuration.titleEditable;
            editorTitle = configuration.editorTitle;

            blocker = configuration.blocker;

            if(buttonNew != null) { buttonNew.Click += ButtonNew_Click; }
            if(buttonEdit != null)  { buttonEdit.Click += ButtonEdit_Click; }

            UpdateField();
        }

        void UpdateField()
        {
            TEntity entity = Storage.LoadEntity<TEntity>(storageId, parentStorageId);

            textBox.Text = entity.Description.Trim().Substring(0, 10) + "...";
        }

        void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var entity = Storage.LoadEntity<TEntity>(storageId, parentStorageId);
            editor = new TEditor();
            if(titleEditable != null) { editor.SetEntityTitleEditable(titleEditable.Value); }
            if(editorTitle != null) { editor.SetEditorTitle(editorTitle); }
            if(blocker != null) { blocker.Visibility = Visibility.Visible; }
            editor.SetEntity(entity, parentStorageId);
            editor.Closed += OnEditorClosed;
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
            editor.Closed += OnEditorClosed;
            editor.ShowDialog();            
        }

        void OnEditorClosed(object? sender, EventArgs e)
        {
            UpdateField();
            if(blocker != null) { blocker.Visibility = Visibility.Hidden; }
            storageId = editor.GetEntity().StorageId;
            editor.Closed -= OnEditorClosed;
        }
    }
}
