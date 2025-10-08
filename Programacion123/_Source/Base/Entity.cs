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
        public string StorageId { get; set; }
        public string StorageClassId { get; set; }

        public Property<string> Title { get; } = new Property<string>("");
        public Property<string> Description { get; } = new Property<string>("");

        public delegate void OnChangedHandler(Entity e);
        public event OnChangedHandler OnUpdated;

        // Validation

        bool checkTitle;
        bool checkDescription;
        
        ValidationResult cachedResult;

        public Entity()
        {
            Title.Value = "Escribe un título";
            Description.Value = "Escribe una descripción";

            StorageId = Guid.NewGuid().ToString();
            storageState = StorageState.detached;

            Title.OnSetted += (previous, current) => { if(previous != current) { OnUpdated?.Invoke(this); checkTitle = true; } };
            Description.OnSetted += (previous, current) => { if(previous != current) { OnUpdated?.Invoke(this); checkDescription = true; } };

            checkTitle = true;
            checkDescription = true;

        }

        protected void InvokeOnUpdated()
        {
            OnUpdated?.Invoke(this);
        }

        public virtual void Invalidate()
        {
            checkTitle = true;
            checkDescription = true;
        }

        public virtual ValidationResult Validate()
        {
            if(!checkTitle && !checkDescription) { return cachedResult; }

            bool invalid = false;

            if(!invalid && checkTitle)
            {
                if(Title.Value.Trim().Length <= 0)
                {
                    cachedResult = ValidationResult.Create(ValidationCode.entityTitleEmpty);
                    invalid = true;
                }

                checkTitle = false;
            }

            if(!invalid && checkDescription)
            {
                if(Description.Value.Trim().Length <= 0)
                {
                    cachedResult = ValidationResult.Create(ValidationCode.entityDescriptionEmpty);
                    invalid = true;
                }

                checkDescription = false;
            }

            if(!invalid)
            {
                cachedResult = ValidationResult.Create(ValidationCode.success);
            }

            return cachedResult;

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
