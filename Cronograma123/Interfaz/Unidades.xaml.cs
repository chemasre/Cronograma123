using System.Windows;
using System.Windows.Controls;
using Cronogramador;
using Message = CronogramaMe.Interfaz.MessageBox;

namespace CronogramaMe
{
    /// <summary>
    /// Interaction logic for Unidades.xaml
    /// </summary>
    public partial class Unidades : Window
    {
        Asignatura asignatura;

        public Unidades(Asignatura a)
        {
            asignatura = a;

            InitializeComponent();

            Owner = Application.Current.MainWindow;

            NombreCurso.Text = a.ObtenNombre();

            ActualizaUnidades();
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void QuitarUnidad_Click(object sender, RoutedEventArgs e)
        {
            int unidad = UnidadQuitar.SelectedIndex + 1;

            if(asignatura.TieneUF(unidad))
            {
                asignatura.EliminaUF(unidad);

                if (UnidadQuitar.SelectedIndex + 1 < UnidadQuitar.Items.Count)
                {
                    UnidadQuitar.SelectedIndex++;
                }
            }
            else
            {
                Message m = new Message(Message.Type.alert, "No has añadido esa unidad");
                m.ShowDialog();

                return;
            }

            ActualizaUnidades();


        }

        private void ActualizaUnidades()
        {
            ListaUnidades.Items.Clear();

            List<string> dias;
            for (int i = 0; i < asignatura.ObtenNumUFs(); i++)
            {
                int unidad = asignatura.ObtenUFPorIndice(i);
                string titulo = asignatura.ObtenTituloUF(unidad);
                int horas = asignatura.ObtenHorasUF(unidad);

                ListaUnidades.Items.Add(unidad + ": " + titulo + " (" + horas + (horas > 1 ? " horas" : " hora") + ")");

            }

        }

        private void AnyadirUnidad_Click(object sender, RoutedEventArgs e)
        {
            int unidad = UnidadAnyadir.SelectedIndex + 1;

            if(!asignatura.TieneUF(unidad))
            {
                string titulo = TituloUnidadAnyadir.Text;
                int horas;
                if(Int32.TryParse(HorasUnidadAnyadir.Text, out horas))
                {
                    asignatura.AnyadeUF(unidad, titulo, horas);

                    if(UnidadAnyadir.SelectedIndex + 1 < UnidadAnyadir.Items.Count)
                    {
                        UnidadAnyadir.SelectedIndex ++;
                    }

                    TituloUnidadAnyadir.Text = "Título unidad";
                }
                else
                {
                    Message m = new Message(Message.Type.alert, "Tienes que escribir un número en la casilla de horas");
                    m.ShowDialog();
                    HorasUnidadAnyadir.Text = "10";
                    return;
                }
            }
            else
            {
                Message m = new Message(Message.Type.alert, "La unidad no se puede añadir porque ya la tienes. Quítala primero.");
                m.ShowDialog();
                return;

            }

            ActualizaUnidades();
        }

        private void NombreCurso_TextChanged(object sender, TextChangedEventArgs e)
        {
            asignatura.PonNombre(NombreCurso.Text);
        }

        private void ListaUnidades_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ListaUnidades.SelectedItem != null)
            {
                string text = ListaUnidades.SelectedItem.ToString();
                string[] parts = text.Split(":");
                int unidad = Int32.Parse(parts[0].Trim());

                UnidadQuitar.SelectedIndex = unidad - 1;

            }
        }
    }
}
