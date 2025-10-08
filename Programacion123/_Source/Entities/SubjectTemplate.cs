namespace Programacion123
{
    public class SubjectTemplate : Entity
    {
        public EntityProperty<GradeTemplate?> GradeTemplate { get; } = new (null);

        public Property<string> SubjectName { get; } = new ("Nombre completo del módulo");
        public Property<string> SubjectCode { get; } = new ("Código del módulo");
        public Property<int> GradeClassroomHours { get; } = new (100);
        public Property<int> GradeCompanyHours { get; } = new (50);
        public ListEntityProperty<CommonText> GeneralObjectives { get; } = new ListEntityProperty<CommonText>();
        public ListEntityProperty<CommonText> GeneralCompetences { get; } = new ListEntityProperty<CommonText>();
        public ListEntityProperty<LearningResult> LearningResults { get; } = new ListEntityProperty<LearningResult>();
        public ListEntityProperty<Content> Contents { get; } = new ListEntityProperty<Content>();

        public SubjectTemplate() : base()
        {
            StorageClassId = "subjecttemplate";

            Title.Value = "Título de la plantilla de módulo";
            Description.Value = "Descripción de la plantilla de módulo";
        }

        public override ValidationResult Validate()
        {
            ValidationResult result = base.Validate();

            if (result.code != ValidationCode.success) { return result; }

            if (GradeTemplate.Value == null) { return ValidationResult.Create(ValidationCode.templateSubjectNotLinkedToGradeTemplate); }

            if (SubjectName.Value.Trim().Length <= 0) { return ValidationResult.Create(ValidationCode.templateSubjectNameEmpty); }
            if (SubjectCode.Value.Trim().Length <= 0) { return ValidationResult.Create(ValidationCode.templateSubjectCodeEmpty); }
            if (GradeClassroomHours.Value <= 0) { return ValidationResult.Create(ValidationCode.templateSubjectClassroomHoursZero); }

            List<CommonText> objectivesList = GeneralObjectives.ToList();
            if (objectivesList.Count <= 0) { return ValidationResult.Create(ValidationCode.templateSubjectNoGeneralObjectivesReferenced); }

            List<CommonText> competencesList = GeneralCompetences.ToList();
            if (competencesList.Count <= 0) { return ValidationResult.Create(ValidationCode.templateSubjectNoGeneralCompetencesReferenced); }

            List<LearningResult> resultsList = LearningResults.ToList();
            if (resultsList.Count <= 0) { return ValidationResult.Create(ValidationCode.templateSubjectNoLearningResults); }
            for (int i = 0; i < resultsList.Count; i++) { if (resultsList[i].Validate().code != ValidationCode.success) { return ValidationResult.Create(ValidationCode.templateSubjectLearningResultsInvalid).WithIndex(i); } }

            List<Content> contentsList = Contents.ToList();
            if (contentsList.Count <= 0) { return ValidationResult.Create(ValidationCode.templateSubjectNoContents); }
            for (int i = 0; i < contentsList.Count; i++) { if (contentsList[i].Validate().code != ValidationCode.success) { return ValidationResult.Create(ValidationCode.templateSubjectContentsInvalid).WithIndex(i); } }


            return ValidationResult.Create(ValidationCode.success);
        }

        public override bool Exists(string storageId, string? parentStorageId)
        {
            return Storage.ExistsData<SubjectTemplateData>(storageId, StorageClassId, parentStorageId);
        }

        public override void Save(string? parentStorageId = null)
        {
            base.Save(parentStorageId);

            SubjectTemplateData data = new();

            data.Title = Title.Value;
            data.Description = Description.Value;

            data.GradeTemplateWeakStorageId = GradeTemplate.Value?.StorageId;

            data.SubjectName = SubjectName.Value;
            data.SubjectCode = SubjectCode.Value;
            data.GradeClassroomHours = GradeClassroomHours.Value;
            data.GradeCompanyHours = GradeCompanyHours.Value;

            List<CommonText> list = GeneralObjectives.ToList();
            data.GeneralObjectivesWeakStorageIds = Storage.GetStorageIds<CommonText>(list);

            list = GeneralCompetences.ToList();
            data.GeneralCompetencesWeakStorageIds = Storage.GetStorageIds<CommonText>(list);

            List<LearningResult> listLearningResults = LearningResults.ToList();
            listLearningResults.ForEach(e => e.Save(StorageId));
            data.LearningResultsStorageIds = Storage.GetStorageIds<LearningResult>(listLearningResults);

            List<Content> listContents = Contents.ToList();
            listContents.ForEach(e => e.Save(StorageId));
            data.ContentsStorageIds = Storage.GetStorageIds<Content>(listContents);

            Storage.SaveData<SubjectTemplateData>(StorageId, StorageClassId, data, parentStorageId);

        }

        public override void LoadOrCreate(string storageId, string? parentStorageId = null)
        {
            base.LoadOrCreate(storageId, parentStorageId);

            if (!Storage.ExistsData<SubjectTemplateData>(storageId, StorageClassId, parentStorageId)) { Save(parentStorageId); }

            SubjectTemplateData data = Storage.LoadData<SubjectTemplateData>(storageId, StorageClassId, parentStorageId);

            Title.Value = data.Title;
            Description.Value = data.Description;

            GradeTemplate.Value = data.GradeTemplateWeakStorageId != null ? Storage.LoadOrCreateEntity<GradeTemplate>(data.GradeTemplateWeakStorageId, null) : null;

            SubjectName.Value = data.SubjectName;
            SubjectCode.Value = data.SubjectCode;
            GradeClassroomHours.Value = data.GradeClassroomHours;
            GradeCompanyHours.Value = data.GradeCompanyHours;

            GeneralObjectives.Set(Storage.FindChildEntities<CommonText>(data.GeneralObjectivesWeakStorageIds));

            GeneralCompetences.Set(Storage.FindChildEntities<CommonText>(data.GeneralCompetencesWeakStorageIds));

            LearningResults.Set(Storage.LoadOrCreateEntities<LearningResult>(data.LearningResultsStorageIds, storageId));

            Contents.Set(Storage.LoadOrCreateEntities<Content>(data.ContentsStorageIds, storageId));

        }

        public override void Delete(string? parentStorageId = null)
        {
            base.Delete(parentStorageId);

            LearningResults.ToList().ForEach(e => e.Delete(StorageId));
            Contents.ToList().ForEach(e => e.Delete(StorageId));

            Storage.DeleteData(StorageId, StorageClassId, parentStorageId);



        }
    }
}
