namespace Programacion123
{
    public class SubjectTemplateData : StorageData
    {
        public string? GradeTemplateWeakStorageId { get; set; } = null;
        public string SubjectName { get; set; } = "Nombre completo del módulo";
        public string SubjectCode { get; set; } = "Código del módulo";
        public int GradeClassroomHours { get; set; } = 100;
        public int GradeCompanyHours { get; set; } = 50;
        public List<string> GeneralObjectivesWeakStorageIds { get; set; } = new List<string>();
        public List<string> GeneralCompetencesWeakStorageIds { get; set; } = new List<string>();
        public List<string> KeyCapacitiesWeakStorageIds { get; set; } = new List<string>();
        public List<string> LearningResultsStorageIds { get; set; } = new List<string>();
        public List<string> ContentsStorageIds { get; set; } = new List<string>();

    }
}
