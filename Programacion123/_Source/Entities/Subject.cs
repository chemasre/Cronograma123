namespace Programacion123
{
    public partial class Subject : Entity
    {
        public EntityProperty<SubjectTemplate?> Template { get; } = new(null);
        public EntityProperty<Calendar?> Calendar { get; } = new(null);
        public EntityProperty<WeekSchedule?> WeekSchedule { get; } = new(null);

        public ListEntityProperty<CommonText> Metodologies { get; } = new ListEntityProperty<CommonText>();
        public ListEntityProperty<CommonText> SpaceResources { get; } = new ListEntityProperty<CommonText>();
        public ListEntityProperty<CommonText> MaterialResources { get; } = new ListEntityProperty<CommonText>();
        public ListEntityProperty<CommonText> EvaluationInstrumentsTypes { get; } = new ListEntityProperty<CommonText>();
        public ListEntityProperty<CommonText> Citations { get; } = new ListEntityProperty<CommonText>();

        public ListEntityProperty<Block> Blocks { get; } = new ListEntityProperty<Block>();

        public DictionaryProperty<LearningResult, float> LearningResultsWeights { get; } = new DictionaryProperty<LearningResult, float>();

        public DictionaryEntityProperty<CommonTextId, CommonText> CommonTexts { get; } = new DictionaryEntityProperty<CommonTextId, CommonText>();

        const uint flagTemplate                  = 1 << 2;
        const uint flagCalendar                  = 1 << 3;
        const uint flagWeekSchedule              = 1 << 4;
        const uint flagMetodologies              = 1 << 5;
        const uint flagSpaceResources            = 1 << 6;
        const uint flagMaterialResources         = 1 << 7;
        const uint flagEvaluationInstrumentTypes = 1 << 8;
        const uint flagCitations                 = 1 << 9;
        const uint flagBlocks                    = 1 << 10;
        const uint flagLearningResultsWeights    = 1 << 11;
        const uint flagCommonTexts               = 1 << 12;

        public Subject() : base()
        {
            StorageClassId = "subject";

            Title.Value = "Título de la programación de módulo";
            Description.Value = "Descripción de la programación de módulo";

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

            CommonTexts[CommonTextId.header1ModuleOrganization].Description.Value = "Escribe una introducción a la organización del módulo específica del módulo";
            CommonTexts[CommonTextId.header1ImportanceJustification].Description.Value = "Escribe una justificación de la importancia del módulo específica del módulo";
            CommonTexts[CommonTextId.header1CurricularElements].Description.Value = "Escribe una introducción a los elementos Curriculares específica del módulo";
            CommonTexts[CommonTextId.header2GeneralObjectives].Description.Value = "Escribe una introducción a los objetivos generales relacionados con el módulo específica del módulo";
            CommonTexts[CommonTextId.header2GeneralCompetences].Description.Value = "Escribe una introducción a los competencias generales, profesionales, personales y sociales específica del módulo";
            CommonTexts[CommonTextId.header2KeyCompetences].Description.Value = "Escribe una introducción a las Capacidades clave específica del módulo";
            CommonTexts[CommonTextId.header1MetodologyAndDidacticOrientations].Description.Value = "Escribe una introducción a la metodología y las orientaciones didácticas específica del módulo";
            CommonTexts[CommonTextId.header2Metodology].Description.Value = "Escribe una introducción a la metodología general y específica de la materia específica del módulo";
            CommonTexts[CommonTextId.header2Diversity].Description.Value = "Escribe una introducción a las medidas de atención al alumnado con necesidad específica de apoyo educativo o con necesidad de compensación educativa: atención a la diversidad específica del módulo";
            CommonTexts[CommonTextId.header1EvaluationSystem].Description.Value = "Escribe una introducción al sistema de evaluación específica del módulo";
            CommonTexts[CommonTextId.header2Evaluation].Description.Value = "Escribe una introducción a las líneas evaluativas específica del módulo";
            CommonTexts[CommonTextId.header2EvaluationTypes].Description.Value = "Escribe una introducción a los tipos de evaluación específica del módulo";
            CommonTexts[CommonTextId.header3OrdinaryEvaluation].Description.Value = "Escribe una introducción a la evaluación ordinaria específica del módulo";
            CommonTexts[CommonTextId.header3ExtraordinaryEvaluation].Description.Value = "Escribe una introducción a la evaluación extraordinaria específica del módulo";
            CommonTexts[CommonTextId.header2EvaluationInstruments].Description.Value = "Escribe una introducción a los instrumentos de evaluación específica del módulo";
            CommonTexts[CommonTextId.header2EvaluationOfProgramming].Description.Value = "Escribe una introducción a la evaluación del funcionamiento de la programación específica del módulo";
            CommonTexts[CommonTextId.header1TraversalElements].Description.Value = "Escribe una introducción a los elementos transversales específica del módulo";
            CommonTexts[CommonTextId.header2TraversalReadingAndTIC].Description.Value = "Escribe una introducción al elemento transversal de fomento de la lectura y tecnologías de la información y de comunicación específica del módulo";
            CommonTexts[CommonTextId.header2TraversalCommunicationEntrepreneurshipAndEducation].Description.Value = "Escribe una introducción al elemento transversal de comunicación audiovisual, emprendimiento, educación cívica y constitucional específica del módulo";
            CommonTexts[CommonTextId.header1Resources].Description.Value = "Escribe una introducción a los recursos didácticos y organizativos específica del módulo";
            CommonTexts[CommonTextId.header2ResourcesSpaces].Description.Value = "Escribe una introducción a los espacios específica del módulo";
            CommonTexts[CommonTextId.header2ResourcesMaterialAndTools].Description.Value = "Escribe una introducción a los materiales y herramientas específica del módulo";
            CommonTexts[CommonTextId.header1SubjectProgramming].Description.Value = "Escribe una introducción a la programación del módulo profesional específica del módulo";
            CommonTexts[CommonTextId.header2LearningResultsAndContents].Description.Value = "Escribe una introducción a los resultados de aprendizaje, criterios de evaluación y contenidos específica del módulo";
            CommonTexts[CommonTextId.header3LearningResults].Description.Value = "Escribe una introducción a los resultados de aprendizaje y criterios de evaluación específica del módulo";
            CommonTexts[CommonTextId.header3Contents].Description.Value = "Escribe una introducción a los contenidos específica del módulo";
            CommonTexts[CommonTextId.header2Blocks].Description.Value = "Escribe una introducción a los bloques de enseñanza-aprendizaje específica del módulo";
            CommonTexts[CommonTextId.header2Activities].Description.Value = "Escribe una introducción a la programación de actividades de enseñanza-aprendizaje específica del módulo";


            Template.OnSetted += (current, next) => { Flags.Add(ref validationFlags, flagTemplate); InvokeOnUpdated(); };
            Template.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagTemplate); InvokeOnUpdated(); };

            Calendar.OnSetted += (current, next) => { Flags.Add(ref validationFlags, flagCalendar); InvokeOnUpdated(); };
            Calendar.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagCalendar); InvokeOnUpdated(); };

            WeekSchedule.OnSetted += (current, next) => { Flags.Add(ref validationFlags, flagWeekSchedule); InvokeOnUpdated(); };
            WeekSchedule.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagWeekSchedule); InvokeOnUpdated(); };

            Metodologies.OnAdded += (element) => { Flags.Add(ref validationFlags, flagMetodologies); InvokeOnUpdated(); };
            Metodologies.OnRemoved += (element) => { Flags.Add(ref validationFlags, flagMetodologies); InvokeOnUpdated(); };
            Metodologies.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagMetodologies); InvokeOnUpdated(); };

            SpaceResources.OnAdded += (element) => { Flags.Add(ref validationFlags, flagSpaceResources); InvokeOnUpdated(); };
            SpaceResources.OnRemoved += (element) => { Flags.Add(ref validationFlags, flagSpaceResources); InvokeOnUpdated(); };
            SpaceResources.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagSpaceResources); InvokeOnUpdated(); };

            MaterialResources.OnAdded += (element) => { Flags.Add(ref validationFlags, flagMaterialResources); InvokeOnUpdated(); };
            MaterialResources.OnRemoved += (element) => { Flags.Add(ref validationFlags, flagMaterialResources); InvokeOnUpdated(); };
            MaterialResources.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagMaterialResources); InvokeOnUpdated(); };

            EvaluationInstrumentsTypes.OnAdded += (element) => { Flags.Add(ref validationFlags, flagEvaluationInstrumentTypes); InvokeOnUpdated(); };
            EvaluationInstrumentsTypes.OnRemoved += (element) => { Flags.Add(ref validationFlags, flagEvaluationInstrumentTypes); InvokeOnUpdated(); };
            EvaluationInstrumentsTypes.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagEvaluationInstrumentTypes); InvokeOnUpdated(); };

            Citations.OnAdded += (element) => { Flags.Add(ref validationFlags, flagCitations); InvokeOnUpdated(); };
            Citations.OnRemoved += (element) => { Flags.Add(ref validationFlags, flagCitations); InvokeOnUpdated(); };
            Citations.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagCitations); InvokeOnUpdated(); };

            Blocks.OnAdded += (element) => { Flags.Add(ref validationFlags, flagBlocks); InvokeOnUpdated(); };
            Blocks.OnRemoved += (element) => { Flags.Add(ref validationFlags, flagBlocks); InvokeOnUpdated(); };
            Blocks.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagBlocks); InvokeOnUpdated(); };

            LearningResultsWeights.OnAdded += (key, element) => { Flags.Add(ref validationFlags, flagLearningResultsWeights); InvokeOnUpdated(); };
            LearningResultsWeights.OnRemoved += (key) => { Flags.Add(ref validationFlags, flagLearningResultsWeights); InvokeOnUpdated(); };
            LearningResultsWeights.OnUpdated += (key, element) => { Flags.Add(ref validationFlags, flagLearningResultsWeights); InvokeOnUpdated(); };

            CommonTexts.OnAdded += (key, element) => { Flags.Add(ref validationFlags, flagCommonTexts); InvokeOnUpdated(); };
            CommonTexts.OnRemoved += (key) => { Flags.Add(ref validationFlags, flagCommonTexts); InvokeOnUpdated(); };
            CommonTexts.OnUpdated += (key, element) => { Flags.Add(ref validationFlags, flagCommonTexts); InvokeOnUpdated(); };
            CommonTexts.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagCommonTexts); InvokeOnUpdated(); };

            Flags.Add(ref validationFlags, flagTemplate);
            Flags.Add(ref validationFlags, flagCalendar);
            Flags.Add(ref validationFlags, flagWeekSchedule);
            Flags.Add(ref validationFlags, flagMetodologies);
            Flags.Add(ref validationFlags, flagSpaceResources);
            Flags.Add(ref validationFlags, flagMaterialResources);
            Flags.Add(ref validationFlags, flagEvaluationInstrumentTypes);
            Flags.Add(ref validationFlags, flagCitations);
            Flags.Add(ref validationFlags, flagBlocks);
            Flags.Add(ref validationFlags, flagLearningResultsWeights);
            Flags.Add(ref validationFlags, flagCommonTexts);

        }

        public override ValidationResult Validate(bool force = false)
        {
            base.Validate(force);

            if(Flags.Test(validationFlags, flagTemplate) || force)
            {
                Utils.Log("Checking linked to valid template", "template");

                validationFails.RemoveAll(e => e.code == ValidationCode.subjectNotLinkedToTemplate);
                validationFails.RemoveAll(e => e.code == ValidationCode.subjectTemplateInvalid);

                if (Template.Value == null) { validationFails.Add(ValidationResult.Create(ValidationCode.subjectNotLinkedToTemplate)); }
                else if (Template.Value.Validate(force).code != ValidationCode.success) { validationFails.Add(ValidationResult.Create(ValidationCode.subjectTemplateInvalid)); }

            }

            if(Flags.Test(validationFlags, flagCalendar) || force)
            {
                Utils.Log("Checking linked to valid calendar", "calendar");

                validationFails.RemoveAll(e => e.code == ValidationCode.subjectNotLinkedToCalendar);
                validationFails.RemoveAll(e => e.code == ValidationCode.subjectCalendarInvalid);

                if (Calendar.Value == null) { validationFails.Add(ValidationResult.Create(ValidationCode.subjectNotLinkedToCalendar)); }
                else if (Calendar.Value.Validate(force).code != ValidationCode.success) { validationFails.Add(ValidationResult.Create(ValidationCode.subjectCalendarInvalid)); }
            }

            if(Flags.Test(validationFlags, flagWeekSchedule) || force)
            {
                Utils.Log("Checking linked to valid week schedule", "weekSchedule");

                validationFails.RemoveAll(e => e.code == ValidationCode.subjectNotLinkedToWeekSchedule);
                validationFails.RemoveAll(e => e.code == ValidationCode.subjectWeekScheduleInvalid);

                if (WeekSchedule.Value == null) { validationFails.Add(ValidationResult.Create(ValidationCode.subjectNotLinkedToWeekSchedule)); }
                else if (WeekSchedule.Value.Validate(force).code != ValidationCode.success) { validationFails.Add(ValidationResult.Create(ValidationCode.subjectWeekScheduleInvalid)); }
            }

            if(Flags.Test(validationFlags, flagMetodologies) || force)
            {
                Utils.Log("Checking has at least one metodology and all metodologies are valid", "metodologies");

                validationFails.RemoveAll(e => e.code == ValidationCode.subjectNoMetodologies);
                validationFails.RemoveAll(e => e.code == ValidationCode.subjectMetodologiesInvalid);

                List<CommonText> metodologiesList = Metodologies.ToList();
                if (metodologiesList.Count <= 0) { validationFails.Add(ValidationResult.Create(ValidationCode.subjectNoMetodologies)); }
                for (int i = 0; i < metodologiesList.Count; i++) { if (metodologiesList[i].Validate(force).code != ValidationCode.success) { validationFails.Add(ValidationResult.Create(ValidationCode.subjectMetodologiesInvalid).WithIndex(i)); } }
            }

            if(Flags.Test(validationFlags, flagSpaceResources) || force)
            {
                Utils.Log("Checking has at least one space resource and all space resources are valid", "spaceResources");

                validationFails.RemoveAll(e => e.code == ValidationCode.subjectNoSpaces);
                validationFails.RemoveAll(e => e.code == ValidationCode.subjectSpaceResourceInvalid);

                List<CommonText> spacesList = SpaceResources.ToList();
                if (spacesList.Count <= 0) { validationFails.Add(ValidationResult.Create(ValidationCode.subjectNoSpaces)); }
                for (int i = 0; i < spacesList.Count; i++) { if (spacesList[i].Validate(force).code != ValidationCode.success) { validationFails.Add(ValidationResult.Create(ValidationCode.subjectSpaceResourceInvalid).WithIndex(i)); } }
            }

            if(Flags.Test(validationFlags, flagMaterialResources) || force)
            {
                Utils.Log("Checking all material resources are valid", "materialResources");

                validationFails.RemoveAll(e => e.code == ValidationCode.subjectMaterialResourceInvalid);

                List<CommonText> materialsList = MaterialResources.ToList();
                for (int i = 0; i < materialsList.Count; i++) { if (materialsList[i].Validate(force).code != ValidationCode.success) { validationFails.Add(ValidationResult.Create(ValidationCode.subjectMaterialResourceInvalid).WithIndex(i)); } }
            }

            if(Flags.Test(validationFlags, flagEvaluationInstrumentTypes) || force)
            {
                Utils.Log("Checking has at least one evaluation instrument type and all evaluation instrument types are valid", "evaluationInstrumentTypes");

                validationFails.RemoveAll(e => e.code == ValidationCode.subjectNoEvaluationInstrumentTypes);
                validationFails.RemoveAll(e => e.code == ValidationCode.subjectInstrumentTypeInvalid);

                List<CommonText> instrumentsList = EvaluationInstrumentsTypes.ToList();
                if (instrumentsList.Count <= 0) { validationFails.Add(ValidationResult.Create(ValidationCode.subjectNoEvaluationInstrumentTypes)); }
                for (int i = 0; i < instrumentsList.Count; i++) { if (instrumentsList[i].Validate(force).code != ValidationCode.success) { validationFails.Add(ValidationResult.Create(ValidationCode.subjectInstrumentTypeInvalid).WithIndex(i)); } }
            }

            if(Flags.Test(validationFlags, flagCitations) || force)
            {
                Utils.Log("Checking all citations are valid", "materialResources");

                validationFails.RemoveAll(e => e.code == ValidationCode.subjectCitationInvalid);

                List<CommonText> citationsList = Citations.ToList();
                for (int i = 0; i < citationsList.Count; i++) { if (citationsList[i].Validate(force).code != ValidationCode.success) { validationFails.Add(ValidationResult.Create(ValidationCode.subjectCitationInvalid).WithIndex(i)); } }
            }

            if(Flags.Test(validationFlags, flagBlocks | flagCalendar | flagWeekSchedule | flagLearningResultsWeights) || force)
            {
                Utils.Log("Checking has at least one block and all blocks are valid", "blocks");

                validationFails.RemoveAll(e => e.code == ValidationCode.subjectNoBlocks);
                validationFails.RemoveAll(e => e.code == ValidationCode.subjectBlockInvalid);

                List<Block> blocksList = Blocks.ToList();
                if (blocksList.Count <= 0) { validationFails.Add(ValidationResult.Create(ValidationCode.subjectNoBlocks)); }
                for (int i = 0; i < blocksList.Count; i++) { if (blocksList[i].Validate(force).code != ValidationCode.success) { validationFails.Add(ValidationResult.Create(ValidationCode.subjectBlockInvalid).WithIndex(i)); } }
            }


            if(Flags.Test(validationFlags, flagLearningResultsWeights) || force)
            {
                Utils.Log("Checking the sum of all learning results' weights is 100", "learningResultsWeights");

                validationFails.RemoveAll(e => e.code == ValidationCode.subjectLearningResultsWeightNotHundredPercent);

                List<KeyValuePair<LearningResult, float>> learningResultsWeightsList = LearningResultsWeights.ToList();

                float sum = 0;
                for (int i = 0; i < learningResultsWeightsList.Count; i++)
                {
                    KeyValuePair<LearningResult, float> r = learningResultsWeightsList[i];
                    sum += r.Value;
                }

                if (sum != 100) { validationFails.Add(ValidationResult.Create(ValidationCode.subjectLearningResultsWeightNotHundredPercent)); }
            }

            if (Flags.Test(validationFlags, flagTemplate | flagBlocks | flagLearningResultsWeights) || force)
            {
                Utils.Log("Checking all learning results are referenced by activities", "template,blocks");
                Utils.Log("Checking activities learning results' weights sum 100 for all learning results", "template,blocks");

                validationFails.RemoveAll(e => e.code == ValidationCode.subjectLearningResultNotReferencedByActivities);
                validationFails.RemoveAll(e => e.code == ValidationCode.subjectActivitiesLearningResultWeightNotHundredPercent);

                if(Template.Value != null)
                {
                    List<LearningResult> learningResultsList = Template.Value.LearningResults.ToList();
                    HashSet<string> referencedLearningResults = new();
                    Dictionary<string, float> learningResultsWeights = new();

                    List<Block> blocksList = Blocks.ToList();

                    foreach (Block b in blocksList)
                    {
                        List<Activity> activitiesList = b.Activities.ToList();

                        foreach (Activity a in activitiesList)
                        {
                            List<CommonText> criteriasList = a.Criterias.ToList();

                            criteriasList.ForEach(c => referencedLearningResults.Add(Storage.FindParentStorageId(c.StorageId, c.StorageClassId)));

                            if (a.EvaluationType.Value != ActivityEvaluationType.NotEvaluable)
                            {
                                List<KeyValuePair<LearningResult, float>> weightsList = a.LearningResultsWeights.ToList();

                                weightsList.ForEach(
                                    r =>
                                    {
                                        if (!learningResultsWeights.ContainsKey(r.Key.StorageId))
                                        { learningResultsWeights.Add(r.Key.StorageId, 0); }

                                        learningResultsWeights[r.Key.StorageId] += r.Value;

                                    });
                            }
                        }
                    }

                    for (int i = 0; i < learningResultsList.Count; i++)
                    {
                        if (!referencedLearningResults.Contains(learningResultsList[i].StorageId)) { validationFails.Add(ValidationResult.Create(ValidationCode.subjectLearningResultNotReferencedByActivities).WithIndex(i)); }
                    }

                    foreach (KeyValuePair<string, float> r in learningResultsWeights)
                    {
                        int index = learningResultsList.FindIndex(r2 => r2.StorageId == r.Key);
                        if (index >= 0 && r.Value != 100) { validationFails.Add(ValidationResult.Create(ValidationCode.subjectActivitiesLearningResultWeightNotHundredPercent).WithIndex(index)); }
                    }
                }

            }

            if (Flags.Test(validationFlags, flagCalendar | flagWeekSchedule | flagBlocks) || force)
            {
                Utils.Log("Checking at least one school day exists", "calendar,weekSchedule,blocks");

                validationFails.RemoveAll(e => e.code == ValidationCode.subjectCalendarAndWeekScheduleLeaveNoSchoolDays);

                if(Calendar.Value != null && WeekSchedule.Value != null)
                {
                    bool foundSchoolDay = false;
                    DateTime d = Calendar.Value.StartDay.Value;
                    while (d <= Calendar.Value.EndDay.Value && !foundSchoolDay)
                    {
                        if (d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday)
                        {
                            if (!Calendar.Value.FreeDays.Contains(d) && WeekSchedule.Value.HoursPerWeekDay[d.DayOfWeek] > 0) { foundSchoolDay = true; }
                        }

                        if (!foundSchoolDay)
                        {
                            d = d.AddDays(1);
                        }
                    }

                    if (!foundSchoolDay) { validationFails.Add(ValidationResult.Create(ValidationCode.subjectCalendarAndWeekScheduleLeaveNoSchoolDays)); }
                }
            }

            if(Flags.Test(validationFlags, flagCommonTexts) || force)
            {
                Utils.Log("Checking all commontexts are valid", "commonTexts");

                validationFails.RemoveAll(e => e.code == ValidationCode.subjectCommonTextInvalid);

                List<KeyValuePair<CommonTextId, CommonText>> commonTexts = CommonTexts.ToList();
                for (int i = 0; i < commonTexts.Count; i++)
                {
                    if (commonTexts[i].Value.Validate(force).code != ValidationCode.success) { validationFails.Add(ValidationResult.Create(ValidationCode.subjectCommonTextInvalid).WithIndex((int)commonTexts[i].Key)); }
                }

            }

            Flags.Remove(ref validationFlags, flagTemplate);
            Flags.Remove(ref validationFlags, flagCalendar);
            Flags.Remove(ref validationFlags, flagWeekSchedule);
            Flags.Remove(ref validationFlags, flagMetodologies);
            Flags.Remove(ref validationFlags, flagSpaceResources);
            Flags.Remove(ref validationFlags, flagMaterialResources);
            Flags.Remove(ref validationFlags, flagEvaluationInstrumentTypes);
            Flags.Remove(ref validationFlags, flagCitations);
            Flags.Remove(ref validationFlags, flagBlocks);
            Flags.Remove(ref validationFlags, flagLearningResultsWeights);
            Flags.Remove(ref validationFlags, flagCommonTexts);


            foreach(ValidationResult fail in validationFails) { Utils.Log(fail.ToString() + " (" + fail.index + ")", "FAILED"); }

            if(validationFails.Count == 0) { return ValidationResult.Create(ValidationCode.success); }
            else { return validationFails[0]; }
        }

        public override bool Exists(string storageId, string? parentStorageId)
        {
            return Storage.ExistsData<SubjectData>(storageId, StorageClassId, parentStorageId);
        }

        public override void Save(string? parentStorageId = null)
        {
            base.Save(parentStorageId);

            SubjectData data = new();

            data.Title = Title.Value;
            data.Description = Description.Value;

            data.SubjectTemplateWeakStorageId = Template.Value?.StorageId;
            data.CalendarWeakStorageId = Calendar.Value?.StorageId;
            data.WeekScheduleWeakStorageId = WeekSchedule.Value?.StorageId;

            List<CommonText> list = Metodologies.ToList();
            list.ForEach(e => e.Save(StorageId));
            data.MetodologiesStorageIds = Storage.GetStorageIds<CommonText>(list);

            list = SpaceResources.ToList();
            list.ForEach(e => e.Save(StorageId));
            data.SpaceResourcesStorageIds = Storage.GetStorageIds<CommonText>(list);

            list = MaterialResources.ToList();
            list.ForEach(e => e.Save(StorageId));
            data.MaterialResourcesStorageIds = Storage.GetStorageIds<CommonText>(list);

            list = EvaluationInstrumentsTypes.ToList();
            list.ForEach(e => e.Save(StorageId));
            data.EvaluationInstrumentsTypesStorageIds = Storage.GetStorageIds<CommonText>(list);

            list = Citations.ToList();
            list.ForEach(e => e.Save(StorageId));
            data.CitationsStorageIds = Storage.GetStorageIds<CommonText>(list);

            List<Block> blockList = Blocks.ToList();
            blockList.ForEach(e => e.Save(StorageId));
            data.BlocksStorageIds = Storage.GetStorageIds<Block>(blockList);

            List<KeyValuePair<LearningResult, float>> resultsList = LearningResultsWeights.ToList();
            List<KeyValuePair<string, float>> resultsWithIds = new();
            foreach (var r in resultsList) { resultsWithIds.Add(KeyValuePair.Create<string, float>(r.Key.StorageId, r.Value)); }
            data.LearningResultsWeakStorageIdsWeights = resultsWithIds;

            List<KeyValuePair<CommonTextId, CommonText>> commonTextList = CommonTexts.ToList();
            commonTextList.ForEach(e => { e.Value.Save(StorageId); data.CommonTextsStorageIds.Add(e.Key, e.Value.StorageId); });

            Storage.SaveData<SubjectData>(StorageId, StorageClassId, data, parentStorageId);

        }

        public override void LoadOrCreate(string storageId, string? parentStorageId = null)
        {
            base.LoadOrCreate(storageId, parentStorageId);

            if (!Storage.ExistsData<SubjectData>(storageId, StorageClassId, parentStorageId)) { Save(parentStorageId); }

            SubjectData data = Storage.LoadData<SubjectData>(storageId, StorageClassId, parentStorageId);

            Title.Value = data.Title;
            Description.Value = data.Description;

            Template.Value = data.SubjectTemplateWeakStorageId != null ? Storage.FindEntity<SubjectTemplate>(data.SubjectTemplateWeakStorageId, null) : null;
            Calendar.Value = data.CalendarWeakStorageId != null ? Storage.FindEntity<Calendar>(data.CalendarWeakStorageId, null) : null;
            WeekSchedule.Value = data.WeekScheduleWeakStorageId != null ? Storage.FindEntity<WeekSchedule>(data.WeekScheduleWeakStorageId, null) : null;

            Metodologies.Set(Storage.LoadOrCreateEntities<CommonText>(data.MetodologiesStorageIds, storageId));

            SpaceResources.Set(Storage.LoadOrCreateEntities<CommonText>(data.SpaceResourcesStorageIds, storageId));
            MaterialResources.Set(Storage.LoadOrCreateEntities<CommonText>(data.MaterialResourcesStorageIds, storageId));

            EvaluationInstrumentsTypes.Set(Storage.LoadOrCreateEntities<CommonText>(data.EvaluationInstrumentsTypesStorageIds, storageId));

            Citations.Set(Storage.LoadOrCreateEntities<CommonText>(data.CitationsStorageIds, storageId));

            Blocks.Set(Storage.LoadOrCreateEntities<Block>(data.BlocksStorageIds, storageId));

            if(Template.Value != null)
            {
                List<KeyValuePair<string, float>> resultsWithIds = data.LearningResultsWeakStorageIdsWeights;
                List<KeyValuePair<LearningResult, float>> resultsList = new();
                foreach (var r in resultsWithIds)
                {
                    LearningResult? result = Storage.FindChildEntity<LearningResult>(r.Key);
                    if (result != null) { resultsList.Add(new KeyValuePair<LearningResult, float>(result, r.Value)); }
                }
                LearningResultsWeights.Set(resultsList);
            }

            foreach (KeyValuePair<CommonTextId, string> keyValue in data.CommonTextsStorageIds)
            { CommonTexts.Set(keyValue.Key, Storage.LoadOrCreateEntity<CommonText>(keyValue.Value, storageId)); }

        }

        public override void Delete(string? parentStorageId = null)
        {
            base.Delete(parentStorageId);

            Metodologies.ToList().ForEach(e => e.Delete(StorageId));
            SpaceResources.ToList().ForEach(e => e.Delete(StorageId));
            MaterialResources.ToList().ForEach(e => e.Delete(StorageId));
            EvaluationInstrumentsTypes.ToList().ForEach(e => e.Delete(StorageId));
            Citations.ToList().ForEach(e => e.Delete(StorageId));

            Blocks.ToList().ForEach(e => e.Delete(StorageId));

            CommonTexts.ToList().ForEach(e => e.Value.Delete(StorageId));

            Storage.DeleteData(StorageId, StorageClassId, parentStorageId);
        }



    }
}
