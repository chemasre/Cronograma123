using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    public class Subject : Entity
    {
        public string ModalityName { get; set; } = "Presencial|remoto";
        public string SchemeName { get; set; } = "Anual";
        public CommonText Justification { get; set; }
        public CommonText MetodologiesIntroduction { get; set; }
        public ListProperty<CommonText> Metodologies { get; } = new ListProperty<CommonText>();
        public CommonText SpecificNeedsIntroduction { get; set; }
        public CommonText SpecificNeedsGeneral { get; set; }
        public ListProperty<CommonText> SpecificNeedsMeasures{ get; }  = new ListProperty<CommonText>();
        public CommonText EvaluationGeneral { get; set; }
        public CommonText EvaluationTypes { get; set; }
        public CommonText OrdinaryEvaluation { get; set; }
        public CommonText ExtraordinaryEvaluation { get; set; }
        public CommonText EvaluationInstrumentsTypesIntroduction { get; set; }
        public ListProperty<CommonText> EvaluationInstrumentsTypes { get; } = new ListProperty<CommonText>();
        public CommonText SelfEvaluation { get; set; }
        public CommonText ResourcesIntroduction { get; set; }
        public ListProperty<CommonText> SpaceResources { get; } = new ListProperty<CommonText>();
        public ListProperty<CommonText> MaterialResources { get; } = new ListProperty<CommonText>();

        public ListProperty<LearningResult> LearningResults { get; } = new ListProperty<LearningResult>();
        public ListProperty<Content> Contents { get; } = new ListProperty<Content>();

        public ListProperty<Block> Blocks { get; } = new ListProperty<Block>();

        internal Subject() : base()
        {
            StorageClassId = "subject";
        }

        public override ValidationResult Validate()
        {
            throw new NotImplementedException();
        }
    }
}
