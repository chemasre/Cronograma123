namespace Programacion123
{
    public class Subject : Entity
    {
        public SubjectTemplate? Template { get; set; }
        public Calendar? Calendar { get; set; }
        public WeekSchedule? WeekSchedule { get; set; }

        public CommonText MetodologiesIntroduction { get; set; } = new CommonText();
        public ListProperty<CommonText> Metodologies { get; } = new ListProperty<CommonText>();
        public CommonText ResourcesIntroduction { get; set; } = new CommonText();
        public ListProperty<CommonText> SpaceResources { get; } = new ListProperty<CommonText>();
        public ListProperty<CommonText> MaterialResources { get; } = new ListProperty<CommonText>();
        public CommonText EvaluationInstrumentTypesIntroduction { get; set; } = new CommonText();
        public ListProperty<CommonText> EvaluationInstrumentsTypes { get; } = new ListProperty<CommonText>();

        public CommonText BlocksIntroduction { get; set; } = new CommonText();
        public ListProperty<Block> Blocks { get; } = new ListProperty<Block>();

        public CommonText EvaluationIntroduction { get; set; } = new CommonText();
        public DictionaryProperty<LearningResult, float> LearningResultsWeights  { get; } = new DictionaryProperty<LearningResult, float>();

        public Subject() : base()
        {
            StorageClassId = "subject";
        }

        public override ValidationResult Validate()
        {
            ValidationResult result = base.Validate();

            if (result.code != ValidationCode.success) { return result;  }

            if (Template == null) { return ValidationResult.Create(ValidationCode.subjectNotLinkedToTemplate); }
            if (Calendar == null) { return ValidationResult.Create(ValidationCode.subjectNotLinkedToCalendar); }
            if (WeekSchedule == null) { return ValidationResult.Create(ValidationCode.subjectNotLinkedToWeekSchedule);  }

            if (Template.Validate().code != ValidationCode.success) { return ValidationResult.Create(ValidationCode.subjectTemplateInvalid); }
            if (Calendar.Validate().code != ValidationCode.success) { return ValidationResult.Create(ValidationCode.subjectCalendarInvalid); }
            if (WeekSchedule.Validate().code != ValidationCode.success) { return ValidationResult.Create(ValidationCode.subjectWeekScheduleInvalid); }

            if (MetodologiesIntroduction.Validate().code != ValidationCode.success) { return ValidationResult.Create(ValidationCode.subjectMetodologiesIntroductionInvalid); }

            List<CommonText> metodologiesList = Metodologies.ToList();
            if (metodologiesList.Count <= 0) { return ValidationResult.Create(ValidationCode.subjectNoMetodologies);  }
            for(int i = 0; i < metodologiesList.Count; i++) { if (metodologiesList[i].Validate().code != ValidationCode.success) { return ValidationResult.Create(ValidationCode.subjectMetodologiesInvalid).WithIndex(i); } }

            if (ResourcesIntroduction.Validate().code != ValidationCode.success) { return ValidationResult.Create(ValidationCode.subjectResourcesIntroductionInvalid); }

            List<CommonText> spacesList = SpaceResources.ToList();
            if (spacesList.Count <= 0) { return ValidationResult.Create(ValidationCode.subjectNoSpaces); }
            for (int i = 0; i < spacesList.Count; i++) { if (spacesList[i].Validate().code != ValidationCode.success) { return ValidationResult.Create(ValidationCode.subjectSpaceResourceInvalid).WithIndex(i); } }

            List<CommonText> materialsList = MaterialResources.ToList();
            for(int i = 0; i < materialsList.Count; i++) { if (materialsList[i].Validate().code != ValidationCode.success) { return ValidationResult.Create(ValidationCode.subjectMaterialResourceInvalid).WithIndex(i); } }

            if (EvaluationInstrumentTypesIntroduction.Validate().code != ValidationCode.success) { return ValidationResult.Create(ValidationCode.subjectEvaluationInstrumentTypesIntroductionInvalid); }

            List<CommonText> instrumentsList = EvaluationInstrumentsTypes.ToList();
            if (instrumentsList.Count <= 0) { return ValidationResult.Create(ValidationCode.subjectNoEvaluationInstrumentTypes); }
            for (int i = 0; i < instrumentsList.Count; i++) { if (instrumentsList[i].Validate().code != ValidationCode.success) { return ValidationResult.Create(ValidationCode.subjectInstrumentTypeInvalid).WithIndex(i); } }

            if (BlocksIntroduction.Validate().code != ValidationCode.success) { return ValidationResult.Create(ValidationCode.subjectBlocksIntroductionInvalid); }

            List<Block> blocksList = Blocks.ToList();
            if (blocksList.Count <= 0) { return ValidationResult.Create(ValidationCode.subjectNoBlocks); }
            for (int i = 0; i < blocksList.Count; i++) { if (blocksList[i].Validate().code != ValidationCode.success) { return ValidationResult.Create(ValidationCode.subjectBlockInvalid).WithIndex(i); } }

            if (EvaluationIntroduction.Validate().code != ValidationCode.success) { return ValidationResult.Create(ValidationCode.subjectEvaluationIntroductionInvalid); }

            List<KeyValuePair<LearningResult, float>> learningResultsWeightsList = LearningResultsWeights.ToList();

            float sum = 0;
            for(int i = 0; i < learningResultsWeightsList.Count; i++)
            {
                KeyValuePair<LearningResult, float> r = learningResultsWeightsList[i];
                //if (r.Value <= 0) { return ValidationResult.Create(ValidationCode.subjectLearningResultWeightInvalid).WithIndex(i);  }
                sum += r.Value;
            }

            if (sum != 100) { return ValidationResult.Create(ValidationCode.subjectLearningResultsWeightNotHundredPercent); }

            List<LearningResult> learningResultsList = Template.LearningResults.ToList();
            HashSet<string> referencedLearningResults = new();
            Dictionary<string, float> learningResultsWeights = new();

            foreach(Block b in blocksList)
            {
                List<Activity> activitiesList = b.Activities.ToList();

                foreach(Activity a in activitiesList)
                {
                    List<CommonText> criteriasList = a.Criterias.ToList();

                    criteriasList.ForEach(c => referencedLearningResults.Add(Storage.FindParentStorageId(c.StorageId, c.StorageClassId)));

                    if(a.IsEvaluable)
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

            for(int i = 0; i < learningResultsList.Count; i++)
            {
                if (!referencedLearningResults.Contains(learningResultsList[i].StorageId)) { return ValidationResult.Create(ValidationCode.subjectLearningResultNotReferencedByActivities).WithIndex(i);  }
            }

            foreach (KeyValuePair<string, float> r in learningResultsWeights)
            {
                int index = learningResultsList.FindIndex(r2 => r2.StorageId == r.Key);
                if (index >= 0 && r.Value != 100) { return ValidationResult.Create(ValidationCode.subjectActivitiesLearningResultWeightNotHundredPercent).WithIndex(index);  }
            }


            return ValidationResult.Create(ValidationCode.success);
        }

        public override bool Exists(string storageId, string? parentStorageId)
        {
            return Storage.ExistsData<SubjectData>(storageId, StorageClassId, parentStorageId);
        }

        public override void Save(string? parentStorageId = null)
        {
            base.Save(parentStorageId);

            SubjectData data = new();

            data.Title = Title;
            data.Description = Description;

            data.SubjectTemplateWeakStorageId = Template?.StorageId;
            data.CalendarWeakStorageId = Calendar?.StorageId;
            data.WeekScheduleWeakStorageId = Calendar?.StorageId;


            data.MetodologiesIntroductionStorageId = MetodologiesIntroduction.StorageId;
            MetodologiesIntroduction.Save(StorageId);

            List<CommonText> list = Metodologies.ToList();
            list.ForEach(e => e.Save(StorageId));            
            data.MetodologiesStorageIds = Storage.GetStorageIds<CommonText>(list);

            data.ResourcesIntroductionStorageId = ResourcesIntroduction.StorageId;
            ResourcesIntroduction.Save(StorageId);

            list = SpaceResources.ToList();
            list.ForEach(e => e.Save(StorageId));            
            data.SpaceResourcesStorageIds = Storage.GetStorageIds<CommonText>(list);

            list = MaterialResources.ToList();
            list.ForEach(e => e.Save(StorageId));            
            data.MaterialResourcesStorageIds = Storage.GetStorageIds<CommonText>(list);

            data.EvaluationInstrumentsTypesIntroductionStorageId = EvaluationInstrumentTypesIntroduction.StorageId;
            EvaluationInstrumentTypesIntroduction.Save(StorageId);

            list = EvaluationInstrumentsTypes.ToList();
            list.ForEach(e => e.Save(StorageId));            
            data.EvaluationInstrumentsTypesStorageIds = Storage.GetStorageIds<CommonText>(list);

            data.BlocksIntroductionStorageId = BlocksIntroduction.StorageId;
            BlocksIntroduction.Save(StorageId);


            List<Block> blockList = Blocks.ToList();
            blockList.ForEach(e => e.Save(StorageId));            
            data.BlocksStorageIds = Storage.GetStorageIds<Block>(blockList);

            data.EvaluationIntroductionStorageId = EvaluationIntroduction.StorageId;
            EvaluationIntroduction.Save(StorageId);

            List< KeyValuePair<LearningResult, float> > resultsList = LearningResultsWeights.ToList();
            List< KeyValuePair<string, float> > resultsWithIds = new();
            foreach(var r in resultsList) { resultsWithIds.Add(KeyValuePair.Create<string, float>(r.Key.StorageId, r.Value)); }
            data.LearningResultsWeakStorageIdsWeights = resultsWithIds;            

            Storage.SaveData<SubjectData>(StorageId, StorageClassId, data, parentStorageId);

        }

        public override void LoadOrCreate(string storageId, string? parentStorageId = null)
        {
            base.LoadOrCreate(storageId, parentStorageId);

            if(!Storage.ExistsData<SubjectData>(storageId, StorageClassId, parentStorageId)) { Save(parentStorageId); }

            SubjectData data = Storage.LoadData<SubjectData>(storageId, StorageClassId, parentStorageId);
            
            Title = data.Title;
            Description = data.Description;

            Template = data.SubjectTemplateWeakStorageId != null ? Storage.LoadOrCreateEntity<SubjectTemplate>(data.SubjectTemplateWeakStorageId, null) : null;
            Calendar = data.CalendarWeakStorageId != null ? Storage.LoadOrCreateEntity<Calendar>(data.CalendarWeakStorageId, null) : null;
            WeekSchedule = data.WeekScheduleWeakStorageId != null ? Storage.LoadOrCreateEntity<WeekSchedule>(data.WeekScheduleWeakStorageId, null) : null;

            MetodologiesIntroduction = Storage.LoadOrCreateEntity<CommonText>(data.MetodologiesIntroductionStorageId, storageId);
            Metodologies.Set(Storage.LoadOrCreateEntities<CommonText>(data.MetodologiesStorageIds, storageId));

            ResourcesIntroduction = Storage.LoadOrCreateEntity<CommonText>(data.ResourcesIntroductionStorageId, storageId);
            SpaceResources.Set(Storage.LoadOrCreateEntities<CommonText>(data.SpaceResourcesStorageIds, storageId));
            MaterialResources.Set(Storage.LoadOrCreateEntities<CommonText>(data.MaterialResourcesStorageIds, storageId));

            EvaluationInstrumentTypesIntroduction = Storage.LoadOrCreateEntity<CommonText>(data.EvaluationInstrumentsTypesIntroductionStorageId, storageId);
            EvaluationInstrumentsTypes.Set(Storage.LoadOrCreateEntities<CommonText>(data.EvaluationInstrumentsTypesStorageIds, storageId));

            BlocksIntroduction = Storage.LoadOrCreateEntity<CommonText>(data.BlocksIntroductionStorageId, storageId);
            Blocks.Set(Storage.LoadOrCreateEntities<Block>(data.BlocksStorageIds, storageId));

            EvaluationIntroduction = Storage.LoadOrCreateEntity<CommonText>(data.EvaluationIntroductionStorageId, storageId);

            List< KeyValuePair<string, float> > resultsWithIds = data.LearningResultsWeakStorageIdsWeights;
            List< KeyValuePair<LearningResult, float> > resultsList = new();
            foreach(var r in resultsWithIds)
            {   LearningResult? result = Storage.FindChildEntity<LearningResult>(r.Key);
                if(result != null) { resultsList.Add(new KeyValuePair<LearningResult, float>(result, r.Value)); }
            }
            LearningResultsWeights.Set(resultsList);
        }

        public override void Delete(string? parentStorageId = null)
        {
            base.Delete(parentStorageId);

            MetodologiesIntroduction.Delete(StorageId);
            Metodologies.ToList().ForEach(e => e.Delete(StorageId));
            ResourcesIntroduction.Delete(StorageId);
            SpaceResources.ToList().ForEach(e => e.Delete(StorageId));
            MaterialResources.ToList().ForEach(e => e.Delete(StorageId));
            EvaluationInstrumentTypesIntroduction.Delete(StorageId);
            EvaluationInstrumentsTypes.ToList().ForEach(e => e.Delete(StorageId));

            BlocksIntroduction.Delete(StorageId);
            Blocks.ToList().ForEach(e => e.Delete(StorageId));

            EvaluationIntroduction.Delete(StorageId);

            Storage.DeleteData(StorageId, StorageClassId, parentStorageId);
        }
    }
}
