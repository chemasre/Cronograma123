using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    public struct GeneratorValidationResult
    {
        public GeneratorValidationCode code;
        public int index;

        public static GeneratorValidationResult Create(GeneratorValidationCode _code) { return new GeneratorValidationResult() { code = _code }; }
        public GeneratorValidationResult WithIndex(int _index) { index = _index; return this; }

        public override string ToString()
        {
            if (code == GeneratorValidationCode.success) { return "No se detectan problemas."; }
            else if(code == GeneratorValidationCode.subjectIsNull) { return "No se ha seleccionado una programación de módulo"; }
            else // code == GeneratorValidationCode.subjectNotValid)
            { return String.Format("La programación del módulo presenta algún problema."); }

        }
    };

    public enum GeneratorValidationCode
    {
        success,
        subjectIsNull,
        subjectNotValid
    };

    public abstract class Generator
    {
        public abstract void Generate(string path);
        public abstract void Save(string path);
        public abstract void LoadOrCreate(string path);
        public abstract GeneratorValidationResult Validate();
    }

}
