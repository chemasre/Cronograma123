namespace Programacion123
{
    public class Grade : Entity
    {
        public string Name { get; set; } = "Nombre del ciclo";
        public string FamilyName { get; set; } = "Nombre de la familia profesional";
        public ListProperty<CommonText> GeneralObjectives { get; } = new ListProperty<CommonText>();
        public ListProperty<CommonText> GeneralCompetences { get; } = new ListProperty<CommonText>();
        public ListProperty<CommonText> KeyCapacities { get; } = new ListProperty<CommonText>();


        internal Grade() : base()
        {
            StorageClassId = "grade";
        }

        public override ValidationResult Validate()
        {
            throw new NotImplementedException();
        }
    }
}
