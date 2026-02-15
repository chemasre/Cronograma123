namespace Programacion123
{
    public class Content : Entity
    {
        public ListEntityProperty<CommonText> Points { get; } = new ListEntityProperty<CommonText>();

        const uint flagPoints = 1 << 2;

        public Content() : base()
        {
            StorageClassId = "content";

            Title.Value = "Título del contenido";
            Description.Value = "Descripción del contenido";

            Points.OnAdded += (element) => { Flags.Add(ref validationFlags, flagPoints); };
            Points.OnRemoved += (element) => { Flags.Add(ref validationFlags, flagPoints); };
            Points.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagPoints); };

            Flags.Add(ref validationFlags, flagPoints);

        }

        public override ValidationResult Validate(bool force = false)
        {
            base.Validate(force);

            if(Flags.Test(validationFlags, flagPoints) || force)
            {
                Utils.Log("Checking at least one points exist and all are valid", "points");

                validationFails.RemoveAll(e => e.code == ValidationCode.contentNoPoints);
                validationFails.RemoveAll(e => e.code == ValidationCode.contentPointInvalid);

                if (Points.Count <= 0) { validationFails.Add(ValidationResult.Create(ValidationCode.contentNoPoints)); }
                for (int i = 0; i < Points.Count; i++) { if (Points[i].Validate(force).code != ValidationCode.success) { validationFails.Add(ValidationResult.Create(ValidationCode.contentPointInvalid).WithIndex(i)); } }
            }

            Flags.Remove(ref validationFlags, flagPoints);

            // FIX: Causes an exception (randomly)
            //foreach(ValidationResult fail in validationFails) { Utils.Log(fail.ToString() + " (" + fail.index + ")", "FAILED"); }

            if(validationFails.Count == 0) { return ValidationResult.Create(ValidationCode.success); }
            else { return validationFails[0]; }
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
