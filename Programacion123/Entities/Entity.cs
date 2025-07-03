using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    internal abstract class Entity
    {
        public enum ValidationResult
        {
            success,
            startDayAfterEndDay, // Calendar
            freeDayOutsideCalendar,
            unitsMissing, // Subject
            weekDayMissing
        };

        public string Title { get; set; }

        public Entity()
        {
            Title = "";
        }

        public abstract ValidationResult Validate();
        public abstract void Load(string fileName);
        public abstract void Save(string fileName);
    }
}
