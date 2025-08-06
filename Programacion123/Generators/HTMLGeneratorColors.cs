using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    public enum DocumentElementColor
    {
        AliceBlue,
        Amethyst,
        AntiqueWhite,
        Aqua,
        Aquamarine,
        Azure,
        Beige,
        Bisque,
        Black,
        BlanchedAlmond,
        Blue,
        BlueViolet,
        Brown,
        BurlyWood,
        CadetBlue,
        Chartreuse,
        Chocolate,
        Coral,
        CornSilk,
        CornFlowerBlue,
        Crimson,
        Cyan,
        DarkMagenta,
        DarkBlue,
        DarkCyan,
        DarkGoldenRod,
        DarkGray,
        DarkGreen,
        DarkKhaki,
        DarkOrange,
        DarkOrchid,
        DarkRed,
        DarkSalmon,
        DarkSeaGreen,
        DarkSlateBlue,
        DarkSlateGray,
        DarkTurquoise,
        DarkViolet,
        DarlOliveGreen,
        DeepPink,
        DeepSkyBlue,
        DimGray,
        DodgerBlue,
        FireBrick,
        FloralWhite,
        ForestGreen,
        Fuchsia,
        Gainsboro,
        GhostWhite,
        Gold,
        GoldenRod,
        Gray,
        Green,
        GreenYellow,
        HoneyDew,
        HotPink,
        IndianRed,
        Indigo,
        Ivory,
        Khaki,
        Lavender,
        LavenderBlush,
        LawnGreen,
        LemonChiffon,
        LightBlue,
        LightCoral,
        LightCyan,
        LightGoldenYellow,
        LightGray,
        LightGreen,
        LightPink,
        LightSalmon,
        LightSeaGreen,
        LightSkyBlue,
        LightSteelBlue,
        LightYellow,
        Lime,
        LimeGreen,
        Linen,
        LightSlateGray,
        Magenta,
        Maroon,
        MediumAquamarine,
        MediumBlue,
        MediumOrchid,
        MediumPurpe,
        MediumSeaGreen,
        MediumSlateBlue,
        MediumSpringGreen,
        MediumTurquoise,
        MediumVioletRed,
        MidnightBlue,
        MistyRose,
        Moccasin,
        NavajoWhite,
        Navy,
        OldLace,
        Olive,
        OliveDrab,
        Orange,
        OrangeRed,
        Orchid,
        PaleGoldenRod,
        PaleGreen,
        PaleTurquoise,
        PalevioletRed,
        PapayaWhip,
        PeachPuff,
        Peru,
        Pink,
        Plum,
        PowderBlue,
        Purple,
        Red,
        RosyBrown,
        RoyalBlue,
        SaddleBrown,
        SaeShell,
        Salmon,
        SandyBrown,
        SeaGreen,
        Sienna,
        Silver,
        SkyBlue,
        SlateBlue,
        SlateGray,
        Snow,
        SpringGreen	,
        SteelBlue,
        Tan,
        Teal,
        Thistle,
        Tomato,
        Turquoise,
        Violet,
        Wheat,
        White,
        WhiteSmoke,
        Yellow,
        YellowGreen
    }

    public partial class HTMLGenerator: Generator
    {

        public void GetRGBFromColor(DocumentElementColor color, out int r, out int g, out int b)
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
