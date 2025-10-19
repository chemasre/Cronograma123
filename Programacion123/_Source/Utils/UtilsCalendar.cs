using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    public partial class Utils
    {
        public static bool IsSchoolDay(DateTime day, Calendar calendar, WeekSchedule weekSchedule)
        {
            if (day >= calendar.StartDay.Value && day <= calendar.EndDay.Value &&
               day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday &&
               weekSchedule.HoursPerWeekDay[day.DayOfWeek] > 0 &&
               !calendar.FreeDays.Contains(day))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public static bool IsChristmas()
        {
            DateTime now = DateTime.Now;
            int year = now.Year;
            int month = now.Month;
            int day = now.Day;

            // now = new DateTime(2025, 12, 25);

            DateTime thisYearChristmasStart = new DateTime(year, Constants.christmasStartMonth, Constants.christmasStartDay);
            DateTime thisYearChristmasEnd = new DateTime(year + 1, Constants.christmasEndMonth, Constants.christmasEndDay);

            DateTime pastYearChristmasStart = new DateTime(year - 1, Constants.christmasStartMonth, Constants.christmasStartDay);
            DateTime pastYearChristmasEnd = new DateTime(year, Constants.christmasEndMonth, Constants.christmasEndDay);

            return (now >= thisYearChristmasStart && now <= thisYearChristmasEnd ||
               now >= pastYearChristmasStart && now <= pastYearChristmasEnd);
        
        }
    }
}
