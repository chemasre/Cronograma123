namespace Programacion123
{
    public enum ActivityStartType
    {
        AsSoonAsPossible,
        Date,
        DayOfWeek
    };

    public enum ActivityEvaluationType
    {
        NotEvaluable,
        Continous,
        Exam
    };

    public class Activity : Entity
    {
        public Property<ActivityStartType> StartType { get; } = new(ActivityStartType.AsSoonAsPossible);
        public Property<DateTime> StartDate  { get; }= new(DateTime.MinValue);
        public Property<DayOfWeek> StartDayOfWeek  { get; }= new(DayOfWeek.Monday);

        public Property<float> Duration { get; } = new(1);

        public Property<bool> NoActivitiesBefore { get; } = new(true);
        public Property<bool> NoActivitiesAfter { get; } = new(true);

        public EntityProperty<CommonText?> Metodology { get; } = new(null);

        public SetEntityProperty<CommonText> ContentPoints { get; } = new SetEntityProperty<CommonText>();
        public SetEntityProperty<CommonText> KeyCompetences { get; } = new SetEntityProperty<CommonText>();
        public SetEntityProperty<CommonText> SpaceResources { get; } = new SetEntityProperty<CommonText>();
        public SetEntityProperty<CommonText> MaterialResources { get; } = new SetEntityProperty<CommonText>();

        public Property<ActivityEvaluationType> EvaluationType { get; } = new(ActivityEvaluationType.NotEvaluable);
        public EntityProperty<CommonText?> EvaluationInstrumentType { get; } = new(null);
        public SetEntityProperty<CommonText> Criterias { get; } = new SetEntityProperty<CommonText>();

        public DictionaryProperty<LearningResult, float> LearningResultsWeights { get; } = new DictionaryProperty<LearningResult, float>();

        public const uint flagStartType                  = 1 << 2; 
        public const uint flagStartDate                  = 1 << 3; 
        public const uint flagStartDayOfWeek             = 1 << 4; 
        public const uint flagDuration                   = 1 << 5; 
        public const uint flagNoActivitiesBefore         = 1 << 6; 
        public const uint flagNoActivitiesAfter          = 1 << 7; 
        public const uint flagMetodology                 = 1 << 8; 
        public const uint flagContentPoints              = 1 << 9; 
        public const uint flagKeyCompetences             = 1 << 10; 
        public const uint flagSpaceResources             = 1 << 11; 
        public const uint flagMaterialResources          = 1 << 12; 
        public const uint flagEvaluationType             = 1 << 13; 
        public const uint flagEvaluationInstrumentType   = 1 << 14; 
        public const uint flagCriterias                  = 1 << 15; 
        public const uint flagLearningResultsWeights     = 1 << 16; 

        
        public Activity() : base()
        {
            StorageClassId = "activity";

            Title.Value = "Título de la actividad";
            Description.Value = "Descripción de la actividad";

            StartType.OnSetted += (current, next) => { Flags.Add(ref validationFlags, flagStartType); InvokeOnUpdated(); };
            StartDate.OnSetted += (current, next) => { Flags.Add(ref validationFlags, flagStartDate); InvokeOnUpdated(); };
            StartDayOfWeek.OnSetted += (current, next) => { Flags.Add(ref validationFlags, flagStartDayOfWeek); InvokeOnUpdated(); };
            Duration.OnSetted += (current, next) => { Flags.Add(ref validationFlags, flagDuration); InvokeOnUpdated(); };

            NoActivitiesBefore.OnSetted += (current, next) => { Flags.Add(ref validationFlags, flagNoActivitiesBefore); InvokeOnUpdated(); };
            NoActivitiesAfter.OnSetted += (current, next) => { Flags.Add(ref validationFlags, flagNoActivitiesAfter); InvokeOnUpdated(); };

            Metodology.OnSetted += (current, next) => { Flags.Add(ref validationFlags, flagMetodology); InvokeOnUpdated(); };
            Metodology.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagMetodology); InvokeOnUpdated(); };

            ContentPoints.OnAdded += (element) => { Flags.Add(ref validationFlags, flagContentPoints); InvokeOnUpdated(); };
            ContentPoints.OnRemoved += (element) => { Flags.Add(ref validationFlags, flagContentPoints); InvokeOnUpdated(); };
            ContentPoints.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagContentPoints); InvokeOnUpdated(); };

            KeyCompetences.OnAdded += (element) => { Flags.Add(ref validationFlags, flagKeyCompetences); InvokeOnUpdated(); };
            KeyCompetences.OnRemoved += (element) => { Flags.Add(ref validationFlags, flagKeyCompetences); InvokeOnUpdated(); };
            KeyCompetences.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagKeyCompetences); InvokeOnUpdated(); };

            SpaceResources.OnAdded += (element) => { Flags.Add(ref validationFlags, flagSpaceResources); InvokeOnUpdated(); };
            SpaceResources.OnRemoved += (element) => { Flags.Add(ref validationFlags, flagSpaceResources); InvokeOnUpdated(); };
            SpaceResources.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagSpaceResources); InvokeOnUpdated(); };

            MaterialResources.OnAdded += (element) => { Flags.Add(ref validationFlags, flagMaterialResources); InvokeOnUpdated(); };
            MaterialResources.OnRemoved += (element) => { Flags.Add(ref validationFlags, flagMaterialResources); InvokeOnUpdated(); };
            MaterialResources.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagMaterialResources); InvokeOnUpdated(); };

            EvaluationType.OnSetted += (previous, current) => { Flags.Add(ref validationFlags, flagEvaluationType); InvokeOnUpdated(); };            
            EvaluationInstrumentType.OnSetted += (previous, current) => { Flags.Add(ref validationFlags, flagEvaluationInstrumentType); InvokeOnUpdated(); };
            EvaluationInstrumentType.OnEntityUpdated += (entity) =>  { Flags.Add(ref validationFlags, flagEvaluationInstrumentType); InvokeOnUpdated(); };

            Criterias.OnAdded += (element) => { Flags.Add(ref validationFlags, flagCriterias); InvokeOnUpdated(); };
            Criterias.OnRemoved += (element) => { Flags.Add(ref validationFlags, flagCriterias); InvokeOnUpdated(); };
            Criterias.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagCriterias); InvokeOnUpdated(); };

            LearningResultsWeights.OnAdded += (key, element) => { Flags.Add(ref validationFlags, flagLearningResultsWeights); InvokeOnUpdated(); };
            LearningResultsWeights.OnRemoved += (key) => { Flags.Add(ref validationFlags, flagLearningResultsWeights); InvokeOnUpdated(); };
            LearningResultsWeights.OnUpdated += (key, element) => { Flags.Add(ref validationFlags, flagLearningResultsWeights); InvokeOnUpdated(); };

            Flags.Add(ref validationFlags, flagStartType);                  
            Flags.Add(ref validationFlags, flagStartDate);                  
            Flags.Add(ref validationFlags, flagStartDayOfWeek);          
            Flags.Add(ref validationFlags, flagDuration);                
            Flags.Add(ref validationFlags, flagNoActivitiesBefore);      
            Flags.Add(ref validationFlags, flagNoActivitiesAfter);       
            Flags.Add(ref validationFlags, flagMetodology);              
            Flags.Add(ref validationFlags, flagContentPoints);           
            Flags.Add(ref validationFlags, flagKeyCompetences);          
            Flags.Add(ref validationFlags, flagSpaceResources);         
            Flags.Add(ref validationFlags, flagMaterialResources);       
            Flags.Add(ref validationFlags, flagEvaluationType);          
            Flags.Add(ref validationFlags, flagEvaluationInstrumentType);
            Flags.Add(ref validationFlags, flagCriterias);               
            Flags.Add(ref validationFlags, flagLearningResultsWeights);  
            
            //validationDependencies[ValidationCode.activityNotLinkedToMetodology]                           = flagMetodology;
            //validationDependencies[ValidationCode.activityNotLinkedToContents]                             = flagContentPoints;
            //validationDependencies[ValidationCode.activityNotLinkedToKeyCompetences]                       = flagKeyCompetences;
            //validationDependencies[ValidationCode.activityEvaluableAndNotLinkedToEvaluationInstrumentType] = flagEvaluationType | flagEvaluationInstrumentType;
            //validationDependencies[ValidationCode.activityEvaluableAndNotLinkedToCriterias]                = flagEvaluationType | flagCriterias;
            //validationDependencies[ValidationCode.activityEvaluableAndNotLinkedToResultsWeights]           = flagEvaluationType | flagLearningResultsWeights;
            //validationDependencies[ValidationCode.activityNotLinkedToSpaceResource]                        = flagSpaceResources;
            //validationDependencies[ValidationCode.activityReferencesResultWithoutWeight]                   = flagEvaluationType | flagCriterias | flagLearningResultsWeights;
            //validationDependencies[ValidationCode.activityDoesntReferenceResultButHasWeight]               = flagEvaluationType | flagCriterias | flagLearningResultsWeights;
            //validationDependencies[ValidationCode.activityCannotSchedule]                                  = flagStartType | flagStartDate | flagStartDayOfWeek;

 
        }

        public override ValidationResult Validate(bool force = false)
        {
            base.Validate(force);

            // Check metodology

            if(Flags.Test(validationFlags, flagMetodology) || force)
            {
                Utils.Log("Checking activity linked to metodology", "metodology");

                validationFails.RemoveAll(e => e.code == ValidationCode.activityNotLinkedToMetodology);
                if (Metodology.Value == null) { validationFails.Add(ValidationResult.Create(ValidationCode.activityNotLinkedToMetodology)); }
            }

            // Check content points

            if(Flags.Test(validationFlags, flagContentPoints) || force)
            {
                Utils.Log("Checking activity linked to content points", "contentPoints");

                validationFails.RemoveAll(e => e.code == ValidationCode.activityNotLinkedToContents);
                if (ContentPoints.Count <= 0) { validationFails.Add(ValidationResult.Create(ValidationCode.activityNotLinkedToContents)); }
            }

            // Check key competences

            if(Flags.Test(validationFlags, flagKeyCompetences) || force)
            {
                Utils.Log("Checking activity linked to key competences", "keyCompetences");

                validationFails.RemoveAll(e => e.code == ValidationCode.activityNotLinkedToKeyCompetences);
                if (KeyCompetences.Count <= 0) { validationFails.Add(ValidationResult.Create(ValidationCode.activityNotLinkedToKeyCompetences)); }
            }

            // Check space resources

            if(Flags.Test(validationFlags, flagSpaceResources) || force)
            {
                Utils.Log("Checking activity linked to space resource", "spaceResources");

                validationFails.RemoveAll(e => e.code == ValidationCode.activityNotLinkedToSpaceResource);
                if (SpaceResources.Count <= 0) { validationFails.Add(ValidationResult.Create(ValidationCode.activityNotLinkedToSpaceResource)); }
            }

            // Check evaluation instrument type

            if(Flags.Test(validationFlags, flagEvaluationType | flagEvaluationInstrumentType) || force)
            {
                Utils.Log("Checking evaluable activity linked to evaluation instrument", "evaluationType, evaluationInstrumentType");

                validationFails.RemoveAll(e => e.code == ValidationCode.activityEvaluableAndNotLinkedToEvaluationInstrumentType);
                if (EvaluationType.Value != ActivityEvaluationType.NotEvaluable)
                {   if (EvaluationInstrumentType.Value == null)
                    { validationFails.Add(ValidationResult.Create(ValidationCode.activityEvaluableAndNotLinkedToEvaluationInstrumentType)); }

                }
            }

            // Check criterias

            if(Flags.Test(validationFlags, flagEvaluationType | flagCriterias) || force)
            {
                Utils.Log("Checking evaluable activity linked to criterias", "evaluationType, criterias");

                validationFails.RemoveAll(e => e.code == ValidationCode.activityEvaluableAndNotLinkedToCriterias);
                if (EvaluationType.Value != ActivityEvaluationType.NotEvaluable)
                {   if (Criterias.Count <= 0)
                    { validationFails.Add(ValidationResult.Create(ValidationCode.activityEvaluableAndNotLinkedToCriterias)); }
                }
            }

            // Result weights

            if(Flags.Test(validationFlags, flagEvaluationType | flagLearningResultsWeights) || force)
            {
                Utils.Log("Checking evaluable activity linked to result weights", "evaluationType, learningResultsWeights");

                validationFails.RemoveAll(e => e.code == ValidationCode.activityEvaluableAndNotLinkedToResultsWeights);
                if (EvaluationType.Value != ActivityEvaluationType.NotEvaluable)
                {   if (LearningResultsWeights.Count <= 0)
                    { validationFails.Add(ValidationResult.Create(ValidationCode.activityEvaluableAndNotLinkedToResultsWeights)); }
                }
            }

            if(Flags.Test(validationFlags, flagEvaluationType | flagLearningResultsWeights | flagCriterias) || force)
            {
                Utils.Log("Checking learning results weights", "evaluationType, learningResultsWeights, criterias");

                validationFails.RemoveAll(e => e.code == ValidationCode.activityReferencesResultWithoutWeight);
                validationFails.RemoveAll(e => e.code == ValidationCode.activityDoesntReferenceResultButHasWeight);

                if (EvaluationType.Value != ActivityEvaluationType.NotEvaluable)
                {
                    string subjectStorageId = Storage.FindParentStorageId(Storage.FindParentStorageId(StorageId, StorageClassId), new Block().StorageClassId);
                    Subject subject = new Subject();
                    subject.LoadOrCreate(subjectStorageId);
                    SubjectTemplate? template = subject.Template.Value;
                    if(template != null)
                    {
                        HashSet<string> referencedLearningResultsIds = new();

                        List<CommonText> criteriasList = Criterias.ToList();
                        for (int i = 0; i < criteriasList.Count; i++)
                        {
                            referencedLearningResultsIds.Add(Storage.FindParentStorageId(criteriasList[i].StorageId, criteriasList[i].StorageClassId));
                        }

                        List<KeyValuePair<LearningResult, float>> learningResultsWeightsList = LearningResultsWeights.ToList();
                    
                        for (int i = 0; i < learningResultsWeightsList.Count; i++)
                        {
                            if (referencedLearningResultsIds.Contains(learningResultsWeightsList[i].Key.StorageId))
                            {
                                if (learningResultsWeightsList[i].Value <= 0)
                                {
                                    int raIndex = template.LearningResults.ToList().FindIndex(r => r.StorageId == learningResultsWeightsList[i].Key.StorageId);
                                    validationFails.Add(ValidationResult.Create(ValidationCode.activityReferencesResultWithoutWeight).WithIndex(raIndex));
                                }
                            }
                            else
                            {
                                if (learningResultsWeightsList[i].Value > 0)
                                {
                                    int raIndex = template.LearningResults.ToList().FindIndex(r => r.StorageId == learningResultsWeightsList[i].Key.StorageId);
                                    validationFails.Add(ValidationResult.Create(ValidationCode.activityDoesntReferenceResultButHasWeight).WithIndex(raIndex));
                                }
                            }
                        }

                    }

                }
            }

            // Scheduling

            if(Flags.Test(validationFlags, flagStartType | flagStartDate | flagStartDayOfWeek | flagDuration | flagNoActivitiesBefore | flagNoActivitiesAfter) || force)
            {
                Utils.Log("Checking scheduling", "startType, startDate, startDayOfWee, duration, noActivitiesBefore, noActivitiesAfter");

                validationFails.RemoveAll(e => e.code == ValidationCode.activityCannotSchedule);

                string subjectStorageId = Storage.FindParentStorageId(Storage.FindParentStorageId(StorageId, StorageClassId), new Block().StorageClassId);
                Subject subject = new Subject();
                subject.LoadOrCreate(subjectStorageId);

                if (!subject.CanScheduleActivities())
                { validationFails.Add(ValidationResult.Create(ValidationCode.activityCannotSchedule)); }
                else if (subject.ScheduleActivities().FindIndex(s => s.activity.StorageId == StorageId) < 0)
                { validationFails.Add(ValidationResult.Create(ValidationCode.activityCannotSchedule)); }
            }

            Flags.Remove(ref validationFlags, flagStartType);                  
            Flags.Remove(ref validationFlags, flagStartDate);                  
            Flags.Remove(ref validationFlags, flagStartDayOfWeek);          
            Flags.Remove(ref validationFlags, flagDuration);                
            Flags.Remove(ref validationFlags, flagNoActivitiesBefore);      
            Flags.Remove(ref validationFlags, flagNoActivitiesAfter);       
            Flags.Remove(ref validationFlags, flagMetodology);              
            Flags.Remove(ref validationFlags, flagContentPoints);           
            Flags.Remove(ref validationFlags, flagKeyCompetences);          
            Flags.Remove(ref validationFlags, flagSpaceResources);         
            Flags.Remove(ref validationFlags, flagMaterialResources);       
            Flags.Remove(ref validationFlags, flagEvaluationType);          
            Flags.Remove(ref validationFlags, flagEvaluationInstrumentType);
            Flags.Remove(ref validationFlags, flagCriterias);               
            Flags.Remove(ref validationFlags, flagLearningResultsWeights);

            // FIX: Causes an exception (randomly)
            //foreach(ValidationResult fail in validationFails) { Utils.Log(fail.ToString() + " (" + fail.index + ")", "FAILED"); }

            if(validationFails.Count == 0) { return ValidationResult.Create(ValidationCode.success); }
            else { return validationFails[0]; }

        }

        public override bool Exists(string storageId, string? parentStorageId)
        {
            return Storage.ExistsData<ActivityData>(storageId, StorageClassId, parentStorageId);
        }

        public override void Save(string? parentStorageId = null)
        {
            base.Save(parentStorageId);

            ActivityData data = new();

            data.Title = Title.Value;
            data.Description = Description.Value;
            data.StartType = StartType.Value;
            data.StartDate = StartDate.Value;
            data.StartDayOfWeek = StartDayOfWeek.Value;
            data.Duration = Duration.Value;
            data.NoActivitiesBefore = NoActivitiesBefore.Value;
            data.NoActivitiesAfter = NoActivitiesAfter.Value;


            data.MetodologyWeakStorageId = Metodology.Value?.StorageId;

            List<CommonText> list = ContentPoints.ToList();
            data.ContentPointsWeakStorageIds = Storage.GetStorageIds<CommonText>(list);

            list = KeyCompetences.ToList();
            data.KeyCompetencesWeakStorageIds = Storage.GetStorageIds<CommonText>(list);

            list = SpaceResources.ToList();
            data.SpaceResourcesWeakStorageIds = Storage.GetStorageIds<CommonText>(list);

            list = MaterialResources.ToList();
            data.MaterialResourcesWeakStorageIds = Storage.GetStorageIds<CommonText>(list);

            data.EvaluationType = EvaluationType.Value;

            data.EvaluationInstrumentTypeWeakStorageId = EvaluationInstrumentType.Value?.StorageId;

            list = Criterias.ToList();
            data.CriteriasWeakStorageIds = Storage.GetStorageIds<CommonText>(list);

            List<KeyValuePair<LearningResult, float>> resultsList = LearningResultsWeights.ToList();
            List<KeyValuePair<string, float>> resultsWithIds = new();
            foreach (var r in resultsList) { resultsWithIds.Add(KeyValuePair.Create<string, float>(r.Key.StorageId, r.Value)); }
            data.LearningResultsWeakStorageIdsWeights = resultsWithIds;

            Storage.SaveData<ActivityData>(StorageId, StorageClassId, data, parentStorageId);

        }

        public override void LoadOrCreate(string storageId, string? parentStorageId = null)
        {
            base.LoadOrCreate(storageId, parentStorageId);

            if (!Storage.ExistsData<ActivityData>(storageId, StorageClassId, parentStorageId)) { Save(parentStorageId); }

            ActivityData data = Storage.LoadData<ActivityData>(storageId, StorageClassId, parentStorageId);

            Title.Value = data.Title;
            Description.Value = data.Description;

            StartType.Value = data.StartType;
            StartDate.Value = data.StartDate;
            StartDayOfWeek.Value = data.StartDayOfWeek;
            Duration.Value = data.Duration;
            NoActivitiesBefore.Value = data.NoActivitiesBefore;
            NoActivitiesAfter.Value = data.NoActivitiesAfter;

            string subjectStorageId = Storage.FindParentStorageId(Storage.FindParentStorageId(StorageId, StorageClassId), new Block().StorageClassId);
            Metodology.Value = data.MetodologyWeakStorageId != null ? Storage.FindEntity<CommonText>(data.MetodologyWeakStorageId, subjectStorageId) : null;

            SubjectData subjectData = Storage.FindData<SubjectData>(subjectStorageId, new Subject().StorageClassId);
            SubjectTemplateData? subjectTemplateData = subjectData.SubjectTemplateWeakStorageId != null ? Storage.FindData<SubjectTemplateData>(subjectData.SubjectTemplateWeakStorageId, new SubjectTemplate().StorageClassId) : null;

            if(subjectTemplateData != null)
            {
                ContentPoints.Set(Storage.FindSiblingEntities<CommonText>(data.ContentPointsWeakStorageIds, subjectTemplateData.ContentsStorageIds));
            }

            GradeTemplateData? gradeTemplateData = subjectTemplateData?.GradeTemplateWeakStorageId != null ? Storage.FindData<GradeTemplateData>(subjectTemplateData.GradeTemplateWeakStorageId, new GradeTemplate().StorageClassId) : null;

            if(gradeTemplateData != null)
            {
                KeyCompetences.Set(Storage.FindSiblingEntities<CommonText>(data.KeyCompetencesWeakStorageIds, subjectTemplateData.GradeTemplateWeakStorageId));
            }

            SpaceResources.Set(Storage.FindSiblingEntities<CommonText>(data.SpaceResourcesWeakStorageIds, subjectStorageId));
            MaterialResources.Set(Storage.FindSiblingEntities<CommonText>(data.MaterialResourcesWeakStorageIds, subjectStorageId));

            EvaluationType.Value = data.EvaluationType;

            EvaluationInstrumentType.Value = data.EvaluationInstrumentTypeWeakStorageId != null ? Storage.FindEntity<CommonText>(data.EvaluationInstrumentTypeWeakStorageId, subjectStorageId) : null;
            
            if(subjectTemplateData != null)
            {
                Criterias.Set(Storage.FindSiblingEntities<CommonText>(data.CriteriasWeakStorageIds, subjectTemplateData.LearningResultsStorageIds));

                List<KeyValuePair<string, float>> resultsWithIds = data.LearningResultsWeakStorageIdsWeights;
                List<KeyValuePair<LearningResult, float>> resultsList = new();
                foreach (var r in resultsWithIds)
                {
                    LearningResult? result = Storage.FindEntity<LearningResult>(r.Key, subjectData.SubjectTemplateWeakStorageId);
                    if(result != null) { resultsList.Add(new KeyValuePair<LearningResult, float>(result, r.Value)); }
                }
                LearningResultsWeights.Set(resultsList);

            }


        }

        public override void Delete(string? parentStorageId = null)
        {
            base.Delete(parentStorageId);

            Storage.DeleteData(StorageId, StorageClassId, parentStorageId);

        }


    }
}
