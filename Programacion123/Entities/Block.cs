namespace Programacion123
{
    public class Block: Entity
    {
        public ListProperty<Activity> Activities { get; } = new ListProperty<Activity>();

        public Block() : base()
        {
            StorageClassId = "block";
        }

        public override ValidationResult Validate()
        {
            return base.Validate();
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
