using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    public class WeekSchedule : Entity
    {
        internal DictionaryProperty<DayOfWeek, int> HoursPerWeekDay { get; } = new DictionaryProperty<DayOfWeek, int>();

        public WeekSchedule()
        {
            StorageClassId = "weekschedule";
            Title = "Horario sin título";
        }

        public override ValidationResult Validate()
        {
            int total = 0;
            HoursPerWeekDay.ToList().ForEach(e => total += e.Value);

            if (Title.Trim().Length <= 0) { return ValidationResult.titleEmpty; }
            else if (total <= 0) { return ValidationResult.oneHourMinimum; }
            else { return ValidationResult.success; }
        }

        public override void Save()
        {
            WeekScheduleData data = new();
            data.HoursPerWeekDay = new(HoursPerWeekDay.ToList());
            data.Title = Title;

            Storage.SaveData<WeekScheduleData>(StorageId, StorageClassId, data);
        }

        public override void Load(string storageId)
        {            
            base.Load(storageId);

            var data = Storage.LoadData<WeekScheduleData>(storageId, StorageClassId);

            Title = data.Title;
            HoursPerWeekDay.Set(data.HoursPerWeekDay.ToList());
        }

    }
}
