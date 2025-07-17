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

    public struct WeakReferenceFieldConfiguration<TEntity>
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
        public string? pickerTitle;
        public Func<List<string>>? pickListQuery;
        public UIElement? blocker;

        public static WeakReferenceFieldConfiguration<TEntity> CreateForTextBox(TextBox _textBox) { WeakReferenceFieldConfiguration<TEntity> c = new(); c.textBox = _textBox; return c; }
        public WeakReferenceFieldConfiguration<TEntity> WithStorageId(string _storageId) { storageId = _storageId; return this; }
        public WeakReferenceFieldConfiguration<TEntity> WithNew(Button _buttonNew) { buttonNew = _buttonNew; return this; }
        public WeakReferenceFieldConfiguration<TEntity> WithEdit(Button _buttonEdit) { buttonEdit = _buttonEdit; return this; }
        public WeakReferenceFieldConfiguration<TEntity> WithParentStorageId(string _parentStorageId) { parentStorageId = _parentStorageId; return this; }
        public WeakReferenceFieldConfiguration<TEntity> WithFieldDisplayType(EntityFieldDisplayType _fieldDisplayType) { fieldDisplayType = _fieldDisplayType; return this; }
        public WeakReferenceFieldConfiguration<TEntity> WithTitleEditable(bool _titleEditable) { titleEditable = _titleEditable; return this; }
        public WeakReferenceFieldConfiguration<TEntity> WithEditorTitle(string _editorTitle) { editorTitle = _editorTitle; return this; }
        public WeakReferenceFieldConfiguration<TEntity> WithPickerTitle(string _pickerTitle) { pickerTitle = _pickerTitle; return this; }
        public WeakReferenceFieldConfiguration<TEntity> WithBlocker(UIElement? _blocker) { blocker = _blocker; return this; }
        public WeakReferenceFieldConfiguration<TEntity> WithPick(Button _buttonPick) { buttonPick = _buttonPick; return this; }
        public WeakReferenceFieldConfiguration<TEntity> WithPickListQuery(Func<List<string>>? _pickListQuery) { pickListQuery = _pickListQuery; return this; }
    }

    public class WeakReferenceFieldController<TEntity, TPicker> where TEntity : Entity, new()
                                                    where TPicker : Window, IEntityPicker<TEntity>, new()
    {
        public delegate void OnStorageIdChanged(WeakReferenceFieldController<TEntity, TPicker> controller, string storageId);

        public event OnStorageIdChanged StorageIdChanged;

        public string? StorageId { get { return storageId; } }

        TextBox textBox;
        string? parentStorageId;
        string? storageId;
        Button? buttonNew;
        Button? buttonEdit;
        Button? buttonPick;
        EntityFieldDisplayType fieldDisplayType;
        bool? titleEditable;
        string? pickerTitle;
        Func<List<string>>? pickListQuery;
        UIElement? blocker;

        TPicker? picker;

        public WeakReferenceFieldController(WeakReferenceFieldConfiguration<TEntity> configuration)
        {
            storageId = configuration.storageId;
            parentStorageId = configuration.parentStorageId;

            textBox = configuration.textBox;
            fieldDisplayType = configuration.fieldDisplayType;
            
            buttonNew = configuration.buttonNew;
            buttonEdit = configuration.buttonEdit;
            buttonPick = configuration.buttonPick;

            pickerTitle = configuration.pickerTitle;

            pickListQuery = configuration.pickListQuery;

            blocker = configuration.blocker;

            if(buttonPick != null) { buttonPick.Click += ButtonPick_Click; buttonPick.ToolTip = "Elegir"; }

            UpdateField();

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
            else
            {
                TEntity entity = Storage.LoadOrCreateEntity<TEntity>(storageId, parentStorageId);

                string trimmed = (fieldDisplayType == EntityFieldDisplayType.description ? entity.Description : entity.Title).Trim();
                textBox.Text = trimmed.Substring(0, Math.Min(100, trimmed.Length)) + "...";
            }
        }

        void ButtonPick_Click(object sender, RoutedEventArgs e)
        {
            var entity = Storage.LoadOrCreateEntity<TEntity>(storageId, parentStorageId);

            picker = new TPicker();
            if(pickerTitle != null) { picker.SetPickerTitle(pickerTitle); }
            if(blocker != null) { blocker.Visibility = Visibility.Visible; }

            List<string> storageIdList;
            if(pickListQuery != null) { storageIdList = pickListQuery.Invoke(); }
            else { storageIdList = Storage.GetStorageIds<TEntity>(Storage.LoadAllEntities<TEntity>(parentStorageId)); }
            List<TEntity> entityList = Storage.LoadOrCreateEntities<TEntity>(storageIdList, parentStorageId);
            picker.InitSinglePicker(entity, entityList);
            picker.Closed += OnDialogClosed;

            picker.ShowDialog();     
        }

        void OnDialogClosed(object? sender, EventArgs e)
        {
            if(blocker != null) { blocker.Visibility = Visibility.Hidden; }

            storageId = picker.GetPickedEntity()?.StorageId;
            StorageIdChanged?.Invoke(this, storageId);
            picker.Closed -= OnDialogClosed;

            UpdateField();

        }
    }

}
