namespace Programacion123
{
    public class LearningResult : Entity
    {
        public ListEntityProperty<CommonText> Criterias { get; } = new ListEntityProperty<CommonText>();

        const uint flagCriterias = 1 << 2;

        public LearningResult() : base()
        {
            StorageClassId = "learningresult";

            Title.Value = "Título del resultado de aprendizaje";
            Description.Value = "Descripción del resultado de aprendizaje";

            Criterias.OnAdded += (element) => { Flags.Add(ref validationFlags, flagCriterias); };
            Criterias.OnRemoved += (element) => { Flags.Add(ref validationFlags, flagCriterias); };
            Criterias.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagCriterias); };

            Flags.Add(ref validationFlags, flagCriterias);

        }

        public override ValidationResult Validate(bool force = false)
        {
            base.Validate(force);

            Utils.PrintLine(Title.Value + ": Learning result validation start");

            if(Flags.Test(validationFlags, flagCriterias) || force)
            {
                Utils.PrintLine("[criterias] => Validating some criteria exist and criterias valid");

                validationFails.RemoveAll(e => e.code == ValidationCode.learningResultNoCriterias);
                validationFails.RemoveAll(e => e.code == ValidationCode.learningResultCriteriaInvalid);

                if (Criterias.Count <= 0) { validationFails.Add(ValidationResult.Create(ValidationCode.learningResultNoCriterias)); }
                for (int i = 0; i < Criterias.Count; i++) { if (Criterias[i].Validate(force).code != ValidationCode.success) { validationFails.Add(ValidationResult.Create(ValidationCode.learningResultCriteriaInvalid).WithIndex(i)); } }
            }

            Flags.Remove(ref validationFlags, flagCriterias);

            Utils.PrintLine(Title.Value + ": Learning result validation end");
            foreach(ValidationResult fail in validationFails) { Utils.PrintLine("FAILED: " + fail.code + "(" + fail.index + ")"); }

            if(validationFails.Count == 0) { return ValidationResult.Create(ValidationCode.success); }
            else { return validationFails[0]; }
        }

        public override bool Exists(string storageId, string? parentStorageId)
        {
            return Storage.ExistsData<LearningResultData>(storageId, StorageClassId, parentStorageId);
        }

        public override void Save(string? parentStorageId)
        {
            base.Save(parentStorageId);

            LearningResultData data = new();

            data.Title = Title.Value;
            data.Description = Description.Value;

            List<CommonText> list = Criterias.ToList();
            list.ForEach(e => e.Save(StorageId));
            data.CriteriasStorageIds = Storage.GetStorageIds<CommonText>(list);

            Storage.SaveData<LearningResultData>(StorageId, StorageClassId, data, parentStorageId);
        }

        public override void LoadOrCreate(string storageId, string? parentStorageId = null)
        {
            base.LoadOrCreate(storageId, parentStorageId);

            if (!Storage.ExistsData<LearningResultData>(storageId, StorageClassId, parentStorageId)) { Save(parentStorageId); }

            LearningResultData data = Storage.LoadData<LearningResultData>(storageId, StorageClassId, parentStorageId);

            Title.Value = data.Title;
            Description.Value = data.Description;

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
