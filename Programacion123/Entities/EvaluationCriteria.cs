using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    public class EvaluationCriteria : Entity
    {
        internal EvaluationCriteria() : base()
        {
            StorageClassId = "evaluationcriteria";
        }

        public override ValidationResult Validate()
        {
            throw new NotImplementedException();
        }
    }
}
