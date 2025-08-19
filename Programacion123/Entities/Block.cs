namespace Programacion123
{
    public class Block: Entity
    {
        public ListProperty<Activity> Activities { get; } = new ListProperty<Activity>();

        public Block() : base()
        {
            StorageClassId = "block";

            Title = "Título del bloque";
            Description = "Descripción del bloque";
        }

        public override ValidationResult Validate()
        {
            ValidationResult result = base.Validate();

            if(result.code != ValidationCode.success) { return result; }

            if (Activities.Count <= 0) { return ValidationResult.Create(ValidationCode.blockNoActivities); }
            for(int i = 0; i < Activities.Count; i++) { if(Activities[i].Validate().code != ValidationCode.success) { return ValidationResult.Create(ValidationCode.blockActivityInvalid).WithIndex(i); }  }

            return ValidationResult.Create(ValidationCode.success);
        }

        public override bool Exists(string storageId, string? parentStorageId)
        {
            return Storage.ExistsData<BlockData>(storageId, StorageClassId, parentStorageId);
        }

        public override void Save(string? parentStorageId)
        {
            base.Save(parentStorageId);

            BlockData data = new();

            data.Title = Title;
            data.Description = Description;

            List<Activity> list = Activities.ToList();
            list.ForEach(e => e.Save(StorageId));            
            data.ActivitiesStorageIds = Storage.GetStorageIds<Activity>(list);

            Storage.SaveData<BlockData>(StorageId, StorageClassId, data, parentStorageId);
        }

        public override void LoadOrCreate(string storageId, string? parentStorageId = null)
        {
            base.LoadOrCreate(storageId, parentStorageId);

            if(!Storage.ExistsData<BlockData>(storageId, StorageClassId, parentStorageId)) { Save(parentStorageId); }

            BlockData data = Storage.LoadData<BlockData>(storageId, StorageClassId, parentStorageId);
            
            Title = data.Title;
            Description = data.Description;

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
