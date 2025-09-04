using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;
using System.Diagnostics;
using System.Reflection;


namespace Programacion123
{
    public class WordDocument
    {
        object missingValue;
        bool closed;

        Document document;
        Application application;

        public WordDocument(Application _app)
        {
            missingValue = Missing.Value;
            document = _app.Documents.Add(ref missingValue, ref missingValue, ref missingValue, ref missingValue);
            application = _app;
        }

        public WordDocument WithMargins(DocumentMargins margins)
        {
            if(closed) { return this; }

            document.PageSetup.TopMargin    = application.CentimetersToPoints(margins.Top);
            document.PageSetup.BottomMargin = application.CentimetersToPoints(margins.Bottom);
            document.PageSetup.LeftMargin   = application.CentimetersToPoints(margins.Left);
            document.PageSetup.RightMargin  = application.CentimetersToPoints(margins.Right);

            return this;
        }

        public WordDocument WithOrientation(DocumentOrientation orientation)
        {            
            if(closed) { return this; }

            document.PageSetup.Orientation = (orientation == DocumentOrientation.Portrait ?
                                                WdOrientation.wdOrientPortrait :
                                                WdOrientation.wdOrientLandscape);

            return this;            
        }

        public WordDocument Foreach<T>(List<T> elements, Action<T, int, WordDocument> action)
        {
            int i = 0;
            foreach(T e in elements) { action.Invoke(e, i, this); i ++; }
            return this;
        }

        public WordDocument WithParagraph(string text, DocumentTextElementId documentStyleId = DocumentTextElementId.NormalText)
        {
            if(closed) { return this; }

            Paragraph paragraph = document.Content.Paragraphs.Add(ref missingValue);

            WdBuiltinStyle style = WdBuiltinStyle.wdStyleNormal;

            if(documentStyleId == DocumentTextElementId.Header1) { style = WdBuiltinStyle.wdStyleHeading1; }
            else if(documentStyleId == DocumentTextElementId.Header2) { style = WdBuiltinStyle.wdStyleHeading2; }
            else if(documentStyleId == DocumentTextElementId.Header3) { style = WdBuiltinStyle.wdStyleHeading3; }
            else if(documentStyleId == DocumentTextElementId.Header4) { style = WdBuiltinStyle.wdStyleHeading4; }

            paragraph.Range.set_Style(style);
            paragraph.Range.Text = text;
            paragraph.Range.InsertParagraphAfter();

            return this;
        }

        public WordDocument WithHeader1(string text) => WithParagraph(text, DocumentTextElementId.Header1);
        public WordDocument WithHeader2(string text) => WithParagraph(text, DocumentTextElementId.Header2);
        public WordDocument WithHeader3(string text) => WithParagraph(text, DocumentTextElementId.Header3);
        public WordDocument WithHeader4(string text) => WithParagraph(text, DocumentTextElementId.Header4);



        public WordDocument WithTextStyle(DocumentTextElementId docStyleId, DocumentTextElementStyle docStyle)
        {
            if(closed) { return this; }

            Style style = document.Styles[WdBuiltinStyle.wdStyleLineNumber];

            if(docStyleId == DocumentTextElementId.Header1) { style = document.Styles[WdBuiltinStyle.wdStyleHeading1]; }
            else if(docStyleId == DocumentTextElementId.Header2) { style = document.Styles[WdBuiltinStyle.wdStyleHeading2]; }
            else if(docStyleId == DocumentTextElementId.Header3) { style = document.Styles[WdBuiltinStyle.wdStyleHeading3]; }
            else if(docStyleId == DocumentTextElementId.Header4) { style = document.Styles[WdBuiltinStyle.wdStyleHeading4]; }
            else if(docStyleId == DocumentTextElementId.Header5) { style = document.Styles[WdBuiltinStyle.wdStyleHeading5]; }
            else if(docStyleId == DocumentTextElementId.Header6) { style = document.Styles[WdBuiltinStyle.wdStyleHeading6]; }
            else if(docStyleId == DocumentTextElementId.NormalText) { style = document.Styles[WdBuiltinStyle.wdStyleNormal]; }
            else if(docStyleId == DocumentTextElementId.Table) { style = document.Styles[WdBuiltinStyle.wdStyleNormalTable]; }
            //if(id == DocumentTextElementId.TableHeader1Text) { style = document.Styles[WdBuiltinStyle.wdStyleTable]; }
            //if(id == DocumentTextElementId.TableHeader2Text) { style = document.Styles[WdBuiltinStyle.wdStyle]; }
            //if(id == DocumentTextElementId.CoverSubjectCode) { style = document.Styles[WdBuiltinStyle.wdStyle]; }
            //if(id == DocumentTextElementId.CoverSubjectName) { style = document.Styles[WdBuiltinStyle.wdStyle]; }
            //if(id == DocumentTextElementId.CoverGradeTypeName) { style = document.Styles[WdBuiltinStyle.wdStyle]; }
            //if(id == DocumentTextElementId.CoverGradeName) { style = document.Styles[WdBuiltinStyle.wdStyle]; }
            else if(docStyleId == DocumentTextElementId.IndexLevel1) { style = document.Styles[WdBuiltinStyle.wdStyleIndex1]; }
            else if(docStyleId == DocumentTextElementId.IndexLevel2) { style = document.Styles[WdBuiltinStyle.wdStyleIndex2]; }
            else if(docStyleId == DocumentTextElementId.IndexLevel3) {  style = document.Styles[WdBuiltinStyle.wdStyleIndex3]; }

            style.Font.Name = (docStyle.FontFamily == DocumentTextElementFontFamily.SansSerif ? "Calibri" : "Times New Roman");
            style.Font.Size = docStyle.FontSize;

            int r;
            int g;
            int b;

            DocumentStyle.GetRGBFromColor(docStyle.FontColor, out r, out g, out b);

            style.Font.Color = (WdColor)(r + (1 << 8) * g + (1 << 16) * b);

            return this;
        }


        public WordDocument Save(string path)
        {
            if(closed) { return this; }

            object pathObject = path;

            document.SaveAs2(ref pathObject);

            return this;
        }

        public WordDocument Close()
        {
            if(closed) { return this; }

            document.Close(ref missingValue, ref missingValue, ref missingValue);
            closed = true;

            return this;
        }
    }
}
