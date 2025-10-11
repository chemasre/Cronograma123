namespace Programacion123
{
    public enum GradeType
    {
        superior,
        medium
    }

    public class GradeTemplate : Entity
    {
        public Property<GradeType> GradeType { get; } = new(Programacion123.GradeType.superior);
        public Property<string> GradeName { get; } = new("Nombre completo del ciclo");
        public Property<string> GradeFamilyName { get; } = new("Nombre de la familia profesional");
        public ListEntityProperty<CommonText> GeneralObjectives { get; } = new ListEntityProperty<CommonText>();
        public ListEntityProperty<CommonText> GeneralCompetences { get; } = new ListEntityProperty<CommonText>();
        public ListEntityProperty<CommonText> KeyCapacities { get; } = new ListEntityProperty<CommonText>();
        public DictionaryEntityProperty<CommonTextId, CommonText> CommonTexts { get; } = new DictionaryEntityProperty<CommonTextId, CommonText>();

        const uint flagGradeType          = 1 << 2;
        const uint flagGradeName          = 1 << 3;
        const uint flagGradeFamilyName    = 1 << 4;
        const uint flagGeneralObjectives  = 1 << 5;
        const uint flagGeneralCompetences = 1 << 6;
        const uint flagKeyCapacities      = 1 << 7;
        const uint flagCommonTexts        = 1 << 8;

        const uint flagAll = ~0U;

        public GradeTemplate() : base()
        {
            StorageClassId = "gradetemplate";

            Title.Value = "Título de la plantilla de ciclo";
            Description.Value = "Descripción de la plantilla de ciclo";

            foreach (CommonTextId id in Enum.GetValues<CommonTextId>())
            {
                CommonTexts.Add(id, new CommonText());
            }

            CommonTexts[CommonTextId.header1ModuleOrganization].Title.Value = "[Encabezado1] Organización del módulo";
            CommonTexts[CommonTextId.header1ImportanceJustification].Title.Value = "[Enabezado1] Justificación de la importancia del módulo";
            CommonTexts[CommonTextId.header1CurricularElements].Title.Value = "[Encabezado1] Elementos Curriculares";
            CommonTexts[CommonTextId.header2GeneralObjectives].Title.Value = "[Encabezado2] Objetivos generales relacionados con el módulo";
            CommonTexts[CommonTextId.header2GeneralCompetences].Title.Value = "[Encabezado2] Competencias generales, profesionales, personales y sociales";
            CommonTexts[CommonTextId.header2KeyCompetences].Title.Value = "[Encabezado2] Capacidades clave";
            CommonTexts[CommonTextId.header1MetodologyAndDidacticOrientations].Title.Value = "[Encabezado1] Metodología. Orientaciones didácticas";
            CommonTexts[CommonTextId.header2Metodology].Title.Value = "[Encabezado2] Metodología general y específica de la materia";
            CommonTexts[CommonTextId.header2Diversity].Title.Value = "[Encabezado2] Medidas de atención al alumnado con necesidad específica de apoyo educativo o con necesidad de compensación educativa: atención a la diversidad";
            CommonTexts[CommonTextId.header1EvaluationSystem].Title.Value = "[Encabezado1] Sistema de evaluación";
            CommonTexts[CommonTextId.header2Evaluation].Title.Value = "[Encabezado2] Líneas evaluativas";
            CommonTexts[CommonTextId.header2EvaluationTypes].Title.Value = "[Encabezado2] Tipos de evaluación";
            CommonTexts[CommonTextId.header3OrdinaryEvaluation].Title.Value = "[Encabezado3] Evaluación ordinaria";
            CommonTexts[CommonTextId.header3ExtraordinaryEvaluation].Title.Value = "[Encabezado3] Evaluación extraordinaria";
            CommonTexts[CommonTextId.header2EvaluationInstruments].Title.Value = "[Encabezado2] Instrumentos de evaluación";
            CommonTexts[CommonTextId.header2EvaluationOfProgramming].Title.Value = "[Encabezado2] Evaluación del funcionamiento de la programación";
            CommonTexts[CommonTextId.header1TraversalElements].Title.Value = "[Encabezado1] Elementos transversales";
            CommonTexts[CommonTextId.header2TraversalReadingAndTIC].Title.Value = "[Encabezado2] Fomento de la lectura y tecnologías de la información y de comunicación";
            CommonTexts[CommonTextId.header2TraversalCommunicationEntrepreneurshipAndEducation].Title.Value = "[Encabezado2] Comunicación audiovisual, emprendimiento, educación cívica y constitucional";
            CommonTexts[CommonTextId.header1Resources].Title.Value = "[Encabezado1] Recursos didácticos y organizativos";
            CommonTexts[CommonTextId.header2ResourcesSpaces].Title.Value = "[Encabezado2] Espacios";
            CommonTexts[CommonTextId.header2ResourcesMaterialAndTools].Title.Value = "[Encabezado2] Materiales y herramientas";
            CommonTexts[CommonTextId.header1SubjectProgramming].Title.Value = "[Encabezado1] Programación del módulo profesional";
            CommonTexts[CommonTextId.header2LearningResultsAndContents].Title.Value = "[Encabezado2] Resultados de aprendizaje, criterios de evaluación y contenidos";
            CommonTexts[CommonTextId.header3LearningResults].Title.Value = "[Encabezado3] Resultados de aprendizaje y criterios de evaluación";
            CommonTexts[CommonTextId.header3Contents].Title.Value = "[Encabezado3] Contenidos";
            CommonTexts[CommonTextId.header2Blocks].Title.Value = "[Encabezado2] Bloques de enseñanza-aprendizaje";
            CommonTexts[CommonTextId.header2Activities].Title.Value = "[Encabezado2] Programación de actividades de enseñanza-aprendizaje";

            CommonTexts[CommonTextId.header1ModuleOrganization].Description.Value = "Escribe una introducción a la organización del módulo común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header1ImportanceJustification].Description.Value = "Escribe una justificación de la importancia del módulo común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header1CurricularElements].Description.Value = "Escribe una introducción a los elementos Curriculares común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header2GeneralObjectives].Description.Value = "Escribe una introducción a los objetivos generales relacionados con el módulo común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header2GeneralCompetences].Description.Value = "Escribe una introducción a los competencias generales, profesionales, personales y sociales común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header2KeyCompetences].Description.Value = "Escribe una introducción a las Capacidades clave común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header1MetodologyAndDidacticOrientations].Description.Value = "Escribe una introducción a la metodología y las orientaciones didácticas común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header2Metodology].Description.Value = "Escribe una introducción a la metodología general y específica de la materia común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header2Diversity].Description.Value = "Escribe una introducción a las medidas de atención al alumnado con necesidad específica de apoyo educativo o con necesidad de compensación educativa: atención a la diversidad común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header1EvaluationSystem].Description.Value = "Escribe una introducción al sistema de evaluación común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header2Evaluation].Description.Value = "Escribe una introducción a las líneas evaluativas común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header2EvaluationTypes].Description.Value = "Escribe una introducción a los tipos de evaluación común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header3OrdinaryEvaluation].Description.Value = "Escribe una introducción a la evaluación ordinaria común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header3ExtraordinaryEvaluation].Description.Value = "Escribe una introducción a la evaluación extraordinaria común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header2EvaluationInstruments].Description.Value = "Escribe una introducción a los instrumentos de evaluación común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header2EvaluationOfProgramming].Description.Value = "Escribe una introducción a la evaluación del funcionamiento de la programación común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header1TraversalElements].Description.Value = "Escribe una introducción a los elementos transversales común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header2TraversalReadingAndTIC].Description.Value = "Escribe una introducción al elemento transversal de fomento de la lectura y tecnologías de la información y de comunicación común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header2TraversalCommunicationEntrepreneurshipAndEducation].Description.Value = "Escribe una introducción al elemento transversal de comunicación audiovisual, emprendimiento, educación cívica y constitucional común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header1Resources].Description.Value = "Escribe una introducción a los recursos didácticos y organizativos común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header2ResourcesSpaces].Description.Value = "Escribe una introducción a los espacios común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header2ResourcesMaterialAndTools].Description.Value = "Escribe una introducción a los materiales y herramientas común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header1SubjectProgramming].Description.Value = "Escribe una introducción a la programación del módulo profesional común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header2LearningResultsAndContents].Description.Value = "Escribe una introducción a los resultados de aprendizaje, criterios de evaluación y contenidos común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header3LearningResults].Description.Value = "Escribe una introducción a los resultados de aprendizaje y criterios de evaluación común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header3Contents].Description.Value = "Escribe una introducción a los contenidos común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header2Blocks].Description.Value = "Escribe una introducción a los bloques de enseñanza-aprendizaje común a todos los módulos del ciclo";
            CommonTexts[CommonTextId.header2Activities].Description.Value = "Escribe una introducción a la programación de actividades de enseñanza-aprendizaje común a todos los módulos del ciclo";

            GradeType.OnSetted += (current, next) => { Flags.Add(ref validationFlags, flagGradeType); InvokeOnUpdated(); };
            GradeName.OnSetted += (current, next) => { Flags.Add(ref validationFlags, flagGradeName); InvokeOnUpdated(); };
            GradeFamilyName.OnSetted += (current, next) => { Flags.Add(ref validationFlags, flagGradeFamilyName); InvokeOnUpdated(); };

            GeneralObjectives.OnAdded += (element) => { Flags.Add(ref validationFlags, flagGeneralObjectives); InvokeOnUpdated(); };
            GeneralObjectives.OnRemoved += (element) => { Flags.Add(ref validationFlags, flagGeneralObjectives); InvokeOnUpdated(); };
            GeneralObjectives.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagGeneralObjectives); InvokeOnUpdated(); };

            GeneralCompetences.OnAdded += (element) => { Flags.Add(ref validationFlags, flagGeneralCompetences); InvokeOnUpdated(); };
            GeneralCompetences.OnRemoved += (element) => { Flags.Add(ref validationFlags, flagGeneralCompetences); InvokeOnUpdated(); };
            GeneralCompetences.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagGeneralCompetences); InvokeOnUpdated(); };

            KeyCapacities.OnAdded += (element) => { Flags.Add(ref validationFlags, flagKeyCapacities); InvokeOnUpdated(); };
            KeyCapacities.OnRemoved += (element) => { Flags.Add(ref validationFlags, flagKeyCapacities); InvokeOnUpdated(); };
            KeyCapacities.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagKeyCapacities); InvokeOnUpdated(); };

            CommonTexts.OnAdded += (key, element) => { Flags.Add(ref validationFlags, flagCommonTexts); InvokeOnUpdated(); };
            CommonTexts.OnRemoved += (key) => { Flags.Add(ref validationFlags, flagCommonTexts); InvokeOnUpdated(); };
            CommonTexts.OnUpdated += (key, element) => { Flags.Add(ref validationFlags, flagCommonTexts); InvokeOnUpdated(); };
            CommonTexts.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagCommonTexts); InvokeOnUpdated(); };

            Flags.Add(ref validationFlags, flagGradeType);
            Flags.Add(ref validationFlags, flagGradeName);
            Flags.Add(ref validationFlags, flagGradeFamilyName);
            Flags.Add(ref validationFlags, flagGeneralObjectives);
            Flags.Add(ref validationFlags, flagGeneralCompetences);
            Flags.Add(ref validationFlags, flagKeyCapacities);
            Flags.Add(ref validationFlags, flagCommonTexts);

        }

        public override ValidationResult Validate(bool force = false)
        {
            base.Validate(force);

            Console.WriteLine(Title.Value + ": Grade template validation start");

            if(Flags.Test(validationFlags, flagGradeName) || force)
            {
                Console.WriteLine("[gradeName] => Validating not empty");

                validationFails.RemoveAll(e => e.code == ValidationCode.templateGradeNameEmpty);

                if (GradeName.Value.Trim().Length <= 0) { validationFails.Add(ValidationResult.Create(ValidationCode.templateGradeNameEmpty)); }
            }

            if(Flags.Test(validationFlags, flagGradeFamilyName) || force)
            {
                Console.WriteLine("[gradeFamilyName] => Validating not empty");

                validationFails.RemoveAll(e => e.code == ValidationCode.templateGradeFamilyNameEmpty);

                if (GradeFamilyName.Value.Trim().Length <= 0) { validationFails.Add(ValidationResult.Create(ValidationCode.templateGradeFamilyNameEmpty)); }
            }

            if(Flags.Test(validationFlags, flagGeneralObjectives) || force)
            {
                Console.WriteLine("[objectives] => Validating at least one exist and all are valid");

                validationFails.RemoveAll(e => e.code == ValidationCode.templateGradeNoGeneralObjectives);
                validationFails.RemoveAll(e => e.code == ValidationCode.templateGradeGeneralObjectiveInvalid);

                List<CommonText> objectivesList = GeneralObjectives.ToList();
                if (objectivesList.Count <= 0) { validationFails.Add(ValidationResult.Create(ValidationCode.templateGradeNoGeneralObjectives)); }
                for (int i = 0; i < objectivesList.Count; i++) { if (objectivesList[i].Validate().code != ValidationCode.success) { validationFails.Add(ValidationResult.Create(ValidationCode.templateGradeGeneralObjectiveInvalid).WithIndex(i)); } }
            }


            if(Flags.Test(validationFlags, flagGeneralCompetences) || force)
            {
                Console.WriteLine("[competences] => Validating at least one exist and all are valid");

                validationFails.RemoveAll(e => e.code == ValidationCode.templateGradeNoGeneralCompetences);
                validationFails.RemoveAll(e => e.code == ValidationCode.templateGradeGeneralCompetenceInvalid);

                List<CommonText> competencesList = GeneralCompetences.ToList();
                if (competencesList.Count <= 0) { validationFails.Add(ValidationResult.Create(ValidationCode.templateGradeNoGeneralCompetences)); }
                for (int i = 0; i < competencesList.Count; i++) { if (competencesList[i].Validate().code != ValidationCode.success) { validationFails.Add(ValidationResult.Create(ValidationCode.templateGradeGeneralCompetenceInvalid).WithIndex(i)); } }
            }

            if(Flags.Test(validationFlags, flagKeyCapacities) || force)
            {
                Console.WriteLine("[capacities] => Validating at least one exist and all are valid");

                validationFails.RemoveAll(e => e.code == ValidationCode.templateGradeNoKeyCapacities);
                validationFails.RemoveAll(e => e.code == ValidationCode.templateGradeKeyCapacitiesInvalid);

                List<CommonText> capacitiesList = KeyCapacities.ToList();
                if (capacitiesList.Count <= 0) { validationFails.Add(ValidationResult.Create(ValidationCode.templateGradeNoKeyCapacities)); }
                for (int i = 0; i < capacitiesList.Count; i++) { if (capacitiesList[i].Validate().code != ValidationCode.success) { validationFails.Add(ValidationResult.Create(ValidationCode.templateGradeKeyCapacitiesInvalid).WithIndex(i)); } }
            }

            if(Flags.Test(validationFlags, flagCommonTexts) || force)
            {
                Console.WriteLine("[commonTexts] => Validating all are valid");

                validationFails.RemoveAll(e => e.code == ValidationCode.templateGradeCommonTextInvalid);

                List<KeyValuePair<CommonTextId, CommonText>> commonTexts = CommonTexts.ToList();
                for (int i = 0; i < commonTexts.Count; i++)
                {
                    if (commonTexts[i].Value.Validate().code != ValidationCode.success) { validationFails.Add(ValidationResult.Create(ValidationCode.templateGradeCommonTextInvalid).WithIndex((int)commonTexts[i].Key)); }
                }
            }

            Flags.Remove(ref validationFlags, flagGradeType);
            Flags.Remove(ref validationFlags, flagGradeName);
            Flags.Remove(ref validationFlags, flagGradeFamilyName);
            Flags.Remove(ref validationFlags, flagGeneralObjectives);
            Flags.Remove(ref validationFlags, flagGeneralCompetences);
            Flags.Remove(ref validationFlags, flagKeyCapacities);
            Flags.Remove(ref validationFlags, flagCommonTexts);

            Console.WriteLine(Title.Value + ": Grade template validation end");
            foreach(ValidationResult fail in validationFails) { Console.WriteLine("FAILED: " + fail.code + "(" + fail.index + ")"); }

            if(validationFails.Count == 0) { return ValidationResult.Create(ValidationCode.success); }
            else { return validationFails[0]; }
        }

        public override bool Exists(string storageId, string? parentStorageId)
        {
            return Storage.ExistsData<GradeTemplateData>(storageId, StorageClassId, parentStorageId);
        }

        public override void Save(string? parentStorageId = null)
        {
            base.Save(parentStorageId);

            GradeTemplateData data = new();

            data.Title = Title.Value;
            data.Description = Description.Value;

            data.GradeType = GradeType.Value;
            data.GradeName = GradeName.Value;
            data.GradeFamilyName = GradeFamilyName.Value;

            List<CommonText> list = GeneralObjectives.ToList();
            list.ForEach(e => e.Save(StorageId));
            data.GeneralObjectivesStorageIds = Storage.GetStorageIds<CommonText>(list);

            list = GeneralCompetences.ToList();
            list.ForEach(e => e.Save(StorageId));
            data.GeneralCompetencesStorageIds = Storage.GetStorageIds<CommonText>(list);

            list = KeyCapacities.ToList();
            list.ForEach(e => e.Save(StorageId));
            data.KeyCapacitiesStorageIds = Storage.GetStorageIds<CommonText>(list);

            List<KeyValuePair<CommonTextId, CommonText>> commonTextList = CommonTexts.ToList();
            commonTextList.ForEach(e => { e.Value.Save(StorageId); data.CommonTextsStorageIds.Add(e.Key, e.Value.StorageId); });

            Storage.SaveData<GradeTemplateData>(StorageId, StorageClassId, data, parentStorageId);

        }

        public override void LoadOrCreate(string storageId, string? parentStorageId = null)
        {
            base.LoadOrCreate(storageId, parentStorageId);

            if (!Storage.ExistsData<GradeTemplateData>(storageId, StorageClassId, parentStorageId)) { Save(parentStorageId); }

            GradeTemplateData data = Storage.LoadData<GradeTemplateData>(storageId, StorageClassId, parentStorageId);

            Title.Value = data.Title;
            Description.Value = data.Description;

            GradeType.Value = data.GradeType;
            GradeName.Value = data.GradeName;
            GradeFamilyName.Value = data.GradeFamilyName;

            GeneralObjectives.Set(Storage.LoadOrCreateEntities<CommonText>(data.GeneralObjectivesStorageIds, storageId));

            GeneralCompetences.Set(Storage.LoadOrCreateEntities<CommonText>(data.GeneralCompetencesStorageIds, storageId));

            KeyCapacities.Set(Storage.LoadOrCreateEntities<CommonText>(data.KeyCapacitiesStorageIds, storageId));

            foreach (KeyValuePair<CommonTextId, string> keyValue in data.CommonTextsStorageIds)
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
