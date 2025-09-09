using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Programacion123
{
    public partial class WordGenerator : Generator
    {

        void GetScreenDpi(out float? dpiX, out float? dpiY)
        {
            dpiX = null;
            dpiY = null;

            PresentationSource? screen = PresentationSource.FromVisual(System.Windows.Application.Current.MainWindow);

            if (screen != null)
            {
                dpiX = (float) (96.0 * screen.CompositionTarget.TransformToDevice.M11);
                dpiY = (float) (96.0 * screen.CompositionTarget.TransformToDevice.M22);
            }
        }
    }
}
