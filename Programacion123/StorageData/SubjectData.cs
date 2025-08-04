namespace Programacion123
{
    public class SubjectData : StorageData
    {
        public string? SubjectTemplateWeakStorageId { get; set; } = null;
        public string? CalendarWeakStorageId { get; set; } = null;
        public string? WeekScheduleWeakStorageId { get; set; } = null;

        public List<string> MetodologiesStorageIds  { get; set; } = new List<string>();

        public List<string> SpaceResourcesStorageIds  { get; set; } = new List<string>();
        public List<string> MaterialResourcesStorageIds { get; set; } = new List<string>();

        public List<string> EvaluationInstrumentsTypesStorageIds  { get; set; } = new List<string>();

        public List<string> BlocksStorageIds  { get; set; } = new List<string>();

        public List< KeyValuePair<string, float> > LearningResultsWeakStorageIdsWeights { get; set; } = new List<KeyValuePair<string, float>>();

        public Dictionary<SubjectCommonTextId, string> CommonTextsStorageIds { get; set; } = new Dictionary<SubjectCommonTextId, string>();

    }
}
