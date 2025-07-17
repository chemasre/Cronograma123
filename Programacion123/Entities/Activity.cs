using Microsoft.Office.Interop.Word;

namespace Programacion123
{
    public class Activity: Entity
    {
        public int Hours { get; set; }

        public CommonText? Metodology = null;

        public SetProperty<CommonText> ContentPoints = new SetProperty<CommonText>();
        public SetProperty<CommonText> SpaceResources = new SetProperty<CommonText>();
        public SetProperty<CommonText> MaterialResources = new SetProperty<CommonText>();

        public bool IsEvaluable = false;
        public SetProperty<CommonText> Criterias = new SetProperty<CommonText>();
        public SetProperty<CommonText> EvaluationInstrumentTypes = new SetProperty<CommonText>();

        public Activity() : base()
        {
            Title = "Actividad sin título";
            Description = "Actividad sin descripción";

            StorageClassId = "activity";
        }

        public override ValidationResult Validate()
        {
            return base.Validate();
        }

        public override bool Exists(string storageId, string? parentStorageId)
        {
            return Storage.ExistsData<ActivityData>(storageId, StorageClassId, parentStorageId);
        }

        public override void Save(string? parentStorageId = null)
        {
            base.Save(parentStorageId);

            ActivityData data = new();

            data.Title = Title;
            data.Description = Description;

            data.MetodologyWeakStorageId = Metodology?.StorageId;

            List<CommonText> list = ContentPoints.ToList();
            data.ContentPointsWeakStorageIds = Storage.GetStorageIds<CommonText>(list);

            list = SpaceResources.ToList();
            data.SpaceResourcesWeakStorageIds = Storage.GetStorageIds<CommonText>(list);

            list = SpaceResources.ToList();
            data.MaterialResourcesWeakStorageIds = Storage.GetStorageIds<CommonText>(list);

            data.IsEvaluable = IsEvaluable;

            list = Criterias.ToList();
            data.CriteriasWeakStorageIds = Storage.GetStorageIds<CommonText>(list);

            list = EvaluationInstrumentTypes.ToList();
            data.EvaluationInstrumentTypesWeakStorageIds = Storage.GetStorageIds<CommonText>(list);


            Storage.SaveData<ActivityData>(StorageId, StorageClassId, data, parentStorageId);

        }

        public override void LoadOrCreate(string storageId, string? parentStorageId = null)
        {
            base.LoadOrCreate(storageId, parentStorageId);

            if(!Storage.ExistsData<ActivityData>(storageId, StorageClassId, parentStorageId)) { Save(parentStorageId); }

            ActivityData data = Storage.LoadData<ActivityData>(storageId, StorageClassId, parentStorageId);
            
            Title = data.Title;
            Description = data.Description;

            string subjectStorageId = Storage.FindParentStorageId(Storage.FindParentStorageId(StorageId, StorageClassId), new Block().StorageClassId);
            Metodology = data.MetodologyWeakStorageId != null ? Storage.LoadEntityIfExists<CommonText>(data.MetodologyWeakStorageId, subjectStorageId) : null;

            SpaceResources.Set(Storage.FindAndLoadChildEntities<CommonText>(data.SpaceResourcesWeakStorageIds));
            MaterialResources.Set(Storage.FindAndLoadChildEntities<CommonText>(data.MaterialResourcesWeakStorageIds));

            IsEvaluable = data.IsEvaluable;

            EvaluationInstrumentTypes.Set(Storage.FindAndLoadChildEntities<CommonText>(data.EvaluationInstrumentTypesWeakStorageIds));
            ContentPoints.Set(Storage.FindAndLoadChildEntities<CommonText>(data.ContentPointsWeakStorageIds));
            Criterias.Set(Storage.FindAndLoadChildEntities<CommonText>(data.CriteriasWeakStorageIds));

        }

        public override void Delete(string? parentStorageId = null)
        {
            base.Delete(parentStorageId);

            Storage.DeleteData(StorageId, StorageClassId, parentStorageId);
        }


    }
}
