using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    public partial class Utils
    {
        public static void PrintLine(string s) { if(Switches.debugConsole) { Console.WriteLine(s); }  }
        public static void Print(string s) { if(Switches.debugConsole) { Console.Write(s); }  }
    }
}
