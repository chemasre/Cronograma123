using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    public partial class Utils
    {
        static StreamWriter? logFile;

        public static void LogInit()
        {
            if(Switches.debugLogEnabled)
            {
                logFile = new StreamWriter(Constants.logFileName);
                logFile.AutoFlush = true;

                Log("Log started " + DateTime.Now.ToString());
            }
        }

        public static void Log(string s, string? category = null)
        {
            if(Switches.debugLogEnabled)
            {
                DateTime now = DateTime.Now;

                string line = Utils.FormatToFit(now.ToLongTimeString(), 12, true);
                if(category != null) { line += "[" + Utils.FormatToFit(category, 12, true, false) + "] "; }
                line += s;
                Console.WriteLine(line);
                LogPanel.Instance.Dispatcher.Invoke(() => LogPanel.Instance.Log(line) );
                logFile.WriteLine(line);
            }
        }


        public static void LogFinish()
        {
            if(Switches.debugLogEnabled)
            {
                logFile.Close();
            }
        }
    }
}
