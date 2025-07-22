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
            if (code == ValidationCode.success) { return "No se detectan problemas"; }
            else if (code == ValidationCode.entityTitleEmpty) { return "Debes escribir un título"; }
            else if (code == ValidationCode.entityDescriptionEmpty) { return "Debes escribir una descripción"; }
            else if (code == ValidationCode.calendarStartDayAfterEndDay) { return "El día de inicio no puede ser posterior al de fin"; }
            else if (code == ValidationCode.calendarFreeDayBeforeStartOrAfterEnd) { return "Tienes que eliminar los días festivos que están antes del primer día del curso o después del último"; }
            else if (code == ValidationCode.calendarNoSchoolDays) { return "El calendario tiene que incluir al menos un día lectivo"; }
            else if (code == ValidationCode.unitsMissing) { return ""; }
            else if (code == ValidationCode.weekScheduleDayMissing) { return "El horario debe incluir al menos un día de la semana"; }
            else if (code == ValidationCode.weekScheduleOneHourMinimum) { return "Tienes que poner al menos una hora"; }
            else if (code == ValidationCode.subjectNotLinkedToTemplate) { return "Debes vincular una plantilla con la asignatura"; }
            else if (code == ValidationCode.subjectNotLinkedToCalendar) { return "Debes vincular un calendario a la asignatura"; }
            else if (code == ValidationCode.subjectNotLinkedToWeekSchedule) { return "Debes vincular un horario a la asignatura"; }
            else if (code == ValidationCode.subjectMetodologiesIntroductionInvalid) { return "Debes revisar la introducción a las metodologías. Hay algún problema en ella."; }
            else if (code == ValidationCode.subjectMetodologiesInvalid) { return String.Format("Debes revisar la metodología que ocupa la posición {0}. Hay algún problema en ella.", index + 1); }
            else if (code == ValidationCode.subjectTemplateInvalid) { return "Debes revisar la plantilla vinculada a la asignatura. Hay algún problema en ella."; }
            else if (code == ValidationCode.subjectCalendarInvalid) { return "Debes revisar el calendario vinculado a la asignatura. Hay algún problema en él."; }
            else if (code == ValidationCode.subjectWeekScheduleInvalid) { return "Debes revisar el horario vinculado a la asignatura. Hay algún problema en él."; }
            else if (code == ValidationCode.subjectResourcesIntroductionInvalid) { return "Debes revisar la introducción a los recursos. Hay algún problema en ella"; }
            else if (code == ValidationCode.subjectSpaceResourceInvalid) { return String.Format("Debes revisar el espacio que ocupa la posición {0}", index + 1); }
            else if (code == ValidationCode.subjectMaterialResourceInvalid) { return String.Format("Debes revisar el material que ocupa la posición {0}", index + 1); }
            else if (code == ValidationCode.subjectEvaluationInstrumentTypesIntroductionInvalid) { return "Debes revisar la introducción a los tipos de instrumento de evaluación. Hay algún problema en ella"; }
            else if (code == ValidationCode.subjectInstrumentTypeInvalid) { return "Debes revisar el tipo de instrumento de evaluación. Hay algún problema en él"; }
            else if (code == ValidationCode.subjectBlocksIntroductionInvalid) { return "Debes revisar la introducción a los bloques. Hay algún problema en ella"; }
            else if (code == ValidationCode.subjectBlockInvalid) { return String.Format("Hay un problema en el bloque que ocupa la posición {0}", index + 1); }
            else if (code == ValidationCode.subjectLearningResultWeightInvalid) { return String.Format("Hay un problema en el peso de resultado de aprendizaje que ocupa la posición {0}", index + 1); }
            else if (code == ValidationCode.subjectLearningResultsWeightNotHundredPercent) { return "Debes revisar la suma de los pesos de los resultados de aprendizaje porque no suma 100"; }
            else if (code == ValidationCode.templateSubjectNameEmpty) { return "El nombre oficial del módulo no puede estar vacío"; }
            else if (code == ValidationCode.templateSubjectCodeEmpty) { return "El código del módulo no puede estar vacío"; }
            else if (code == ValidationCode.templateGradeNameEmpty) { return "El nombre del ciclo no puede estar vacío"; }
            else if (code == ValidationCode.templateClassroomHoursZero) { return "La asignatura debe incluir al menos una hora de clase en el centro"; }
            else if (code == ValidationCode.templateGradeFamilyNameEmpty) { return "El nombre de la familia profesional no puede estar vacío"; }
            else if (code == ValidationCode.templateGeneralObjectivesIntroductionInvalid) { return "Debes revisar la introducción a los objetivos generales. Hay algún problema en ella"; }
            else if (code == ValidationCode.templateGeneralObjectiveInvalid) { return String.Format("Debes revisar el objetivo general que ocupa la posición {0}", index + 1); }
            else if (code == ValidationCode.templateGeneralCompetencesIntroductionInvalid) { return "Debes revisar la introducción a las competencias generales. Hay algún problema en ella"; }
            else if (code == ValidationCode.templateGeneralCompetenceInvalid) { return String.Format("Debes revisar la competencia general que ocupa la posición {0}", index + 1); }
            else if (code == ValidationCode.templateKeyCapacitiesIntroductionInvalid) { return "Debes revisar la introducción a las capacidades clave. Hay algún problema en ella"; }
            else if (code == ValidationCode.templateKeyCapacitiesInvalid) { return String.Format("Debes revisar la capacidad clave que ocupa la posición {0}", index + 1); }
            else if (code == ValidationCode.templateLearningResultsIntroductionInvalid) { return "Debes revisar la introducción a los resultados de aprendizaje. Hay algún problema en ella"; }
            else if (code == ValidationCode.templateLearningResultsInvalid) { return String.Format("Debes revisar el resultado de aprendizaje que ocupa la posición {0}", index + 1); }
            else if (code == ValidationCode.templateContentsIntroductionInvalid) { return "Debes revisar la introducción a los contenidos. Hay algún problema en ella"; }
            else if (code == ValidationCode.templateContentsInvalid) { return String.Format("Debes revisar el contenido que ocupa la posición {0}", index + 1); }
            else if (code == ValidationCode.templateNoGeneralObjectives) { return "La plantilla debe contener al menos un objetivo general";  }
            else if (code == ValidationCode.templateNoGeneralCompetences) { return "La plantilla debe contener al menos una competencia general";}
            else if (code == ValidationCode.templateNoKeyCapacities) { return "La plantilla debe contener al menos una capacidad clave";}
            else if (code == ValidationCode.templateNoLearningResults) { return "La plantilla debe contener al menos un resultado de aprendizaje";}
            else if (code == ValidationCode.templateNoContents) { return "La plantilla debe incluir al menos un contenido";}
            else if (code == ValidationCode.activityNotLinkedToMetodology) { return "Debes seleccionar alguna metodología para la actividad"; }
            else if (code == ValidationCode.activityNotLinkedToContents) { return "Debes seleccionar algún contenido para la actividad"; }
            else if (code == ValidationCode.activityEvaluableAndNotLinkedToEvaluationInstrumentType) { return "Al ser evauable, debes seleccionar algún instrumento de evaluación para la actividad"; }
            else if (code == ValidationCode.activityEvaluableAndNotLinkedToCriterias) { return "Al ser evauable, debes seleccionar algún criterio de evaluación para la actividad"; }
            else if (code == ValidationCode.activityEvaluableAndNotLinkedToResultsWeights) { return "Al ser evauable, debes seleccionar algún peso en los resultados de aprendizaje para la actividad"; }
            else if (code == ValidationCode.activityReferencesResultWithoutWeight) { return String.Format("El resultado de aprendizaje que ocupa la columna {0} en la tabla de pesos está referenciado por al menos un criterio de evaluación pero no tiene peso", index + 1); }
            else if (code == ValidationCode.activityDoesntReferenceResultButHasWeight) { return String.Format("El resultado de aprendizaje que ocupa la columna {0} en la tabla de pesos no está referenciado por ningún criterio", index + 1); }
            else if (code == ValidationCode.contentNoPoints) { return "El contenido debe tener al menos un punto"; }
            else if (code == ValidationCode.contentPointInvalid) { return String.Format("Debes revisar el punto de contenido que ocupa la posición {0}", index + 1); }
            else if (code == ValidationCode.learningResultNoCriterias) { return "El resultado de aprendizaje debe tener al menos un criterio"; }
            else if (code == ValidationCode.learningResultCriteriaInvalid) { return String.Format("Debes revisar el criterio que ocupa la posición {0}", index + 1); }
            else if (code == ValidationCode.blockNoActivities) { return "El bloque debe tener al menos una actividad"; }
            else // code == ValidationCode.blockActivityInvalid)
            { return String.Format("Debes revisar la actividad que ocupa la posición {0}", index + 1); }

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
        templateNoGeneralObjectives,
        templateNoGeneralCompetences,
        templateNoKeyCapacities,
        templateNoLearningResults,
        templateNoContents,
        activityNotLinkedToMetodology,
        activityNotLinkedToContents,
        activityEvaluableAndNotLinkedToEvaluationInstrumentType,
        activityEvaluableAndNotLinkedToCriterias,
        activityEvaluableAndNotLinkedToResultsWeights,
        activityReferencesResultWithoutWeight,
        activityDoesntReferenceResultButHasWeight,
        contentNoPoints,
        contentPointInvalid,
        learningResultNoCriterias,
        learningResultCriteriaInvalid,
        blockNoActivities,
        blockActivityInvalid,
    };
}
