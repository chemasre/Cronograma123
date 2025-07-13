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

        public TEntity? GetEntity()
        {
            if(ComboEntitities.SelectedIndex < 0) { return null; }
            else { return entities[ComboEntitities.SelectedIndex]; }
        }

        public void SetEntity(TEntity? _entity, string? _parentStorageId = null)
        {
            entities = Storage.LoadEntities<TEntity>(_parentStorageId);

            ComboEntitities.Items.Clear();
            entities.ForEach(e => ComboEntitities.Items.Add(e.Title) );

            ComboEntitities.SelectedIndex = entities.FindIndex(e => e.StorageId == _entity.StorageId);
            
        }

        public void SetPickerTitle(string title)
        {
            TextPickerTitle.Text = title;
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
