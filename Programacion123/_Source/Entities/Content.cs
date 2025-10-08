namespace Programacion123
{
    public class Content : Entity
    {
        public ListEntityProperty<CommonText> Points { get; } = new ListEntityProperty<CommonText>();

        public Content() : base()
        {
            StorageClassId = "content";

            Title.Value = "Título del contenido";
            Description.Value = "Descripción del contenido";
        }

        public override ValidationResult Validate()
        {
            ValidationResult result = base.Validate();

            if (result.code != ValidationCode.success) { return result; }

            if (Points.Count <= 0) { return ValidationResult.Create(ValidationCode.contentNoPoints); }
            for (int i = 0; i < Points.Count; i++) { if (Points[i].Validate().code != ValidationCode.success) { return ValidationResult.Create(ValidationCode.contentPointInvalid).WithIndex(i); } }

            return ValidationResult.Create(ValidationCode.success);
        }

        public override bool Exists(string storageId, string? parentStorageId)
        {
            return Storage.ExistsData<ContentData>(storageId, StorageClassId, parentStorageId);
        }

        public override void Save(string? parentStorageId)
        {
            base.Save(parentStorageId);

            ContentData data = new();

            data.Title = Title.Value;
            data.Description = Description.Value;

            List<CommonText> list = Points.ToList();
            list.ForEach(e => e.Save(StorageId));
            data.PointsStorageIds = Storage.GetStorageIds<CommonText>(list);

            Storage.SaveData<ContentData>(StorageId, StorageClassId, data, parentStorageId);
        }

        public override void LoadOrCreate(string storageId, string? parentStorageId = null)
        {
            base.LoadOrCreate(storageId, parentStorageId);

            if (!Storage.ExistsData<ContentData>(storageId, StorageClassId, parentStorageId)) { Save(parentStorageId); }

            ContentData data = Storage.LoadData<ContentData>(storageId, StorageClassId, parentStorageId);

            Title.Value = data.Title;
            Description.Value = data.Description;

            Points.Set(Storage.LoadOrCreateEntities<CommonText>(data.PointsStorageIds, storageId));

        }

        public override void Delete(string? parentStorageId = null)
        {
            base.Delete(parentStorageId);

            Points.ToList().ForEach(e => e.Delete(StorageId));

            Storage.DeleteData(StorageId, StorageClassId, parentStorageId);


        }

    }
}
