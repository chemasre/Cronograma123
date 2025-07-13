namespace Programacion123
{
    public enum GradeType
    {
        superior,
        medium
    }

    public class SubjectTemplate : Entity
    {
        public string SubjectName { get; set; } = "Nombre completo del módulo";
        public string SubjectCode { get; set; } = "Código del módulo";
        public GradeType GradeType { get; set; } = GradeType.superior;
        public string GradeName { get; set; } = "Nombre completo del grado";
        public int GradeClassroomHours { get; set; } = 100;
        public int GradeCompanyHours { get; set; } = 50;
        public string GradeFamilyName { get; set; } = "Nombre de la familia profesional";
        public CommonText GeneralObjectivesIntroduction { get; set; } = new CommonText();
        public ListProperty<CommonText> GeneralObjectives { get; } = new ListProperty<CommonText>();
        public CommonText GeneralCompetencesIntroduction { get; set; } = new CommonText();
        public ListProperty<CommonText> GeneralCompetences { get; } = new ListProperty<CommonText>();
        public CommonText KeyCapacitiesIntroduction { get; set; } = new CommonText();
        public ListProperty<CommonText> KeyCapacities { get; } = new ListProperty<CommonText>();
        public CommonText LearningResultsIntroduction { get; set; } = new CommonText();
        public ListProperty<LearningResult> LearningResults { get; } = new ListProperty<LearningResult>();
        public CommonText ContentsIntroduction { get; set; } = new CommonText();
        public ListProperty<Content> Contents { get; } = new ListProperty<Content>();

        public SubjectTemplate() : base()
        {
            StorageClassId = "subjecttemplate";
        }

        public override ValidationResult Validate()
        {
            throw new NotImplementedException();
        }

        public override bool Exists(string storageId, string? parentStorageId)
        {
            return Storage.ExistsData<SubjectTemplateData>(storageId, StorageClassId, parentStorageId);
        }

        public override void Save(string? parentStorageId = null)
        {
            base.Save(parentStorageId);

            SubjectTemplateData data = new();

            data.Title = Title;
            data.Description = Description;

            data.SubjectName = SubjectName;
            data.SubjectCode = SubjectCode;
            data.GradeType = GradeType;
            data.GradeName = GradeName;
            data.GradeFamilyName = GradeFamilyName;
            data.GradeClassroomHours = GradeClassroomHours;
            data.GradeCompanyHours = GradeCompanyHours;

            data.GeneralObjectivesIntroductionStorageId = GeneralObjectivesIntroduction.StorageId;
            GeneralObjectivesIntroduction.Save(StorageId);

            List<CommonText> list = GeneralObjectives.ToList();
            list.ForEach(e => e.Save(StorageId));            
            data.GeneralObjectivesStorageIds = Storage.GetStorageIds<CommonText>(list);

            data.GeneralCompetencesIntroductionStorageId = GeneralCompetencesIntroduction.StorageId;
            GeneralCompetencesIntroduction.Save(StorageId);

            list = GeneralCompetences.ToList();
            list.ForEach(e => e.Save(StorageId));
            data.GeneralCompetencesStorageIds = Storage.GetStorageIds<CommonText>(list);

            data.LearningResultsIntroductionStorageId = LearningResultsIntroduction.StorageId;
            LearningResultsIntroduction.Save(StorageId);

            List<LearningResult> listLearningResults = LearningResults.ToList();
            listLearningResults.ForEach(e => e.Save(StorageId));            
            data.LearningResultsStorageIds = Storage.GetStorageIds<LearningResult>(listLearningResults);

            data.ContentsIntroductionStorageId = ContentsIntroduction.StorageId;
            ContentsIntroduction.Save(StorageId);

            List<Content> listContents = Contents.ToList();
            listContents.ForEach(e => e.Save(StorageId));            
            data.ContentsStorageIds = Storage.GetStorageIds<Content>(listContents);

            Storage.SaveData<SubjectTemplateData>(StorageId, StorageClassId, data, parentStorageId);

        }

        public override void LoadOrCreate(string storageId, string? parentStorageId = null)
        {
            base.LoadOrCreate(storageId, parentStorageId);

            if(!Storage.ExistsData<SubjectTemplateData>(storageId, StorageClassId, parentStorageId)) { Save(parentStorageId); }

            SubjectTemplateData data = Storage.LoadData<SubjectTemplateData>(storageId, StorageClassId, parentStorageId);
            
            Title = data.Title;
            Description = data.Description;

            SubjectName = data.SubjectName;
            SubjectCode = data.SubjectCode;
            GradeType = data.GradeType;
            GradeName = data.GradeName;
            GradeFamilyName = data.GradeFamilyName;
            GradeClassroomHours = data.GradeClassroomHours;
            GradeCompanyHours = data.GradeCompanyHours;

            GeneralObjectivesIntroduction = Storage.LoadOrCreateEntity<CommonText>(data.GeneralObjectivesIntroductionStorageId, storageId);

            GeneralObjectives.Set(Storage.LoadEntities<CommonText>(data.GeneralObjectivesStorageIds, storageId));

            GeneralCompetencesIntroduction = Storage.LoadOrCreateEntity<CommonText>(data.GeneralCompetencesIntroductionStorageId, storageId);

            GeneralCompetences.Set(Storage.LoadEntities<CommonText>(data.GeneralCompetencesStorageIds, storageId));

            LearningResultsIntroduction = Storage.LoadOrCreateEntity<CommonText>(data.LearningResultsIntroductionStorageId, storageId);

            LearningResults.Set(Storage.LoadEntities<LearningResult>(data.LearningResultsStorageIds, storageId));

            ContentsIntroduction = Storage.LoadOrCreateEntity<CommonText>(data.ContentsIntroductionStorageId, storageId);

            Contents.Set(Storage.LoadEntities<Content>(data.ContentsStorageIds, storageId));

        }

        public override void Delete(string? parentStorageId = null)
        {
            base.Delete(parentStorageId);

            GeneralObjectivesIntroduction.Delete(StorageId);
            GeneralObjectives.ToList().ForEach(e => e.Delete(StorageId));
            GeneralCompetencesIntroduction.Delete(StorageId);
            GeneralCompetences.ToList().ForEach(e => e.Delete(StorageId));
            LearningResultsIntroduction.Delete(StorageId);
            LearningResults.ToList().ForEach(e => e.Delete(StorageId));
            ContentsIntroduction.Delete(StorageId);
            Contents.ToList().ForEach(e => e.Delete(StorageId));

            Storage.DeleteData(StorageId, StorageClassId, parentStorageId);


        }
    }
}
