namespace Programacion123
{
    public class CalendarData : StorageData
    {
        public DateTime StartDay { get; set; }
        public DateTime EndDay { get; set; }
        public HashSet<DateTime> FreeDays { get; set; }
    };
}
