using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    public struct HTMLGeneratorSettings
    {
        public DocumentStyle DocumentStyle { get; set; }
        public Dictionary<DocumentCoverElementId, DocumentCoverElementStyle> CoverElementStyles { get; set; }
        public Dictionary<DocumentTextElementId, DocumentTextElementStyle> TextElementStyles { get; set; }
        public Dictionary<DocumentTableElementId, DocumentTableElementStyle> TableElementStyles { get; set; }

        public HTMLGeneratorSettings()
        {
            DocumentStyle = new DocumentStyle()
            {
                LogoBase64 = null,
                CoverBase64 = null,
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
            TextElementStyles[DocumentTextElementId.Header1] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 32, Bold = true, Margins = new() { Bottom = 32 } };
            TextElementStyles[DocumentTextElementId.Header2] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 26, Bold = true, Margins = new() { Bottom = 26 } };
            TextElementStyles[DocumentTextElementId.Header3] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 22, Bold = true, Margins = new() { Bottom = 22 } };
            TextElementStyles[DocumentTextElementId.Header4] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 18, Bold = true, Margins = new() { Bottom = 18 } };
            TextElementStyles[DocumentTextElementId.Header5] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 16, Bold = true, Margins = new() { Bottom = 16 } };
            TextElementStyles[DocumentTextElementId.Header6] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 14, Bold = true, Margins = new() { Bottom = 14 } };
            TextElementStyles[DocumentTextElementId.NormalText] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 12, Margins = new() { Bottom = 12 } };
            TextElementStyles[DocumentTextElementId.Table] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 10 };
            TextElementStyles[DocumentTextElementId.TableHeader1Text] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.White, FontSize = 10, Bold = true };
            TextElementStyles[DocumentTextElementId.TableHeader2Text] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.White, FontSize = 10, Bold = true };
            TextElementStyles[DocumentTextElementId.CoverSubjectCode] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 18, Bold = true, Margins = new() { Bottom = 18 } };
            TextElementStyles[DocumentTextElementId.CoverSubjectName] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 32, Bold = true, Margins = new() { Bottom = 32 } };
            TextElementStyles[DocumentTextElementId.CoverGradeTypeName] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 22, Bold = true, Margins = new() { Bottom = 22 } };
            TextElementStyles[DocumentTextElementId.CoverGradeName] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 26, Bold = true, Margins = new() { Bottom = 26 } };
            TextElementStyles[DocumentTextElementId.IndexLevel1] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 16, Bold = false, Margins = new() { Bottom = 16 } };
            TextElementStyles[DocumentTextElementId.IndexLevel2] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 14, Bold = false, Margins = new() { Bottom = 14 } };
            TextElementStyles[DocumentTextElementId.IndexLevel3] = new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 12, Bold = false, Margins = new() { Bottom = 12 } };

            TableElementStyles = new();

            TableElementStyles[DocumentTableElementId.TableNormalCell] = new() { BackgroundColor = DocumentElementColor.White, Padding = new() { Top = 0, Bottom = 0, Left = 0, Right = 0 } };
            TableElementStyles[DocumentTableElementId.TableHeader1Cell] = new() { BackgroundColor = DocumentElementColor.Gray, Padding = new() { Top = 0, Bottom = 0, Left = 0, Right = 0 } };
            TableElementStyles[DocumentTableElementId.TableHeader2Cell] = new() { BackgroundColor = DocumentElementColor.LightGray, Padding = new() { Top = 0, Bottom = 0, Left = 0, Right = 0 } };

        }
    }
}
