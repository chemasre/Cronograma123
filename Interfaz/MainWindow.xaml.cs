using Cronogramador;
using CronogramaMe.Interfaz;
using Microsoft.Win32;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Message = CronogramaMe.Interfaz.MessageBox;

namespace CronogramaMe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        Interfaz.Config config;

        public const string companyName = "Sinestesia Game Design";
        public const string projectName = "Cronograma123";
        public const string projectUrl = @"https:\\sinestesiagamedesign.es\cronogramador";
        public const string projectVersion = "0.9";
        public const string projectYear = "2024";
        public const string projectLicense = "CC BY-NC-ND";

        Cronogramador.Cronograma cronograma;
        Cronogramador.Calendario calendario;
        Cronogramador.Asignatura asignatura;

        public MainWindow()
        {
            CultureInfo culture = new System.Globalization.CultureInfo("es-ES");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            calendario = new Cronogramador.Calendario();
            if (File.Exists("calendario.json")) { calendario.Carga("calendario.json"); }
            else
            {   DateTime now = DateTime.Now;
                int d = now.Day;
                int m = now.Month;
                int a = now.Year;
                DateTime dia = new DateTime(a, m, d);

                calendario.PonDiaInicio(dia);
                calendario.PonDiaFin(dia);
            }

            asignatura = new Cronogramador.Asignatura();
            if(File.Exists("asignatura.json")) { asignatura.Carga("asignatura.json"); }
            else { asignatura.PonNombre("Nombre de mi asignatura"); }

            InitializeComponent();

            Title = projectName + " v" + projectVersion;
            Titulo.Text = projectName;
            RenderOptions.SetBitmapScalingMode(WWW, BitmapScalingMode.HighQuality);

            cronograma = new Cronogramador.Cronograma(calendario, asignatura);
            if (!File.Exists("config.json")) { cronograma.GuardaConfiguracion("config.json"); }
            cronograma.CargaConfiguracion("config.json");

            config = new Interfaz.Config();
            if (!File.Exists("configInterfaz.json")) { GuardaConfiguracion("configInterfaz.json"); }
            CargaConfiguracion("configInterfaz.json");

            Message m2 = null;

            if (config.compruebaExcelDisponibleAlIniciar)
            {

                ShowOverlay();

                m2 = new Message(Message.Type.info, "A continuación comprobaremos si tienes una versión compatible de Excel instalada en el sistema");
                m2.ShowDialog();

                if (!cronograma.CompruebaExcelDisponible())
                {
                    m2 = new Message(Message.Type.alert, "No se ha encontrado una versión compatible de Excel instalada. La aplicación se tiene que cerrar ahora");
                    m2.ShowDialog();
                    Close();
                    return;
                }
                else
                {
                    m2 = new Message(Message.Type.info, "¡Felicidades! Se ha encontrado una versión compatible de Excel instalada");
                    m2.ShowDialog();
                }

                m2 = new Message(Message.Type.question, "¿Quieres comprobar si excel está disponible la próxima vez que inicies el programa?");
                m2.ShowDialog();

                if(!m2.result) { config.compruebaExcelDisponibleAlIniciar = false; }


                HideOverlay();
            }


            if (config.primeraEjecucion)
            {
                m2 = new Message(Message.Type.question, "Parece que es la primera vez que ejecutas el programa ¿quieres ver el tutorial?");
                m2.ShowDialog();

                if (m2.result)
                {
                    OpenTutorial();
                }
                else
                {
                    m2 = new Message(Message.Type.info, "Recuerda que tienes el tutorial disponible en la ventana principal del programa");
                    m2.ShowDialog();
                }

                config.primeraEjecucion = false;
            }



        }

        public void GuardaConfiguracion(string nombreFichero)
        {
            var stream = new FileStream(nombreFichero, FileMode.Create, FileAccess.Write);
            var writer = new StreamWriter(stream);
            JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerOptions.Default);
            options.WriteIndented = true;
            writer.Write(JsonSerializer.Serialize<Interfaz.Config>(config, options));
            writer.Close();
        }

        public void CargaConfiguracion(string nombreFichero)
        {
            var stream = new FileStream(nombreFichero, FileMode.Open, FileAccess.Read);

            var reader = new StreamReader(stream);
            string text = reader.ReadToEnd();

            config = JsonSerializer.Deserialize<Interfaz.Config>(text);

            reader.Close();
        }

        private void Calendario_Click(object sender, RoutedEventArgs e)
        {
            ShowOverlay();
            Calendario c = new Calendario(calendario);
            c.ShowDialog();
            HideOverlay();
        }

        private void Horario_Click(object sender, RoutedEventArgs e)
        {
            ShowOverlay();
            Horario h = new Horario(asignatura);
            h.ShowDialog();
            HideOverlay();
        }

        private void Unidades_Click(object sender, RoutedEventArgs e)
        {
            ShowOverlay();
            Unidades u = new Unidades(asignatura);
            u.ShowDialog();
            HideOverlay();
        }

        private void Genera_Click(object sender, RoutedEventArgs e)
        {

            ShowOverlay();

            Message m = null;

            if(!cronograma.CompruebaExcelDisponible())
            {
                m = new Message(Message.Type.alert, "No tienes una versión compatible de Excel instalada en el sistema");
                m.ShowDialog();
                HideOverlay();
                return;
            }

            Asignatura.Completitud completaAsignatura = asignatura.CompruebaCompleta();
            if(completaAsignatura == Asignatura.Completitud.faltanDiasSemana)
            {
                m = new Message(Message.Type.alert, "Falta completar el horario porque no has introducido ningún día de la semana");
                m.ShowDialog();

                HideOverlay();
                return;
            }
            else if(completaAsignatura == Asignatura.Completitud.faltanUnidades)
            {
                m = new Message(Message.Type.alert, "Falta completar la asignatura porque no has introducido ninguna unidad");
                m.ShowDialog();

                HideOverlay();
                return;
            }

            m = new Message(Message.Type.question, "¿Quieres que las unidades siempre empiecen en un día nuevo? Si dices que sí, puede que te queden unas horas libres el día en que termines una unidad");
            m.ShowDialog();
            cronograma.ObtenConfiguracion().empezarUfsEnDiaNuevo = m.result;

            m = new Message(Message.Type.question, "¿Quieres aplicar un estilo continuo?\nSi dices que sí, se resaltarán con el color correspondiente todos los días entre el inicio y el fin de una unidad.\n" +
                                                   "Si dices que no, sólo se resaltarán los días en que hay clase.");
            m.ShowDialog();
            cronograma.ObtenConfiguracion().estiloContinuo = m.result;


            SaveFileDialog saveFile = new SaveFileDialog();           
            saveFile.AddExtension = true;
            saveFile.CheckPathExists = true;
            saveFile.OverwritePrompt = true;
            saveFile.Filter = "Todos los archivos (*.*)|*.*|Hoja de cálculo de Excel (*.xlsx)|*.xlsx";
            saveFile.FilterIndex = 2;
            saveFile.DefaultExt = "xlsx";

            if (saveFile.ShowDialog().GetValueOrDefault())
            {
                cronograma.GeneraExcel(saveFile.FileName);

                Cronograma.Resultado resultado = cronograma.CompruebaResultado();

                if(resultado == Cronograma.Resultado.hayDiasMasAllaDelFinal)
                {
                    m = new Message(Message.Type.alert, "Se han generado días de clase más allá del final del curso. Ajusta las fechas de inicio y fin o amplía el horario");
                    m.ShowDialog();
                }
                else if(resultado == Cronograma.Resultado.errorAlGuardarFichero)
                {
                    m = new Message(Message.Type.error, "Se ha producido un error al guardar el fichero. Si lo tienes abierto en Excel, ciérralo y vuelve a generar el cronograma");
                    m.ShowDialog();
                }

                Process p = new Process();
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.FileName = saveFile.FileName;
                p.StartInfo.Verb = "open";
                p.Start();
            }

            HideOverlay();
        }

        private void VentanaPrincipal_Closed(object sender, EventArgs e)
        {
            GuardaConfiguracion("configInterfaz.json");
            cronograma.GuardaConfiguracion("config.json");
            calendario.Guarda("calendario.json");
            asignatura.Guarda("asignatura.json");
        }

        private void HideOverlay()
        {
            Overlay.Visibility = Visibility.Hidden;
        }

        private void ShowOverlay()
        {
            Overlay.Visibility = Visibility.Visible;
        }

        private void WWW_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = projectUrl;
            p.StartInfo.Verb = "open";
            p.Start();
        }

        private void About_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowOverlay();
            Interfaz.About a = new Interfaz.About();
            a.ShowDialog();
            HideOverlay();
        }

        private void Tutorial_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenTutorial();
        }

        private void OpenTutorial()
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = config.tutorialUrl;
            p.StartInfo.Verb = "open";
            p.Start();
        }

        private void Reset_MouseDown(object sender, MouseButtonEventArgs e)
        {
            bool limpiarCalendario = false;
            bool limpiarHorario = false;
            bool limpiarCurso = false;
            bool limpiarTodo = false;

            Message m = new Message(Message.Type.question, "¿Quieres dejar el programa como si lo acabaras de instalar?");
            m.ShowDialog();
            limpiarTodo = m.result;

            if(!limpiarTodo)
            {
                m = new Message(Message.Type.question, "¿Quieres reiniciar el calendario?");
                m.ShowDialog();
                limpiarCalendario = m.result;

                m = new Message(Message.Type.question, "¿Quieres reiniciar el horario?");
                m.ShowDialog();
                limpiarHorario = m.result;

                m = new Message(Message.Type.question, "¿Quieres reiniciar el curso?");
                m.ShowDialog();
                limpiarCurso = m.result;
            }

            if(limpiarTodo)
            {
                File.Delete("configInterfaz.json");
                File.Delete("config.json");

                config = new Interfaz.Config();
                cronograma.ReiniciaConfiguracion();
            }

            if(limpiarCalendario || limpiarTodo)
            {
                calendario.Reinicia();
                DateTime now = DateTime.Now;
                int d = now.Day;
                int m2 = now.Month;
                int a = now.Year;
                DateTime dia = new DateTime(a, m2, d);

                calendario.PonDiaInicio(dia);
                calendario.PonDiaFin(dia);

                calendario.Guarda("calendario.json");
            }

            if (limpiarHorario || limpiarTodo)
            {
                asignatura.ReiniciaHorario();
                
            }

            if (limpiarCurso || limpiarTodo)
            {
                asignatura.PonNombre("Nombre de mi asignatura");
                asignatura.ReiniciaUFs();
            }

            if(limpiarTodo || limpiarHorario || limpiarCurso)
            {
                asignatura.Guarda("asignatura.json");
            }
        }
    }
}