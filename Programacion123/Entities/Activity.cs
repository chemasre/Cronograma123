namespace Programacion123
{
    public class Activity: Entity
    {
        public int Hours { get; set; }
        public ListProperty<CommonText> Metodologies;
        public SetProperty<Content> Contents;
        public CommonText ClassroomOrganization;
        public ListProperty<CommonText> Resources;

        public bool IsEvaluable;
        public SetProperty<CommonText> EvaluationCriteria;
        public DictionaryProperty<LearningResult, float> ResultsWeight { get; } = new DictionaryProperty<LearningResult, float>();

        internal Activity() : base()
        {
            StorageClassId = "activity";
        }

        public override ValidationResult Validate()
        {
            throw new NotImplementedException();
        }

        public override bool Exists(string storageId, string? parentStorageId)
        {
            return Storage.ExistsData<ActivityData>(storageId, StorageClassId, parentStorageId);
        }
    }
}
