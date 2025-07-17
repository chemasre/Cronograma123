namespace Programacion123
{
    public class ActivityData : StorageData
    {
        public int Hours { get; set; }

        public string? MetodologyWeakStorageId { get; set; } = null;

        public List<string> ContentPointsWeakStorageIds  { get; set; } = new List<string>();
        public List<string> SpaceResourcesWeakStorageIds  { get; set; } = new List<string>();
        public List<string> MaterialResourcesWeakStorageIds  { get; set; } = new List<string>();

        public bool IsEvaluable { get; set; }

        public List<string> CriteriasWeakStorageIds  { get; set; } = new List<string>();
        public List<string> EvaluationInstrumentTypesWeakStorageIds  { get; set; } = new List<string>();

    }
}
