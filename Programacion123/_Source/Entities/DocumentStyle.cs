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
        public float Right { get; set; }
    }

    public struct DocumentTableElementPadding
    {
        public int Top { get; set; }
        public int Bottom { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
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


    public partial class DocumentStyle : Entity
    {
        public string? LogoBase64 { get; set; } = null;
        public string? CoverBase64 { get; set; } = null;
        public DocumentSize Size { get; set; } = DocumentSize.A4;
        public DocumentOrientation Orientation { get; set; } = DocumentOrientation.Portrait;
        public DocumentMargins Margins { get; set; } = new() { Top = 2.0f, Bottom = 2.5f, Left = 1.5f, Right = 1.0f };

        public Dictionary<DocumentCoverElementId, DocumentCoverElementStyle> CoverElementStyles { get; set; } = new() {
                                            { DocumentCoverElementId.Logo, new() { Position = new() { Left = 0, Top = 0 } } },
                                            { DocumentCoverElementId.SubjectCode, new() { Position = new() { Left = 0, Top = 0 } } },
                                            { DocumentCoverElementId.SubjectName, new() { Position = new() { Left = 0, Top = 0 } } },
                                            { DocumentCoverElementId.GradeTypeName, new() { Position = new() { Left = 0, Top = 0 } } },
                                            { DocumentCoverElementId.GradeName , new() { Position = new() { Left = 0, Top = 0 } } }
                                        };

        public Dictionary<DocumentTextElementId, DocumentTextElementStyle> TextElementStyles { get; set; } = new() {
                                            { DocumentTextElementId.Header1, new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 32, Bold = true, Margins = new() { Bottom = 32 } } },
                                            { DocumentTextElementId.Header2, new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 26, Bold = true, Margins = new() { Bottom = 26 } } },
                                            { DocumentTextElementId.Header3, new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 22, Bold = true, Margins = new() { Bottom = 22 } } },
                                            { DocumentTextElementId.Header4, new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 18, Bold = true, Margins = new() { Bottom = 18 } } },
                                            { DocumentTextElementId.Header5, new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 16, Bold = true, Margins = new() { Bottom = 16 } } },
                                            { DocumentTextElementId.Header6, new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 14, Bold = true, Margins = new() { Bottom = 14 } } },
                                            { DocumentTextElementId.NormalText, new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 12, Margins = new() { Bottom = 12 } } },
                                            { DocumentTextElementId.TableText, new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 10 } },
                                            { DocumentTextElementId.TableHeader1Text, new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.White, FontSize = 10, Bold = true } },
                                            { DocumentTextElementId.TableHeader2Text, new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.White, FontSize = 10, Bold = true } },
                                            { DocumentTextElementId.CoverSubjectCode, new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 18, Bold = true, Margins = new() { Bottom = 18 } } },
                                            { DocumentTextElementId.CoverSubjectName, new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 32, Bold = true, Margins = new() { Bottom = 32 } } },
                                            { DocumentTextElementId.CoverGradeTypeName, new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 22, Bold = true, Margins = new() { Bottom = 22 } } },
                                            { DocumentTextElementId.CoverGradeName, new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 26, Bold = true, Margins = new() { Bottom = 26 } } },
                                            { DocumentTextElementId.IndexLevel1, new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 16, Bold = false, Margins = new() { Bottom = 16 } } },
                                            { DocumentTextElementId.IndexLevel2, new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 14, Bold = false, Margins = new() { Bottom = 14 } } },
                                            { DocumentTextElementId.IndexLevel3, new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 12, Bold = false, Margins = new() { Bottom = 12 } } },
                                            { DocumentTextElementId.IndexTitle, new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 32, Bold = true, Margins = new() { Bottom = 32 }  } },
                                            { DocumentTextElementId.WeightsTableText, new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.Black, FontSize = 10 } },
                                            { DocumentTextElementId.WeightsTableHeader1Text, new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.White, FontSize = 10, Bold = true } },
                                            { DocumentTextElementId.WeightsTableHeader2Text, new() { FontFamily = DocumentTextElementFontFamily.SansSerif, FontColor = DocumentElementColor.White, FontSize = 10, Bold = true } }
                                        };
        public Dictionary<DocumentTableElementId, DocumentTableElementStyle> TableElementStyles { get; set; } = new() {
                                            { DocumentTableElementId.TableNormalCell, new() { BackgroundColor = DocumentElementColor.White, Padding = new() { Top = 0, Bottom = 0, Left = 0, Right = 0 } } },
                                            { DocumentTableElementId.TableHeader1Cell, new() { BackgroundColor = DocumentElementColor.Gray, Padding = new() { Top = 0, Bottom = 0, Left = 0, Right = 0 } } },
                                            { DocumentTableElementId.TableHeader2Cell, new() { BackgroundColor = DocumentElementColor.LightGray, Padding = new() { Top = 0, Bottom = 0, Left = 0, Right = 0 } } },
                                            { DocumentTableElementId.TableWeightsNormalCell, new() { BackgroundColor = DocumentElementColor.White, Padding = new() { Top = 0, Bottom = 0, Left = 0, Right = 0 } } },
                                            { DocumentTableElementId.TableWeightsHeader1Cell, new() { BackgroundColor = DocumentElementColor.Gray, Padding = new() { Top = 0, Bottom = 0, Left = 0, Right = 0 } } },
                                            { DocumentTableElementId.TableWeightsHeader2Cell, new() { BackgroundColor = DocumentElementColor.LightGray, Padding = new() { Top = 0, Bottom = 0, Left = 0, Right = 0 } } }
                                        };

        public DocumentStyle()
        {
            StorageClassId = "documentstyle";

            Title = "Título del estilo";
            Description = "Descripción del estilo";

        }

        public override bool Exists(string storageId, string? parentStorageId)
        {
            return Storage.ExistsData<DocumentStyleData>(storageId, StorageClassId, parentStorageId);
        }

        public override void Save(string? parentStorageId = null)
        {
            base.Save(parentStorageId);

            DocumentStyleData data = new();

            data.Title = Title;
            data.Description = Description;

            data.LogoBase64 = LogoBase64;
            data.CoverBase64 = CoverBase64;
            data.Size = Size;
            data.Orientation = Orientation;
            data.Margins = Margins;

            data.CoverElementStyles = CoverElementStyles.ToList();
            data.TextElementStyles = TextElementStyles.ToList();
            data.TableElementStyles = TableElementStyles.ToList();

            Storage.SaveData<DocumentStyleData>(StorageId, StorageClassId, data, parentStorageId);

        }

        public override void LoadOrCreate(string storageId, string? parentStorageId = null)
        {
            base.LoadOrCreate(storageId, parentStorageId);

            if (!Storage.ExistsData<DocumentStyleData>(storageId, StorageClassId, parentStorageId)) { Save(parentStorageId); }

            DocumentStyleData data = Storage.LoadData<DocumentStyleData>(storageId, StorageClassId, parentStorageId);

            Title = data.Title;
            Description = data.Description;

            LogoBase64 = data.LogoBase64;
            CoverBase64 = data.CoverBase64;
            Size = data.Size;
            Orientation = data.Orientation;
            Margins = data.Margins;

            CoverElementStyles = new(data.CoverElementStyles);
            TextElementStyles = new(data.TextElementStyles);
            TableElementStyles = new(data.TableElementStyles);

        }

        public override void Delete(string? parentStorageId = null)
        {
            base.Delete(parentStorageId);

            Storage.DeleteData(StorageId, StorageClassId, parentStorageId);

        }



    }
}
