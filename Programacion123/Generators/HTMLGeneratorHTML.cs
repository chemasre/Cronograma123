using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    public enum DocumentCoverElementId
    {
        Logo,
        SubjectCode,
        SubjectName,
        GradeTypeName,
        GradeName
    }

    public enum DocumentTextElementId
    {
        Header1,
        Header2,
        Header3,
        Header4,
        Header5,
        Header6,
        NormalText,
        Table,
        TableHeader1Text,
        TableHeader2Text,
        CoverSubjectCode,
        CoverSubjectName,
        CoverGradeTypeName,
        CoverGradeName
    }

    public enum DocumentTableElementId
    {
        TableNormalCell,
        TableHeader1Cell,
        TableHeader2Cell
    
    }

    public partial class HTMLGenerator : Generator
    {
        public string GenerateHTML()
        {
            SubjectTemplate? subjectTemplate = Subject.Template;
            GradeTemplate? gradeTemplate = subjectTemplate?.GradeTemplate;
            Func<GradeCommonTextId, string?> getGradeCommonText = (id) => { return gradeTemplate?.CommonTexts[id].Description; };

            string? gradeTypeName = (gradeTemplate.GradeType == GradeType.superior ?
                                    "Ciclo formativo de grado superior" : "Ciclo formativo de grado medio");

            List<ActivitySchedule>? schedule = null;
            
            if(Subject.CanScheduleActivities()) { schedule = Subject.ScheduleActivities(); }


            string html =
                "<!DOCTYPE html>" +
                Tag.Create("html").WithInner(
                        Tag.Create("head")
                            .WithInner(Tag.Create("meta").WithParam("charset", "UTF-8"))
                            .WithInner(Tag.Create("title").WithInner("Programación didáctica del módulo " + subjectTemplate.SubjectName))
                            .WithInner(Tag.Create("style").WithInner(GenerateCSS())
                            )
                    )
                    .WithInner(
                        Tag.Create("body")
                            .WithInner(Tag.Create("img").WithClass("coverLogo").WithParam("src","data:image/png;base64," + DocumentStyle.LogoBase64))
                            .WithInner(Tag.Create("div").WithClass("coverSubjectCode").WithInner("Módulo profesional " + subjectTemplate.SubjectCode))
                            .WithInner(Tag.Create("div").WithClass("coverSubjectName").WithInner(subjectTemplate.SubjectName))
                            .WithInner(Tag.Create("div").WithClass("coverGradeTypeName").WithInner(gradeTypeName)
                            .WithInner(Tag.Create("div").WithClass("coverGradeName").WithInner(gradeTemplate.GradeName))
                            .WithInner(Tag.Create("h1").WithInner("Organización del módulo"))
                            .WithInner(
                                Table.Create().WithRow().WithCell(gradeTypeName + " - " + gradeTemplate.GradeName, 1, 3).WithCellClass("tableHeader1")
                                              .WithRow().WithCell("<b>Módulo profesional:</b> MP" + subjectTemplate.SubjectCode + " - " + subjectTemplate.SubjectName, 1, 3).WithCellClass("tableHeader2")
                                              .WithRow().WithCell("<b>Horas centro educativo:</b> " + subjectTemplate.GradeClassroomHours)
                                                        .WithCell("<b>Horas empresa:</b> " + subjectTemplate.GradeCompanyHours)
                                                        .WithCell("<b>Horas totales:</b> " + (subjectTemplate.GradeClassroomHours + subjectTemplate.GradeCompanyHours))
                                              .WithRow().WithCell("<b>Modalidad:</b> Presencial", 1, 2)
                                                        .WithCell("<b>Régimen:</b> Anual")
                                              .WithRow().WithCell("<b>Familia profesional:</b> " + gradeTemplate.GradeFamilyName, 1, 3)
                            )
                            .WithInner(
                                Table.Create().WithRow().WithCell("MP" + subjectTemplate.SubjectCode + ": " + subjectTemplate.SubjectName, 1, 3).WithCellClass("tableHeader1")
                                                        .WithCell("Horas totales/mínimas", 1, 2).WithCellClass("tableHeader1")
                                                        .WithCell(subjectTemplate.GradeClassroomHours + "h").WithCellClass("tableHeader1")
                                              .WithRow().WithCell("Bloques de enseñanza").WithCellClass("tableHeader2")
                                                        .WithCell("RAs").WithCellClass("tableHeader2")
                                                        .WithCell("CEs").WithCellClass("tableHeader2")
                                                        .WithCell("Duración").WithCellClass("tableHeader2")
                                                        .WithCell("Fecha de inicio").WithCellClass("tableHeader2")
                                                        .WithCell("Fecha de fin").WithCellClass("tableHeader2")
                                              .WithRowForeach<Block>(Subject.Blocks.ToList(),
                                                        (b, i, t) =>
                                                        {
                                                            List<int> referencedResults = Subject.GetBlockReferencedLearningResultIndexes(i);
                                                            List<SubjectLearningResultCriteriaIndex> referencedCriterias = Subject.GetBlockReferencedLearningResultCriteriaIndexes(i);

                                                            string rasText = "";
                                                            bool first = true;
                                                            referencedResults.ForEach((i) => { rasText += (first?"":", ") + String.Format("RA{0}", i + 1); first = false; });

                                                            string criteriasText = "";
                                                            first = true;
                                                            referencedCriterias.ForEach((c) => { criteriasText += (first ? "" : ", ") + String.Format("{0}.{1}", c.learningResultIndex + 1, c.criteriaIndex + 1); first = false; });

                                                            float hours = 0;

                                                            ActivitySchedule? startActivitySchedule = null;
                                                            ActivitySchedule? endActivitySchedule = null;

                                                            int aIndex = 0;
                                                            List<Activity> activitiesList = b.Activities.ToList();
                                                            foreach(Activity a in activitiesList)
                                                            {
                                                                hours += a.Duration;

                                                                if(schedule != null)
                                                                {
                                                                    if(aIndex == 0) { startActivitySchedule = schedule.Find(s => s.activity.StorageId == a.StorageId); }
                                                                    if(aIndex == activitiesList.Count - 1) { endActivitySchedule = schedule.Find(s => s.activity.StorageId == a.StorageId); }
                                                                }

                                                                aIndex ++;
                                                            }


                                                            t.WithCell(String.Format("<b>Bloque {0}:</b> {1}", i + 1, b.Title));
                                                            t.WithCell(rasText);
                                                            t.WithCell(criteriasText);
                                                            t.WithCell(String.Format("{0}h", hours));
                                                            t.WithCell(startActivitySchedule.HasValue ? Utils.FormatDate(startActivitySchedule.Value.start.day) : "");
                                                            t.WithCell(endActivitySchedule.HasValue ? Utils.FormatDate(endActivitySchedule.Value.start.day) : "");
                                                        }
                                               )
                            )
                            .WithInner(Tag.Create("h1").WithInner("Justificación de la importancia del módulo"))
                            .WithInner(Tag.Create("h1").WithInner("Elementos curriculares"))
                            .WithInner(Tag.Create("h2").WithInner("Objetivos generales relacionados con el módulo"))
                            .WithInner(Tag.Create("h2").WithInner("Competencias profesionales, personales y sociales"))
                            .WithInner(Tag.Create("h2").WithInner("Capacidades clave"))
                            .WithInnerForeach<CommonText>(subjectTemplate.KeyCapacities.ToList(),
                                (c, i, l) =>
                                {
                                    l.Add(Tag.Create("h3").WithInner(c.Title));
                                    l.Add(Tag.Create("div").WithInner(c.Description));
                                }
                             )                                
                            .WithInner(Tag.Create("h1").WithInner("Metodología. Orientaciones didácticas"))
                            .WithInner(Tag.Create("div").WithInner(getGradeCommonText.Invoke(GradeCommonTextId.introductionToMetodologies)))
                            .WithInner(Tag.Create("h2").WithInner("Metodología general y específica de la materia"))
                            .WithInner(Tag.Create("div").WithInner(getGradeCommonText.Invoke(GradeCommonTextId.schoolPolicyMetodology)))
                            .WithInnerForeach<CommonText>(Subject.Metodologies.ToList(),
                                (c, i, l) =>
                                {
                                    l.Add(Tag.Create("h3").WithInner(c.Title));
                                    l.Add(Tag.Create("div").WithInner(c.Description));
                                }
                            )
                            .WithInner(Tag.Create("h2").WithInner("Medidas de atención al alumnado con necesidad específica de apoyo educativo o con necesidad de compensación educativa: atención a la diversidad"))
                            .WithInner(Tag.Create("div").WithInner(getGradeCommonText.Invoke(GradeCommonTextId.introductionToDiversity)))
                            .WithInner(Tag.Create("h3").WithInner("Medidas generales del centro"))
                            .WithInner(Tag.Create("div").WithInner(getGradeCommonText.Invoke(GradeCommonTextId.schoolPolicyDiversity)))
                            .WithInner(Tag.Create("h1").WithInner("Sistema de evaluación"))
                            .WithInner(Tag.Create("h1").WithInner("Instrumentos de evaluación"))
                            .WithInnerForeach<CommonText>(Subject.EvaluationInstrumentsTypes.ToList(),
                                (c, i, l) =>
                                {
                                    l.Add(Tag.Create("h3").WithInner(c.Title));
                                    l.Add(Tag.Create("div").WithInner(c.Description));
                                }
                            )
                            .WithInner(Tag.Create("h1").WithInner("Evaluación del funcionamiento de la programación"))
                            .WithInner(Tag.Create("h1").WithInner("Recursos didácticos y organizativos"))
                            .WithInner(Tag.Create("h2").WithInner("Espacios requeridos"))
                            .WithInnerForeach<CommonText>(Subject.SpaceResources.ToList(),
                                (c, i, l) =>
                                {
                                    l.Add(Tag.Create("h3").WithInner(c.Title));
                                    l.Add(Tag.Create("div").WithInner(c.Description));
                                }
                             )
                            .WithInner(Tag.Create("h2").WithInner("Materiales y herramientas"))
                            .WithInnerForeach<CommonText>(Subject.MaterialResources.ToList(),
                                (c, i, l) =>
                                {
                                    l.Add(Tag.Create("h3").WithInner(c.Title));
                                    l.Add(Tag.Create("div").WithInner(c.Description));
                                }
                             )
                            .WithInner(Tag.Create("h1").WithInner("Programación del módulo profesional"))
                            .WithInner(Tag.Create("h2").WithInner("Resultados de aprendizaje, criterios de evaluación y contenidos"))
                            .WithInner(Tag.Create("h3").WithInner("Resultados de aprendizaje y criterios de evaluación"))
                            .WithInner(Tag.Create("div").WithInner(getGradeCommonText.Invoke(GradeCommonTextId.introductionToLearningResults)))
                            .WithInnerForeach<LearningResult>(subjectTemplate.LearningResults.ToList(),
                                (r, i, l) =>
                                {
                                    l.Add(Tag.Create("h4").WithInner(String.Format("RA{0}: ", i + 1) + r.Description)
                                        .WithInner(Tag.Create("h5").WithInner("Criterios"))
                                        .WithInnerForeach<CommonText>(subjectTemplate.LearningResults.ToList()[i].Criterias.ToList(),
                                            (c, j, l) =>
                                            {
                                                l.Add(Tag.Create("h6").WithInner(String.Format("{0}.{1}: ", i + 1, j + 1) + c.Description));
                                            }
                                        )
                                    );
                                }
                             )
                            .WithInner(Tag.Create("h3").WithInner("Contenidos"))
                            .WithInner(Tag.Create("div").WithInner(getGradeCommonText.Invoke(GradeCommonTextId.introductionToContents)))
                            .WithInnerForeach<Content>(subjectTemplate.Contents.ToList(),
                                (c, i, l) =>
                                {
                                    l.Add(Tag.Create("h4").WithInner(String.Format("{0}: ", i + 1) + c.Description)
                                        .WithInnerForeach<CommonText>(subjectTemplate.Contents.ToList()[i].Points.ToList(),
                                            (p, j, l) =>
                                            {
                                                l.Add(Tag.Create("h5").WithInner(String.Format("{0}.{1}: ", i + 1, j + 1) + p.Description));
                                            }
                                        )
                                    );
                                }
                            )
                            .WithInner(Tag.Create("h3").WithInner("Bloques de enseñanza y aprendizaje"))
                            .WithInnerForeach<Block>(Subject.Blocks.ToList(),
                                (b, i, l) =>
                                {
                                    l.Add(
                                        Table.Create()
                                        .WithRow()
                                            .WithCell(String.Format("Bloque {0}", i + 1)).WithCell("this").WithCell("is").WithCell("a").WithCell("test")
                                        .WithRow()
                                            .WithCell("second").WithCell("row").WithCell("has").WithCell("colspan", 1, 2)
                                    );
                                }
                            )
                            .WithInner(Tag.Create("h1").WithInner("Programación del módulo profesional"))

                    )
                )
                .ToString();

            return html;
        }

    }
}
