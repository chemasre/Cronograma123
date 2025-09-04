using System;
using System.Collections.Generic;
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
        public string? CoverBase64 { get; set; }
        public DocumentSize Size { get; set; }
        public DocumentOrientation Orientation { get; set; }
        public DocumentMargins Margins { get; set; }

        public Dictionary<DocumentCoverElementId, DocumentCoverElementStyle> CoverElementStyles { get; set; }
        public Dictionary<DocumentTextElementId, DocumentTextElementStyle> TextElementStyles { get; set; }
        public Dictionary<DocumentTableElementId, DocumentTableElementStyle> TableElementStyles { get; set; }

        public DocumentStyle()
        {
            LogoBase64 = null;
            CoverBase64 = null;
            Size = DocumentSize.A4;
            Orientation = DocumentOrientation.Portrait;
            Margins = new() { Top = 2.0f, Bottom = 2.5f, Left = 1.5f, Right = 1.0f };

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

        public static void GetRGBFromColor(DocumentElementColor color, out int r, out int g, out int b)
        {
            if(color ==DocumentElementColor.AliceBlue) { r = 240; g =248; b=255; }
            else if(color ==DocumentElementColor.Amethyst) { r = 153; g =102; b=204; }
            else if(color ==DocumentElementColor.AntiqueWhite) { r = 250; g =235; b=215; }
            else if(color ==DocumentElementColor.Aqua) { r = 0; g =255; b=255; }
            else if(color ==DocumentElementColor.Aquamarine) { r = 127; g =255; b=212; }
            else if(color ==DocumentElementColor.Azure) { r = 240; g =255; b=255; }
            else if(color ==DocumentElementColor.Beige) { r = 245; g =245; b=220; }
            else if(color ==DocumentElementColor.Bisque) { r = 255; g =228; b=196; }
            else if(color ==DocumentElementColor.Black) { r = 0; g =0; b=0; }
            else if(color ==DocumentElementColor.BlanchedAlmond) { r = 255; g =235; b=205; }
            else if(color ==DocumentElementColor.Blue) { r = 0; g =0; b=255; }
            else if(color ==DocumentElementColor.BlueViolet) { r = 138; g =43; b =226; }
            else if(color ==DocumentElementColor.Brown) { r = 165; g =42; b=42; }
            else if(color ==DocumentElementColor.BurlyWood) { r = 222; g =184; b=135; }
            else if(color ==DocumentElementColor.CadetBlue) { r = 95; g =158; b=160; }
            else if(color ==DocumentElementColor.Chartreuse) { r = 127; g =255; b=0; }
            else if(color ==DocumentElementColor.Chocolate) { r = 210; g =105; b=30; }
            else if(color ==DocumentElementColor.Coral) { r = 255; g =127; b=80; }
            else if(color ==DocumentElementColor.CornSilk) { r = 255; g =248; b=220; }
            else if(color ==DocumentElementColor.CornFlowerBlue) { r = 100; g =149; b=237; }
            else if(color ==DocumentElementColor.Crimson) { r = 220; g =20; b=60; }
            else if(color ==DocumentElementColor.Cyan) { r = 0; g =255; b=255; }
            else if(color ==DocumentElementColor.DarkMagenta) { r = 139; g =0; b=139; }
            else if(color ==DocumentElementColor.DarkBlue) { r = 0; g =0; b=139; }
            else if(color ==DocumentElementColor.DarkCyan) { r = 0; g =139; b=139; }
            else if(color ==DocumentElementColor.DarkGoldenRod) { r = 184; g =134; b=11; }
            else if(color ==DocumentElementColor.DarkGray) { r = 169; g =169; b=169; }
            else if(color ==DocumentElementColor.DarkGreen) { r = 0; g =100; b=0; }
            else if(color ==DocumentElementColor.DarkKhaki) { r = 189; g =183; b=107; }
            else if(color ==DocumentElementColor.DarkOrange) { r = 255; g =140; b=0; }
            else if(color ==DocumentElementColor.DarkOrchid) { r = 153; g =50; b=204; }
            else if(color ==DocumentElementColor.DarkRed) { r = 139; g =0; b=0; }
            else if(color ==DocumentElementColor.DarkSalmon) { r = 223; g =150; b=122; }
            else if(color ==DocumentElementColor.DarkSeaGreen) { r = 143; g =188; b=143; }
            else if(color ==DocumentElementColor.DarkSlateBlue) { r = 72; g =216; b=176; }
            else if(color ==DocumentElementColor.DarkSlateGray) { r = 47; g =79; b=79; }
            else if(color ==DocumentElementColor.DarkTurquoise) { r = 0; g =206; b=209; }
            else if(color ==DocumentElementColor.DarkViolet) { r = 148; g =0; b=211; }
            else if(color ==DocumentElementColor.DarlOliveGreen) { r = 85; g =107; b=47; }
            else if(color ==DocumentElementColor.DeepPink) { r = 255; g =20; b=147; }
            else if(color ==DocumentElementColor.DeepSkyBlue) { r = 0; g =191; b=255; }
            else if(color ==DocumentElementColor.DimGray) { r = 105; g =105; b=105; }
            else if(color ==DocumentElementColor.DodgerBlue) { r = 30; g =144; b=255; }
            else if(color ==DocumentElementColor.FireBrick) { r = 178; g =34; b=34; }
            else if(color ==DocumentElementColor.FloralWhite) { r = 255; g =250; b=240; }
            else if(color ==DocumentElementColor.ForestGreen) { r = 34; g =139; b=34; }
            else if(color ==DocumentElementColor.Fuchsia) { r = 255; g =0; b=255; }
            else if(color ==DocumentElementColor.Gainsboro) { r = 220; g =220; b=220; }
            else if(color ==DocumentElementColor.GhostWhite) { r = 248; g =248; b=255; }
            else if(color ==DocumentElementColor.Gold) { r = 255; g =215; b=0; }
            else if(color ==DocumentElementColor.GoldenRod) { r = 218; g =165; b=32; }
            else if(color ==DocumentElementColor.Gray) { r = 128; g =128; b=128; }
            else if(color ==DocumentElementColor.Green) { r = 0; g =128; b=0; }
            else if(color ==DocumentElementColor.GreenYellow) { r = 173; g =255; b=47; }
            else if(color ==DocumentElementColor.HoneyDew) { r = 240; g =255; b=240; }
            else if(color ==DocumentElementColor.HotPink) { r = 255; g =105; b=180; }
            else if(color ==DocumentElementColor.IndianRed) { r = 205; g =92; b=92; }
            else if(color ==DocumentElementColor.Indigo) { r = 75; g =0; b=130; }
            else if(color ==DocumentElementColor.Ivory) { r = 255; g =255; b=240; }
            else if(color ==DocumentElementColor.Khaki) { r = 240; g =230; b=140; }
            else if(color ==DocumentElementColor.Lavender) { r = 230; g =230; b=250; }
            else if(color ==DocumentElementColor.LavenderBlush) { r = 255; g =240; b=245; }
            else if(color ==DocumentElementColor.LawnGreen) { r = 124; g =252; b=0; }
            else if(color ==DocumentElementColor.LemonChiffon) { r = 255; g =250; b=205; }
            else if(color ==DocumentElementColor.LightBlue) { r = 173; g =216; b=230; }
            else if(color ==DocumentElementColor.LightCoral) { r = 240; g =128; b=128; }
            else if(color ==DocumentElementColor.LightCyan) { r = 224; g =255; b=255; }
            else if(color ==DocumentElementColor.LightGoldenYellow) { r = 250; g =250; b=210; }
            else if(color ==DocumentElementColor.LightGray) { r = 211; g =211; b=211; }
            else if(color ==DocumentElementColor.LightGreen) { r = 144; g =238; b=144; }
            else if(color ==DocumentElementColor.LightPink) { r = 255; g =182; b=193; }
            else if(color ==DocumentElementColor.LightSalmon) { r = 255; g =160; b=122; }
            else if(color ==DocumentElementColor.LightSeaGreen) { r = 32; g =178; b=170; }
            else if(color ==DocumentElementColor.LightSkyBlue) { r = 135; g =206; b=205; }
            else if(color ==DocumentElementColor.LightSteelBlue) { r = 176; g =196; b=222; }
            else if(color ==DocumentElementColor.LightYellow) { r = 255; g =255; b=224; }
            else if(color ==DocumentElementColor.Lime) { r = 0; g =255; b=0; }
            else if(color ==DocumentElementColor.LimeGreen) { r = 50; g =205; b=50; }
            else if(color ==DocumentElementColor.Linen) { r = 250; g =240; b=230; }
            else if(color ==DocumentElementColor.LightSlateGray) { r = 119; g =136; b=153; }
            else if(color ==DocumentElementColor.Magenta) { r = 255; g =0; b=255; }
            else if(color ==DocumentElementColor.Maroon) { r = 128; g =0; b=0; }
            else if(color ==DocumentElementColor.MediumAquamarine) { r = 102; g =205; b=170; }
            else if(color ==DocumentElementColor.MediumBlue) { r = 0; g =0; b=205; }
            else if(color ==DocumentElementColor.MediumOrchid) { r = 186; g =85; b=211; }
            else if(color ==DocumentElementColor.MediumPurpe) { r = 147; g =112; b=219; }
            else if(color ==DocumentElementColor.MediumSeaGreen) { r = 60; g =179; b=113; }
            else if(color ==DocumentElementColor.MediumSlateBlue) { r = 123; g =104; b=238; }
            else if(color ==DocumentElementColor.MediumSpringGreen) { r = 0; g =250; b=154; }
            else if(color ==DocumentElementColor.MediumTurquoise) { r = 72; g =209; b=204; }
            else if(color ==DocumentElementColor.MediumVioletRed) { r = 199; g =21; b=133; }
            else if(color ==DocumentElementColor.MidnightBlue) { r = 25; g =25; b=112; }
            else if(color ==DocumentElementColor.MistyRose) { r = 255; g =228; b=225; }
            else if(color ==DocumentElementColor.Moccasin) { r = 255; g =228; b=181; }
            else if(color ==DocumentElementColor.NavajoWhite) { r = 255; g =222; b=173; }
            else if(color ==DocumentElementColor.Navy) { r = 0; g =0; b=128; }
            else if(color ==DocumentElementColor.OldLace) { r = 253; g =245; b=230; }
            else if(color ==DocumentElementColor.Olive) { r = 128; g =128; b=0; }
            else if(color ==DocumentElementColor.OliveDrab) { r = 107; g =142; b=35; }
            else if(color ==DocumentElementColor.Orange) { r = 255; g =165; b=0; }
            else if(color ==DocumentElementColor.OrangeRed) { r = 255; g =69; b=0; }
            else if(color ==DocumentElementColor.Orchid) { r = 218; g =112; b=214; }
            else if(color ==DocumentElementColor.PaleGoldenRod) { r = 238; g =232; b=170; }
            else if(color ==DocumentElementColor.PaleGreen) { r = 152; g =251; b=152; }
            else if(color ==DocumentElementColor.PaleTurquoise) { r = 175; g =238; b=238; }
            else if(color ==DocumentElementColor.PalevioletRed) { r = 219; g =112; b=147; }
            else if(color ==DocumentElementColor.PapayaWhip) { r = 255; g =239; b=213; }
            else if(color ==DocumentElementColor.PeachPuff) { r = 255; g =218; b=185; }
            else if(color ==DocumentElementColor.Peru) { r = 205; g =133; b=63; }
            else if(color ==DocumentElementColor.Pink) { r = 255; g =192; b=203; }
            else if(color ==DocumentElementColor.Plum) { r = 221; g =160; b=221; }
            else if(color ==DocumentElementColor.PowderBlue) { r = 176; g =224; b=230; }
            else if(color ==DocumentElementColor.Purple) { r = 128; g =0; b=128; }
            else if(color ==DocumentElementColor.Red) { r = 255; g =0; b=0; }
            else if(color ==DocumentElementColor.RosyBrown) { r = 188; g =143; b=143; }
            else if(color ==DocumentElementColor.RoyalBlue) { r = 188; g =143; b=143; }
            else if(color ==DocumentElementColor.SaddleBrown) { r = 139; g =69; b=19; }
            else if(color ==DocumentElementColor.SaeShell) { r = 255; g =245; b=238; }
            else if(color ==DocumentElementColor.Salmon) { r = 250; g =128; b=114; }
            else if(color ==DocumentElementColor.SandyBrown) { r = 244; g =164; b=96; }
            else if(color ==DocumentElementColor.SeaGreen) { r = 46; g =139; b=87; }
            else if(color ==DocumentElementColor.Sienna) { r = 160; g =82; b=45; }
            else if(color ==DocumentElementColor.Silver) { r = 192; g =192; b=192; }
            else if(color ==DocumentElementColor.SkyBlue) { r = 135; g =206; b=235; }
            else if(color ==DocumentElementColor.SlateBlue) { r = 106; g =90; b=205; }
            else if(color ==DocumentElementColor.SlateGray) { r = 112; g =128; b=144; }
            else if(color ==DocumentElementColor.Snow) { r = 255; g =250; b=250; }
            else if(color ==DocumentElementColor.SpringGreen ) { r = 0; g =255; b=127; }
            else if(color ==DocumentElementColor.SteelBlue) { r = 70; g =130; b=180; }
            else if(color ==DocumentElementColor.Tan) { r = 210; g =180; b=140; }
            else if(color ==DocumentElementColor.Teal) { r = 0; g =128; b=128; }
            else if(color ==DocumentElementColor.Thistle) { r = 216; g =191; b=216; }
            else if(color ==DocumentElementColor.Tomato) { r = 255; g =99; b=71; }
            else if(color ==DocumentElementColor.Turquoise) { r = 64; g =224; b=208; }
            else if(color ==DocumentElementColor.Violet) { r = 238; g =130; b=238; }
            else if(color ==DocumentElementColor.Wheat) { r = 245; g =222; b=179; }
            else if(color ==DocumentElementColor.White) { r = 255; g =255; b=255; }
            else if(color ==DocumentElementColor.WhiteSmoke) { r = 245; g =245; b=245; }
            else if(color ==DocumentElementColor.Yellow) { r = 255; g =255; b=0; }
            else // (color ==DocumentElementColor.YellowGreen)
            { r = 154; g =205; b=50; }
        }
    }

}
