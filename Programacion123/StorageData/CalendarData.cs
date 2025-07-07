using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    public class CalendarData : StorageData
    {
        public DateTime StartDay { get; set; }
        public DateTime EndDay { get; set; }
        public HashSet<DateTime> FreeDays { get; set; }
    };
}
