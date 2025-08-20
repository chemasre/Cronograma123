namespace Programacion123
{
    public class WeekScheduleData : StorageData
    {
        public HashSet< KeyValuePair<DayOfWeek, int> > HoursPerWeekDay { get; set; }
    }
}
