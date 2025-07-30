using Microsoft.Office.Interop.Word;

namespace Programacion123
{
    public enum ActivityStartType
    {
        AsSoonAsPossible,
        Date,
        DayOfWeek
    };

    public class Activity: Entity
    {
        public ActivityStartType StartType { get; set; } = ActivityStartType.AsSoonAsPossible;
        public DateTime StartDate { get; set; }
        public DayOfWeek StartDayOfWeek { get; set; } = DayOfWeek.Monday;

        public float Duration { get; set; } = 1;

        public bool NoActivitiesBefore { get; set; } = true;
        public bool NoActivitiesAfter { get; set; } = true;

        public CommonText? Metodology = null;

        public SetProperty<CommonText> ContentPoints { get; } = new SetProperty<CommonText>();
        public SetProperty<CommonText> SpaceResources { get; } = new SetProperty<CommonText>();
        public SetProperty<CommonText> MaterialResources { get; } = new SetProperty<CommonText>();

        public bool IsEvaluable = false;
        public CommonText? EvaluationInstrumentType = null;
        public SetProperty<CommonText> Criterias { get; }= new SetProperty<CommonText>();

        public DictionaryProperty<LearningResult, float> LearningResultsWeights { get; } = new DictionaryProperty<LearningResult, float>();

        public Activity() : base()
        {
            Title = "Actividad sin título";
            Description = "Actividad sin descripción";

            StorageClassId = "activity";
        }

        public override ValidationResult Validate()
        {
            ValidationResult result = base.Validate();

            if(result.code != ValidationCode.success) { return result; }

            if(Metodology == null) { return ValidationResult.Create(ValidationCode.activityNotLinkedToMetodology); }

            if(ContentPoints.Count <= 0) { return ValidationResult.Create(ValidationCode.activityNotLinkedToContents); } 

            if(IsEvaluable)
            {
                if(EvaluationInstrumentType == null) { return ValidationResult.Create(ValidationCode.activityEvaluableAndNotLinkedToEvaluationInstrumentType); }

                if(Criterias.Count <= 0) { return ValidationResult.Create(ValidationCode.activityEvaluableAndNotLinkedToCriterias); }

                if(LearningResultsWeights.Count <= 0) { return ValidationResult.Create(ValidationCode.activityEvaluableAndNotLinkedToResultsWeights); }

                if (SpaceResources.Count <= 0) { return ValidationResult.Create(ValidationCode.activityNotLinkedToSpaceResource);  }

                HashSet<string> referencedLearningResultsIds = new();

                List<CommonText> criteriasList = Criterias.ToList();
                for(int i = 0; i < criteriasList.Count; i++)
                {
                    referencedLearningResultsIds.Add(Storage.FindParentStorageId(criteriasList[i].StorageId, criteriasList[i].StorageClassId));
                }

                List< KeyValuePair<LearningResult, float> > learningResultsWeightsList = LearningResultsWeights.ToList();

                for(int i = 0; i < learningResultsWeightsList.Count; i++)
                {
                    if(referencedLearningResultsIds.Contains(learningResultsWeightsList[i].Key.StorageId))
                    {
                        if(learningResultsWeightsList[i].Value <= 0) { return ValidationResult.Create(ValidationCode.activityReferencesResultWithoutWeight).WithIndex(i); }
                    }
                    else
                    {
                        if(learningResultsWeightsList[i].Value > 0) { return ValidationResult.Create(ValidationCode.activityDoesntReferenceResultButHasWeight).WithIndex(i); }
                    }
                }
            }

            return ValidationResult.Create(ValidationCode.success);

        }

        public override bool Exists(string storageId, string? parentStorageId)
        {
            return Storage.ExistsData<ActivityData>(storageId, StorageClassId, parentStorageId);
        }

        public override void Save(string? parentStorageId = null)
        {
            base.Save(parentStorageId);

            ActivityData data = new();

            data.Title = Title;
            data.Description = Description;
            data.StartType = StartType;
            data.StartDate = StartDate;
            data.StartDayOfWeek = StartDayOfWeek;
            data.Duration = Duration;
            data.NoActivitiesBefore = NoActivitiesBefore;
            data.NoActivitiesAfter = NoActivitiesAfter;


            data.MetodologyWeakStorageId = Metodology?.StorageId;

            List<CommonText> list = ContentPoints.ToList();
            data.ContentPointsWeakStorageIds = Storage.GetStorageIds<CommonText>(list);

            list = SpaceResources.ToList();
            data.SpaceResourcesWeakStorageIds = Storage.GetStorageIds<CommonText>(list);

            list = MaterialResources.ToList();
            data.MaterialResourcesWeakStorageIds = Storage.GetStorageIds<CommonText>(list);

            data.IsEvaluable = IsEvaluable;

            data.EvaluationInstrumentTypeWeakStorageId = EvaluationInstrumentType?.StorageId;

            list = Criterias.ToList();
            data.CriteriasWeakStorageIds = Storage.GetStorageIds<CommonText>(list);

            List< KeyValuePair<LearningResult, float> > resultsList = LearningResultsWeights.ToList();
            List< KeyValuePair<string, float> > resultsWithIds = new();
            foreach(var r in resultsList) { resultsWithIds.Add(KeyValuePair.Create<string, float>(r.Key.StorageId, r.Value)); }
            data.LearningResultsWeakStorageIdsWeights = resultsWithIds;

            Storage.SaveData<ActivityData>(StorageId, StorageClassId, data, parentStorageId);

        }

        public override void LoadOrCreate(string storageId, string? parentStorageId = null)
        {
            base.LoadOrCreate(storageId, parentStorageId);

            if(!Storage.ExistsData<ActivityData>(storageId, StorageClassId, parentStorageId)) { Save(parentStorageId); }

            ActivityData data = Storage.LoadData<ActivityData>(storageId, StorageClassId, parentStorageId);
            
            Title = data.Title;
            Description = data.Description;

            StartType = data.StartType;
            StartDate = data.StartDate;
            StartDayOfWeek = data.StartDayOfWeek;
            Duration = data.Duration;
            NoActivitiesBefore = data.NoActivitiesBefore;
            NoActivitiesAfter = data.NoActivitiesAfter;

            string subjectStorageId = Storage.FindParentStorageId(Storage.FindParentStorageId(StorageId, StorageClassId), new Block().StorageClassId);
            Metodology = data.MetodologyWeakStorageId != null ? Storage.FindEntity<CommonText>(data.MetodologyWeakStorageId, subjectStorageId) : null;

            ContentPoints.Set(Storage.FindChildEntities<CommonText>(data.ContentPointsWeakStorageIds));
            SpaceResources.Set(Storage.FindChildEntities<CommonText>(data.SpaceResourcesWeakStorageIds));
            MaterialResources.Set(Storage.FindChildEntities<CommonText>(data.MaterialResourcesWeakStorageIds));

            IsEvaluable = data.IsEvaluable;

            EvaluationInstrumentType = data.EvaluationInstrumentTypeWeakStorageId!= null ? Storage.FindEntity<CommonText>(data.EvaluationInstrumentTypeWeakStorageId, subjectStorageId) : null;
            Criterias.Set(Storage.FindChildEntities<CommonText>(data.CriteriasWeakStorageIds));

            List< KeyValuePair<string, float> > resultsWithIds = data.LearningResultsWeakStorageIdsWeights;
            List< KeyValuePair<LearningResult, float> > resultsList = new();
            foreach(var r in resultsWithIds)
            {   LearningResult result = Storage.FindChildEntity<LearningResult>(r.Key);
                resultsList.Add(new KeyValuePair<LearningResult, float>(result, r.Value));
            }
            LearningResultsWeights.Set(resultsList);

        }

        public override void Delete(string? parentStorageId = null)
        {
            base.Delete(parentStorageId);

            Storage.DeleteData(StorageId, StorageClassId, parentStorageId);

        }


    }
}
