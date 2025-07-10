namespace Programacion123
{
    public class SubjectTemplateData : StorageData
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string GeneralObjectivesIntroductionStorageId { get; set; }
        public List<string> GeneralObjectivesStorageIds { get; set; } = new List<string>();
        public List<string> GeneralCompetencesStorageIds { get; set; } = new List<string>();
    }
}
