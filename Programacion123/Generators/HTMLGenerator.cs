using System.IO;
using System.Text;
using System.Text.Json;

namespace Programacion123
{
   
    public partial class HTMLGenerator : Generator
    {
        public Subject Subject;

        public const string SettingsId = "HTMLGenerator";

        public DocumentStyle DocumentStyle { get; set; }
        public Dictionary<DocumentCoverElementId, DocumentCoverElementStyle> CoverElementStyles { get; set; }
        public Dictionary<DocumentTextElementId, DocumentTextElementStyle> TextElementStyles { get; set; }
        public Dictionary<DocumentTableElementId, DocumentTableElementStyle> TableElementStyles { get; set; }

        public HTMLGenerator()
        {
            HTMLGeneratorSettings defaultSettings = new();

            DocumentStyle = defaultSettings.DocumentStyle;
            CoverElementStyles = defaultSettings.CoverElementStyles;
            TextElementStyles = defaultSettings.TextElementStyles;
            TableElementStyles = defaultSettings.TableElementStyles;

        }

        /// <summary>
        /// Requires validation result to be success
        /// </summary>
        public override void Generate(string outputPath)
        {
            FileStreamOptions options = new() { Access = FileAccess.Write, Mode = FileMode.Create };
            StreamWriter writer = new(outputPath, Encoding.UTF8, options);
    
            string html = GenerateHTML();
    
            writer.Write(html);
            writer.Close();
    
        }
    
        public override void LoadOrCreateSettings()
        {
            HTMLGeneratorSettings settings = Settings.LoadOrCreateSettings<HTMLGeneratorSettings>(SettingsId);

            CoverElementStyles = settings.CoverElementStyles;
            DocumentStyle = settings.DocumentStyle;
            TextElementStyles = settings.TextElementStyles;
            TableElementStyles = settings.TableElementStyles;
    
        }
    
        public override void SaveSettings()
        {
            HTMLGeneratorSettings settings = new();

            settings.CoverElementStyles = CoverElementStyles;
            settings.DocumentStyle = DocumentStyle;
            settings.TextElementStyles = TextElementStyles;
            settings.TableElementStyles = TableElementStyles;

            Settings.SaveSettings<HTMLGeneratorSettings>(SettingsId, settings);    
        }
    
        public override GeneratorValidationResult Validate()
        {
            if(Subject == null) { return GeneratorValidationResult.Create(GeneratorValidationCode.subjectIsNull); }
            else if(Subject.Validate().code != ValidationCode.success) { return GeneratorValidationResult.Create(GeneratorValidationCode.subjectNotValid); }
    
            return GeneratorValidationResult.Create(GeneratorValidationCode.success);
    
        }
    

    }
}
