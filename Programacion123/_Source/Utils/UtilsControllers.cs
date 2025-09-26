using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;

namespace Programacion123
{
    partial class Utils
    {
        public static void SetButtonAvailable(Button button, bool available)
        {
            button.IsEnabled = available;
            button.Opacity = available ? 1.0f : Constants.buttonNotAvailableOpacity;
            button.Effect = available ? (Effect)Application.Current.Resources[Constants.buttonAvailableEffect] : null;
        }

    }
}
