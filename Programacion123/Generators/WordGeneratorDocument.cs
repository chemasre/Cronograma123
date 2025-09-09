using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;
using Microsoft.Office.Core;
using System.Diagnostics;
using System.Reflection;
using Microsoft.VisualBasic;
using System.Numerics;
using System.Media;
using System.IO;
using System.Windows.Media.Imaging;
using MSHTML;
using System.Net.Cache;


namespace Programacion123
{
    public class WordDocument
    {
        const string TextStyleTable = "TableText";
        const string TextStyleTableHeader1 = "TableHeader1Text";
        const string TextStyleTableHeader2 = "TableHeader2Text";
        const string TextStyleCoverSubjectCode = "CoverSubjectCode";
        const string TextStyleCoverSubjectName = "CoverSubjectName";
        const string TextStyleCoverGradeTypeName = "CoverGradeTypeName";
        const string TextStyleCoverGradeName = "CoverGradeName";


        object missingValue;
        bool closed;

        Application application;
        Document document;
        Table table;
        Row row;
        Cell cell;
        TableOfContents? index;

        Dictionary<DocumentTextElementId, string> generatorTextStyleIdToWordStyleId;
        Dictionary<DocumentCoverElementId, Vector2> generatorCoverElementIdToPosition;

        HashSet<string> wordStylesCache;

        float referenceDpiX = 96;
        float referenceDpiY = 96;

        public static WordDocument Create(Application _app) { return new WordDocument(_app); }

        WordDocument(Application _app)
        {
            missingValue = Missing.Value;
            document = _app.Documents.Add(ref missingValue, ref missingValue, ref missingValue, ref missingValue);
            generatorTextStyleIdToWordStyleId = new();
            generatorCoverElementIdToPosition = new();
            wordStylesCache = new();

            foreach(Style s in document.Styles) { wordStylesCache.Add(s.NameLocal); }

            #if DEBUG

            _app.Visible = false;

            #endif

            application = _app;
        }

        public WordDocument WithReferenceDpi(float dpiX, float dpiY)
        {
            referenceDpiX = dpiX;
            referenceDpiY = dpiY;

            return this;
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
            if(condition)
            {
                ifAction.Invoke(this);
            }

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


            string wordStyleId = generatorTextStyleIdToWordStyleId[documentStyleId];

            paragraph.Range.Text = text;
            paragraph.Range.set_Style(wordStyleId);
            paragraph.Range.InsertParagraphAfter();


            return this;
        }

        public WordDocument WithHeader1(string text) => WithParagraph(text, DocumentTextElementId.Header1);
        public WordDocument WithHeader2(string text) => WithParagraph(text, DocumentTextElementId.Header2);
        public WordDocument WithHeader3(string text) => WithParagraph(text, DocumentTextElementId.Header3);
        public WordDocument WithHeader4(string text) => WithParagraph(text, DocumentTextElementId.Header4);

        public WordDocument WithCoverElementPosition(DocumentCoverElementId coverElementId, DocumentCoverElementPosition position)
        {
            generatorCoverElementIdToPosition[coverElementId] = new Vector2(position.Left, position.Top);

            return this;
        }

        public WordDocument WithTextStyle(DocumentTextElementId generatorTextStyleId, DocumentTextElementStyle generatorTextStyle)
        {
            if(closed) { return this; }

            string wordStyleId;

            Style? wordStyle = null;

            if(generatorTextStyleId == DocumentTextElementId.Header1) { wordStyleId = document.Styles[WdBuiltinStyle.wdStyleHeading1].NameLocal; }
            else if(generatorTextStyleId == DocumentTextElementId.Header2) { wordStyleId = document.Styles[WdBuiltinStyle.wdStyleHeading2].NameLocal; }
            else if(generatorTextStyleId == DocumentTextElementId.Header3) { wordStyleId = document.Styles[WdBuiltinStyle.wdStyleHeading3].NameLocal; }
            else if(generatorTextStyleId == DocumentTextElementId.Header4) { wordStyleId = document.Styles[WdBuiltinStyle.wdStyleHeading4].NameLocal; }
            else if(generatorTextStyleId == DocumentTextElementId.Header5) { wordStyleId = document.Styles[WdBuiltinStyle.wdStyleHeading5].NameLocal; }
            else if(generatorTextStyleId == DocumentTextElementId.Header6) { wordStyleId = document.Styles[WdBuiltinStyle.wdStyleHeading6].NameLocal; }
            else if(generatorTextStyleId == DocumentTextElementId.NormalText) { wordStyleId = document.Styles[WdBuiltinStyle.wdStyleNormal].NameLocal; }
            else if(generatorTextStyleId == DocumentTextElementId.IndexLevel1) { wordStyleId = document.Styles[WdBuiltinStyle.wdStyleTOC1].NameLocal; }
            else if(generatorTextStyleId == DocumentTextElementId.IndexLevel2) { wordStyleId = document.Styles[WdBuiltinStyle.wdStyleTOC2].NameLocal; }
            else if(generatorTextStyleId == DocumentTextElementId.IndexLevel3) {  wordStyleId = document.Styles[WdBuiltinStyle.wdStyleTOC3].NameLocal; }
            else
            {
                if(generatorTextStyleId == DocumentTextElementId.TableText) { wordStyleId = TextStyleTable; }
                else if(generatorTextStyleId == DocumentTextElementId.TableHeader1Text) { wordStyleId = TextStyleTableHeader1; }
                else if(generatorTextStyleId == DocumentTextElementId.TableHeader2Text) { wordStyleId = TextStyleTableHeader2; }
                else if(generatorTextStyleId == DocumentTextElementId.CoverSubjectCode) { wordStyleId = TextStyleCoverSubjectCode; }
                else if(generatorTextStyleId == DocumentTextElementId.CoverSubjectName) { wordStyleId = TextStyleCoverSubjectName; }
                else if(generatorTextStyleId == DocumentTextElementId.CoverGradeTypeName) { wordStyleId = TextStyleCoverGradeTypeName; }
                else // docStyleId == DocumentTextElementId.CoverGradeName
                { wordStyleId = TextStyleCoverGradeName; }

             }

            generatorTextStyleIdToWordStyleId.Add(generatorTextStyleId, wordStyleId);

            if(wordStylesCache.Contains(wordStyleId))
            {
                wordStyle = document.Styles[wordStyleId];
            }
            else
            {
                wordStyle = document.Styles.Add(wordStyleId);
                wordStylesCache.Add(wordStyleId);
            }


            //Styles styles = ;
            //bool found = false;
            //foreach(Style s in styles)
            //{
            //    if(s.NameLocal == wordStyleId)
            //    { 
            //        found = true;
            //    }
            //}
            //if(!found) {  }
            //else { wordStyle = styles[wordStyleId]; }
                
            wordStyle.Font.Name = (generatorTextStyle.FontFamily == DocumentTextElementFontFamily.SansSerif ? "Calibri" : "Times New Roman");
            wordStyle.Font.Size = generatorTextStyle.FontSize;
            wordStyle.Font.Bold = generatorTextStyle.Bold ? 1 : 0;
            wordStyle.Font.Italic = generatorTextStyle.Italic ? 1 : 0;
            wordStyle.Font.Underline = generatorTextStyle.Underscore ? WdUnderline.wdUnderlineSingle : WdUnderline.wdUnderlineNone;

            ParagraphFormat paragraphFormat = wordStyle.ParagraphFormat;

            if(generatorTextStyle.Align == DocumentTextElementAlign.Left) { paragraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft; }
            else if(generatorTextStyle.Align == DocumentTextElementAlign.Right) { paragraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight; }
            else if(generatorTextStyle.Align == DocumentTextElementAlign.Center) { paragraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter; }
            else // docStyle.Align == DocumentTextElementAlign.Justify
            { paragraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify; }

            wordStyle.ParagraphFormat = paragraphFormat;

            wordStyle.ParagraphFormat.SpaceAfter = generatorTextStyle.Margins.Bottom;
            wordStyle.ParagraphFormat.SpaceBefore = generatorTextStyle.Margins.Top;
            wordStyle.ParagraphFormat.LeftIndent = generatorTextStyle.Margins.Left;
            wordStyle.ParagraphFormat.RightIndent = generatorTextStyle.Margins.Right;

            int r;
            int g;
            int b;

            DocumentStyle.GetRGBFromColor(generatorTextStyle.FontColor, out r, out g, out b);

            wordStyle.Font.Color = (WdColor)(r + (1 << 8) * g + (1 << 16) * b);

            return this;
        }


        public WordDocument Save(string path)
        {
            if(closed) { return this; }

            if(index != null) { index.Update(); }

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

        public WordDocument WithCell(int row, int column, string text, string textStyle = TextStyleTable)
        {
            Console.WriteLine("Row: " + row + " Column: " + column + " Content: " + text);

            table.Cell(row, column).Range.Text = text;
            table.Cell(row, column).Range.set_Style(textStyle);

            return this;
        }

        public WordDocument WithCellHeader1(int row, int column, string text)
        {
            WithCell(row, column, text, TextStyleTableHeader1);

            return this;
        }
        
        public WordDocument WithCellHeader2(int row, int column, string text)
        {
            WithCell(row, column, text, TextStyleTableHeader2);

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
            titleRange.Collapse(WdCollapseDirection.wdCollapseEnd);

            titleRange.Text = "Índice";
            titleRange.set_Style(WdBuiltinStyle.wdStyleHeading1);
            titleRange.InsertParagraphAfter();


            var indexRange = document.Range(titleRange.End);
            indexRange.Collapse(WdCollapseDirection.wdCollapseStart);

            index = document.TablesOfContents.Add(indexRange);

            return this;
        }

        public WordDocument WithCoverTextElement(string text, DocumentTextElementId textStyleId, DocumentCoverElementId coverStyleId)
        {
            Vector2 position = generatorCoverElementIdToPosition[coverStyleId];

            Microsoft.Office.Interop.Word.Shape shape = document.Shapes.AddShape((int)MsoAutoShapeType.msoShapeRectangle,
                                        application.CentimetersToPoints(position.X) + document.PageSetup.LeftMargin, 
                                        application.CentimetersToPoints(position.Y) + document.PageSetup.TopMargin,
                                        application.CentimetersToPoints(1),
                                        application.CentimetersToPoints(30)
                                        );

            shape.TextFrame.AutoSize = 1;
            shape.TextFrame.WordWrap = 0;
            shape.TextFrame.TextRange.Text = text;
            shape.TextFrame.TextRange.set_Style(generatorTextStyleIdToWordStyleId[textStyleId]);
            shape.TextFrame.TextRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            shape.TextFrame.VerticalAnchor = MsoVerticalAnchor.msoAnchorTop;
            shape.TextFrame.TextRange.ParagraphFormat.SpaceBefore = 0;
            shape.TextFrame.TextRange.ParagraphFormat.SpaceAfter = 0;

            shape.Fill.Visible = (MsoTriState)TriState.False;  
            shape.Line.Visible = (MsoTriState)TriState.False;
            return this;
        }

        public WordDocument WithCoverImageElement(string imageBase64, DocumentCoverElementId coverStyleId)
        {

            string tempPath = Path.GetTempPath();

            string tempFilePath = tempPath + Guid.NewGuid().ToString();

            byte[] imageBytes = Convert.FromBase64String(imageBase64);
            File.WriteAllBytes(tempFilePath, imageBytes);

            // This code makes word automatic scaling of images behave like the web preview, as the
            // first takes into account the image file's dpi and the other doesn't.
            
            MemoryStream m = new(imageBytes);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = m;
            image.EndInit();

            // Get image dpi

            double dpiHorizontal = image.DpiY;
            double dpiVertical = image.DpiY;

            Vector2 position = generatorCoverElementIdToPosition[coverStyleId];

            Microsoft.Office.Interop.Word.Shape shape = document.Shapes.AddPicture(tempFilePath,
                                      false, true,
                                      application.CentimetersToPoints(position.X),
                                      application.CentimetersToPoints(position.Y));

            // Scale image


            shape.ScaleWidth((float)dpiHorizontal / referenceDpiX, (MsoTriState)MsoTriState.msoTrue);
            shape.ScaleHeight((float)dpiVertical / referenceDpiY, (MsoTriState)MsoTriState.msoTrue);

            shape.PictureFormat.TransparentBackground = (MsoTriState)MsoTriState.msoTrue;

            File.Delete(tempFilePath);

            return this;
        }

        public WordDocument WithPageBreak()
        {
            var range = document.Range();
            range.Collapse(WdCollapseDirection.wdCollapseEnd);

            range.InsertBreak();

            return this;
        }

        public WordDocument WithPageNumbering()
        {
            foreach(Section s in document.Sections)
            {
                HeaderFooter footer = s.Footers[WdHeaderFooterIndex.wdHeaderFooterPrimary];

                footer.PageNumbers.Add(WdParagraphAlignment.wdAlignParagraphRight, false);
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
