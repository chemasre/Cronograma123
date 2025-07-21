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

        public DictionaryProperty<LearningResult, float> LearningResultsWeights  { get; } = new DictionaryProperty<LearningResult, float>();

        public Subject() : base()
        {
            StorageClassId = "subject";
        }

        public override ValidationResult Validate()
        {
            ValidationResult result = base.Validate();

            if (result != ValidationResult.success) { return result;  }

            if (Template == null) { return ValidationResult.subjectNotLinkedToTemplate; }
            if (Calendar == null) { return ValidationResult.subjectNotLinkedToCalendar; }
            if (WeekSchedule == null) { return ValidationResult.subjectNotLinkedToWeekSchedule;  }

            if (Template.Validate() != ValidationResult.success) { return ValidationResult.subjectTemplateInvalid; }
            if (Calendar.Validate() != ValidationResult.success) { return ValidationResult.subjectCalendarInvalid; }
            if (WeekSchedule.Validate() != ValidationResult.success) { return ValidationResult.subjectWeekScheduleInvalid; }

            if (MetodologiesIntroduction.Validate() != ValidationResult.success) { return ValidationResult.subjectMetodologiesIntroductionInvalid; }

            List<CommonText> metodologiesList = Metodologies.ToList();
            foreach (CommonText m in metodologiesList) { if (m.Validate() != ValidationResult.success) { return ValidationResult.subjectMetodologiesInvalid; } }

            if (ResourcesIntroduction.Validate() != ValidationResult.success) { return ValidationResult.subjectResourcesIntroductionInvalid; }

            List<CommonText> spacesList = SpaceResources.ToList();
            foreach (CommonText s in spacesList) { if (s.Validate() != ValidationResult.success) { return ValidationResult.subjectSpaceResourceInvalid; } }

            List<CommonText> materialsList = MaterialResources.ToList();
            foreach (CommonText m in materialsList) { if (m.Validate() != ValidationResult.success) { return ValidationResult.subjectMaterialResourceInvalid; } }

            if (EvaluationInstrumentTypesIntroduction.Validate() != ValidationResult.success) { return ValidationResult.subjectEvaluationInstrumentTypesIntroductionInvalid; }

            List<CommonText> instrumentsList = EvaluationInstrumentsTypes.ToList();
            foreach (CommonText i in instrumentsList) { if (i.Validate() != ValidationResult.success) { return ValidationResult.subjectInstrumentTypeInvalid; } }

            if (BlocksIntroduction.Validate() != ValidationResult.success) { return ValidationResult.subjectBlocksIntroductionInvalid; }

            List<Block> blocksList = Blocks.ToList();
            foreach (Block b in blocksList) { if (b.Validate() != ValidationResult.success) { return ValidationResult.subjectBlockInvalid; } }

            List<KeyValuePair<LearningResult, float>> learningResultsWeightsList = LearningResultsWeights.ToList();

            float sum = 0;
            foreach (KeyValuePair<LearningResult, float> r in learningResultsWeightsList)
            {
                if (r.Value <= 0) { return ValidationResult.subjectLearningResultWeightInvalid;  }
                sum += r.Value;
            }

            if (sum != 100) { return ValidationResult.subjectLearningResultsWeightNotHundredPercent; }

            return ValidationResult.success;
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

            Storage.DeleteData(StorageId, StorageClassId, parentStorageId);
        }
    }
}
