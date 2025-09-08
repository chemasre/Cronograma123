using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    public struct GeneratorSettings
    {
        public DocumentStyle DocumentStyle { get; set; }

        public GeneratorSettings()
        {
            DocumentStyle = new();
        }
    }
}
