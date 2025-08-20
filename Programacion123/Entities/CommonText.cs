namespace Programacion123
{
    public enum CommonTextId
    {
        header1ModuleOrganization,

        header1ImportanceJustification,

        header1CurricularElements,
        header2GeneralObjectives,
        header2GeneralCompetences,
        header2KeyCompetences,

        header1MetodologyAndDidacticOrientations,
        header2Metodology,
        header2Diversity,

        header1EvaluationSystem,
        header2Evaluation,
        header2EvaluationTypes,
        header3OrdinaryEvaluation,
        header3ExtraordinaryEvaluation,

        header2EvaluationInstruments,
        header2EvaluationOfProgramming,

        header1TraversalElements,
        header2TraversalReadingAndTIC,
        header2TraversalCommunicationEntrepreneurshipAndEducation,

        header1Resources,
        header2ResourcesSpaces,
        header2ResourcesMaterialAndTools,

        header1SubjectProgramming,
        header2LearningResultsAndContents,
        header3LearningResults,
        header3Contents,
        header2Blocks,
        header2Activities,
            
    }



    public class CommonText: Entity
    {

        public CommonText() : base()
        {
            StorageClassId = "commontext";

            Title = "Escribe un título";
            Description = "Escribe un texto";

        }

        public override ValidationResult Validate()
        {
            return base.Validate();
        }

        public override bool Exists(string storageId, string? parentStorageId)
        {
            return Storage.ExistsData<CommonTextData>(storageId, StorageClassId, parentStorageId);
        }

        public override void Save(string? parentStorageId = null)
        {
            base.Save(parentStorageId);

            CommonTextData data = new();
            data.Title = Title;
            data.Description = Description;

            Storage.SaveData<CommonTextData>(StorageId, StorageClassId, data, parentStorageId);
        }

        public override void LoadOrCreate(string storageId, string? parentStorageId = null)
        {
            base.LoadOrCreate(storageId, parentStorageId);

            if(!Storage.ExistsData<CommonTextData>(storageId, StorageClassId, parentStorageId)) { Save(parentStorageId); }

            CommonTextData data = Storage.LoadData<CommonTextData>(storageId, StorageClassId, parentStorageId);

            Title = data.Title;
            Description = data.Description;

        }

        public override void Delete(string? parentStorageId = null)
        {
            base.Delete(parentStorageId);

            Storage.DeleteData(StorageId, StorageClassId, parentStorageId);
        }
    }
}
