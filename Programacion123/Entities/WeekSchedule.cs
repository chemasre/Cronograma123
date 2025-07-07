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
        }

        public override ValidationResult Validate()
        {
            int total = 0;
            HoursPerWeekDay.ToList().ForEach(e => total += e.Value);

            if(total <= 0) { return ValidationResult.oneHourMinimum; }
            else { return ValidationResult.success; }
        }

        public override void Save()
        {
            WeekScheduleData data = new();
            data.HoursPerWeekDay = new(HoursPerWeekDay.ToList());

            Storage.SaveData<WeekScheduleData>(StorageId, StorageClassId, data);
        }

        public void Load(string storageId)
        {            
            base.Load(storageId);
            throw new NotImplementedException();
        }

    }
}
