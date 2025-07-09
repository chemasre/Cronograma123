namespace Programacion123
{
    public class Content : Entity
    {
        public ListProperty<CommonText> Detail { get; } = new ListProperty<CommonText>();

        internal Content() : base()
        {
            StorageClassId = "content";
        }

        public override ValidationResult Validate()
        {
            throw new NotImplementedException();
        }
    }
}
