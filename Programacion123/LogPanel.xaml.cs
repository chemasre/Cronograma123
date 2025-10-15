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
    /// Lógica de interacción para LogPanel.xaml
    /// </summary>
    public partial class LogPanel : Window
    {

        public static LogPanel Instance { get { if(instance == null) { instance = new(); } return instance; } }

        List<string> lines;
        StringBuilder stringBuilder;

        static LogPanel? instance = null;


        public LogPanel()
        {
            InitializeComponent();

            lines = new();
            stringBuilder = new();

        }

        public void Clear() { lines.Clear(); stringBuilder.Clear();  TextContent.Text = ""; TextContent.ScrollToEnd();}
        public void Log(string s)
        {
            if(lines.Count + 1 > Constants.logMaxLineBlocks * Constants.logLineBlockSize)
            {
                lines.RemoveRange(0, Constants.logLineBlockSize);
                stringBuilder.Clear();
                lines.ForEach((s) => stringBuilder.AppendLine(s));
            }

            lines.Add(s);
            stringBuilder.AppendLine(s);

            TextContent.Text = stringBuilder.ToString();
            TextContent.ScrollToEnd();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
