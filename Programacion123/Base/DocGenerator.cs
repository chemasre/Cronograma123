using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    public abstract class DocGenerator
    {
        public abstract void Generate(Subject _subject, string path);
        public abstract void SaveConfiguration(string path);
        public abstract void LoadConfiguration(string path);
    }

}
