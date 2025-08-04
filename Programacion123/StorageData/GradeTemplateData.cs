namespace Programacion123
{
    public class GradeTemplateData : StorageData
    {
        public string GradeName { get; set; } = "Nombre completo del ciclo";
        public GradeType GradeType { get; set; } = GradeType.superior;
        public string GradeFamilyName { get; set; } = "Nombre de la familia profesional";
        public List<string> GeneralObjectivesStorageIds { get; set; } = new List<string>();
        public List<string> GeneralCompetencesStorageIds { get; set; } = new List<string>();
        public List<string> KeyCapacitiesStorageIds { get; set; } = new List<string>();
        public Dictionary<GradeCommonTextId, string> CommonTextsStorageIds { get; set; } = new Dictionary<GradeCommonTextId, string>();
    }
}
