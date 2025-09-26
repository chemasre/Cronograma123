namespace Programacion123
{
    class DocumentStyleData : StorageData
    {
        public string? LogoBase64 { get; set; }
        public string? CoverBase64 { get; set; }
        public DocumentSize Size { get; set; }
        public DocumentOrientation Orientation { get; set; }
        public DocumentMargins Margins { get; set; }

        public List<KeyValuePair<DocumentCoverElementId, DocumentCoverElementStyle>> CoverElementStyles { get; set; } = new();
        public List<KeyValuePair<DocumentTextElementId, DocumentTextElementStyle>> TextElementStyles { get; set; } = new();
        public List<KeyValuePair<DocumentTableElementId, DocumentTableElementStyle>> TableElementStyles { get; set; } = new();



    }
}
