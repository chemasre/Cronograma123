using System.IO;
using System.Text;

namespace Programacion123
{
    public partial class HTMLGenerator : Generator
    {
        public const string SettingsId = "HTMLGenerator";

        public HTMLGenerator()
        {
            LineBreak = "<br>";
            NonBreakingSpace = "&nbsp;";
        }

        /// <summary>
        /// Requires validation result to be success
        /// </summary>
        public override GeneratorResult Generate(string outputPath)
        {
            GeneratorResult result = GeneratorResult.Create(GeneratorResultCode.success);

            FileStreamOptions options = new() { Access = FileAccess.Write, Mode = FileMode.Create };
            StreamWriter writer = new(outputPath, Encoding.UTF8, options);

            if (writer == null) { result = GeneratorResult.Create(GeneratorResultCode.fileWriteError); }
            else
            {
                string html = GenerateHTML();

                writer.Write(html);
                writer.Close();
            }

            return result;
        }


        public override GeneratorValidationResult Validate(bool force = false)
        {
            if (Subject == null) { return GeneratorValidationResult.Create(GeneratorValidationCode.subjectIsNull); }
            else if (Subject.Validate(force).code != ValidationCode.success) { return GeneratorValidationResult.Create(GeneratorValidationCode.subjectNotValid); }

            return GeneratorValidationResult.Create(GeneratorValidationCode.success);

        }


    }
}
