using Microsoft.Office.Interop.Word;
using System.Windows.Media;

namespace Programacion123
{
    public class SubjectTemplate : Entity
    {
        public GradeTemplate? GradeTemplate { get; set; }

        public string SubjectName { get; set; } = "Nombre completo del módulo";
        public string SubjectCode { get; set; } = "Código del módulo";
        public int GradeClassroomHours { get; set; } = 100;
        public int GradeCompanyHours { get; set; } = 50;
        public ListProperty<CommonText> GeneralObjectives { get; } = new ListProperty<CommonText>();
        public ListProperty<CommonText> GeneralCompetences { get; } = new ListProperty<CommonText>();
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
            ValidationResult result = base.Validate();

            if(result.code != ValidationCode.success) { return result; }

            if (GradeTemplate == null) { return ValidationResult.Create(ValidationCode.templateSubjectNotLinkedToGradeTemplate); }

            if (SubjectName.Trim().Length <= 0) { return ValidationResult.Create(ValidationCode.templateSubjectNameEmpty); }
            if(SubjectCode.Trim().Length <= 0) { return ValidationResult.Create(ValidationCode.templateSubjectCodeEmpty); }
            if(GradeClassroomHours <= 0) { return ValidationResult.Create(ValidationCode.templateSubjectClassroomHoursZero); }

            List<CommonText> objectivesList = GeneralObjectives.ToList();
            if (objectivesList.Count <= 0) { return ValidationResult.Create(ValidationCode.templateSubjectNoGeneralObjectivesReferenced);  }
            
            List<CommonText> competencesList = GeneralCompetences.ToList();
            if (competencesList.Count <= 0) { return ValidationResult.Create(ValidationCode.templateSubjectNoGeneralCompetencesReferenced); }

            List<CommonText> capacitiesList = KeyCapacities.ToList();
            if (capacitiesList.Count <= 0) { return ValidationResult.Create(ValidationCode.templateSubjectNoKeyCapacitiesReferenced); }

            if(LearningResultsIntroduction.Validate().code != ValidationCode.success) { return ValidationResult.Create(ValidationCode.templateSubjectLearningResultsIntroductionInvalid); }

            List<LearningResult> resultsList = LearningResults.ToList();
            if (resultsList.Count <= 0) { return ValidationResult.Create(ValidationCode.templateSubjectNoLearningResults); }
            for (int i = 0; i < resultsList.Count; i++) { if(resultsList[i].Validate().code != ValidationCode.success) { return ValidationResult.Create(ValidationCode.templateSubjectLearningResultsInvalid).WithIndex(i); } }

            if(ContentsIntroduction.Validate().code != ValidationCode.success) { return ValidationResult.Create(ValidationCode.templateSubjectContentsIntroductionInvalid); }

            List<Content> contentsList = Contents.ToList();
            if (contentsList.Count <= 0) { return ValidationResult.Create(ValidationCode.templateSubjectNoContents); }
            for (int i = 0; i < contentsList.Count; i++) { if(contentsList[i].Validate().code != ValidationCode.success) { return ValidationResult.Create(ValidationCode.templateSubjectContentsInvalid).WithIndex(i); } }


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

            data.Title = Title;
            data.Description = Description;

            data.GradeTemplateWeakStorageId = GradeTemplate?.StorageId;

            data.SubjectName = SubjectName;
            data.SubjectCode = SubjectCode;
            data.GradeClassroomHours = GradeClassroomHours;
            data.GradeCompanyHours = GradeCompanyHours;

            List<CommonText> list = GeneralObjectives.ToList();           
            data.GeneralObjectivesWeakStorageIds = Storage.GetStorageIds<CommonText>(list);

            list = GeneralCompetences.ToList();
            data.GeneralCompetencesWeakStorageIds = Storage.GetStorageIds<CommonText>(list);

            list = KeyCapacities.ToList();
            data.KeyCapacitiesWeakStorageIds= Storage.GetStorageIds<CommonText>(list);

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

            GradeTemplate = data.GradeTemplateWeakStorageId != null ? Storage.LoadOrCreateEntity<GradeTemplate>(data.GradeTemplateWeakStorageId, null) : null;

            SubjectName = data.SubjectName;
            SubjectCode = data.SubjectCode;
            GradeClassroomHours = data.GradeClassroomHours;
            GradeCompanyHours = data.GradeCompanyHours;

            GeneralObjectives.Set(Storage.FindChildEntities<CommonText>(data.GeneralObjectivesWeakStorageIds));

            GeneralCompetences.Set(Storage.FindChildEntities<CommonText>(data.GeneralCompetencesWeakStorageIds));

            KeyCapacities.Set(Storage.FindChildEntities<CommonText>(data.KeyCapacitiesWeakStorageIds));

            LearningResultsIntroduction = Storage.LoadOrCreateEntity<CommonText>(data.LearningResultsIntroductionStorageId, storageId);

            LearningResults.Set(Storage.LoadOrCreateEntities<LearningResult>(data.LearningResultsStorageIds, storageId));

            ContentsIntroduction = Storage.LoadOrCreateEntity<CommonText>(data.ContentsIntroductionStorageId, storageId);

            Contents.Set(Storage.LoadOrCreateEntities<Content>(data.ContentsStorageIds, storageId));

        }

        public override void Delete(string? parentStorageId = null)
        {
            base.Delete(parentStorageId);

            LearningResultsIntroduction.Delete(StorageId);
            LearningResults.ToList().ForEach(e => e.Delete(StorageId));
            ContentsIntroduction.Delete(StorageId);
            Contents.ToList().ForEach(e => e.Delete(StorageId));

            Storage.DeleteData(StorageId, StorageClassId, parentStorageId);



        }
    }
}
