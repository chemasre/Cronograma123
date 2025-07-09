namespace Programacion123
{
    public class LearningResult : Entity
    {
        public ListProperty<EvaluationCriteria> Criterias { get; } = new ListProperty<EvaluationCriteria>();

        internal LearningResult() : base()
        {
            StorageClassId = "learningresult";
        }

        public override ValidationResult Validate()
        {
            throw new NotImplementedException();
        }
    }
}
