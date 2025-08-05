using System.IO;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;

namespace Programacion123
{
    public struct HTMLGeneratorConfiguration
    {
        public string AdditionalCss { get; set; }
        public string? LogoBase64 { get; set; }
    }

    public partial class HTMLGenerator : Generator
    {
        public Subject Subject;

        public string AdditionalCss { get; set; }
        public string? LogoBase64 { get; set; }

        public HTMLGenerator()
        {
            AdditionalCss = defaultCss;
            LogoBase64 = null;
        }

        public override void Generate(string outputPath)
        {
            FileStreamOptions options = new() { Access = FileAccess.Write, Mode = FileMode.Create };
            StreamWriter writer = new(outputPath, Encoding.UTF8, options);

            string html = GenerateHTML();

            writer.Write(html);
            writer.Close();

        }

        public override void LoadOrCreate(string path)
        {
            if(!File.Exists(path))
            {
                Save(path);
            }
            else
            {
                string json = File.ReadAllText(path, Encoding.UTF8);
                HTMLGeneratorConfiguration config = JsonSerializer.Deserialize<HTMLGeneratorConfiguration>(json);

                AdditionalCss = config.AdditionalCss;
                LogoBase64 = config.LogoBase64;

            }
        }

        public override void Save(string path)
        {
            HTMLGeneratorConfiguration config = new();

            config.AdditionalCss = AdditionalCss;
            config.LogoBase64 = LogoBase64;

            string json = JsonSerializer.Serialize<HTMLGeneratorConfiguration>(config);
            File.WriteAllText(path, json, Encoding.UTF8);

        }

        public override GeneratorValidationResult Validate()
        {
            if(Subject == null) { return GeneratorValidationResult.Create(GeneratorValidationCode.subjectIsNull); }
            else if(Subject.Validate().code != ValidationCode.success) { return GeneratorValidationResult.Create(GeneratorValidationCode.subjectNotValid); }
            
            return GeneratorValidationResult.Create(GeneratorValidationCode.success);
            
        }
    }
}
