using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;

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
            GeneratorSettings settings = Settings.LoadOrCreateSettings<GeneratorSettings>(SettingsId);

            Style = new()
            {
                LogoBase64 = new(settings.DocumentStyle.LogoBase64),
                CoverBase64 = new(settings.DocumentStyle.CoverBase64),
                Size = settings.DocumentStyle.Size,
                Orientation = settings.DocumentStyle.Orientation,
                Margins = settings.DocumentStyle.Margins,
                CoverElementStyles = new(settings.DocumentStyle.CoverElementStyles),
                TextElementStyles =  new(settings.DocumentStyle.TextElementStyles),
                TableElementStyles = new(settings.DocumentStyle.TableElementStyles)

            };

    
        }
    
        public override void SaveSettings()
        {
            GeneratorSettings settings = new();

            Debug.Assert(Style.HasValue);

            DocumentStyle style = Style.Value;

            settings.DocumentStyle = new()
            {
                LogoBase64 = new(style.LogoBase64),
                CoverBase64 = new(style.CoverBase64),
                Size = style.Size,
                Orientation = style.Orientation,
                Margins = style.Margins,
                CoverElementStyles = new(style.CoverElementStyles),
                TextElementStyles = new(style.TextElementStyles),
                TableElementStyles = new(style.TableElementStyles)

            };

            Settings.SaveSettings<GeneratorSettings>(SettingsId, settings);    
        }
    
        public override GeneratorValidationResult Validate()
        {
            if(Subject == null) { return GeneratorValidationResult.Create(GeneratorValidationCode.subjectIsNull); }
            else if(Subject.Validate().code != ValidationCode.success) { return GeneratorValidationResult.Create(GeneratorValidationCode.subjectNotValid); }
    
            return GeneratorValidationResult.Create(GeneratorValidationCode.success);
    
        }
    

    }
}
