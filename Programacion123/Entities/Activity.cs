using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    public class Activity: Entity
    {
        public int Hours { get; set; }
        public ListProperty<CommonText> Metodologies;
        public SetProperty<Content> Contents;
        public CommonText ClassroomOrganization;
        public ListProperty<CommonText> Resources;

        public bool IsEvaluable;
        public SetProperty<EvaluationCriteria> EvaluationCriteria;
        public DictionaryProperty<LearningResult, float> ResultsWeight { get; } = new DictionaryProperty<LearningResult, float>();

        internal Activity() : base()
        {
            StorageClassId = "activity";
        }

        public override ValidationResult Validate()
        {
            throw new NotImplementedException();
        }
    }
}
