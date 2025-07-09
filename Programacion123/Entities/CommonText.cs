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

        public override void Save(string? parentStorageId = null)
        {
            base.Save(parentStorageId);

            CommonTextData data = new();
            data.Title = Title;
            data.Description = Description;

            Storage.SaveData<CommonTextData>(StorageId, StorageClassId, data, parentStorageId);
        }

        public override void Load(string storageId, string? parentStorageId = null)
        {
            base.Load(storageId, parentStorageId);

            CommonTextData data = Storage.LoadData<CommonTextData>(storageId, StorageClassId, parentStorageId);

            Title = data.Title;
            Description = data.Description;

        }
    }
}
