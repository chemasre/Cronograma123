using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Programacion123
{

    /// <summary>
    /// Lógica de interacción para EntityPickerBase.xaml
    /// </summary>
    public partial class EntityPickerBase : Window
    {
        public EntityPickerBase()
        {
            InitializeComponent();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();

        }
    }

    public class EntityPicker<TEntity> : EntityPickerBase, IEntityPicker<TEntity> where TEntity:Entity, new()
    {
        List<TEntity>? entities;
        bool isMultiPickerMode;
        List<MultiSelectItem> multiSelectItemList;
        EntityFormatContent formatContent;
        EntityFormatIndex formatIndex;
        Func<TEntity, int, string>? formatter;

        class MultiSelectItem
        {
            public TEntity entity;
            public int index;

            EntityPicker<TEntity> picker;

            public MultiSelectItem(TEntity _entity, EntityPicker<TEntity> _picker)
            {
                entity = _entity;
                picker = _picker;

            }


            public override string ToString()
            {
                string formatted;
                if(picker.formatter == null) { formatted = Utils.FormatEntity<TEntity>(entity, index, picker.formatContent, picker.formatIndex); }
                else { formatted = picker.formatter.Invoke(entity, index); }

                return formatted;
            }
        }        

        public TEntity? GetPickedEntity()
        {
            if(ListBoxEntities.SelectedIndex < 0) { return null; }
            else { return entities[ListBoxEntities.SelectedIndex]; }
        }

        public void SetSinglePickerEntities(TEntity? _pickedEntity, List<TEntity> _entities)
        {
            entities = _entities;

            int index = 0;
            ListBoxEntities.Items.Clear();
            entities.ForEach(
                (e) =>
                {
                    string formatted;
                    if(formatter == null) { formatted = Utils.FormatEntity<TEntity>(e, index, formatContent, formatIndex); }
                    else { formatted = formatter.Invoke(e, index); }
                    ListBoxEntities.Items.Add(formatted);
                    index ++;
                });

            ListBoxEntities.SelectionMode = SelectionMode.Single;
            ListBoxEntities.SelectedIndex = entities.FindIndex(e => e.StorageId == _pickedEntity.StorageId);
            
            isMultiPickerMode = false;
        }        

        public void SetPickerTitle(string title)
        {
            TextPickerTitle.Text = title;
        }

        public List<TEntity> GetPickedEntities()
        {
            List<TEntity> result = new();

            foreach(MultiSelectItem e in ListBoxEntities.SelectedItems) { result.Add(e.entity); }

            return result;
        }

        public void SetMultiPickerEntities(List<TEntity> selectedEntities, List<TEntity> _entities)
        {
            entities = _entities;

            multiSelectItemList = new();
            ListBoxEntities.Items.Clear();
            int index = 0;

            foreach(TEntity e in entities)
            {
                MultiSelectItem item = new(e, this);
                item.index = index;
                ListBoxEntities.Items.Add(item);
                multiSelectItemList.Add(item);
                index ++;
            }

            ListBoxEntities.SelectionMode = SelectionMode.Extended;
            ListBoxEntities.SelectedItems.Clear();

            foreach(TEntity e1 in selectedEntities)
            {
                ListBoxEntities.SelectedItems.Add(multiSelectItemList.Find(e2 => e2.entity.StorageId == e1.StorageId));
            }

            isMultiPickerMode = true;
        }

        void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        public void SetFormat(EntityFormatContent _formatContent, EntityFormatIndex _formatIndex = EntityFormatIndex.none)
        {
            formatContent = _formatContent;
            formatIndex = _formatIndex;
        }

        public void SetFormatter(Func<TEntity, int, string>? _formatter)
        {
            formatter = _formatter;
        }
    }
}
