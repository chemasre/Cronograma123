namespace Programacion123
{
    public class SubjectData : StorageData
    {
        public string? SubjectTemplateWeakStorageId { get; set; } = null;
        public string? CalendarWeakStorageId { get; set; } = null;
        public string? WeekScheduleWeakStorageId { get; set; } = null;

        public string MetodologiesIntroductionStorageId { get; set; }
        public List<string> MetodologiesStorageIds  { get; set; } = new List<string>();

        public string ResourcesIntroductionStorageId { get; set; }
        public List<string> SpaceResourcesStorageIds  { get; set; } = new List<string>();
        public List<string> MaterialResourcesStorageIds { get; set; } = new List<string>();

        public string EvaluationInstrumentsTypesIntroductionStorageId { get; set; }
        public List<string> EvaluationInstrumentsTypesStorageIds  { get; set; } = new List<string>();

        public string BlocksIntroductionStorageId { get; set; }
        public List<string> BlocksStorageIds  { get; set; } = new List<string>();

        public List< KeyValuePair<string, float> > LearningResultsWeakStorageIdsWeights { get; set; } = new List<KeyValuePair<string, float>>();

    }
}
