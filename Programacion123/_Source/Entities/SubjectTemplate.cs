namespace Programacion123
{
    public class SubjectTemplate : Entity
    {
        public EntityProperty<GradeTemplate?> GradeTemplate { get; } = new (null);

        public Property<string> SubjectName { get; } = new ("Nombre completo del módulo");
        public Property<string> SubjectCode { get; } = new ("Código del módulo");
        public Property<int> GradeClassroomHours { get; } = new (100);
        public Property<int> GradeCompanyHours { get; } = new (50);
        public ListEntityProperty<CommonText> GeneralObjectives { get; } = new ListEntityProperty<CommonText>();
        public ListEntityProperty<CommonText> GeneralCompetences { get; } = new ListEntityProperty<CommonText>();
        public ListEntityProperty<LearningResult> LearningResults { get; } = new ListEntityProperty<LearningResult>();
        public ListEntityProperty<Content> Contents { get; } = new ListEntityProperty<Content>();

        const uint flagGradeTemplate            = 1 << 2;
        const uint flagSubjectName              = 1 << 3;
        const uint flagSubjectCode              = 1 << 4;
        const uint flagGradeClassroomHours      = 1 << 5;
        const uint flagGradeCompanyHours        = 1 << 6;
        const uint flagGradeGeneralObjectives   = 1 << 7;
        const uint flagGradeGeneralCompetences  = 1 << 8;
        const uint flagGradeLearningResults     = 1 << 9;
        const uint flagGradeContents            = 1 << 10;

        const uint flagAll = ~0U;

        public SubjectTemplate() : base()
        {
            StorageClassId = "subjecttemplate";

            Title.Value = "Título de la plantilla de módulo";
            Description.Value = "Descripción de la plantilla de módulo";

            GradeTemplate.OnSetted += (current, next) => { Flags.Add(ref validationFlags, flagGradeTemplate); InvokeOnUpdated(); };
            GradeTemplate.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagGradeTemplate); InvokeOnUpdated(); };

            SubjectName.OnSetted += (current, next) => { Flags.Add(ref validationFlags, flagSubjectName); InvokeOnUpdated(); };
            SubjectCode.OnSetted += (current, next) => { Flags.Add(ref validationFlags, flagSubjectCode); InvokeOnUpdated(); };
            GradeClassroomHours.OnSetted += (current, next) => { Flags.Add(ref validationFlags, flagGradeClassroomHours); InvokeOnUpdated(); };
            GradeCompanyHours.OnSetted += (current, next) => { Flags.Add(ref validationFlags, flagGradeCompanyHours); InvokeOnUpdated(); };

            GeneralObjectives.OnAdded += (element) => { Flags.Add(ref validationFlags, flagGradeGeneralObjectives); InvokeOnUpdated(); };
            GeneralObjectives.OnRemoved += (element) => { Flags.Add(ref validationFlags, flagGradeGeneralObjectives); InvokeOnUpdated(); };
            GeneralObjectives.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagGradeGeneralObjectives); InvokeOnUpdated(); };

            GeneralCompetences.OnAdded += (element) => { Flags.Add(ref validationFlags, flagGradeGeneralCompetences); InvokeOnUpdated(); };
            GeneralCompetences.OnRemoved += (element) => { Flags.Add(ref validationFlags, flagGradeGeneralCompetences); InvokeOnUpdated(); };
            GeneralCompetences.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagGradeGeneralCompetences); InvokeOnUpdated(); };

            LearningResults.OnAdded += (element) => { Flags.Add(ref validationFlags, flagGradeLearningResults); InvokeOnUpdated(); };
            LearningResults.OnRemoved += (element) => { Flags.Add(ref validationFlags, flagGradeLearningResults); InvokeOnUpdated(); };
            LearningResults.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagGradeLearningResults); InvokeOnUpdated(); };

            Contents.OnAdded += (element) => { Flags.Add(ref validationFlags, flagGradeContents); InvokeOnUpdated(); };
            Contents.OnRemoved += (element) => { Flags.Add(ref validationFlags, flagGradeContents); InvokeOnUpdated(); };
            Contents.OnEntityUpdated += (entity) => { Flags.Add(ref validationFlags, flagGradeContents); InvokeOnUpdated(); };

            Flags.Add(ref validationFlags, flagGradeTemplate);            
            Flags.Add(ref validationFlags, flagSubjectName);              
            Flags.Add(ref validationFlags, flagSubjectCode);              
            Flags.Add(ref validationFlags, flagGradeClassroomHours);      
            Flags.Add(ref validationFlags, flagGradeCompanyHours);        
            Flags.Add(ref validationFlags, flagGradeGeneralObjectives);   
            Flags.Add(ref validationFlags, flagGradeGeneralCompetences);  
            Flags.Add(ref validationFlags, flagGradeLearningResults);     
            Flags.Add(ref validationFlags, flagGradeContents);            
        }

        public override ValidationResult Validate(bool force = false)
        {
            base.Validate(force);

            if(Flags.Test(validationFlags, flagGradeTemplate) || force)
            {
                Utils.Log("Validating grade template linked and valid", "gradeTemplate");

                validationFails.RemoveAll(e => e.code == ValidationCode.templateSubjectNotLinkedToGradeTemplate);

                if (GradeTemplate.Value == null) { validationFails.Add(ValidationResult.Create(ValidationCode.templateSubjectNotLinkedToGradeTemplate)); }
                else if(GradeTemplate.Value.Validate(force).code != ValidationCode.success) { validationFails.Add(ValidationResult.Create(ValidationCode.templateSubjectLinkedGradeTemplateInvalid)); }
            }

            if(Flags.Test(validationFlags, flagSubjectName) || force)
            {
                Utils.Log("Validating subject name not empty", "subjectName");

                validationFails.RemoveAll(e => e.code == ValidationCode.templateSubjectNameEmpty);

                if (SubjectName.Value.Trim().Length <= 0) { validationFails.Add(ValidationResult.Create(ValidationCode.templateSubjectNameEmpty)); }
            }

            if(Flags.Test(validationFlags, flagSubjectCode) || force)
            {
                Utils.Log("Validating subject code not empty", "subjectCode");

                validationFails.RemoveAll(e => e.code == ValidationCode.templateSubjectCodeEmpty);

                if (SubjectCode.Value.Trim().Length <= 0) { validationFails.Add(ValidationResult.Create(ValidationCode.templateSubjectCodeEmpty)); }
            }

            if(Flags.Test(validationFlags, flagGradeClassroomHours) || force)
            {
                Utils.Log("Validating some classroom hour exist", "classRoomHours");

                validationFails.RemoveAll(e => e.code == ValidationCode.templateSubjectClassroomHoursZero);

                if (GradeClassroomHours.Value <= 0) { validationFails.Add(ValidationResult.Create(ValidationCode.templateSubjectClassroomHoursZero)); }
            }

            if(Flags.Test(validationFlags, flagGradeGeneralObjectives) || force)
            {
                Utils.Log("Validating some objective exist", "generalObjectives");

                validationFails.RemoveAll(e => e.code == ValidationCode.templateSubjectNoGeneralObjectivesReferenced);

                List<CommonText> objectivesList = GeneralObjectives.ToList();
                if (objectivesList.Count <= 0) { validationFails.Add(ValidationResult.Create(ValidationCode.templateSubjectNoGeneralObjectivesReferenced)); }
            }

            if(Flags.Test(validationFlags, flagGradeGeneralCompetences) || force)
            {
                Utils.Log("Validating some competence exist", "generalCompetences");

                validationFails.RemoveAll(e => e.code == ValidationCode.templateSubjectNoGeneralCompetencesReferenced);

                List<CommonText> competencesList = GeneralCompetences.ToList();
                if (competencesList.Count <= 0) { validationFails.Add(ValidationResult.Create(ValidationCode.templateSubjectNoGeneralCompetencesReferenced)); }
            }

            if(Flags.Test(validationFlags, flagGradeLearningResults) || force)
            {
                Utils.Log("Validating some learning result exist and all are valid", "learningResults");

                validationFails.RemoveAll(e => e.code == ValidationCode.templateSubjectNoLearningResults);
                validationFails.RemoveAll(e => e.code == ValidationCode.templateSubjectLearningResultsInvalid);

                List<LearningResult> resultsList = LearningResults.ToList();
                if (resultsList.Count <= 0) { validationFails.Add(ValidationResult.Create(ValidationCode.templateSubjectNoLearningResults)); }
                for (int i = 0; i < resultsList.Count; i++) { if (resultsList[i].Validate(force).code != ValidationCode.success) { validationFails.Add(ValidationResult.Create(ValidationCode.templateSubjectLearningResultsInvalid).WithIndex(i)); } }
            }

            if(Flags.Test(validationFlags, flagGradeContents) || force)
            {
                Utils.Log("Validating some content point exist and all are valid", "contents");

                validationFails.RemoveAll(e => e.code == ValidationCode.templateSubjectNoContents);
                validationFails.RemoveAll(e => e.code == ValidationCode.templateSubjectContentsInvalid);

                List<Content> contentsList = Contents.ToList();
                if (contentsList.Count <= 0) { validationFails.Add(ValidationResult.Create(ValidationCode.templateSubjectNoContents)); }
                for (int i = 0; i < contentsList.Count; i++) { if (contentsList[i].Validate(force).code != ValidationCode.success) { validationFails.Add(ValidationResult.Create(ValidationCode.templateSubjectContentsInvalid).WithIndex(i)); } }
            }

            Flags.Remove(ref validationFlags, flagGradeTemplate);            
            Flags.Remove(ref validationFlags, flagSubjectName);              
            Flags.Remove(ref validationFlags, flagSubjectCode);              
            Flags.Remove(ref validationFlags, flagGradeClassroomHours);      
            Flags.Remove(ref validationFlags, flagGradeCompanyHours);        
            Flags.Remove(ref validationFlags, flagGradeGeneralObjectives);   
            Flags.Remove(ref validationFlags, flagGradeGeneralCompetences);  
            Flags.Remove(ref validationFlags, flagGradeLearningResults);     
            Flags.Remove(ref validationFlags, flagGradeContents);            

            foreach(ValidationResult fail in validationFails) { Utils.Log(fail.ToString() + " (" + fail.index + ")", "FAILED"); }

            if(validationFails.Count == 0) { return ValidationResult.Create(ValidationCode.success); }
            else { return validationFails[0]; }

        }

        public override bool Exists(string storageId, string? parentStorageId)
        {
            return Storage.ExistsData<SubjectTemplateData>(storageId, StorageClassId, parentStorageId);
        }

        public override void Save(string? parentStorageId = null)
        {
            base.Save(parentStorageId);

            SubjectTemplateData data = new();

            data.Title = Title.Value;
            data.Description = Description.Value;

            data.GradeTemplateWeakStorageId = GradeTemplate.Value?.StorageId;

            data.SubjectName = SubjectName.Value;
            data.SubjectCode = SubjectCode.Value;
            data.GradeClassroomHours = GradeClassroomHours.Value;
            data.GradeCompanyHours = GradeCompanyHours.Value;

            List<CommonText> list = GeneralObjectives.ToList();
            data.GeneralObjectivesWeakStorageIds = Storage.GetStorageIds<CommonText>(list);

            list = GeneralCompetences.ToList();
            data.GeneralCompetencesWeakStorageIds = Storage.GetStorageIds<CommonText>(list);

            List<LearningResult> listLearningResults = LearningResults.ToList();
            listLearningResults.ForEach(e => e.Save(StorageId));
            data.LearningResultsStorageIds = Storage.GetStorageIds<LearningResult>(listLearningResults);

            List<Content> listContents = Contents.ToList();
            listContents.ForEach(e => e.Save(StorageId));
            data.ContentsStorageIds = Storage.GetStorageIds<Content>(listContents);

            Storage.SaveData<SubjectTemplateData>(StorageId, StorageClassId, data, parentStorageId);

        }

        public override void LoadOrCreate(string storageId, string? parentStorageId = null)
        {
            base.LoadOrCreate(storageId, parentStorageId);

            if (!Storage.ExistsData<SubjectTemplateData>(storageId, StorageClassId, parentStorageId)) { Save(parentStorageId); }

            SubjectTemplateData data = Storage.LoadData<SubjectTemplateData>(storageId, StorageClassId, parentStorageId);

            Title.Value = data.Title;
            Description.Value = data.Description;

            GradeTemplate.Value = data.GradeTemplateWeakStorageId != null ? Storage.FindEntity<GradeTemplate>(data.GradeTemplateWeakStorageId, null) : null;

            SubjectName.Value = data.SubjectName;
            SubjectCode.Value = data.SubjectCode;
            GradeClassroomHours.Value = data.GradeClassroomHours;
            GradeCompanyHours.Value = data.GradeCompanyHours;

            if(GradeTemplate.Value != null)
            {
                GeneralObjectives.Set(Storage.FindSiblingEntities<CommonText>(data.GeneralObjectivesWeakStorageIds, GradeTemplate.Value.StorageId));
                GeneralCompetences.Set(Storage.FindSiblingEntities<CommonText>(data.GeneralCompetencesWeakStorageIds, GradeTemplate.Value.StorageId));
            }

            LearningResults.Set(Storage.LoadOrCreateEntities<LearningResult>(data.LearningResultsStorageIds, storageId));

            Contents.Set(Storage.LoadOrCreateEntities<Content>(data.ContentsStorageIds, storageId));

        }

        public override void Delete(string? parentStorageId = null)
        {
            base.Delete(parentStorageId);

            LearningResults.ToList().ForEach(e => e.Delete(StorageId));
            Contents.ToList().ForEach(e => e.Delete(StorageId));

            Storage.DeleteData(StorageId, StorageClassId, parentStorageId);



        }
    }
}
