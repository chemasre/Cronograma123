using System.Windows;

namespace Programacion123
{
    public enum ConfirmIconType
    {
        info,
        warning
    }


    /// <summary>
    /// Lógica de interacción para ConfirmDialog.xaml
    /// </summary>
    public partial class ConfirmDialog : Window
    {
        bool result;

        Action<bool>? closeAction;

        public ConfirmDialog()
        {
            InitializeComponent();

            Closed += ConfirmDialog_Closed;
        }

        public void Init(ConfirmIconType _iconType, string _title, string _content, Action<bool> _closeAction)
        {
            TextTitle.Text = _title;
            TextContent.Text = _content;
            closeAction = _closeAction;

            IconWarning.Visibility = (_iconType == ConfirmIconType.warning ? Visibility.Visible : Visibility.Hidden);
            IconInfo.Visibility = (_iconType == ConfirmIconType.info ? Visibility.Visible : Visibility.Hidden);
        }

        private void ConfirmDialog_Closed(object? sender, EventArgs e)
        {
            closeAction?.Invoke(result);
        }

        private void ButtonAccept_Click(object sender, RoutedEventArgs e)
        {
            result = true;
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            result = false;
            Close();
        }
    }
}
