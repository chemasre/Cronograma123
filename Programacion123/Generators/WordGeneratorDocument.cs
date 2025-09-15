using System.IO;
using System.Numerics;
using System.Reflection;
using System.Windows.Media.Imaging;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;
using Microsoft.VisualBasic;
using FillFormat = Microsoft.Office.Interop.Word.FillFormat;
using LineFormat = Microsoft.Office.Interop.Word.LineFormat;
using PictureFormat = Microsoft.Office.Interop.Word.PictureFormat;
using Range = Microsoft.Office.Interop.Word.Range;
using Shape = Microsoft.Office.Interop.Word.Shape;
using Shapes = Microsoft.Office.Interop.Word.Shapes;
using TextFrame = Microsoft.Office.Interop.Word.TextFrame;



namespace Programacion123
{
    public class WordDocument
    {
        public const string TextStyleTable = "TableText";
        public const string TextStyleTableHeader1 = "TableHeader1Text";
        public const string TextStyleTableHeader2 = "TableHeader2Text";
        public const string TextStyleCoverSubjectCode = "CoverSubjectCode";
        public const string TextStyleCoverSubjectName = "CoverSubjectName";
        public const string TextStyleCoverGradeTypeName = "CoverGradeTypeName";
        public const string TextStyleCoverGradeName = "CoverGradeName";
        public const string TextStyleWeightsTable = "TableWeightsText";
        public const string TextStyleWeightsTableHeader1 = "WeightsTableHeader1Text";
        public const string TextStyleWeightsTableHeader2 = "WeightsTableHeader2Text";

        public const string CellStyleNormal = "NormalCell";
        public const string CellStyleHeader1 = "Header1Cell";
        public const string CellStyleHeader2 = "Header2Cell";
        public const string CellStyleWeightsNormal = "WeightsNormalCell";
        public const string CellStyleWeightsHeader1 = "WeightsHeader1Cell";
        public const string CellStyleWeightsHeader2 = "WeightsHeader2Cell";

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
        Dictionary<DocumentTableElementId, DocumentTableElementStyle> generatorTableElementIdToStyle;


        HashSet<string> wordStylesCache;

        float referenceDpiX = 96;
        float referenceDpiY = 96;

        public static WordDocument Create(Application _app) { return new WordDocument(_app); }

        WordDocument(Application _app)
        {
            missingValue = Missing.Value;

            Documents documents = _app.Documents;

            document = documents.Add(ref missingValue, ref missingValue, ref missingValue, ref missingValue);

            Styles styles = document.Styles;

            generatorTextStyleIdToWordStyleId = new();
            generatorCoverElementIdToPosition = new();
            generatorTableElementIdToStyle = new();
            wordStylesCache = new();

            for(int i = 1 ; i <= styles.Count; i++)
            {
                Style s = styles[i];
                wordStylesCache.Add(s.NameLocal);
            }

            _app.Visible = true;

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

            PageSetup setup = document.PageSetup;

            setup.TopMargin    = application.CentimetersToPoints(margins.Top);
            setup.BottomMargin = application.CentimetersToPoints(margins.Bottom);
            setup.LeftMargin   = application.CentimetersToPoints(margins.Left);
            setup.RightMargin  = application.CentimetersToPoints(margins.Right);

            return this;
        }

        public WordDocument WithOrientation(DocumentOrientation orientation)
        {            
            if(closed) { return this; }

            PageSetup setup = document.PageSetup;

            setup.Orientation = (orientation == DocumentOrientation.Portrait ?
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

            Range content = document.Content;
            Paragraphs paragraphs = content.Paragraphs;

            Paragraph paragraph = paragraphs.Add(ref missingValue);


            string wordStyleId = generatorTextStyleIdToWordStyleId[documentStyleId];

            Range paragraphRange = paragraph.Range;

            paragraphRange.Text = text;
            paragraphRange.set_Style(wordStyleId);
            paragraphRange.InsertParagraphAfter();


            return this;
        }

        public WordDocument WithHeader1(string text) => WithParagraph(text, DocumentTextElementId.Header1);
        public WordDocument WithHeader2(string text) => WithParagraph(text, DocumentTextElementId.Header2);
        public WordDocument WithHeader3(string text) => WithParagraph(text, DocumentTextElementId.Header3);
        public WordDocument WithHeader4(string text) => WithParagraph(text, DocumentTextElementId.Header4);

        public WordDocument WithCoverElementPosition(DocumentCoverElementId coverElementId, DocumentCoverElementPosition position)
        {
            if(closed) { return this; }

            generatorCoverElementIdToPosition[coverElementId] = new Vector2(position.Left, position.Top);

            return this;
        }

        public WordDocument WithTableElementStyle(DocumentTableElementId generatorTableElementStyleId, DocumentTableElementStyle generatorTableElementStyle)
        {
            if(closed) { return this; }

            string wordStyleId;

            if(generatorTableElementStyleId == DocumentTableElementId.TableNormalCell) { wordStyleId = CellStyleNormal; }
            else if(generatorTableElementStyleId == DocumentTableElementId.TableHeader1Cell) { wordStyleId = CellStyleHeader1; }
            else if(generatorTableElementStyleId == DocumentTableElementId.TableHeader2Cell) { wordStyleId = CellStyleHeader2; }
            else if(generatorTableElementStyleId == DocumentTableElementId.TableWeightsNormalCell) { wordStyleId = CellStyleWeightsNormal; }
            else if(generatorTableElementStyleId == DocumentTableElementId.TableWeightsHeader1Cell) { wordStyleId = CellStyleWeightsHeader1; }
            else // generatorTableElementStyleId == DocumentTableElementId.TableWeightsHeader2Cell)
            { wordStyleId = CellStyleWeightsHeader2; }

            Style style = GetOrCreateWordStyle(wordStyleId);

            ParagraphFormat format = style.ParagraphFormat;

            format.SpaceAfter = generatorTableElementStyle.Padding.Bottom;
            format.SpaceBefore = generatorTableElementStyle.Padding.Top;
            format.LeftIndent = generatorTableElementStyle.Padding.Left;
            format.RightIndent = generatorTableElementStyle.Padding.Right;

            int r;
            int g;
            int b;

            DocumentStyle.GetRGBFromColor(generatorTableElementStyle.BackgroundColor, out r, out g, out b);

            Shading shading = style.Shading;

            shading.BackgroundPatternColor = (WdColor)(r + (1 << 8) * g + (1 << 16) * b);

            return this;
        }

        Style GetOrCreateWordStyle(string wordStyleId)
        {
            Style style;
            Styles styles = document.Styles;

            if(wordStylesCache.Contains(wordStyleId))
            {
                style = styles[wordStyleId];
            }
            else
            {
                style = styles.Add(wordStyleId);
                wordStylesCache.Add(wordStyleId);
            }

            return style;

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
            else if(generatorTextStyleId == DocumentTextElementId.IndexTitle) {  wordStyleId = document.Styles[WdBuiltinStyle.wdStyleTocHeading].NameLocal; }
            else
            {
                if(generatorTextStyleId == DocumentTextElementId.TableText) { wordStyleId = TextStyleTable; }
                else if(generatorTextStyleId == DocumentTextElementId.TableHeader1Text) { wordStyleId = TextStyleTableHeader1; }
                else if(generatorTextStyleId == DocumentTextElementId.TableHeader2Text) { wordStyleId = TextStyleTableHeader2; }
                else if(generatorTextStyleId == DocumentTextElementId.CoverSubjectCode) { wordStyleId = TextStyleCoverSubjectCode; }
                else if(generatorTextStyleId == DocumentTextElementId.CoverSubjectName) { wordStyleId = TextStyleCoverSubjectName; }
                else if(generatorTextStyleId == DocumentTextElementId.CoverGradeTypeName) { wordStyleId = TextStyleCoverGradeTypeName; }
                else if(generatorTextStyleId == DocumentTextElementId.CoverGradeName) { wordStyleId = TextStyleCoverGradeName; }
                else if(generatorTextStyleId == DocumentTextElementId.WeightsTableText) { wordStyleId = TextStyleWeightsTable; }
                else if(generatorTextStyleId == DocumentTextElementId.WeightsTableHeader1Text) { wordStyleId = TextStyleWeightsTableHeader1; }
                else // generatorTextStyleId == DocumentTextElementId.WeightsTableHeader2Text
                { wordStyleId = TextStyleWeightsTableHeader2; }

             }

            generatorTextStyleIdToWordStyleId.Add(generatorTextStyleId, wordStyleId);

            wordStyle = GetOrCreateWordStyle(wordStyleId);

            Font font = wordStyle.Font;
                
            font.Name = (generatorTextStyle.FontFamily == DocumentTextElementFontFamily.SansSerif ? "Calibri" : "Times New Roman");
            font.Size = generatorTextStyle.FontSize;
            font.Bold = generatorTextStyle.Bold ? 1 : 0;
            font.Italic = generatorTextStyle.Italic ? 1 : 0;
            font.Underline = generatorTextStyle.Underscore ? WdUnderline.wdUnderlineSingle : WdUnderline.wdUnderlineNone;

            ParagraphFormat paragraphFormat = wordStyle.ParagraphFormat;

            if(generatorTextStyle.Align == DocumentTextElementAlign.Left) { paragraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft; }
            else if(generatorTextStyle.Align == DocumentTextElementAlign.Right) { paragraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight; }
            else if(generatorTextStyle.Align == DocumentTextElementAlign.Center) { paragraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter; }
            else // docStyle.Align == DocumentTextElementAlign.Justify
            { paragraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify; }

            wordStyle.ParagraphFormat = paragraphFormat;

            paragraphFormat.SpaceAfter = generatorTextStyle.Margins.Bottom;
            paragraphFormat.SpaceBefore = generatorTextStyle.Margins.Top;
            paragraphFormat.LeftIndent = generatorTextStyle.Margins.Left;
            paragraphFormat.RightIndent = generatorTextStyle.Margins.Right;

            int r;
            int g;
            int b;

            DocumentStyle.GetRGBFromColor(generatorTextStyle.FontColor, out r, out g, out b);

            font.Color = (WdColor)(r + (1 << 8) * g + (1 << 16) * b);

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

            Tables tables = document.Tables;

            table = tables.Add(range, rows, columns, WdDefaultTableBehavior.wdWord9TableBehavior, WdAutoFitBehavior.wdAutoFitContent);
            
            Range tableRange = table.Range;

            tableRange.set_Style("TableText");

            table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
            table.PreferredWidth = 100;

            return this;
        }

        public WordDocument WithCell(int row, int column, string text, string textStyleId = TextStyleTable, string cellStyleId = CellStyleNormal)
        {
            Console.WriteLine("Row: " + row + " Column: " + column + " Content: " + text);

            cell = table.Cell(row, column);
            Range cellRange = cell.Range;

            cellRange.Text = text;
            cellRange.set_Style(textStyleId);

            Style cellStyle = GetOrCreateWordStyle(cellStyleId);

            Shading shading = cell.Shading;

            shading.BackgroundPatternColor = cellStyle.Shading.BackgroundPatternColor;
            cell.LeftPadding = cellStyle.ParagraphFormat.LeftIndent;
            cell.RightPadding = cellStyle.ParagraphFormat.RightIndent;
            cell.TopPadding = cellStyle.ParagraphFormat.SpaceBefore;
            cell.BottomPadding = cellStyle.ParagraphFormat.SpaceAfter;

            cell = table.Cell(row, column);

            return this;
        }

        public WordDocument WithEmptyCell(int row, int column)
        {
            cell = table.Cell(row, column);
            Shading cellShading = cell.Shading;

            cellShading.BackgroundPatternColor = WdColor.wdColorAutomatic;

            Borders borders = cell.Borders;

            Border b1 = borders[WdBorderType.wdBorderTop];
            Border b2 = borders[WdBorderType.wdBorderBottom];
            Border b3 = borders[WdBorderType.wdBorderLeft];
            Border b4 = borders[WdBorderType.wdBorderRight];

            b1.LineStyle = WdLineStyle.wdLineStyleNone;
            b2.LineStyle = WdLineStyle.wdLineStyleNone;
            b3.LineStyle = WdLineStyle.wdLineStyleNone;
            b4.LineStyle = WdLineStyle.wdLineStyleNone;

            cell = table.Cell(row, column);

            return this;
        }

        public WordDocument WithCellHeader1(int row, int column, string text)
        {
            WithCell(row, column, text, TextStyleTableHeader1, CellStyleHeader1);

            return this;
        }
        
        public WordDocument WithCellHeader2(int row, int column, string text)
        {
            WithCell(row, column, text, TextStyleTableHeader2, CellStyleHeader2);

            return this;
        }

        public WordDocument WithCellBorders(bool top, bool bottom, bool left, bool right)
        {
            Borders borders = cell.Borders;

            borders[WdBorderType.wdBorderTop].LineStyle = (top ? WdLineStyle.wdLineStyleSingle : WdLineStyle.wdLineStyleNone);
            borders[WdBorderType.wdBorderBottom].LineStyle = (bottom ? WdLineStyle.wdLineStyleSingle : WdLineStyle.wdLineStyleNone);
            borders[WdBorderType.wdBorderLeft].LineStyle = (left ? WdLineStyle.wdLineStyleSingle : WdLineStyle.wdLineStyleNone);
            borders[WdBorderType.wdBorderRight].LineStyle = (right ? WdLineStyle.wdLineStyleSingle : WdLineStyle.wdLineStyleNone);

            return this;
        }

        public WordDocument WithCellSpan(int row, int column, int rowSpan, int colSpan)
        {
            Console.WriteLine("Row: " + row + " Column: " + column + " Rowspan: " + rowSpan + " Colspan: " + colSpan);

            Cell cell1 = table.Cell(row, column);

            if(rowSpan > 1 || colSpan > 1)
            {
                Cell cell2 = table.Cell(row + rowSpan - 1, column + colSpan - 1);

                cell1.Merge(cell2);
            }

            return this;
        }

        public WordDocument WithIndex()
        {
            var titleRange = document.Range();
            titleRange.Collapse(WdCollapseDirection.wdCollapseEnd);

            titleRange.Text = "Índice";
            titleRange.set_Style(generatorTextStyleIdToWordStyleId[DocumentTextElementId.IndexTitle]);
            titleRange.InsertParagraphAfter();


            var indexRange = document.Range(titleRange.End);
            indexRange.Collapse(WdCollapseDirection.wdCollapseStart);

            TablesOfContents tablesOfContents = document.TablesOfContents;

            index = tablesOfContents.Add(indexRange);

            return this;
        }

        public WordDocument WithCoverTextElement(string text, DocumentTextElementId textStyleId, DocumentCoverElementId coverStyleId)
        {
            Vector2 position = generatorCoverElementIdToPosition[coverStyleId];

            PageSetup pageSetup = document.PageSetup;
            Shapes shapes = document.Shapes;

            Shape shape = shapes.AddShape((int)MsoAutoShapeType.msoShapeRectangle,
                            application.CentimetersToPoints(position.X) + pageSetup.LeftMargin, 
                            application.CentimetersToPoints(position.Y) + pageSetup.TopMargin,
                            application.CentimetersToPoints(1),
                            application.CentimetersToPoints(30)
                            );

            TextFrame textFrame = shape.TextFrame;

            textFrame.AutoSize = 1;
            textFrame.WordWrap = 0;

            Range textRange = textFrame.TextRange;

            textRange.Text = text;
            textRange.set_Style(generatorTextStyleIdToWordStyleId[textStyleId]);

            ParagraphFormat paragraphFormat = textRange.ParagraphFormat;

            textRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            textFrame.VerticalAnchor = MsoVerticalAnchor.msoAnchorTop;
            paragraphFormat.SpaceBefore = 0;
            paragraphFormat.SpaceAfter = 0;

            FillFormat fill = shape.Fill;
            LineFormat line = shape.Line;

            fill.Visible = (MsoTriState)TriState.False;  
            line.Visible = (MsoTriState)TriState.False;

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

            Shapes shapes = document.Shapes;

            Shape shape = shapes.AddPicture(tempFilePath,
                        false, true,
                        application.CentimetersToPoints(position.X),
                        application.CentimetersToPoints(position.Y));

            // Scale image


            shape.ScaleWidth((float)dpiHorizontal / referenceDpiX, (MsoTriState)MsoTriState.msoTrue);
            shape.ScaleHeight((float)dpiVertical / referenceDpiY, (MsoTriState)MsoTriState.msoTrue);

            PictureFormat pictureFormat = shape.PictureFormat;

            pictureFormat.TransparentBackground = (MsoTriState)MsoTriState.msoTrue;

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
            Sections sections = document.Sections;

            for(int i = 1; i <= sections.Count; i++)
            {
                Section s = sections[i];
                HeadersFooters headersFooters = s.Footers;

                HeaderFooter footer = headersFooters[WdHeaderFooterIndex.wdHeaderFooterPrimary];

                PageNumbers pageNumbers = footer.PageNumbers;

                pageNumbers.Add(WdParagraphAlignment.wdAlignParagraphRight, false);
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
