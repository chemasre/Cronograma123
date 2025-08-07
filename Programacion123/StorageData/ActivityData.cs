namespace Programacion123
{
    public class ActivityData : StorageData
    {
        public ActivityStartType StartType { get; set; }
        public DateTime StartDate { get; set; }
        public DayOfWeek StartDayOfWeek { get; set; }

        public float Duration { get; set; }
        public bool NoActivitiesBefore { get; set; }
        public bool NoActivitiesAfter { get; set; }

        public string? MetodologyWeakStorageId { get; set; } = null;

        public List<string> ContentPointsWeakStorageIds  { get; set; } = new List<string>();
        public List<string> KeyCompetencesWeakStorageIds  { get; set; } = new List<string>();
        public List<string> SpaceResourcesWeakStorageIds  { get; set; } = new List<string>();
        public List<string> MaterialResourcesWeakStorageIds  { get; set; } = new List<string>();

        public bool IsEvaluable { get; set; }

        public string? EvaluationInstrumentTypeWeakStorageId  { get; set; } = null;
        public List<string> CriteriasWeakStorageIds  { get; set; } = new List<string>();

        public List< KeyValuePair<string, float> > LearningResultsWeakStorageIdsWeights { get; set; } = new List<KeyValuePair<string, float>>();

    }
}
