using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    public struct DocumentCoverElementPosition
    {
        public float Top { get; set; }
        public float Left { get; set; }

    }

    public struct DocumentMargins
    {
        public float Top { get; set; }
        public float Bottom { get; set; }
        public float Left { get; set; }
        public float Right {  get; set; }
    }

    public struct DocumentTableElementPadding
    {
        public int Top { get; set; }
        public int Bottom { get; set; }
        public int Left { get; set; }
        public int Right {  get; set; }
    }

    public struct DocumentTextElementMargins
    {
        public int Top { get; set; }
        public int Bottom { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
    }


    public enum DocumentSize
    {
        A4,
        A5
    }

    public enum DocumentOrientation
    {
        Portrait,
        Landscape
    }

    public enum DocumentTextElementFontFamily
    {
        SansSerif,
        Serif        
    }

    public enum DocumentTextElementAlign
    {
        Left,
        Center,
        Right,
        Justify
    }

    public struct DocumentCoverElementStyle
    {
        public DocumentCoverElementPosition Position { get; set; }
    
    }

    public struct DocumentTextElementStyle
    {
        public DocumentTextElementFontFamily FontFamily { get; set; }
        public int FontSize { get; set; }
        public DocumentElementColor FontColor { get; set; }
        public bool Bold { get; set; }
        public bool Italic { get; set; }
        public bool Underscore { get; set; }
        public DocumentTextElementAlign Align { get; set; }
        public DocumentTextElementMargins Margins { get; set; }
    
    }
    
    public struct DocumentTableElementStyle
    {
        public DocumentElementColor BackgroundColor { get; set; }
        public DocumentTableElementPadding Padding { get; set; }
    }
    
    public struct DocumentStyle
    {
        public string? LogoBase64 { get; set; }
        public DocumentSize Size { get; set; }
        public DocumentOrientation Orientation { get; set; }
        public DocumentMargins Margins { get; set; }
    }

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
            if(id == DocumentCoverElementId.Logo) { builder.AppendLine(".coverLogo {"); }
            else if(id == DocumentCoverElementId.SubjectCode) { builder.AppendLine(".coverSubjectCode {"); }
            else if(id == DocumentCoverElementId.SubjectName) { builder.AppendLine(".coverSubjectName {"); }
            else if(id == DocumentCoverElementId.GradeTypeName) { builder.AppendLine(".coverGradeTypeName {"); }
            else if(id == DocumentCoverElementId.GradeName) { builder.AppendLine(".coverGradeName{"); }

            if(!CoverElementStyles.ContainsKey(id)) { CoverElementStyles.Add(id, new DocumentCoverElementStyle()); }
            DocumentCoverElementStyle style = CoverElementStyles[id];

            builder.AppendLine(String.Format("position:absolute;"));
            builder.AppendLine(String.Format(CultureInfo.InvariantCulture, "top:{0}cm;", style.Position.Top));
            builder.AppendLine(String.Format(CultureInfo.InvariantCulture, "left:{0}cm;", style.Position.Left));

            builder.AppendLine("}");
        }

        void AppendCSSTextElement(DocumentTextElementId id, StringBuilder builder)
        {
            string selector;

            if (id == DocumentTextElementId.Header1) { selector = "h1"; }
            else if (id == DocumentTextElementId.Header2) { selector = "h2"; }
            else if (id == DocumentTextElementId.Header3) { selector = "h3"; }
            else if (id == DocumentTextElementId.Header4) { selector = "h4"; }
            else if (id == DocumentTextElementId.Header5) { selector = "h5"; }
            else if (id == DocumentTextElementId.Header6) { selector = "h6"; }
            else if (id == DocumentTextElementId.NormalText) { selector = "div"; }
            else if (id == DocumentTextElementId.Table) { selector = "table"; }
            else if (id == DocumentTextElementId.TableHeader1Text) { selector = ".tableHeader1"; }
            else if (id == DocumentTextElementId.TableHeader2Text) { selector = ".tableHeader2"; }
            else if (id == DocumentTextElementId.CoverSubjectCode) { selector = ".coverSubjectCode"; }
            else if (id == DocumentTextElementId.CoverSubjectName) { selector = ".coverSubjectName"; }
            else if (id == DocumentTextElementId.CoverGradeTypeName) { selector = ".coverGradeTypeName"; }
            else if (id == DocumentTextElementId.CoverGradeName) { selector = ".coverGradeNam"; }
            else if (id == DocumentTextElementId.IndexLevel1) { selector = ".indexLevel1"; }
            else if (id == DocumentTextElementId.IndexLevel2) { selector = ".indexLevel2"; }
            else // id == DocumentTextElementId.IndexLevel3
            { selector = ".indexLevel3"; }

            builder.AppendLine(String.Format("{0} {{", selector));

            if (!TextElementStyles.ContainsKey(id)) { TextElementStyles.Add(id, new DocumentTextElementStyle()); }
            DocumentTextElementStyle style = TextElementStyles[id];

            builder.AppendLine(String.Format("font-size:{0}pt;", style.FontSize));
            builder.AppendLine(String.Format("font-family:{0};", style.FontFamily == DocumentTextElementFontFamily.SansSerif ? "sans-serif" : "serif"));
            builder.AppendLine(String.Format("font-weight:{0};",style.Bold ? "bold" : "normal"));
            builder.AppendLine(String.Format("font-style:{0};",style.Italic ? "italic" : "normal"));
            builder.AppendLine(String.Format("text-decoration:{0};",style.Underscore ? "underline" : "none"));
            builder.AppendLine(String.Format("margin-top:{0}pt;", style.Margins.Top));
            builder.AppendLine(String.Format("margin-bottom:{0}pt;", style.Margins.Bottom));
            builder.AppendLine(String.Format("margin-left:{0}pt;", style.Margins.Left));
            builder.AppendLine(String.Format("margin-right:{0}pt;", style.Margins.Right));

            string align;
            if (style.Align == DocumentTextElementAlign.Left) { align = "left"; }
            else if (style.Align == DocumentTextElementAlign.Center) { align = "center"; }
            else if (style.Align == DocumentTextElementAlign.Right) { align = "right"; }
            else // (style.Align == DocumentTextElementAlign.Justify)
            { align = "justify"; }

            builder.AppendLine(String.Format("text-align:{0};", align));

            int r; int g; int b;
            GetRGBFromColor(style.FontColor, out r, out g, out b);
            builder.AppendLine(String.Format("color:rgb({0},{1},{2});", r, g, b));

            builder.AppendLine("}");

            builder.AppendLine(String.Format("{0} a:visited,a:hover,a:link,a:visited {{", selector));
            builder.AppendLine(String.Format("color:rgb({0},{1},{2});", r, g, b));
            builder.AppendLine("}");


        }

        void AppendCSSTableElement(DocumentTableElementId id, StringBuilder builder)
        {
            
            if(id == DocumentTableElementId.TableNormalCell) { builder.AppendLine("td {"); }
            else if(id == DocumentTableElementId.TableHeader1Cell) { builder.AppendLine(".tableHeader1 {"); }
            else // id == DocumentTableElementId.TableHeader2Cell
            { builder.AppendLine(".tableHeader2 {"); }

            if(!TableElementStyles.ContainsKey(id)) { TableElementStyles.Add(id, new DocumentTableElementStyle()); }
            DocumentTableElementStyle style = TableElementStyles[id];

            int r; int g; int b;
            GetRGBFromColor(style.BackgroundColor, out r, out g, out b);
            builder.AppendLine(String.Format("background-color:rgb({0},{1},{2});", r, g, b));

            builder.AppendLine(String.Format("padding-top:{0}pt;", style.Padding.Top));
            builder.AppendLine(String.Format("padding-bottom:{0}pt;", style.Padding.Bottom));
            builder.AppendLine(String.Format("padding-left:{0}pt;", style.Padding.Left));
            builder.AppendLine(String.Format("padding-right:{0}pt;", style.Padding.Right));

            builder.AppendLine("}");
        }

        internal string GenerateCSS()
        {
            float width;
            float height;
            GetDimensionsFromSizeAndOrientation(DocumentStyle.Size, DocumentStyle.Orientation, out width, out height);

            StringBuilder builder = new();
            builder.AppendLine("body {");
            builder.AppendLine(String.Format(CultureInfo.InvariantCulture, "width:{0:0.00}cm;", width));
            builder.AppendLine(String.Format(CultureInfo.InvariantCulture, "padding:{0:0.00}cm {1:0.00}cm {2:0.00}cm {3:0.00}cm;",
                                                    DocumentStyle.Margins.Top,
                                                    DocumentStyle.Margins.Right,
                                                    DocumentStyle.Margins.Bottom,
                                                    DocumentStyle.Margins.Left));
            builder.AppendLine("}");

            float marginHorizontal = (DocumentStyle.Margins.Left + DocumentStyle.Margins.Right);
            float marginVertical = (DocumentStyle.Margins.Top + DocumentStyle.Margins.Bottom);


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

            return builder.ToString();

        }
    }
}
