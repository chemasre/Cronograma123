namespace Programacion123
{
    public enum GradeType
    {
        superior,
        medium
    }

    public class GradeTemplate : Entity
    {
        public GradeType GradeType { get; set; } = GradeType.superior;
        public string GradeName { get; set; } = "Nombre completo del ciclo";
        public string GradeFamilyName { get; set; } = "Nombre de la familia profesional";
        public ListProperty<CommonText> GeneralObjectives { get; } = new ListProperty<CommonText>();
        public ListProperty<CommonText> GeneralCompetences { get; } = new ListProperty<CommonText>();
        public ListProperty<CommonText> KeyCapacities { get; } = new ListProperty<CommonText>();
        public DictionaryProperty<CommonTextId, CommonText> CommonTexts { get; } = new DictionaryProperty<CommonTextId, CommonText>();
     
        public GradeTemplate() : base()
        {
            StorageClassId = "gradetemplate";

            foreach(CommonTextId id in Enum.GetValues<CommonTextId>())
            {
                CommonTexts.Add(id, new CommonText());
            }

            CommonTexts[CommonTextId.header1ModuleOrganization].Title =          "[Encabezado1] Organización del módulo";
            CommonTexts[CommonTextId.header1ImportanceJustification].Title =     "[Enabezado1] Justificación de la importancia del módulo";
            CommonTexts[CommonTextId.header1CurricularElements].Title =          "[Encabezado1] Elementos Curriculares";
            CommonTexts[CommonTextId.header2GeneralObjectives].Title =           "[Encabezado2] Objetivos generales relacionados con el módulo";
            CommonTexts[CommonTextId.header2GeneralCompetences].Title =          "[Encabezado2] Competencias generales, profesionales, personales y sociales";
            CommonTexts[CommonTextId.header2KeyCompetences].Title =               "[Encabezado2] Capacidades clave";
            CommonTexts[CommonTextId.header1MetodologyAndDidacticOrientations].Title = "[Encabezado1] Metodología. Orientaciones didácticas";
            CommonTexts[CommonTextId.header2Metodology].Title =                  "[Encabezado2] Metodología general y específica de la materia";
            CommonTexts[CommonTextId.header2Diversity].Title =                   "[Encabezado2] Medidas de atención al alumnado con necesidad específica de apoyo educativo o con necesidad de compensación educativa: atención a la diversidad";
            CommonTexts[CommonTextId.header1EvaluationSystem].Title =            "[Encabezado1] Sistema de evaluación";
            CommonTexts[CommonTextId.header2Evaluation].Title =                  "[Encabezado2] Líneas evaluativas";
            CommonTexts[CommonTextId.header2EvaluationTypes].Title =             "[Encabezado2] Tipos de evaluación";
            CommonTexts[CommonTextId.header3OrdinaryEvaluation].Title =          "[Encabezado3] Evaluación ordinaria";
            CommonTexts[CommonTextId.header3ExtraordinaryEvaluation].Title =     "[Encabezado3] Evaluación extraordinaria";
            CommonTexts[CommonTextId.header2EvaluationInstruments].Title =       "[Encabezado2] Instrumentos de evaluación";
            CommonTexts[CommonTextId.header2EvaluationOfProgramming].Title =     "[Encabezado2] Evaluación del funcionamiento de la programación";
            CommonTexts[CommonTextId.header1TraversalElements].Title =           "[Encabezado1] Elementos transversales";
            CommonTexts[CommonTextId.header2TraversalReadingAndTIC].Title =      "[Encabezado2] Fomento de la lectura y tecnologías de la información y de comunicación";
            CommonTexts[CommonTextId.header2TraversalCommunicationEntrepreneurshipAndEducation].Title = "[Encabezado2] Comunicación audiovisual, emprendimiento, educación cívica y constitucional";
            CommonTexts[CommonTextId.header1Resources].Title =                   "[Encabezado1] Recursos didácticos y organizativos";
            CommonTexts[CommonTextId.header2ResourcesSpaces].Title =              "[Encabezado2] Espacios";
            CommonTexts[CommonTextId.header2ResourcesMaterialAndTools].Title =   "[Encabezado2] Materiales y herramientas";
            CommonTexts[CommonTextId.header1SubjectProgramming].Title =          "[Encabezado1] Programación del módulo profesional";
            CommonTexts[CommonTextId.header2LearningResultsAndContents].Title =  "[Encabezado2] Resultados de aprendizaje, criterios de evaluación y contenidos";
            CommonTexts[CommonTextId.header3LearningResults].Title =             "[Encabezado3] Resultados de aprendizaje y criterios de evaluación";
            CommonTexts[CommonTextId.header3Contents].Title =                    "[Encabezado3] Contenidos";
            CommonTexts[CommonTextId.header2Blocks].Title =                      "[Encabezado2] Bloques de enseñanza-aprendizaje";
            CommonTexts[CommonTextId.header2Activities].Title =                  "[Encabezado2] Programación de actividades de enseñanza-aprendizaje";

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

            List<KeyValuePair<CommonTextId, CommonText> > commonTexts = CommonTexts.ToList();
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

            List<KeyValuePair<CommonTextId, CommonText>> commonTextList = CommonTexts.ToList();
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

            foreach(KeyValuePair<CommonTextId, string> keyValue in data.CommonTextsStorageIds)
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
