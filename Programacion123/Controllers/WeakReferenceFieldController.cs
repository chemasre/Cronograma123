using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Programacion123
{

    public struct WeakReferenceFieldConfiguration<TEntity>
    {
        public string? parentStorageId;
        public string? storageId;
        public TextBox? textBox;
        public Button? buttonPick;
        public EntityFormatContent formatContent;
        public EntityFormatIndex formatIndex;
        public Func<TEntity, int, string>? formatter;
        public bool? titleEditable;
        public string? editorTitle;
        public string? pickerTitle;
        public Func<List<string>>? pickListQuery;
        public List<string>? pickList;
        public UIElement? blocker;

        public static WeakReferenceFieldConfiguration<TEntity> CreateForTextBox(TextBox _textBox) { WeakReferenceFieldConfiguration<TEntity> c = new(); c.textBox = _textBox; return c; }
        public WeakReferenceFieldConfiguration<TEntity> WithStorageId(string _storageId) { storageId = _storageId; return this; }
        public WeakReferenceFieldConfiguration<TEntity> WithParentStorageId(string _parentStorageId) { parentStorageId = _parentStorageId; return this; }
        public WeakReferenceFieldConfiguration<TEntity> WithFormat(EntityFormatContent _formatContent, EntityFormatIndex _formatIndex = EntityFormatIndex.None) { formatContent = _formatContent; formatIndex = _formatIndex; return this; }
        public WeakReferenceFieldConfiguration<TEntity> WithFormatter(Func<TEntity, int, string>? _formatter) { formatter = _formatter; return this; }
        public WeakReferenceFieldConfiguration<TEntity> WithTitleEditable(bool _titleEditable) { titleEditable = _titleEditable; return this; }
        public WeakReferenceFieldConfiguration<TEntity> WithEditorTitle(string _editorTitle) { editorTitle = _editorTitle; return this; }
        public WeakReferenceFieldConfiguration<TEntity> WithPickerTitle(string _pickerTitle) { pickerTitle = _pickerTitle; return this; }
        public WeakReferenceFieldConfiguration<TEntity> WithBlocker(UIElement? _blocker) { blocker = _blocker; return this; }
        public WeakReferenceFieldConfiguration<TEntity> WithPick(Button _buttonPick) { buttonPick = _buttonPick; return this; }
        public WeakReferenceFieldConfiguration<TEntity> WithPickListQuery(Func<List<string>> _pickListQuery) { pickListQuery = _pickListQuery; return this; }
        public WeakReferenceFieldConfiguration<TEntity> WithPickList(List<string> _pickList) { pickList = _pickList; return this; }
    }

    public class WeakReferenceFieldController<TEntity, TPicker> where TEntity : Entity, new()
                                                    where TPicker : Window, IEntityPicker<TEntity>, new()
    {
        public delegate void OnStorageIdChanged(WeakReferenceFieldController<TEntity, TPicker> controller);

        public event OnStorageIdChanged Changed;

        public string? StorageId { get { return storageId; } }

        TextBox textBox;
        string? parentStorageId;
        string? storageId;
        Button? buttonPick;
        EntityFormatContent formatContent;
        EntityFormatIndex formatIndex;
        Func<TEntity, int, string>? formatter;
        bool? titleEditable;
        string? pickerTitle;
        Func<List<string>>? pickListQuery;
        List<string>? pickList;
        UIElement? blocker;

        TPicker? picker;

        public WeakReferenceFieldController(WeakReferenceFieldConfiguration<TEntity> configuration)
        {
            storageId = configuration.storageId;
            parentStorageId = configuration.parentStorageId;

            textBox = configuration.textBox;
            textBox.Background = new SolidColorBrush((Color)Application.Current.Resources["ColorLocked"]);
            textBox.IsReadOnly = true;
            formatContent = configuration.formatContent;
            formatIndex = configuration.formatIndex;
            formatter = configuration.formatter;
            
            buttonPick = configuration.buttonPick;

            pickerTitle = configuration.pickerTitle;

            pickListQuery = configuration.pickListQuery;
            pickList = configuration.pickList;

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

                if(formatter != null) { textBox.Text = formatter.Invoke(entity, 0); }
                string s = (formatContent == EntityFormatContent.Description ? entity.Description : entity.Title).Trim();
                textBox.Text = s.Substring(0, Math.Min(100, s.Length)) + "...";
            }
        }

        void ButtonPick_Click(object sender, RoutedEventArgs e)
        {
            var entity = Storage.LoadOrCreateEntity<TEntity>(storageId, parentStorageId);

            picker = new TPicker();
            if(pickerTitle != null) { picker.SetPickerTitle(pickerTitle); }
            picker.SetFormat(formatContent, formatIndex);
            picker.SetFormatter(formatter);
            if(blocker != null) { blocker.Visibility = Visibility.Visible; }

            List<string> storageIdList;
            List<TEntity> entityList;
            if(pickListQuery != null)
            {
                // We use find "Find" because we don't assume all storage id's share the same parent
                storageIdList = pickListQuery.Invoke();
                entityList = Storage.FindEntities<TEntity>(storageIdList);
            }
            else if(pickList != null)
            {
                // We use find "Find" because we don't assume all storage id's share the same parent
                storageIdList = pickList;
                entityList = Storage.FindEntities<TEntity>(storageIdList);
            }
            else
            {
                // We use find "LoadOrCreate" because in this case we know all entities share the same parent
                storageIdList = Storage.GetStorageIds<TEntity>(Storage.LoadAllEntities<TEntity>(parentStorageId));
                entityList = Storage.LoadOrCreateEntities<TEntity>(storageIdList, parentStorageId);
            }

            picker.SetSinglePickerEntities(entity, entityList);
            picker.Closed += OnDialogClosed;

            picker.ShowDialog();     
        }

        void OnDialogClosed(object? sender, EventArgs e)
        {
            if(blocker != null) { blocker.Visibility = Visibility.Hidden; }

            if(!picker.GetWasCancelled())
            {
                storageId = picker.GetPickedEntity()?.StorageId;
                Changed?.Invoke(this);
            }

            picker.Closed -= OnDialogClosed;

            UpdateField();

        }

        public void Finish()
        {
            if(buttonPick != null) { buttonPick.Click -= ButtonPick_Click; }
        }
    }

}
