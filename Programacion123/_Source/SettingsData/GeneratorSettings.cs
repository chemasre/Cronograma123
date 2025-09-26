namespace Programacion123
{
    public struct GeneratorSettings
    {
        public DocumentStyle DocumentStyle { get; set; }

        public GeneratorSettings()
        {
            DocumentStyle = new();
        }
    }
}
