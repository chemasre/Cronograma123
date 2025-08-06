using System;
using System.Globalization;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using static System.Net.Mime.MediaTypeNames;

namespace Programacion123
{
    public struct GeneratorData
    {
        public DocumentStyle DocumentStyle { get; set; }
        public Dictionary<DocumentCoverElementId, DocumentCoverElementStyle> CoverElementStyles { get; set; }
        public Dictionary<DocumentTextElementId, DocumentTextElementStyle> TextElementStyles { get; set; }
        public Dictionary<DocumentTableElementId, DocumentTableElementStyle> TableElementStyles { get; set; }
    }
    
    public partial class HTMLGenerator : Generator
    {
        public Subject Subject;
    
        public DocumentStyle DocumentStyle { get; set; }
        public Dictionary<DocumentCoverElementId, DocumentCoverElementStyle> CoverElementStyles { get; set; }
        public Dictionary<DocumentTextElementId, DocumentTextElementStyle> TextElementStyles { get; set; }
        public Dictionary<DocumentTableElementId, DocumentTableElementStyle> TableElementStyles { get; set; }
    
        public HTMLGenerator()
        {
            DocumentStyle = new DocumentStyle()
            {
                LogoBase64 = null,
                Size = DocumentSize.A4,
                Orientation = DocumentOrientation.Portrait,
                Margins = new() { Top = 2.0f, Bottom = 2.5f, Left = 1.5f, Right = 1.0f }
            };

            CoverElementStyles = new();

            CoverElementStyles[DocumentCoverElementId.Logo] = new() { Position = new() { Left = 0, Top = 0 } };
            CoverElementStyles[DocumentCoverElementId.SubjectCode] = new() { Position = new() { Left = 0, Top = 0 } };
            CoverElementStyles[DocumentCoverElementId.SubjectName] = new() { Position = new() { Left = 0, Top = 0 } };
            CoverElementStyles[DocumentCoverElementId.GradeTypeName] = new() { Position = new() { Left = 0, Top = 0 } };
            CoverElementStyles[DocumentCoverElementId.GradeName] = new() { Position = new() { Left = 0, Top = 0 } };
    
            TextElementStyles = new();
            TextElementStyles[DocumentTextElementId.Header1] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 32, Bold = true, Margins = new () { Bottom = 32 } };
            TextElementStyles[DocumentTextElementId.Header2] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 26, Bold = true, Margins = new () { Bottom = 26 } };
            TextElementStyles[DocumentTextElementId.Header3] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 22, Bold = true, Margins = new () { Bottom = 22 } };
            TextElementStyles[DocumentTextElementId.Header4] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 18, Bold = true, Margins = new () { Bottom = 18 } };
            TextElementStyles[DocumentTextElementId.Header5] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 16, Bold = true, Margins = new () { Bottom = 16 } };
            TextElementStyles[DocumentTextElementId.Header6] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 14, Bold = true, Margins = new () { Bottom = 14 } };
            TextElementStyles[DocumentTextElementId.NormalText] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 12, Margins = new () { Bottom = 12 } };
            TextElementStyles[DocumentTextElementId.Table] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 10 };
            TextElementStyles[DocumentTextElementId.TableHeader1Text] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 10, Bold = true };
            TextElementStyles[DocumentTextElementId.TableHeader2Text] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 10, Bold = true };
            TextElementStyles[DocumentTextElementId.CoverSubjectCode] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 18, Bold = true, Margins = new () { Bottom = 18 } };
            TextElementStyles[DocumentTextElementId.CoverSubjectName] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 32, Bold = true, Margins = new () { Bottom = 32 } };
            TextElementStyles[DocumentTextElementId.CoverGradeTypeName] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 22, Bold = true, Margins = new () { Bottom = 22 } };
            TextElementStyles[DocumentTextElementId.CoverGradeName] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 26, Bold = true, Margins = new () { Bottom = 26 } };
    
    
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
                GeneratorData config = JsonSerializer.Deserialize<GeneratorData>(json);
    
                DocumentStyle = config.DocumentStyle;
                TextElementStyles = config.TextElementStyles;
                TableElementStyles = config.TableElementStyles;
    
            }
        }
    
        public override void Save(string path)
        {
            GeneratorData config = new();
    
            config.DocumentStyle = DocumentStyle;
            config.TextElementStyles = TextElementStyles;
            config.TableElementStyles = TableElementStyles;
    
            string json = JsonSerializer.Serialize<GeneratorData>(config);
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
