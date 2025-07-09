using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    public enum StorageState
    {
        detached,
        dirty,
        saved
    };


    public abstract class Entity
    {

        protected StorageState StorageState { get { return storageState; } }

        protected StorageState storageState;

        public enum ValidationResult
        {
            success,
            titleEmpty,
            startDayAfterEndDay, // Calendar
            freeDayBeforeStartOrAfterEnd,
            noSchoolDays,
            unitsMissing, // Subject
            weekDayMissing,
            oneHourMinimum // Unit
        };

        public string StorageId { get; set; }
        public string StorageClassId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public Entity()
        {
            Title = "";
            StorageId = Guid.NewGuid().ToString();
            storageState = StorageState.detached;
        }

        public abstract ValidationResult Validate();

        public virtual void SetDirty()
        {
            storageState = StorageState.dirty;
        }

        public virtual void Load(string storageId)
        {
            StorageId = storageId;
            storageState = StorageState.saved;
        }

        public virtual void Save()
        {
            storageState = StorageState.saved;
        }



    }
}
