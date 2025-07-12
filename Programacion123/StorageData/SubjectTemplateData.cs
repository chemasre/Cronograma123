namespace Programacion123
{
    public class SubjectTemplateData : StorageData
    {
        public string SubjectName { get; set; } = "Nombre completo del módulo";
        public string SubjectCode { get; set; } = "Código del módulo";
        public GradeType GradeType { get; set; } = GradeType.superior;
        public string GradeName { get; set; } = "Nombre completo del grado";
        public int GradeClassroomHours { get; set; } = 100;
        public int GradeCompanyHours { get; set; } = 50;
        public string GradeFamilyName { get; set; } = "Nombre de la familia profesional";
        public string GeneralObjectivesIntroductionStorageId { get; set; }
        public List<string> GeneralObjectivesStorageIds { get; set; } = new List<string>();
        public string GeneralCompetencesIntroductionStorageId { get; set; }
        public List<string> GeneralCompetencesStorageIds { get; set; } = new List<string>();
        public string LearningResultsIntroductionStorageId { get; set; }
        public List<string> LearningResultsStorageIds { get; set; } = new List<string>();
        public string ContentsIntroductionStorageId { get; set; }
        public List<string> ContentsStorageIds { get; set; } = new List<string>();
    }
}
