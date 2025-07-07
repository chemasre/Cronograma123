using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    internal class LearningResultCriteria
    {

    }

    internal class LearningResult
    {

    }

    internal class Requirements : Entity
    {
        internal ListProperty<LearningResult> LearningResults { get; } = new ListProperty<LearningResult>();

        internal Requirements() : base()
        {
            StorageClassId = "requirement";
        }

        public override ValidationResult Validate()
        {
            throw new NotImplementedException();
        }
    }
}
