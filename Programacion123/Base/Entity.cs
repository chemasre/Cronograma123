using System.Numerics;

namespace Programacion123
{
    public enum StorageState
    {
        detached,
        dirty,
        saved
    };


    public abstract class Entity
    {

        protected StorageState StorageState { get { return storageState; } }

        protected StorageState storageState;

        public struct ValidationResult
        {
            public ValidationCode code;
            public int index;

            public static ValidationResult Create(ValidationCode _code) { return new ValidationResult() { code = _code }; }
            public ValidationResult WithIndex(int _index) { index = _index; return this; }

            public override string ToString()
            {
                if(code == ValidationCode.success) { return "No se detectan problemas"; }
                else if(code == ValidationCode.entityTitleEmpty) { return "Debes escribir un título"; }
                else if(code == ValidationCode.entityDescriptionEmpty) { return "Debes escribir una descripción"; }
                else if(code == ValidationCode.calendarStartDayAfterEndDay) { return "El día de inicio no puede ser posterior al de fin"; }
                else if(code == ValidationCode.calendarFreeDayBeforeStartOrAfterEnd) { return "Tienes que eliminar los días festivos que están antes del primer día del curso o después del último"; }
                else if(code == ValidationCode.calendarNoSchoolDays) { return "El calendario tiene que incluir al menos un día lectivo"; }
                else if(code == ValidationCode.unitsMissing) { return ""; }
                else if(code == ValidationCode.weekScheduleDayMissing) { return "El horario debe incluir al menos un día de la semana"; }
                else if(code == ValidationCode.weekScheduleOneHourMinimum) { return "Tienes que poner al menos una hora"; }
                else if(code == ValidationCode.subjectNotLinkedToTemplate) { return "Debes vincular una plantilla con la asignatura"; }
                else if(code == ValidationCode.subjectNotLinkedToCalendar) { return "Debes vincular un calendario a la asignatura"; }
                else if(code == ValidationCode.subjectNotLinkedToWeekSchedule) { return "Debes vincular un horario a la asignatura"; }
                else if(code == ValidationCode.subjectMetodologiesIntroductionInvalid) { return "Debes revisar la introducción a las metodologías. Hay algún problema en ella."; }
                else if(code == ValidationCode.subjectMetodologiesInvalid) { return String.Format("Debes revisar la metodología que ocupa la posición {0}. Hay algún problema en ella.", index + 1); }
                else if(code == ValidationCode.subjectTemplateInvalid) { return "Debes revisar la plantilla vinculada a la asignatura. Hay algún problema en ella."; }
                else if(code == ValidationCode.subjectCalendarInvalid) { return "Debes revisar el calendario vinculado a la asignatura. Hay algún problema en él."; }
                else if(code == ValidationCode.subjectWeekScheduleInvalid) { return "Debes revisar el horario vinculado a la asignatura. Hay algún problema en él."; }
                else if(code == ValidationCode.subjectResourcesIntroductionInvalid) { return "Debes revisar la introducción a los recursos. Hay algún problema en ella"; }
                else if(code == ValidationCode.subjectSpaceResourceInvalid) { return ""; }
                else if(code == ValidationCode.subjectMaterialResourceInvalid) { return ""; }
                else if(code == ValidationCode.subjectEvaluationInstrumentTypesIntroductionInvalid) { return "Debes revisar la introducción a los tipos de instrumento de evaluación. Hay algún problema en ella"; }
                else if(code == ValidationCode.subjectInstrumentTypeInvalid) { return ""; }
                else if(code == ValidationCode.subjectBlocksIntroductionInvalid) { return "Debes revisar la introducción a los bloques. Hay algún problema en ella"; }
                else if(code == ValidationCode.subjectBlockInvalid) { return ""; }
                else if(code == ValidationCode.subjectLearningResultWeightInvalid) { return ""; }
                else if(code == ValidationCode.subjectLearningResultsWeightNotHundredPercent) { return ""; }
                else if(code == ValidationCode.templateSubjectNameEmpty) { return ""; }
                else if(code == ValidationCode.templateSubjectCodeEmpty) { return ""; }
                else if(code == ValidationCode.templateGradeNameEmpty) { return ""; }
                else if(code == ValidationCode.templateClassroomHoursZero) { return ""; }
                else if(code == ValidationCode.templateGradeFamilyNameEmpty) { return ""; }
                else if(code == ValidationCode.templateGeneralObjectivesIntroductionInvalid) { return "Debes revisar la introducción a los objetivos generales. Hay algún problema en ella"; }
                else if(code == ValidationCode.templateGeneralObjectiveInvalid) { return ""; }
                else if(code == ValidationCode.templateGeneralCompetencesIntroductionInvalid) { return "Debes revisar la introducción a las competencias generales. Hay algún problema en ella"; }
                else if(code == ValidationCode.templateGeneralCompetenceInvalid) { return ""; }
                else if(code == ValidationCode.templateKeyCapacitiesIntroductionInvalid) { return "Debes revisar la introducción a las capacidades clave. Hay algún problema en ella"; }
                else if(code == ValidationCode.templateKeyCapacitiesInvalid) { return ""; }
                else if(code == ValidationCode.templateLearningResultsIntroductionInvalid) { return "Debes revisar la introducción a los resultados de aprendizaje. Hay algún problema en ella"; }
                else if(code == ValidationCode.templateLearningResultsInvalid) { return ""; }
                else if(code == ValidationCode.templateContentsIntroductionInvalid) { return "Debes revisar la introducción a los contenidos. Hay algún problema en ella"; }
                else if(code == ValidationCode.templateContentsInvalid) { return ""; }
                else if(code == ValidationCode.activityNotLinkedToMetodology) { return ""; }
                else if(code == ValidationCode.activityNotLinkedToContents) { return ""; }
                else if(code == ValidationCode.activityEvaluableAndNotLinkedToEvaluationInstrumentType) { return ""; }
                else if(code == ValidationCode.activityEvaluableAndNotLinkedToCriterias) { return ""; }
                else if(code == ValidationCode.activityEvaluableAndNotLinkedToResultsWeights) { return ""; }
                else if(code == ValidationCode.activityReferencesResultWithoutWeight) { return ""; }
                else if(code == ValidationCode.activityDoesntReferenceResultButHasWeight) { return ""; }
                else if(code == ValidationCode.contentPointInvalid) { return ""; }
                else if(code == ValidationCode.learningResultCriteriaInvalid) { return ""; }
                else // code == ValidationCode.blockActivityInvalid)
                {  return ""; }

            }
        }

        public enum ValidationCode
        {
            success,
            entityTitleEmpty,
            entityDescriptionEmpty,
            calendarStartDayAfterEndDay, // Calendar
            calendarFreeDayBeforeStartOrAfterEnd,
            calendarNoSchoolDays,
            unitsMissing, // Subject
            weekScheduleDayMissing,
            weekScheduleOneHourMinimum, // Unit
            subjectNotLinkedToTemplate,
            subjectNotLinkedToCalendar,
            subjectNotLinkedToWeekSchedule,
            subjectMetodologiesIntroductionInvalid,
            subjectMetodologiesInvalid,
            subjectTemplateInvalid,
            subjectCalendarInvalid,
            subjectWeekScheduleInvalid,
            subjectResourcesIntroductionInvalid,
            subjectSpaceResourceInvalid,
            subjectMaterialResourceInvalid,
            subjectEvaluationInstrumentTypesIntroductionInvalid,
            subjectInstrumentTypeInvalid,
            subjectBlocksIntroductionInvalid,
            subjectBlockInvalid,
            subjectLearningResultWeightInvalid,
            subjectLearningResultsWeightNotHundredPercent,
            templateSubjectNameEmpty,
            templateSubjectCodeEmpty,
            templateGradeNameEmpty,
            templateClassroomHoursZero,
            templateGradeFamilyNameEmpty,
            templateGeneralObjectivesIntroductionInvalid,
            templateGeneralObjectiveInvalid,
            templateGeneralCompetencesIntroductionInvalid,
            templateGeneralCompetenceInvalid,
            templateKeyCapacitiesIntroductionInvalid,
            templateKeyCapacitiesInvalid,
            templateLearningResultsIntroductionInvalid,
            templateLearningResultsInvalid,
            templateContentsIntroductionInvalid,
            templateContentsInvalid,
            activityNotLinkedToMetodology,
            activityNotLinkedToContents,
            activityEvaluableAndNotLinkedToEvaluationInstrumentType,
            activityEvaluableAndNotLinkedToCriterias,
            activityEvaluableAndNotLinkedToResultsWeights,
            activityReferencesResultWithoutWeight,
            activityDoesntReferenceResultButHasWeight,
            contentPointInvalid,
            learningResultCriteriaInvalid,
            blockActivityInvalid
        };

        public string StorageId { get; set; }
        public string StorageClassId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public Entity()
        {
            Title = "Sin título";
            Description = "Sin descripción";
            StorageId = Guid.NewGuid().ToString();
            storageState = StorageState.detached;
        }

        public virtual ValidationResult Validate()
        {
            if(Title.Trim().Length <= 0) { return ValidationResult.Create(ValidationCode.entityTitleEmpty); }
            else if(Description.Trim().Length <= 0) { return ValidationResult.Create(ValidationCode.entityDescriptionEmpty); }
            else { return ValidationResult.Create(ValidationCode.success); }
        }

        public virtual void SetDirty()
        {
            storageState = StorageState.dirty;
        }

        public virtual bool Exists(string storageId, string? parentStorageId)
        {
            return false;
        }

        public virtual void LoadOrCreate(string storageId, string? parentStorageId = null)
        {
            StorageId = storageId;
            storageState = StorageState.saved;
        }

        public virtual void Save(string? parentStorageId = null)
        {
            storageState = StorageState.saved;
        }

        public virtual void Delete(string? parentStorageId = null)
        {
            storageState = StorageState.detached;
        }

    }
}
