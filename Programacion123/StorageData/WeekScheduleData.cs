using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    public class WeekScheduleData : StorageData
    {
        public HashSet< KeyValuePair<DayOfWeek, int> > HoursPerWeekDay { get; set; }
    }
}
