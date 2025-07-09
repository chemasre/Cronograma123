using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    public class Block: Entity
    {
        public ListProperty<Activity> Activities { get; } = new ListProperty<Activity>();

        internal Block() : base()
        {
            StorageClassId = "block";
        }

        public override ValidationResult Validate()
        {
            throw new NotImplementedException();
        }
    }
}
