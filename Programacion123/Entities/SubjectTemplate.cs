using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    public class SubjectTemplate : Entity
    {
        public string Name { get; set; } = "Nombre del módulo";
        public string Code { get; set; } = "Código del módulo";
        public CommonText GeneralObjectivesIntroduction { get; set; }
        public ListProperty<CommonText> GeneralObjectives { get; } = new ListProperty<CommonText>();
        public CommonText GeneralCompetencesIntroduction { get; set; }
        public ListProperty<CommonText> GeneralCompetences { get; } = new ListProperty<CommonText>();
        public CommonText KeyCapacitiesIntroduction { get; set; }
        public ListProperty<CommonText> KeyCapacities { get; } = new ListProperty<CommonText>();
        public ListProperty<LearningResult> LearningResults { get; } = new ListProperty<LearningResult>();
        public ListProperty<Content> Contents { get; } = new ListProperty<Content>();

        internal SubjectTemplate() : base()
        {
            StorageClassId = "subjecttemplate";
        }

        public override ValidationResult Validate()
        {
            throw new NotImplementedException();
        }
    }
}
