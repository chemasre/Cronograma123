namespace Programacion123
{
    public class Block: Entity
    {
        public ListProperty<CommonText> Activities { get; } = new ListProperty<CommonText>();

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

            List<CommonText> list = Activities.ToList();
            list.ForEach(e => e.Save(StorageId));            
            data.ActivitiesStorageIds = Storage.GetStorageIds<CommonText>(list);

            Storage.SaveData<BlockData>(StorageId, StorageClassId, data, parentStorageId);
        }

        public override void LoadOrCreate(string storageId, string? parentStorageId = null)
        {
            base.LoadOrCreate(storageId, parentStorageId);

            if(!Storage.ExistsData<BlockData>(storageId, StorageClassId, parentStorageId)) { Save(parentStorageId); }

            BlockData data = Storage.LoadData<BlockData>(storageId, StorageClassId, parentStorageId);
            
            Title = data.Title;
            Description = data.Description;

            Activities.Set(Storage.LoadEntities<CommonText>(data.ActivitiesStorageIds, storageId));

        }

        public override void Delete(string? parentStorageId = null)
        {
            base.Delete(parentStorageId);

            Activities.ToList().ForEach(e => e.Delete(StorageId));

            Storage.DeleteData(StorageId, StorageClassId, parentStorageId);


        }

    }
}
