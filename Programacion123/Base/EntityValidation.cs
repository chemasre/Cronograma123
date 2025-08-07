using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    public struct ValidationResult
    {
        public ValidationCode code;
        public int index;

        public static ValidationResult Create(ValidationCode _code) { return new ValidationResult() { code = _code }; }
        public ValidationResult WithIndex(int _index) { index = _index; return this; }

        public override string ToString()
        {
            if (code == ValidationCode.success) { return "No se detectan problemas."; }
            else if (code == ValidationCode.entityTitleEmpty) { return "El título está vacío."; }
            else if (code == ValidationCode.entityDescriptionEmpty) { return "La descripción está vacía."; }
            else if (code == ValidationCode.calendarStartDayAfterEndDay) { return "El día de inicio es posterior al de fin."; }
            else if (code == ValidationCode.calendarFreeDayBeforeStartOrAfterEnd) { return "Hay días festivos anteriores al primer día del curso o posteriores al último."; }
            else if (code == ValidationCode.calendarNoSchoolDays) { return "El calendario del curso no tiene días lectivos."; }
            else if (code == ValidationCode.unitsMissing) { return ""; }
            else if (code == ValidationCode.weekScheduleDayMissing) { return "El horario no incluye ningún día de la semana."; }
            else if (code == ValidationCode.weekScheduleOneHourMinimum) { return "El horario no reserva ninguna hora para el módulo."; }
            else if (code == ValidationCode.subjectNotLinkedToTemplate) { return "El módulo ha de estar vinculado a una plantilla."; }
            else if (code == ValidationCode.subjectNotLinkedToCalendar) { return "El módulo ha de estar vinculado a un calendario."; }
            else if (code == ValidationCode.subjectNotLinkedToWeekSchedule) { return "El módulo ha de estar vinculado a un horario."; }
            else if (code == ValidationCode.subjectMetodologiesInvalid) { return String.Format("La metodología {0} presenta algún problema.", index + 1); }
            else if (code == ValidationCode.subjectTemplateInvalid) { return "La plantilla vinculada a el módulo presenta algún problema."; }
            else if (code == ValidationCode.subjectCalendarInvalid) { return "El calendario vinculado a el módulo presenta algún problema."; }
            else if (code == ValidationCode.subjectWeekScheduleInvalid) { return "El horario vinculado a el módulo presenta algún problema."; }
            else if (code == ValidationCode.subjectSpaceResourceInvalid) { return String.Format("En la pestaña de recursos educativos, el espacio {0} presenta algún problema.", index + 1); }
            else if (code == ValidationCode.subjectMaterialResourceInvalid) { return String.Format("En la pestaña de recursos educativos, el material {0} presenta algún problema.", index + 1); }
            else if (code == ValidationCode.subjectInstrumentTypeInvalid) { return String.Format("El tipo de instrumento de evaluación {0} presenta algún problema.", index + 1); }
            else if (code == ValidationCode.subjectBlockInvalid) { return String.Format("El bloque {0} presenta algún problema.", index + 1); }
            //else if (code == ValidationCode.subjectLearningResultWeightInvalid) { return String.Format("Hay un problema en el peso de resultado de aprendizaje que ocupa la posición {0}.", index + 1); }
            else if (code == ValidationCode.subjectLearningResultsWeightNotHundredPercent) { return "La suma de los pesos de los resultados de aprendizaje en la pestaña evaluación no es 100."; }
            else if (code == ValidationCode.subjectNoMetodologies) { return "El módulo al menos debe contar con una metodología."; }
            else if (code == ValidationCode.subjectNoSpaces) { return "El módulo al menos debe contar con un recurso de tipo espacio."; }
            else if (code == ValidationCode.subjectNoEvaluationInstrumentTypes) { return "El módulo al menos debe contar con un tipo de instrumento de evaluación."; }
            else if (code == ValidationCode.subjectNoBlocks) { return "El módulo al menos debe contar con un bloque."; }
            else if (code == ValidationCode.subjectEvaluationIntroductionInvalid) { return "El texto introductorio a la evaluación presenta algún problema."; }
            else if (code == ValidationCode.subjectLearningResultNotReferencedByActivities) { return String.Format("Ninguna actividad hace referencia al resultado de aprendizaje {0} en sus criterios de evaluación.", index + 1); }
            else if (code == ValidationCode.subjectActivitiesLearningResultWeightNotHundredPercent) { return String.Format("En el resultado de aprendizaje {0}, la suma de los pesos de las actividades evaluables debe ser cien.", index + 1); }
            else if (code == ValidationCode.subjectCalendarAndWeekScheduleLeaveNoSchoolDays) { return "La combinación de calendario y horario no permite ningún día lectivo."; }
            else if (code == ValidationCode.subjectCommonTextInvalid) { return String.Format("El texto común {0} presenta algún problema.",index + 1); }
            else if (code == ValidationCode.subjectCitationInvalid) { return String.Format("La cita {0} presenta algún problema.",index + 1); }
            else if (code == ValidationCode.templateGradeNameEmpty) { return "El nombre del ciclo está vacío."; }
            else if (code == ValidationCode.templateGradeFamilyNameEmpty) { return "El nombre de la familia profesional está vacío."; }
            else if (code == ValidationCode.templateGradeGeneralObjectiveInvalid) { return String.Format("El objetivo general {0} presenta algún problema.", index + 1); }
            else if (code == ValidationCode.templateGradeGeneralCompetenceInvalid) { return String.Format("La competencia general {0} presenta algún problema.", index + 1); }
            else if (code == ValidationCode.templateGradeKeyCapacitiesInvalid) { return String.Format("La capacidad clave {0} presenta algún problema.", index + 1); }
            else if (code == ValidationCode.templateGradeNoGeneralObjectives) { return "La plantilla al menos debe contar con un objetivo general."; }
            else if (code == ValidationCode.templateGradeNoGeneralCompetences) { return "La plantilla al menos debe contar con una competencia general."; }
            else if (code == ValidationCode.templateGradeNoKeyCapacities) { return "La plantilla al menos debe contar con una capacidad clave."; }
            else if (code == ValidationCode.templateGradeCommonTextInvalid) { return String.Format("El texto común {0} presenta algún problema", index + 1); }
            else if (code == ValidationCode.templateSubjectNameEmpty) { return "El nombre oficial del módulo está vacío."; }
            else if (code == ValidationCode.templateSubjectCodeEmpty) { return "El código del módulo está vacío."; }
            else if (code == ValidationCode.templateSubjectClassroomHoursZero) { return "El módulo al menos debe contar con una hora de clase en el centro."; }
            else if (code == ValidationCode.templateSubjectLearningResultsInvalid) { return String.Format("El resultado de aprendizaje {0} presenta algún problema.", index + 1); }
            else if (code == ValidationCode.templateSubjectContentsInvalid) { return String.Format("El contenido {0} presenta algún problema.", index + 1); }
            else if (code == ValidationCode.templateSubjectNoLearningResults) { return "La plantilla al menos debe contar con un resultado de aprendizaje."; }
            else if (code == ValidationCode.templateSubjectNoContents) { return "La plantilla al menos debe incluir un contenido."; }
            else if (code == ValidationCode.activityNotLinkedToMetodology) { return "En la pestaña de datos esenciales, la actividad no está vinculada a ninguna metodología."; }
            else if (code == ValidationCode.activityNotLinkedToContents) { return "La actividad no está vinculada a ningún contenido."; }
            else if (code == ValidationCode.activityNotLinkedToKeyCompetences) { return "La actividad no hace referencia a ninguna competencia clave."; }
            else if (code == ValidationCode.activityEvaluableAndNotLinkedToEvaluationInstrumentType) { return "La actividad es evaluable pero no está vinculada a ningún instrumento de evaluación."; }
            else if (code == ValidationCode.activityEvaluableAndNotLinkedToCriterias) { return "La actividad es evauable pero no está vinculada a ningún criterio de evaluación."; }
            else if (code == ValidationCode.activityEvaluableAndNotLinkedToResultsWeights) { return "La actividad es evaluable pero no tiene peso en ningún resultado de aprendizaje."; }
            else if (code == ValidationCode.activityNotLinkedToSpaceResource) { return "La actividad no está vinculada a ningún recurso didáctico de tipo espacio."; }
            else if (code == ValidationCode.activityReferencesResultWithoutWeight) { return String.Format("El resultado de aprendizaje {0} está referenciado por al menos un criterio de evaluación pero su peso es cero.", index + 1); }
            else if (code == ValidationCode.activityDoesntReferenceResultButHasWeight) { return String.Format("El resultado de aprendizaje {0} tiene un peso mayor que cero y no está referenciado por ningún criterio de evaluación.", index + 1); }
            else if (code == ValidationCode.activityCannotSchedule) { return "La actividad no se puede planificar."; }
            else if (code == ValidationCode.contentNoPoints) { return "El contenido debe incluir al menos un punto."; }
            else if (code == ValidationCode.contentPointInvalid) { return String.Format("El punto {0} presenta algún problema.", index + 1); }
            else if (code == ValidationCode.learningResultNoCriterias) { return "El resultado de aprendizaje al menos debe contar con un criterio."; }
            else if (code == ValidationCode.learningResultCriteriaInvalid) { return String.Format("El criterio {0} presenta algún problema.", index + 1); }
            else if (code == ValidationCode.blockNoActivities) { return "El bloque al menos debe contar con una actividad."; }
            else if (code == ValidationCode.templateSubjectNotLinkedToGradeTemplate) { return "La plantilla de módulo no está vinculada a una plantilla de ciclo."; }
            else if (code == ValidationCode.templateSubjectNoGeneralObjectivesReferenced) { return "La plantilla de módulo no hace referencia a ningún objetivo general."; }
            else if (code == ValidationCode.templateSubjectNoGeneralCompetencesReferenced) { return "La plantilla de módulo no hace referencia a ninguna competencia general."; }
            else // code == ValidationCode.blockActivityInvalid)
            { return String.Format("La actividad {0} presenta algún problema.", index + 1); }

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
        subjectMetodologiesInvalid,
        subjectTemplateInvalid,
        subjectCalendarInvalid,
        subjectWeekScheduleInvalid,
        subjectSpaceResourceInvalid,
        subjectMaterialResourceInvalid,
        subjectInstrumentTypeInvalid,
        subjectBlockInvalid,
        //subjectLearningResultWeightInvalid,
        subjectLearningResultsWeightNotHundredPercent,
        subjectNoMetodologies,
        subjectNoSpaces,
        subjectNoEvaluationInstrumentTypes,
        subjectNoBlocks,
        subjectEvaluationIntroductionInvalid,
        subjectLearningResultNotReferencedByActivities,
        subjectActivitiesLearningResultWeightNotHundredPercent,
        subjectCalendarAndWeekScheduleLeaveNoSchoolDays,
        subjectCommonTextInvalid,
        subjectCitationInvalid,

        templateGradeNameEmpty,
        templateGradeFamilyNameEmpty,
        templateGradeGeneralObjectiveInvalid,
        templateGradeGeneralCompetenceInvalid,
        templateGradeKeyCapacitiesInvalid,
        templateGradeNoGeneralObjectives,
        templateGradeNoGeneralCompetences,
        templateGradeNoKeyCapacities,
        templateGradeCommonTextInvalid,

        templateSubjectNameEmpty,
        templateSubjectCodeEmpty,
        templateSubjectClassroomHoursZero,
        templateSubjectLearningResultsInvalid,
        templateSubjectContentsInvalid,
        templateSubjectNoLearningResults,
        templateSubjectNoContents,
        templateSubjectNotLinkedToGradeTemplate,
        templateSubjectNoGeneralObjectivesReferenced,
        templateSubjectNoGeneralCompetencesReferenced,

        activityNotLinkedToMetodology,
        activityNotLinkedToContents,
        activityNotLinkedToKeyCompetences,
        activityNotLinkedToSpaceResource,
        activityEvaluableAndNotLinkedToEvaluationInstrumentType,
        activityEvaluableAndNotLinkedToCriterias,
        activityEvaluableAndNotLinkedToResultsWeights,
        activityReferencesResultWithoutWeight,
        activityDoesntReferenceResultButHasWeight,
        activityCannotSchedule,

        contentNoPoints,
        contentPointInvalid,
        learningResultNoCriterias,
        learningResultCriteriaInvalid,
        blockNoActivities,
        blockActivityInvalid,
    };
}
