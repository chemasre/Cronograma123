namespace Programacion123
{
    public class SubjectTemplate : Entity
    {
        public string Name { get; set; } = "Nombre del módulo";
        public string Code { get; set; } = "Código del módulo";
        public CommonText GeneralObjectivesIntroduction { get; set; } = new CommonText();
        public ListProperty<CommonText> GeneralObjectives { get; } = new ListProperty<CommonText>();
        public CommonText GeneralCompetencesIntroduction { get; set; } = new CommonText();
        public ListProperty<CommonText> GeneralCompetences { get; } = new ListProperty<CommonText>();
        public CommonText KeyCapacitiesIntroduction { get; set; } = new CommonText();
        public ListProperty<CommonText> KeyCapacities { get; } = new ListProperty<CommonText>();
        public ListProperty<LearningResult> LearningResults { get; } = new ListProperty<LearningResult>();
        public ListProperty<Content> Contents { get; } = new ListProperty<Content>();

        public SubjectTemplate() : base()
        {
            StorageClassId = "subjecttemplate";
        }

        public override ValidationResult Validate()
        {
            throw new NotImplementedException();
        }

        public override void Save(string? parentStorageId = null)
        {
            base.Save(parentStorageId);

            SubjectTemplateData data = new();

            data.Title = Title;
            data.Description = Description;
            data.GeneralObjectivesIntroductionStorageId = GeneralObjectivesIntroduction.StorageId;
            GeneralObjectivesIntroduction.Save(StorageId);

            List<CommonText> list = GeneralObjectives.ToList();
            list.ForEach(e => e.Save(StorageId));            
            data.GeneralObjectivesStorageIds = Storage.GetStorageIds<CommonText>(list);

            list = GeneralCompetences.ToList();
            list.ForEach(e => e.Save(StorageId));
            data.GeneralCompetencesStorageIds = Storage.GetStorageIds<CommonText>(list);

            Storage.SaveData<SubjectTemplateData>(StorageId, StorageClassId, data, parentStorageId);

        }

        public override void LoadOrCreate(string storageId, string? parentStorageId = null)
        {
            base.LoadOrCreate(storageId, parentStorageId);

            if(!Storage.ExistsData<SubjectTemplateData>(storageId, StorageClassId, parentStorageId)) { Save(parentStorageId); }

            SubjectTemplateData data = Storage.LoadData<SubjectTemplateData>(storageId, StorageClassId, parentStorageId);
            
            Title = data.Title;
            Description = data.Description;
            GeneralObjectivesIntroduction = Storage.LoadEntity<CommonText>(data.GeneralObjectivesIntroductionStorageId, storageId);

            GeneralObjectives.Set(Storage.LoadEntities<CommonText>(data.GeneralObjectivesStorageIds, storageId));

            GeneralCompetences.Set(Storage.LoadEntities<CommonText>(data.GeneralCompetencesStorageIds, storageId));
        }

        public override void Delete(string? parentStorageId = null)
        {
            base.Delete(parentStorageId);

            GeneralObjectivesIntroduction.Delete(StorageId);
            GeneralObjectives.ToList().ForEach(e => e.Delete(StorageId));
            GeneralCompetencesIntroduction.Delete(StorageId);
            GeneralCompetences.ToList().ForEach(e => e.Delete(StorageId));

            Storage.DeleteData(StorageId, StorageClassId, parentStorageId);


        }
    }
}
