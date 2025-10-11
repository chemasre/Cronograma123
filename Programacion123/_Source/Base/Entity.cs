using System.ComponentModel.DataAnnotations;

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
        public string StorageId { get; set; }
        public string StorageClassId { get; set; }

        public Property<string> Title { get; } = new Property<string>("");
        public Property<string> Description { get; } = new Property<string>("");

        public delegate void OnChangedHandler(Entity e);
        public event OnChangedHandler OnUpdated;

        // Validation

        public const uint flagTitle       = 1 << 0; 
        public const uint flagDescription = 1 << 1; 

        protected uint validationFlags = Flags.Empty();
        
        protected List<ValidationResult> validationFails;

        public Entity()
        {
            Title.Value = "Escribe un título";
            Description.Value = "Escribe una descripción";

            StorageId = Guid.NewGuid().ToString();

            Title.OnSetted += (previous, current) => { OnUpdated?.Invoke(this); Flags.Add(ref validationFlags, flagTitle); };
            Description.OnSetted += (previous, current) => { OnUpdated?.Invoke(this); Flags.Add(ref validationFlags, flagDescription); };

            validationFails = new();

            // Add flags

            Flags.Add(ref validationFlags, flagTitle);
            Flags.Add(ref validationFlags, flagDescription);

        }

        protected void InvokeOnUpdated()
        {
            OnUpdated?.Invoke(this);
        }

        public virtual ValidationResult Validate(bool force = false)
        {
            Console.WriteLine("***************************************************");
            Console.WriteLine(Title.Value + ": Entity validation start");

            if(Flags.Test(validationFlags, flagTitle) || force)
            {
                Console.WriteLine("[title] => Checking not empty");
                validationFails.RemoveAll((v) => v.code == ValidationCode.entityTitleEmpty);
                if(Title.Value.Trim().Length <= 0) { validationFails.Add(ValidationResult.Create(ValidationCode.entityTitleEmpty)); }
            }

            if(Flags.Test(validationFlags, flagDescription) || force)
            {
                Console.WriteLine("[description] => Checking not empty");
                validationFails.RemoveAll((v) => v.code == ValidationCode.entityDescriptionEmpty);
                if(Description.Value.Trim().Length <= 0) { validationFails.Add(ValidationResult.Create(ValidationCode.entityDescriptionEmpty)); }
            }

            // Remove flags

            Flags.Remove(ref validationFlags, flagTitle);
            Flags.Remove(ref validationFlags, flagDescription);

            Console.WriteLine(Title.Value + ": Entity validation end");
            foreach(ValidationResult fail in validationFails) { Console.WriteLine("FAILED: " + fail.code + "(" + fail.index + ")"); }

            if(validationFails.Count == 0) { return ValidationResult.Create(ValidationCode.success); }
            else { return validationFails[0]; }

        }


        public virtual bool Exists(string storageId, string? parentStorageId)
        {
            return false;
        }

        public virtual void LoadOrCreate(string storageId, string? parentStorageId = null)
        {
            StorageId = storageId;
        }

        public virtual void Save(string? parentStorageId = null)
        {
        }

        public virtual void Delete(string? parentStorageId = null)
        {
        }

    }
}
