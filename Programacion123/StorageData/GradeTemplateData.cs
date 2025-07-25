namespace Programacion123
{
    public class GradeTemplateData : StorageData
    {
        public string GradeName { get; set; } = "Nombre completo del ciclo";
        public GradeType GradeType { get; set; } = GradeType.superior;
        public string GradeFamilyName { get; set; } = "Nombre de la familia profesional";
        public string GeneralObjectivesIntroductionStorageId { get; set; }
        public List<string> GeneralObjectivesStorageIds { get; set; } = new List<string>();
        public string GeneralCompetencesIntroductionStorageId { get; set; }
        public List<string> GeneralCompetencesStorageIds { get; set; } = new List<string>();
        public string KeyCapacitiesIntroductionStorageId { get; set; }
        public List<string> KeyCapacitiesStorageIds { get; set; } = new List<string>();
    }
}
