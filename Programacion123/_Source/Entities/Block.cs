namespace Programacion123
{
    public class Block : Entity
    {
        public ListEntityProperty<Activity> Activities { get; } = new ListEntityProperty<Activity>();

        bool checkActivities;

        ValidationResult cachedResult;

        public Block() : base()
        {
            StorageClassId = "block";

            Title.Value = "Título del bloque";
            Description.Value = "Descripción del bloque";

            Activities.OnAdded += (element) => { checkActivities = true; };
            Activities.OnRemoved += (element) => { checkActivities = true; };
            Activities.OnEntityUpdated += (entity) => { checkActivities = true; };

            checkActivities = true;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            checkActivities = true;
        }

        public override ValidationResult Validate()
        {
            ValidationResult baseResult = base.Validate();

            if (baseResult.code != ValidationCode.success) { return baseResult; }

            if(!checkActivities) { return cachedResult; }

            bool invalidFound = false;

            if(!invalidFound && checkActivities)
            {
                if (Activities.Count <= 0)
                {
                    cachedResult = ValidationResult.Create(ValidationCode.blockNoActivities);
                    invalidFound = true;
                }

                if(!invalidFound)
                {
                    for (int i = 0; i < Activities.Count; i++)
                    {
                        if (Activities[i].Validate().code != ValidationCode.success)
                        {
                            cachedResult = ValidationResult.Create(ValidationCode.blockActivityInvalid).WithIndex(i);
                            invalidFound = true;
                        }
                    }
                }

                checkActivities = false;
            }


            if(!invalidFound)
            {
                cachedResult = ValidationResult.Create(ValidationCode.success);
            }

            return cachedResult;

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
