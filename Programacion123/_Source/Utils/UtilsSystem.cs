using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    partial class Utils
    {
        public static void OpenUrl(string url)
        {
            ProcessStartInfo info = new();
            info.FileName = url;
            info.UseShellExecute = true;
            Process.Start(info);
        }

        public static void OpenFolder(string folder)
        {
            ProcessStartInfo info = new();
            info.FileName = folder;
            info.UseShellExecute = true;
            Process.Start(info);
        }



    }
}
