namespace Programacion123
{
    public class SubjectTemplate : Entity
    {
        public string Name { get; set; } = "Nombre del módulo";
        public string Code { get; set; } = "Código del módulo";
        public CommonText GeneralObjectivesIntroduction { get; set; }
        public ListProperty<CommonText> GeneralObjectives { get; } = new ListProperty<CommonText>();
        public CommonText GeneralCompetencesIntroduction { get; set; }
        public ListProperty<CommonText> GeneralCompetences { get; } = new ListProperty<CommonText>();
        public CommonText KeyCapacitiesIntroduction { get; set; }
        public ListProperty<CommonText> KeyCapacities { get; } = new ListProperty<CommonText>();
        public ListProperty<LearningResult> LearningResults { get; } = new ListProperty<LearningResult>();
        public ListProperty<Content> Contents { get; } = new ListProperty<Content>();

        public SubjectTemplate() : base()
        {
            StorageClassId = "subjecttemplate";
            GeneralObjectivesIntroduction = new CommonText();
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

            Storage.SaveData<SubjectTemplateData>(StorageId, StorageClassId, data, parentStorageId);

        }

        public override void Load(string storageId, string? parentStorageId = null)
        {
            base.Load(storageId, parentStorageId);

            SubjectTemplateData data = Storage.LoadData<SubjectTemplateData>(storageId, StorageClassId, parentStorageId);
            
            Title = data.Title;
            Description = data.Description;
            GeneralObjectivesIntroduction = Storage.LoadEntity<CommonText>(data.GeneralObjectivesIntroductionStorageId, storageId);

            GeneralObjectives.Set(Storage.LoadEntities<CommonText>(data.GeneralObjectivesStorageIds, storageId));
        }
    }
}
