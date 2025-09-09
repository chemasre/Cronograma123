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
        const string TableTextStyleName = "TableText";
        const string TableHeader1TextStyleName = "TableHeader1Text";
        const string TableHeader2TextStyleName = "TableHeader2Text";

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

            Style? style = null;

            if(docStyleId == DocumentTextElementId.Header1) { style = document.Styles[WdBuiltinStyle.wdStyleHeading1]; }
            else if(docStyleId == DocumentTextElementId.Header2) { style = document.Styles[WdBuiltinStyle.wdStyleHeading2]; }
            else if(docStyleId == DocumentTextElementId.Header3) { style = document.Styles[WdBuiltinStyle.wdStyleHeading3]; }
            else if(docStyleId == DocumentTextElementId.Header4) { style = document.Styles[WdBuiltinStyle.wdStyleHeading4]; }
            else if(docStyleId == DocumentTextElementId.Header5) { style = document.Styles[WdBuiltinStyle.wdStyleHeading5]; }
            else if(docStyleId == DocumentTextElementId.Header6) { style = document.Styles[WdBuiltinStyle.wdStyleHeading6]; }
            else if(docStyleId == DocumentTextElementId.NormalText) { style = document.Styles[WdBuiltinStyle.wdStyleNormal]; }
            else if(docStyleId == DocumentTextElementId.TableText ||
                    docStyleId == DocumentTextElementId.TableHeader1Text ||
                    docStyleId == DocumentTextElementId.TableHeader2Text)
            {
                string name;
                if(docStyleId == DocumentTextElementId.TableText) { name = TableTextStyleName; }
                else if(docStyleId == DocumentTextElementId.TableHeader1Text) { name = TableHeader1TextStyleName; }
                else // docStyleId == DocumentTextElementId.TableHeader2Text
                { name = TableHeader2TextStyleName; }

                Styles styles = document.Styles;
                bool found = false;
                foreach(Style s in styles)
                {
                    if(s.NameLocal == name)
                    { 
                        found = true;
                    }
                }
                if(!found) { style = styles.Add(name); }
                else { style = styles[name]; }
             }
            //if(id == DocumentTextElementId.CoverSubjectCode) { style = document.Styles[WdBuiltinStyle.wdStyle]; }
            //if(id == DocumentTextElementId.CoverSubjectName) { style = document.Styles[WdBuiltinStyle.wdStyle]; }
            //if(id == DocumentTextElementId.CoverGradeTypeName) { style = document.Styles[WdBuiltinStyle.wdStyle]; }
            //if(id == DocumentTextElementId.CoverGradeName) { style = document.Styles[WdBuiltinStyle.wdStyle]; }
            else if(docStyleId == DocumentTextElementId.IndexLevel1) { style = document.Styles[WdBuiltinStyle.wdStyleTOC1]; }
            else if(docStyleId == DocumentTextElementId.IndexLevel2) { style = document.Styles[WdBuiltinStyle.wdStyleTOC2]; }
            else if(docStyleId == DocumentTextElementId.IndexLevel3) {  style = document.Styles[WdBuiltinStyle.wdStyleTOC3]; }

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
            
            table.Range.set_Style("TableText");

            return this;
        }

        public WordDocument WithCell(int row, int column, string text, string style = TableTextStyleName)
        {
            Console.WriteLine("Row: " + row + " Column: " + column + " Content: " + text);

            table.Cell(row, column).Range.Text = text;
            table.Cell(row, column).Range.set_Style(style);

            return this;
        }

        public WordDocument WithCellHeader1(int row, int column, string text)
        {
            WithCell(row, column, text, TableHeader1TextStyleName);

            return this;
        }
        
        public WordDocument WithCellHeader2(int row, int column, string text)
        {
            WithCell(row, column, text, TableHeader2TextStyleName);

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

        public WordDocument WithIndex()
        {
            var titleRange = document.Range();
            titleRange.Collapse(WdCollapseDirection.wdCollapseStart);

            titleRange.Text = "Índice";
            titleRange.set_Style(WdBuiltinStyle.wdStyleHeading1);
            titleRange.InsertParagraphAfter();


            var indexRange = document.Range(titleRange.End);
            indexRange.Collapse(WdCollapseDirection.wdCollapseStart);

            document.TablesOfContents.Add(indexRange);




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
