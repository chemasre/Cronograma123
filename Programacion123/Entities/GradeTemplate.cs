namespace Programacion123
{
    public enum GradeType
    {
        superior,
        medium
    }

    public enum GradeCommonTextId
    {
        introductionToGeneralObjectives,
        introductionToGeneralCompetences,
        introductionToKeyCapacities,
        introductionToContents,
        introductionToLearningResults,
        introductionToEvaluationInstruments,
        introductionToResources,
        introductionToMetodologies,
        introductionToDiversity,
        introductionToEvaluation,
        schoolPolicyDiversity,
        schoolPolicyMetodology,
        schoolPolicyEvaluation,
        schoolPolicyOrdinaryEvaluation,
        schoolPolicyExtraordinaryEvaluation
    }

    public class GradeTemplate : Entity
    {
        public GradeType GradeType { get; set; } = GradeType.superior;
        public string GradeName { get; set; } = "Nombre completo del ciclo";
        public string GradeFamilyName { get; set; } = "Nombre de la familia profesional";
        public ListProperty<CommonText> GeneralObjectives { get; } = new ListProperty<CommonText>();
        public ListProperty<CommonText> GeneralCompetences { get; } = new ListProperty<CommonText>();
        public ListProperty<CommonText> KeyCapacities { get; } = new ListProperty<CommonText>();
        public DictionaryProperty<GradeCommonTextId, CommonText> CommonTexts { get; } = new DictionaryProperty<GradeCommonTextId, CommonText>();
     
        public GradeTemplate() : base()
        {
            StorageClassId = "gradetemplate";

            foreach(GradeCommonTextId id in Enum.GetValues<GradeCommonTextId>())
            {
                CommonTexts.Add(id, new CommonText());
            }

            CommonTexts[GradeCommonTextId.introductionToGeneralObjectives].Title = "Introducción a los objectivos generales";
            CommonTexts[GradeCommonTextId.introductionToGeneralCompetences].Title = "Introducción a las competencias profesionales, personales y sociales";
            CommonTexts[GradeCommonTextId.introductionToKeyCapacities].Title = "Introducción a las capacidades clave";
            CommonTexts[GradeCommonTextId.introductionToContents].Title = "Introducción a los contenidos";
            CommonTexts[GradeCommonTextId.introductionToLearningResults].Title = "Introducción a los resultados de aprendizaje";
            CommonTexts[GradeCommonTextId.introductionToEvaluationInstruments].Title = "Introducción a los instrumentos de evaluación";
            CommonTexts[GradeCommonTextId.introductionToResources].Title = "Introducción a los recursos";
            CommonTexts[GradeCommonTextId.introductionToMetodologies].Title = "Introducción a las metodologías";
            CommonTexts[GradeCommonTextId.introductionToDiversity].Title = "Introducción a atención a la diversidad";
            CommonTexts[GradeCommonTextId.introductionToEvaluation].Title = "Introducción a la evaluación";
            CommonTexts[GradeCommonTextId.schoolPolicyDiversity].Title = "Líneas generales de atención a la diversidad del centro educativo";
            CommonTexts[GradeCommonTextId.schoolPolicyMetodology].Title = "Líneas metodológicas generales del centro educativo";
            CommonTexts[GradeCommonTextId.schoolPolicyEvaluation].Title = "Líneas generales del centro educativo para la evaluación";
            CommonTexts[GradeCommonTextId.schoolPolicyOrdinaryEvaluation].Title = "Líneas generales del centro educativo para la evaluación ordinaria";
            CommonTexts[GradeCommonTextId.schoolPolicyExtraordinaryEvaluation].Title = "Líneas generales del centro educativo para la evaluación extraordinaria";
        }

        public override ValidationResult Validate()
        {
            ValidationResult result = base.Validate();

            if(result.code != ValidationCode.success) { return result; }

            if (GradeName.Trim().Length <= 0) { return ValidationResult.Create(ValidationCode.templateGradeNameEmpty); }
            if(GradeFamilyName.Trim().Length <= 0) { return ValidationResult.Create(ValidationCode.templateGradeFamilyNameEmpty); }

            List<CommonText> objectivesList = GeneralObjectives.ToList();
            if (objectivesList.Count <= 0) { return ValidationResult.Create(ValidationCode.templateGradeNoGeneralObjectives);  }
            for(int i = 0; i < objectivesList.Count; i++) { if(objectivesList[i].Validate().code != ValidationCode.success) { return ValidationResult.Create(ValidationCode.templateGradeGeneralObjectiveInvalid).WithIndex(i); } }

            List<CommonText> competencesList = GeneralCompetences.ToList();
            if (competencesList.Count <= 0) { return ValidationResult.Create(ValidationCode.templateGradeNoGeneralCompetences); }
            for (int i = 0; i < competencesList.Count; i++) { if(competencesList[i].Validate().code != ValidationCode.success) { return ValidationResult.Create(ValidationCode.templateGradeGeneralCompetenceInvalid).WithIndex(i); } }

            List<CommonText> capacitiesList = KeyCapacities.ToList();
            if (capacitiesList.Count <= 0) { return ValidationResult.Create(ValidationCode.templateGradeNoKeyCapacities); }
            for (int i = 0; i < capacitiesList.Count; i++) { if(capacitiesList[i].Validate().code != ValidationCode.success) { return ValidationResult.Create(ValidationCode.templateGradeKeyCapacitiesInvalid).WithIndex(i); } }

            List<KeyValuePair<GradeCommonTextId, CommonText> > commonTexts = CommonTexts.ToList();
            for(int i = 0; i < commonTexts.Count; i++)
            {
                if(commonTexts[i].Value.Validate().code != ValidationCode.success) { return ValidationResult.Create(ValidationCode.templateGradeCommonTextInvalid).WithIndex((int)commonTexts[i].Key); }
            }

            return ValidationResult.Create(ValidationCode.success);
        }

        public override bool Exists(string storageId, string? parentStorageId)
        {
            return Storage.ExistsData<GradeTemplateData>(storageId, StorageClassId, parentStorageId);
        }

        public override void Save(string? parentStorageId = null)
        {
            base.Save(parentStorageId);

            GradeTemplateData data = new();

            data.Title = Title;
            data.Description = Description;

            data.GradeType = GradeType;
            data.GradeName = GradeName;
            data.GradeFamilyName = GradeFamilyName;

            List<CommonText> list = GeneralObjectives.ToList();
            list.ForEach(e => e.Save(StorageId));            
            data.GeneralObjectivesStorageIds = Storage.GetStorageIds<CommonText>(list);

            list = GeneralCompetences.ToList();
            list.ForEach(e => e.Save(StorageId));
            data.GeneralCompetencesStorageIds = Storage.GetStorageIds<CommonText>(list);

            list = KeyCapacities.ToList();
            list.ForEach(e => e.Save(StorageId));
            data.KeyCapacitiesStorageIds= Storage.GetStorageIds<CommonText>(list);

            List<KeyValuePair<GradeCommonTextId, CommonText>> commonTextList = CommonTexts.ToList();
            commonTextList.ForEach(e => { e.Value.Save(StorageId); data.CommonTextsStorageIds.Add(e.Key, e.Value.StorageId); });

            Storage.SaveData<GradeTemplateData>(StorageId, StorageClassId, data, parentStorageId);

        }

        public override void LoadOrCreate(string storageId, string? parentStorageId = null)
        {
            base.LoadOrCreate(storageId, parentStorageId);

            if(!Storage.ExistsData<GradeTemplateData>(storageId, StorageClassId, parentStorageId)) { Save(parentStorageId); }

            GradeTemplateData data = Storage.LoadData<GradeTemplateData>(storageId, StorageClassId, parentStorageId);
            
            Title = data.Title;
            Description = data.Description;

            GradeType = data.GradeType;
            GradeName = data.GradeName;
            GradeFamilyName = data.GradeFamilyName;

            GeneralObjectives.Set(Storage.LoadOrCreateEntities<CommonText>(data.GeneralObjectivesStorageIds, storageId));

            GeneralCompetences.Set(Storage.LoadOrCreateEntities<CommonText>(data.GeneralCompetencesStorageIds, storageId));

            KeyCapacities.Set(Storage.LoadOrCreateEntities<CommonText>(data.KeyCapacitiesStorageIds, storageId));

            foreach(KeyValuePair<GradeCommonTextId, string> keyValue in data.CommonTextsStorageIds)
            { CommonTexts.Set(keyValue.Key, Storage.LoadOrCreateEntity<CommonText>(keyValue.Value, storageId)); }

        }

        public override void Delete(string? parentStorageId = null)
        {
            base.Delete(parentStorageId);

            GeneralObjectives.ToList().ForEach(e => e.Delete(StorageId));
            GeneralCompetences.ToList().ForEach(e => e.Delete(StorageId));
            KeyCapacities.ToList().ForEach(e => e.Delete(StorageId));
            CommonTexts.ToList().ForEach(e => e.Value.Delete(StorageId));

            Storage.DeleteData(StorageId, StorageClassId, parentStorageId);



        }
    }
}
