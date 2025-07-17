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
            descriptionEmpty,
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
            Title = "Sin título";
            Description = "Sin descripción";
            StorageId = Guid.NewGuid().ToString();
            storageState = StorageState.detached;
        }

        public virtual ValidationResult Validate()
        {
            if(Title.Trim().Length <= 0) { return ValidationResult.titleEmpty; }
            else if(Description.Trim().Length <= 0) { return ValidationResult.descriptionEmpty; }
            else { return ValidationResult.success; }
        }

        public virtual void SetDirty()
        {
            storageState = StorageState.dirty;
        }

        public virtual bool Exists(string storageId, string? parentStorageId)
        {
            return false;
        }

        public virtual void LoadOrCreate(string storageId, string? parentStorageId = null)
        {
            StorageId = storageId;
            storageState = StorageState.saved;
        }

        public virtual void Save(string? parentStorageId = null)
        {
            storageState = StorageState.saved;
        }

        public virtual void Delete(string? parentStorageId = null)
        {
            storageState = StorageState.detached;
        }

    }
}
