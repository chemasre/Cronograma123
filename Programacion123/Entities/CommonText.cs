namespace Programacion123
{
    public class CommonText: Entity
    {

        public CommonText() : base()
        {
            StorageClassId = "commontext";
        }

        public override ValidationResult Validate()
        {
            return base.Validate();
        }

        public override bool Exists(string storageId, string? parentStorageId)
        {
            return Storage.ExistsData<CommonTextData>(storageId, StorageClassId, parentStorageId);
        }

        public override void Save(string? parentStorageId = null)
        {
            base.Save(parentStorageId);

            CommonTextData data = new();
            data.Title = Title;
            data.Description = Description;

            Storage.SaveData<CommonTextData>(StorageId, StorageClassId, data, parentStorageId);
        }

        public override void LoadOrCreate(string storageId, string? parentStorageId = null)
        {
            base.LoadOrCreate(storageId, parentStorageId);

            if(!Storage.ExistsData<CommonTextData>(storageId, StorageClassId, parentStorageId)) { Save(parentStorageId); }

            CommonTextData data = Storage.LoadData<CommonTextData>(storageId, StorageClassId, parentStorageId);

            Title = data.Title;
            Description = data.Description;

        }

        public override void Delete(string? parentStorageId = null)
        {
            base.Delete(parentStorageId);

            Storage.DeleteData(StorageId, StorageClassId, parentStorageId);
        }
    }
}
