using System;
using System.Collections.Generic;
using System.Linq;
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

        class MultiSelectItem
        {
            public TEntity entity;

            public MultiSelectItem(TEntity _entity)
            {
                entity = _entity;
            }
            public override string ToString()
            {
                return entity.Title;
            }
        }

        public TEntity? GetPickedEntity()
        {
            if(ListBoxEntities.SelectedIndex < 0) { return null; }
            else { return entities[ListBoxEntities.SelectedIndex]; }
        }

        public void InitSinglePicker(TEntity? _pickedEntity, List<TEntity> _entities)
        {
            entities = _entities;

            ListBoxEntities.Items.Clear();
            entities.ForEach(e => ListBoxEntities.Items.Add(e.Title) );

            ListBoxEntities.SelectionMode = SelectionMode.Single;
            ListBoxEntities.SelectedIndex = entities.FindIndex(e => e.StorageId == _pickedEntity.StorageId);
            
            isMultiPickerMode = false;
        }

        public void SetPickerTitle(string title)
        {
            TextPickerTitle.Text = title;
        }

        List<TEntity> IEntityPicker<TEntity>.GetPickedEntities()
        {
            List<TEntity> result = new();

            foreach(MultiSelectItem e in ListBoxEntities.SelectedItems) { result.Add(e.entity); }

            return result;
        }

        void IEntityPicker<TEntity>.InitMultiPicker(List<TEntity> selectedEntities, List<TEntity> _entities)
        {
            entities = _entities;

            multiSelectItemList = new();
            ListBoxEntities.Items.Clear();

            foreach(TEntity e in entities)
            {
                MultiSelectItem item = new(e);
                ListBoxEntities.Items.Add(item);
                multiSelectItemList.Add(item);
            }

            ListBoxEntities.SelectionMode = SelectionMode.Extended;
            ListBoxEntities.SelectedItems.Clear();

            foreach(TEntity e1 in selectedEntities)
            {
                ListBoxEntities.SelectedItems.Add(multiSelectItemList.Find(e2 => e2.entity.StorageId == e1.StorageId));
            }

            isMultiPickerMode = true;
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
