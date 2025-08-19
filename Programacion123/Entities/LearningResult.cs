namespace Programacion123
{
    public class LearningResult : Entity
    {
        public ListProperty<CommonText> Criterias { get; } = new ListProperty<CommonText>();

        public LearningResult() : base()
        {
            StorageClassId = "learningresult";

            Title = "Título del resultado de aprendizaje";
            Description = "Descripción del resultado de aprendizaje";
        }

        public override ValidationResult Validate()
        {
            ValidationResult result = base.Validate();

            if(result.code != ValidationCode.success) { return result; }

            if (Criterias.Count <= 0) { return ValidationResult.Create(ValidationCode.learningResultNoCriterias);  }
            for(int i = 0; i < Criterias.Count; i++) { if(Criterias[i].Validate().code != ValidationCode.success) { return ValidationResult.Create(ValidationCode.learningResultCriteriaInvalid).WithIndex(i); } }

            return ValidationResult.Create(ValidationCode.success);
        }

        public override bool Exists(string storageId, string? parentStorageId)
        {
            return Storage.ExistsData<LearningResultData>(storageId, StorageClassId, parentStorageId);
        }

        public override void Save(string? parentStorageId)
        {
            base.Save(parentStorageId);

            LearningResultData data = new();

            data.Title = Title;
            data.Description = Description;

            List<CommonText> list = Criterias.ToList();
            list.ForEach(e => e.Save(StorageId));            
            data.CriteriasStorageIds = Storage.GetStorageIds<CommonText>(list);

            Storage.SaveData<LearningResultData>(StorageId, StorageClassId, data, parentStorageId);
        }

        public override void LoadOrCreate(string storageId, string? parentStorageId = null)
        {
            base.LoadOrCreate(storageId, parentStorageId);

            if(!Storage.ExistsData<LearningResultData>(storageId, StorageClassId, parentStorageId)) { Save(parentStorageId); }

            LearningResultData data = Storage.LoadData<LearningResultData>(storageId, StorageClassId, parentStorageId);
            
            Title = data.Title;
            Description = data.Description;

            Criterias.Set(Storage.LoadOrCreateEntities<CommonText>(data.CriteriasStorageIds, storageId));

        }

        public override void Delete(string? parentStorageId = null)
        {
            base.Delete(parentStorageId);

            Criterias.ToList().ForEach(e => e.Delete(StorageId));

            Storage.DeleteData(StorageId, StorageClassId, parentStorageId);


        }
    }
}
