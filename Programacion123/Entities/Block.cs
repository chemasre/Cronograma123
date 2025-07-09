namespace Programacion123
{
    public class Block: Entity
    {
        public ListProperty<Activity> Activities { get; } = new ListProperty<Activity>();

        internal Block() : base()
        {
            StorageClassId = "block";
        }

        public override ValidationResult Validate()
        {
            throw new NotImplementedException();
        }
    }
}
