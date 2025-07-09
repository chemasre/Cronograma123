using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    public class CommonText: Entity
    {
        internal CommonText() : base()
        {
            StorageClassId = "commontext";
        }

        public override ValidationResult Validate()
        {
            throw new NotImplementedException();
        }
    }
}
