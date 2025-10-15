namespace Programacion123
{
    public class Block : Entity
    {
        public ListEntityProperty<Activity> Activities { get; } = new ListEntityProperty<Activity>();

        public const uint flagActivities = 1 << 2; 

        public Block() : base()
        {
            StorageClassId = "block";

            Title.Value = "Título del bloque";
            Description.Value = "Descripción del bloque";

            Activities.OnAdded += (element) => { Flags.Add(ref validationFlags, flagActivities); };
            Activities.OnRemoved += (element) => { Flags.Add(ref validationFlags, flagActivities); };
            Activities.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagActivities); };

            Flags.Add(ref validationFlags, flagActivities);
        }

        public override ValidationResult Validate(bool force = false)
        {
            base.Validate(force);

            if(Flags.Test(validationFlags, flagActivities) || force)
            {
                Utils.Log("Checking activities exist and are valid", "activities");

                validationFails.RemoveAll(e => e.code == ValidationCode.blockNoActivities);
                validationFails.RemoveAll(e => e.code == ValidationCode.blockActivityInvalid);

                if (Activities.Count <= 0)
                {
                    validationFails.Add(ValidationResult.Create(ValidationCode.blockNoActivities));
                }

                for (int i = 0; i < Activities.Count; i++)
                {
                    if(Activities[i].Validate().code != ValidationCode.success)
                    {
                        validationFails.Add(ValidationResult.Create(ValidationCode.blockActivityInvalid).WithIndex(i));
                    }
                }

            }

            Flags.Remove(ref validationFlags, flagActivities);

            foreach(ValidationResult fail in validationFails) { Utils.Log(fail.ToString() + " (" + fail.index + ")", "FAILED"); }

            if(validationFails.Count == 0) { return ValidationResult.Create(ValidationCode.success); }
            else { return validationFails[0]; }

        }

        public override bool Exists(string storageId, string? parentStorageId)
        {
            return Storage.ExistsData<BlockData>(storageId, StorageClassId, parentStorageId);
        }

        public override void Save(string? parentStorageId)
        {
            base.Save(parentStorageId);

            BlockData data = new();

            data.Title = Title.Value;
            data.Description = Description.Value;

            List<Activity> list = Activities.ToList();
            list.ForEach(e => e.Save(StorageId));
            data.ActivitiesStorageIds = Storage.GetStorageIds<Activity>(list);

            Storage.SaveData<BlockData>(StorageId, StorageClassId, data, parentStorageId);
        }

        public override void LoadOrCreate(string storageId, string? parentStorageId = null)
        {
            base.LoadOrCreate(storageId, parentStorageId);

            if (!Storage.ExistsData<BlockData>(storageId, StorageClassId, parentStorageId)) { Save(parentStorageId); }

            BlockData data = Storage.LoadData<BlockData>(storageId, StorageClassId, parentStorageId);

            Title.Value = data.Title;
            Description.Value = data.Description;

            Activities.Set(Storage.LoadOrCreateEntities<Activity>(data.ActivitiesStorageIds, storageId));

        }

        public override void Delete(string? parentStorageId = null)
        {
            base.Delete(parentStorageId);

            Activities.ToList().ForEach(e => e.Delete(StorageId));

            Storage.DeleteData(StorageId, StorageClassId, parentStorageId);


        }


    }
}
