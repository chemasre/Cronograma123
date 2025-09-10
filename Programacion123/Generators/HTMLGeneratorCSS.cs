using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Programacion123
{
    public partial class HTMLGenerator : Generator
    {
        public void GetDimensionsFromSizeAndOrientation(DocumentSize size, DocumentOrientation orientation, out float width, out float height)
        {
            float w;
            float h;
            if(size == DocumentSize.A4) { w = 21; h = 29.7f; }
            else  // size == DocumentSize.A5)
            { w = 29.7f / 2.0f; h = 21; }
    
            if(orientation == DocumentOrientation.Landscape) { float t = w; w = h; h = t; }
    
            width = w;
            height = h;
        }

        void AppendCSSCoverElement(DocumentCoverElementId id, StringBuilder builder)
        {
            Debug.Assert(Style.HasValue);

            DocumentStyle styleValue = Style.Value;

            if(id == DocumentCoverElementId.Logo) { builder.AppendLine(".coverLogo {"); }
            else if(id == DocumentCoverElementId.SubjectCode) { builder.AppendLine(".coverSubjectCode {"); }
            else if(id == DocumentCoverElementId.SubjectName) { builder.AppendLine(".coverSubjectName {"); }
            else if(id == DocumentCoverElementId.GradeTypeName) { builder.AppendLine(".coverGradeTypeName {"); }
            else if(id == DocumentCoverElementId.GradeName) { builder.AppendLine(".coverGradeName{"); }
            else // id == DocumentCoverElementId.Cover
            { builder.AppendLine(".coverCover {"); }

            if(!styleValue.CoverElementStyles.ContainsKey(id)) { styleValue.CoverElementStyles.Add(id, new DocumentCoverElementStyle()); }
            DocumentCoverElementStyle coverStyle = styleValue.CoverElementStyles[id];

            builder.AppendLine(String.Format("position:absolute;"));
            builder.AppendLine(String.Format(CultureInfo.InvariantCulture, "top:{0}cm;", coverStyle.Position.Top));
            builder.AppendLine(String.Format(CultureInfo.InvariantCulture, "left:{0}cm;", coverStyle.Position.Left));

            builder.AppendLine("}");
        }

        void AppendCSSTextElement(DocumentTextElementId id, StringBuilder builder)
        {
            Debug.Assert(Style.HasValue);

            DocumentStyle styleValue = Style.Value;

            string selector;

            if (id == DocumentTextElementId.Header1) { selector = "h1"; }
            else if (id == DocumentTextElementId.Header2) { selector = "h2"; }
            else if (id == DocumentTextElementId.Header3) { selector = "h3"; }
            else if (id == DocumentTextElementId.Header4) { selector = "h4"; }
            else if (id == DocumentTextElementId.Header5) { selector = "h5"; }
            else if (id == DocumentTextElementId.Header6) { selector = "h6"; }
            else if (id == DocumentTextElementId.NormalText) { selector = "div"; }
            else if (id == DocumentTextElementId.TableText) { selector = "table"; }
            else if (id == DocumentTextElementId.TableHeader1Text) { selector = ".tableHeader1"; }
            else if (id == DocumentTextElementId.TableHeader2Text) { selector = ".tableHeader2"; }
            else if (id == DocumentTextElementId.CoverSubjectCode) { selector = ".coverSubjectCode"; }
            else if (id == DocumentTextElementId.CoverSubjectName) { selector = ".coverSubjectName"; }
            else if (id == DocumentTextElementId.CoverGradeTypeName) { selector = ".coverGradeTypeName"; }
            else if (id == DocumentTextElementId.CoverGradeName) { selector = ".coverGradeName"; }
            else if (id == DocumentTextElementId.IndexLevel1) { selector = ".indexLevel1"; }
            else if (id == DocumentTextElementId.IndexLevel2) { selector = ".indexLevel2"; }
            else if (id == DocumentTextElementId.IndexLevel3) { selector = ".indexLevel3"; }
            else if (id == DocumentTextElementId.IndexTitle){ selector = ".indexTitle"; }
            else if (id == DocumentTextElementId.WeightsTableText) { selector = ".weightsTable"; }
            else if (id == DocumentTextElementId.WeightsTableHeader1Text) { selector = ".weightsTableHeader1"; }
            else // (id == DocumentTextElementId.WeightsTableHeader2Text)
            { selector = ".weightsTableHeader2"; }

            builder.AppendLine(String.Format("{0} {{", selector));

            if (!styleValue.TextElementStyles.ContainsKey(id)) { styleValue.TextElementStyles.Add(id, new DocumentTextElementStyle()); }
            DocumentTextElementStyle textStyle = styleValue.TextElementStyles[id];

            builder.AppendLine(String.Format("font-size:{0}pt;", textStyle.FontSize));
            builder.AppendLine(String.Format("font-family:{0};", textStyle.FontFamily == DocumentTextElementFontFamily.SansSerif ? "sans-serif" : "serif"));
            builder.AppendLine(String.Format("font-weight:{0};",textStyle.Bold ? "bold" : "normal"));
            builder.AppendLine(String.Format("font-style:{0};",textStyle.Italic ? "italic" : "normal"));
            builder.AppendLine(String.Format("text-decoration:{0};",textStyle.Underscore ? "underline" : "none"));
            builder.AppendLine(String.Format("margin-top:{0}pt;", textStyle.Margins.Top));
            builder.AppendLine(String.Format("margin-bottom:{0}pt;", textStyle.Margins.Bottom));
            builder.AppendLine(String.Format("margin-left:{0}pt;", textStyle.Margins.Left));
            builder.AppendLine(String.Format("margin-right:{0}pt;", textStyle.Margins.Right));

            string align;
            if (textStyle.Align == DocumentTextElementAlign.Left) { align = "left"; }
            else if (textStyle.Align == DocumentTextElementAlign.Center) { align = "center"; }
            else if (textStyle.Align == DocumentTextElementAlign.Right) { align = "right"; }
            else // (textStyle.Align == DocumentTextElementAlign.Justify)
            { align = "justify"; }

            builder.AppendLine(String.Format("text-align:{0};", align));

            int r; int g; int b;
            DocumentStyle.GetRGBFromColor(textStyle.FontColor, out r, out g, out b);
            builder.AppendLine(String.Format("color:rgb({0},{1},{2});", r, g, b));

            builder.AppendLine("}");

            builder.AppendLine(String.Format("{0} a:visited,{0} a:hover,{0} a:link,{0} a:visited {{", selector));
            builder.AppendLine(String.Format("color:rgb({0},{1},{2});", r, g, b));
            builder.AppendLine("}");


        }

        void AppendCSSTableElement(DocumentTableElementId id, StringBuilder builder)
        {
            Debug.Assert(Style.HasValue);

            DocumentStyle StyleValue = Style.Value;
            
            if(id == DocumentTableElementId.TableNormalCell) { builder.AppendLine("td {"); }
            else if(id == DocumentTableElementId.TableHeader1Cell) { builder.AppendLine(".tableHeader1 {"); }
            else if(id == DocumentTableElementId.TableHeader2Cell) { builder.AppendLine(".tableHeader2 {"); }
            else if(id == DocumentTableElementId.TableWeightsNormalCell) { builder.AppendLine(".weightsTable {"); }
            else if(id == DocumentTableElementId.TableWeightsHeader1Cell) { builder.AppendLine(".weightsTableHeader1 {"); }
            else // id == DocumentTableElementId.TableHeader2Cell
            { builder.AppendLine(".weightsTableHeader2 {"); }

            if(!StyleValue.TableElementStyles.ContainsKey(id)) { StyleValue.TableElementStyles.Add(id, new DocumentTableElementStyle()); }
            DocumentTableElementStyle style = StyleValue.TableElementStyles[id];

            int r; int g; int b;
            DocumentStyle.GetRGBFromColor(style.BackgroundColor, out r, out g, out b);
            builder.AppendLine(String.Format("background-color:rgb({0},{1},{2});", r, g, b));

            builder.AppendLine(String.Format("padding-top:{0}pt;", style.Padding.Top));
            builder.AppendLine(String.Format("padding-bottom:{0}pt;", style.Padding.Bottom));
            builder.AppendLine(String.Format("padding-left:{0}pt;", style.Padding.Left));
            builder.AppendLine(String.Format("padding-right:{0}pt;", style.Padding.Right));

            builder.AppendLine("}");
        }

        internal string GenerateCSS(bool isPreview = false)
        {
            Debug.Assert(Style.HasValue);
            
            DocumentStyle styleValue = Style.Value;

            float width;
            float height;
            GetDimensionsFromSizeAndOrientation(styleValue.Size, styleValue.Orientation, out width, out height);

            StringBuilder builder = new();
            builder.AppendLine("body {");
            builder.AppendLine(String.Format(CultureInfo.InvariantCulture, "width:{0:0.00}cm;", width));
            builder.AppendLine(String.Format(CultureInfo.InvariantCulture, "padding:{0:0.00}cm {1:0.00}cm {2:0.00}cm {3:0.00}cm;",
                                                    styleValue.Margins.Top,
                                                    styleValue.Margins.Right,
                                                    styleValue.Margins.Bottom,
                                                    styleValue.Margins.Left));
            builder.AppendLine("}");

            float marginHorizontal = (styleValue.Margins.Left + styleValue.Margins.Right);
            float marginVertical = (styleValue.Margins.Top + styleValue.Margins.Bottom);


            builder.AppendLine(".cover {");
            builder.AppendLine("position:relative;");
            builder.AppendLine(String.Format(CultureInfo.InvariantCulture, "width:{0:0.00}cm;", width - marginHorizontal));
            builder.AppendLine(String.Format(CultureInfo.InvariantCulture, "height:{0:0.00}cm;", height - marginVertical));
            builder.AppendLine("padding:0cm; margin: 0cm;");
            builder.AppendLine("}");

            Enum.GetValues<DocumentCoverElementId>().ToList().ForEach(e => AppendCSSCoverElement(e, builder));

            Enum.GetValues<DocumentTextElementId>().ToList().ForEach(e => AppendCSSTextElement(e, builder));

            Enum.GetValues<DocumentTableElementId>().ToList().ForEach(e => AppendCSSTableElement(e, builder));

            builder.AppendLine("table { width:100%; }");
            builder.AppendLine("a { text-decoration: none; }");

            builder.AppendLine(".emptyCell { background: none; }");

            if(isPreview)
            {
                builder.Append(".cover {border-style:dashed; border-width:2pt; border-color:black; }");
                builder.Append(".pageBreak {border-bottom-style:dashed; border-bottom-width:2pt; border-color:black; display:block; text-align:center; }");
            }
            else
            {
                builder.AppendLine(".pageBreak {page-break-after:always; display:inline; }");
                builder.AppendLine("h1 {page-break-after:avoid; }");
                builder.AppendLine("h2 {page-break-after:avoid; }");
                builder.AppendLine("h3 {page-break-after:avoid; }");
                builder.AppendLine("h4 {page-break-after:avoid; }");
                builder.AppendLine("h5 {page-break-after:avoid; }");
                builder.AppendLine("h6 {page-break-after:avoid; }");
                builder.AppendLine("tr {page-break-after:avoid; }");
            }


            return builder.ToString();

        }
    }
}
