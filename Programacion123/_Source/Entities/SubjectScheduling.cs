using static Programacion123.Subject;

namespace Programacion123
{
    public struct ActivitySchedule
    {
        public SchoolDayHour start;
        public SchoolDayHour end;
        public Activity activity;
    }

    public partial class Subject : Entity
    {
        public struct SchoolDayHour
        {
            public DateTime day;
            public float hour;
        }

        public struct SchoolDayHourCursor
        {
            Calendar calendar;
            WeekSchedule weekSchedule;
            DateTime day;
            float hour;
            bool skipToNewDay;

            public SchoolDayHour? PointedValue
            {
                get
                {
                    if (day <= calendar.EndDay.Value) { return new SchoolDayHour() { day = day, hour = hour }; }
                    else { return null; }
                }
            }

            public SchoolDayHourCursor(Calendar _calendar, WeekSchedule _weekSchedule)
            {
                calendar = _calendar;
                weekSchedule = _weekSchedule;
                day = _calendar.StartDay.Value;
                hour = 0;
                skipToNewDay = false;
            }

            public void Reset()
            {
                day = calendar.StartDay.Value;
                hour = 0;
                skipToNewDay = false;
            }

            public void GotoActivityStart(Activity activity)
            {
                DateTime lookupDay = day;

                if (skipToNewDay || activity.NoActivitiesBefore.Value && hour > 0) { lookupDay = lookupDay.AddDays(1); hour = 0; skipToNewDay = false; }

                bool found = false;

                while (!found && lookupDay <= calendar.EndDay.Value)
                {
                    if (Utils.IsSchoolDay(lookupDay, calendar, weekSchedule) &&
                       weekSchedule.HoursPerWeekDay[lookupDay.DayOfWeek] > 0 &&
                       (
                        !(activity.StartType.Value == ActivityStartType.Date) ||
                          activity.StartType.Value == ActivityStartType.Date && lookupDay == activity.StartDate.Value
                       )
                       &&
                       (
                        !(activity.StartType.Value == ActivityStartType.DayOfWeek) ||
                          activity.StartType.Value == ActivityStartType.DayOfWeek && lookupDay.DayOfWeek == activity.StartDayOfWeek.Value
                       ) &&
                       hour < weekSchedule.HoursPerWeekDay[lookupDay.DayOfWeek])
                    {
                        found = true;
                    }
                    else
                    {
                        lookupDay = lookupDay.AddDays(1);
                        hour = 0;
                    }
                }

                if (found)
                {
                    day = lookupDay;
                }
                else
                {
                    day = calendar.EndDay.Value.AddDays(1);
                }
            }

            public void GotoActivityEnd(Activity activity)
            {
                float pending = activity.Duration.Value;

                while (pending > 0 && day <= calendar.EndDay.Value)
                {
                    if (Utils.IsSchoolDay(day, calendar, weekSchedule) && weekSchedule.HoursPerWeekDay[day.DayOfWeek] > 0)
                    {
                        pending -= weekSchedule.HoursPerWeekDay[day.DayOfWeek] - hour;
                    }

                    if (pending > 0) { day = day.AddDays(1); hour = 0; }
                    else if (pending <= 0)
                    {
                        hour = weekSchedule.HoursPerWeekDay[day.DayOfWeek] + pending;

                        skipToNewDay = activity.NoActivitiesAfter.Value;
                    }
                }
            }


        };

        public struct ActivityCursor
        {
            public List<Block> blocks;
            public int blockIndex;
            public int activityIndex;

            public Activity? PointedValue
            {
                get
                {
                    if (blockIndex >= blocks.Count) { return null; }
                    else if (activityIndex >= blocks[blockIndex].Activities.Count) { return null; }
                    else { return blocks[blockIndex].Activities[activityIndex]; }
                }
            }

            public ActivityCursor(List<Block> _blocks)
            {
                blocks = _blocks;
                blockIndex = 0;
                activityIndex = 0;
            }

            public void Reset()
            {
                blockIndex = 0;
                activityIndex = 0;
            }

            public void Next()
            {
                if (blockIndex >= blocks.Count) { return; }

                if (activityIndex >= blocks[blockIndex].Activities.Count) { return; }

                activityIndex++;

                if(activityIndex >= blocks[blockIndex].Activities.Count)
                {
                    activityIndex = 0;
                    blockIndex++;
                }
            }

        };

        public bool CanScheduleActivities(bool force = false)
        {
            if (Calendar.Value == null) { return false; }
            else if (Calendar.Value.Validate(force).code != ValidationCode.success) { return false; }
            else if (WeekSchedule.Value == null) { return false; }
            else if (WeekSchedule.Value.Validate(force).code != ValidationCode.success) { return false; }
            else if (Blocks.Count <= 0) { return false; }
            else if (!Blocks.ToList().TrueForAll(b =>  b.Activities.Count == 0 || b.Activities.Count > 0 && b.Activities.ToList().TrueForAll(a => a.Duration.Value > 0))) { return false; }

            return true;
        }

        public List<ActivitySchedule> ScheduleActivities()
        {
            List<ActivitySchedule> output = new();

            ActivityCursor activityCursor = new(Blocks.ToList());
            SchoolDayHourCursor schoolDayHourCursor = new(Calendar.Value, WeekSchedule.Value);

            activityCursor.Reset();
            schoolDayHourCursor.Reset();

            bool scheduled = true;

            while (scheduled && activityCursor.PointedValue != null && schoolDayHourCursor.PointedValue != null)
            {
                SchoolDayHour? startTime = null;
                SchoolDayHour? endTime = null;

                Activity activity = activityCursor.PointedValue;

                schoolDayHourCursor.GotoActivityStart(activity);
                startTime = schoolDayHourCursor.PointedValue;
                if (startTime != null)
                {
                    schoolDayHourCursor.GotoActivityEnd(activity);
                    endTime = schoolDayHourCursor.PointedValue;
                }

                if (startTime != null && endTime != null)
                {
                    output.Add(new ActivitySchedule() { activity = activity, start = startTime.Value, end = endTime.Value });
                    scheduled = true;
                }
                else
                {
                    scheduled = false;
                }

                if (scheduled)
                {
                    activityCursor.Next();
                }

            }

            return output;
        }

    }
}
