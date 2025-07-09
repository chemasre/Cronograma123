using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    public class LearningResult : Entity
    {
        public ListProperty<EvaluationCriteria> Criterias { get; } = new ListProperty<EvaluationCriteria>();

        internal LearningResult() : base()
        {
            StorageClassId = "learningresult";
        }

        public override ValidationResult Validate()
        {
            throw new NotImplementedException();
        }
    }
}
