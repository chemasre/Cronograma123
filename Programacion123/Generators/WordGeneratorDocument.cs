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

        Application application;
        Document document;
        Table table;
        Row row;
        Cell cell;

        public static WordDocument Create(Application _app) { return new WordDocument(_app); }

        WordDocument(Application _app)
        {
            missingValue = Missing.Value;
            document = _app.Documents.Add(ref missingValue, ref missingValue, ref missingValue, ref missingValue);

            #if DEBUG

            _app.Visible = false;

            #endif

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

        public WordDocument If(bool condition, Action<WordDocument> ifAction)
        {
            if(condition) { ifAction.Invoke(this); }

            return this;
        }

        public WordDocument Do(Action<WordDocument> action)
        {
            action.Invoke(this);

            return this;
        }

        public WordDocument WithParagraph(string text, DocumentTextElementId documentStyleId = DocumentTextElementId.NormalText)
        {
            if(closed) { return this; }

            Paragraph paragraph = document.Content.Paragraphs.Add(ref missingValue);

            WdBuiltinStyle style;

            if(documentStyleId == DocumentTextElementId.Header1) { style = WdBuiltinStyle.wdStyleHeading1; }
            else if(documentStyleId == DocumentTextElementId.Header2) { style = WdBuiltinStyle.wdStyleHeading2; }
            else if(documentStyleId == DocumentTextElementId.Header3) { style = WdBuiltinStyle.wdStyleHeading3; }
            else if(documentStyleId == DocumentTextElementId.Header4) { style = WdBuiltinStyle.wdStyleHeading4; }
            else // documentStyleId == DocumentTextElementid.NormalText
            { style = WdBuiltinStyle.wdStyleNormal; }

            paragraph.Range.Text = text;
            paragraph.Range.set_Style(style);
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
            style.Font.Bold = docStyle.Bold ? 1 : 0;
            style.Font.Italic = docStyle.Italic ? 1 : 0;
            style.Font.Underline = docStyle.Underscore ? WdUnderline.wdUnderlineSingle : WdUnderline.wdUnderlineNone;

            ParagraphFormat paragraphFormat = style.ParagraphFormat;

            if(docStyle.Align == DocumentTextElementAlign.Left) { paragraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft; }
            else if(docStyle.Align == DocumentTextElementAlign.Right) { paragraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight; }
            else if(docStyle.Align == DocumentTextElementAlign.Center) { paragraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter; }
            else // docStyle.Align == DocumentTextElementAlign.Justify
            { paragraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify; }

            style.ParagraphFormat = paragraphFormat;

            style.ParagraphFormat.SpaceAfter = docStyle.Margins.Bottom;
            style.ParagraphFormat.SpaceBefore = docStyle.Margins.Top;
            style.ParagraphFormat.LeftIndent = docStyle.Margins.Left;
            style.ParagraphFormat.RightIndent = docStyle.Margins.Right;

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

        public WordDocument WithTable(int rows, int columns)
        {
            Console.WriteLine("Table Rows: " + rows + " Columns: " + columns);

            missingValue = Missing.Value;
            Microsoft.Office.Interop.Word.Range range = document.Content;
            range.InsertParagraphAfter();
            range.Collapse(WdCollapseDirection.wdCollapseEnd);

            table = document.Tables.Add(range, rows, columns, WdDefaultTableBehavior.wdWord9TableBehavior, WdAutoFitBehavior.wdAutoFitContent);

            return this;
        }

        public WordDocument WithCellContent(int row, int column, string text)
        {
            Console.WriteLine("Row: " + row + " Column: " + column + " Content: " + text);

            table.Cell(row, column).Range.Text = text;

            return this;
        }

        public WordDocument WithCellSpan(int row, int column, int rowSpan, int colSpan)
        {
            Console.WriteLine("Row: " + row + " Column: " + column + " Rowspan: " + rowSpan + " Colspan: " + colSpan);

            if(rowSpan > 1 || colSpan > 1)
            {
                table.Cell(row, column).Merge(table.Cell(row + rowSpan - 1, column + colSpan - 1));
            }

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
