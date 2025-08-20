using System.Windows;

namespace Programacion123
{
    public enum ConfirmIconType
    {
        info,
        warning
    }

    public enum ConfirmChooseType
    {
        acceptAndCancel,
        acceptOnly
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

        public void Init(ConfirmIconType _iconType, string _title, string _content, ConfirmChooseType _chooseType, Action<bool> _closeAction)
        {
            TextTitle.Text = _title;
            TextContent.Text = _content;
            closeAction = _closeAction;

            IconWarning.Visibility = (_iconType == ConfirmIconType.warning ? Visibility.Visible : Visibility.Hidden);
            IconInfo.Visibility = (_iconType == ConfirmIconType.info ? Visibility.Visible : Visibility.Hidden);

            LabelAccept.Visibility = (_chooseType == ConfirmChooseType.acceptAndCancel ? Visibility.Visible : Visibility.Hidden);
            LabelCancel.Visibility = (_chooseType == ConfirmChooseType.acceptAndCancel ? Visibility.Visible : Visibility.Hidden);
            ButtonAccept.Visibility = (_chooseType == ConfirmChooseType.acceptAndCancel ? Visibility.Visible : Visibility.Hidden);
            ButtonCancel.Visibility = (_chooseType == ConfirmChooseType.acceptAndCancel ? Visibility.Visible : Visibility.Hidden);

            LabelAcceptSingle.Visibility = (_chooseType == ConfirmChooseType.acceptOnly ? Visibility.Visible : Visibility.Hidden);
            ButtonAcceptSingle.Visibility = (_chooseType == ConfirmChooseType.acceptOnly ? Visibility.Visible : Visibility.Hidden);

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
