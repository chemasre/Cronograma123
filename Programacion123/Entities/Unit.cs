namespace Programacion123
{
    internal class Unit : Entity
    {
        internal int Id { get; set; }
        internal int Hours { get; set; }

        public Unit()
        {
            Id = 1;
            Hours = 1;

            StorageClassId = "unit";
        }

        public override bool Exists(string storageId, string? parentStorageId)
        {
            return Storage.ExistsData<UnitData>(storageId, StorageClassId, parentStorageId);
        }

        public override void Save(string? parentStorageId = null)
        {
            base.Save(parentStorageId);

            UnitData data = new();
            data.Id = Id;
            data.Hours = Hours;

            Storage.SaveData<UnitData>(StorageId, StorageClassId, data);
        }

        public void Load(string storageId, string? parentStorageId = null)
        {

            base.LoadOrCreate(storageId, parentStorageId);

            UnitData data = Storage.LoadData<UnitData>(storageId, StorageClassId);

            Id = data.Id;
            Hours = data.Hours;

        }


        public override ValidationResult Validate()
        {
           if(Hours <= 0) { return ValidationResult.oneHourMinimum; }
           else { return ValidationResult.success; }
        }
    };
}
