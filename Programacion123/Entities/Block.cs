namespace Programacion123
{
    public class Block: Entity
    {
        public ListProperty<Activity> Activities { get; } = new ListProperty<Activity>();

        internal Block() : base()
        {
            StorageClassId = "block";
        }

        public override ValidationResult Validate()
        {
            throw new NotImplementedException();
        }

        public override bool Exists(string storageId, string? parentStorageId)
        {
            return Storage.ExistsData<BlockData>(storageId, StorageClassId, parentStorageId);
        }
    }
}
