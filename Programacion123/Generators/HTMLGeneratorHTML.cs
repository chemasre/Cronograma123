using System.Diagnostics;
using System.Globalization;

namespace Programacion123
{
    public partial class HTMLGenerator : Generator
    {
        /// <summary>
        /// Requires validation result to be success
        /// </summary>
        public string GenerateHTML(bool isPreview = false)
        {
            Debug.Assert(Style.HasValue);
            Debug.Assert(Subject != null);
            Debug.Assert(Subject.Template != null);
            Debug.Assert(Subject.Template.GradeTemplate != null);

            DocumentStyle style = Style.Value;
            SubjectTemplate subjectTemplate = Subject.Template;


            GradeTemplate gradeTemplate = Subject.Template.GradeTemplate;

            List<DocumentIndexItem> index = BuildIndex();

            Action<string, int, List<Tag>> addCommonTextTags =
                (s, i, l) =>
                {
                    l.Add(Tag.Create("div").WithInner(s));
                };


            string gradeTypeName = GetGradeTypeName();

            List<ActivitySchedule>? schedule = null;
            
            if((Subject?.CanScheduleActivities()).GetValueOrDefault()) { schedule = Subject.ScheduleActivities(); }

            // https://awkwardcoder.blogspot.com/2011/08/manipulating-web-browser-scroll.html

            string javascript = 
            @"function getVerticalScrollPosition() {
            return document.documentElement.scrollTop.toString();
            }
            function setVerticalScrollPosition(position) {
            document.documentElement.scrollTop = position;
            }";

            // https://stackoverflow.com/questions/8218377/rendering-page-breaks-in-word-with-html
            Tag pageBreak = Tag.Create(isPreview ? "div" : "br").WithClass("pageBreak").WithInnerIf(isPreview, "Salto de página");

            string html =
                "<!DOCTYPE html>" +
                Tag.Create("html").WithParam("lang", "es")
                    .WithInner(
                        Tag.Create("head")
                            .WithInner(Tag.Create("meta").WithParam("charset", "UTF-8"))
                            .WithInner(Tag.Create("title").WithInner("Programación didáctica del módulo " + subjectTemplate.SubjectName))
                            .WithInner(Tag.Create("style").WithInner(GenerateCSS(isPreview)))
                            .WithInner(Tag.Create("script").WithInner(javascript))
                    )
                    .WithInner(
                        Tag.Create("body")
                            .WithInner(Tag.Create("div").WithClass("cover")
                               .WithInner(Tag.Create("img").WithClass("coverLogo").WithParam("src", "data:image/png;base64," + style.LogoBase64))
                               .WithInner(Tag.Create("img").WithClass("coverCover").WithParam("src", "data:image/png;base64," + style.CoverBase64))
                               .WithInner(Tag.Create("div").WithClass("coverSubjectCode").WithInner("Módulo profesional " + subjectTemplate.SubjectCode))
                               .WithInner(Tag.Create("div").WithClass("coverSubjectName").WithInner(subjectTemplate.SubjectName))
                               .WithInner(Tag.Create("div").WithClass("coverGradeTypeName").WithInner(gradeTypeName))
                               .WithInner(Tag.Create("div").WithClass("coverGradeName").WithInner(gradeTemplate.GradeName))
                             )
                            .WithInner(pageBreak)

                            //////////////////////////////////////////////////////////////////
                            ///////////////////// ÍNDICE DE CONTENIDOS  //////////////////////
                            //////////////////////////////////////////////////////////////////

                            .WithInner(Tag.Create("div").WithInner("Contenidos").WithId("Indice").WithClass("indexTitle"))
                            .WithInnerForeach<DocumentIndexItem>(index,
                                (item, i, l) =>
                                {
                                    l.Add(Tag.Create("div").WithInner(Tag.Create("a").WithParam("href", String.Format("#Apartado{0}", i + 1))
                                                            .WithInner(item.Title)).WithClass("indexLevel1"));

                                    int subitemIndex = 0;
                                    foreach(DocumentIndexItem subitem in item.Subitems)
                                    {
                                        l.Add(Tag.Create("div").WithInner(Tag.Create("a").WithParam("href", String.Format("#Apartado{0}-{1}", i + 1, subitemIndex + 1))
                                                                .WithInner(subitem.Title)).WithClass("indexLevel2"));
                                        int subSubitemIndex = 0;
                                        foreach(DocumentIndexItem subsubitem in subitem.Subitems)
                                        {
                                            l.Add(Tag.Create("div").WithInner(Tag.Create("a").WithParam("href", String.Format("#Apartado{0}-{1}-{2}", i + 1, subitemIndex + 1, subSubitemIndex + 1))
                                                                    .WithInner(subsubitem.Title)).WithClass("indexLevel3"));
                                            subSubitemIndex++;
                                        }

                                        subitemIndex++;
                                    }
                                }
                            )
                            .WithInner(pageBreak)

                            //////////////////////////////////////////////////////////////////
                            ///////////// Nivel 1: Organización del módulo ///////////////////
                            //////////////////////////////////////////////////////////////////

                            .WithInner(Tag.Create("h1").WithInner(index[0].Title).WithId("Apartado1"))
                            .WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header1ModuleOrganization), addCommonTextTags)
                            .WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header1ModuleOrganization), addCommonTextTags)
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
                                Table.Create().WithRow().WithCell(subjectTemplate.SubjectCode + ": " + subjectTemplate.SubjectName, 1, 3).WithCellClass("tableHeader1")
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
                                                            List<int> referencedResults = Subject.QueryBlockReferencedLearningResultIndexes(i);
                                                            List<SubjectLearningResultCriteriaIndex> referencedCriterias = Subject.QueryBlockReferencedLearningResultCriteriaIndexes(i);

                                                            string rasText = "";
                                                            bool first = true;
                                                            referencedResults.ForEach((i) => { rasText += (first?"":", ") + String.Format("RA{0}", i + 1); first = false; });

                                                            string criteriasText = "";
                                                            first = true;
                                                            referencedCriterias.ForEach((c) => { criteriasText += (first ? "" : ", ") + String.Format("{0}.{1}", c.learningResultIndex + 1, c.criteriaIndex + 1); first = false; });

                                                            float hours = Subject.QueryBlockDuration(i);

                                                            ActivitySchedule? startActivitySchedule = null;
                                                            ActivitySchedule? endActivitySchedule = null;

                                                            int aIndex = 0;
                                                            List<Activity> activitiesList = b.Activities.ToList();
                                                            foreach(Activity a in activitiesList)
                                                            {
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
                            .WithInner(pageBreak)

                            /////////////////////////////////////////////////////////////////////////////////////
                            ///////////// Nivel 1: Justificación de la importancia del módulo ///////////////////
                            /////////////////////////////////////////////////////////////////////////////////////

                            .WithInner(Tag.Create("h1").WithInner(index[1].Title).WithId("Apartado2"))
                            .WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header1ImportanceJustification), addCommonTextTags)
                            .WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header1ImportanceJustification), addCommonTextTags)
                            .WithInner(pageBreak)

                            /////////////////////////////////////////////////////////////////
                            ///////////// Nivel 1: Elementos curriculares ///////////////////
                            /////////////////////////////////////////////////////////////////

                            .WithInner(Tag.Create("h1").WithInner(index[2].Title).WithId("Apartado3"))
                            .WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header1CurricularElements), addCommonTextTags)
                            .WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header1CurricularElements), addCommonTextTags)
                            .WithInner(Tag.Create("h2").WithInner(index[2].Subitems[0].Title).WithId("Apartado3-1"))
                            .WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2GeneralObjectives), addCommonTextTags)
                            .WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2GeneralObjectives), addCommonTextTags)
                            .WithInner(Tag.Create("div")
                                .WithInnerForeach<CommonText>(subjectTemplate.GeneralObjectives.ToList(),
                                    (o, i, l) =>
                                    {
                                        int index = subjectTemplate.GradeTemplate.GeneralObjectives.ToList().FindIndex(_o => _o.StorageId == o.StorageId);
                                        l.Add(Tag.Create("div").WithInner(String.Format("{0}. {1}", Utils.FormatLetterPrefixLowercase(index), o.Description)));
                                    }
                                )
                             )
                            .WithInner(Tag.Create("h2").WithInner(index[2].Subitems[1].Title).WithId("Apartado3-2"))
                            .WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2GeneralCompetences), addCommonTextTags)
                            .WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2GeneralCompetences), addCommonTextTags)
                            .WithInner(Tag.Create("div")
                                .WithInnerForeach<CommonText>(subjectTemplate.GeneralCompetences.ToList(),
                                    (c, i, l) =>
                                    {
                                        int index = subjectTemplate.GradeTemplate.GeneralCompetences.ToList().FindIndex(_c => _c.StorageId == c.StorageId);
                                        l.Add(Tag.Create("div").WithInner(String.Format("{0}. {1}", Utils.FormatLetterPrefixLowercase(index), c.Description)));
                                    }
                                )
                             )
                            .WithInner(Tag.Create("h2").WithInner(index[2].Subitems[2].Title).WithId("Apartado3-3"))
                            .WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2KeyCompetences), addCommonTextTags)
                            .WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2KeyCompetences), addCommonTextTags)
                            .WithInnerForeach<int>(Subject.QueryReferencedKeyCompetencesIndexes(),
                                (c, i, l) =>
                                {
                                    l.Add(Tag.Create("h3").WithInner(gradeTemplate.KeyCapacities[c].Title));
                                    l.Add(Tag.Create("div").WithInner(gradeTemplate.KeyCapacities[c].Description));
                                }
                             )
                            .WithInner(pageBreak)

                            /////////////////////////////////////////////////////////////////////////////////
                            ///////////// Nivel 1: Metodología y orientaciones didácticas ///////////////////
                            /////////////////////////////////////////////////////////////////////////////////

                            .WithInner(Tag.Create("h1").WithInner(index[3].Title).WithId("Apartado4"))
                            .WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header1MetodologyAndDidacticOrientations), addCommonTextTags)
                            .WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header1MetodologyAndDidacticOrientations), addCommonTextTags)
                            .WithInner(Tag.Create("h2").WithInner("Metodología general y específica de la materia").WithId("Apartado4-1"))
                            .WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2Metodology), addCommonTextTags)
                            .WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2Metodology), addCommonTextTags)
                            .WithInnerForeach<CommonText>(Subject.Metodologies.ToList(),
                                (c, i, l) =>
                                {
                                    l.Add(Tag.Create("h3").WithInner(c.Title).WithId(String.Format("Apartado4-1-{0}", i + 1)));
                                    l.Add(Tag.Create("div").WithInner(c.Description));
                                }
                            )
                            .WithInner(Tag.Create("h2").WithInner("Medidas de atención al alumnado con necesidad específica de apoyo educativo o con necesidad de compensación educativa: atención a la diversidad").WithId("Apartado4-2"))
                            .WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2Diversity), addCommonTextTags)
                            .WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2Diversity), addCommonTextTags)
                            .WithInner(pageBreak)

                            ////////////////////////////////////////////////////////////////
                            ///////////// Nivel 1: Sistema de evaluación ///////////////////
                            ////////////////////////////////////////////////////////////////

                            .WithInner(Tag.Create("h1").WithInner(index[4].Title).WithId("Apartado5"))
                            .WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header1EvaluationSystem), addCommonTextTags)
                            .WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header1EvaluationSystem), addCommonTextTags)
                            .WithInner(Tag.Create("h2").WithInner("Instrumentos de evaluación").WithId("Apartado5-1"))
                            .WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2EvaluationInstruments), addCommonTextTags)
                            .WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2EvaluationInstruments), addCommonTextTags)
                            .WithInnerForeach<CommonText>(Subject.EvaluationInstrumentsTypes.ToList(),
                                (c, i, l) =>
                                {
                                    l.Add(Tag.Create("h3").WithInner(c.Title).WithId(String.Format("Apartado5-1-{0}", i + 1)));
                                    l.Add(Tag.Create("div").WithInner(c.Description));
                                }
                            )
                            .WithInner(Tag.Create("h2").WithInner("Evaluación del funcionamiento de la programación").WithId("Apartado5-2"))
                            .WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2EvaluationOfProgramming), addCommonTextTags)
                            .WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2EvaluationOfProgramming), addCommonTextTags)
                            .WithInner(pageBreak)

                            ///////////////////////////////////////////////////////////////////
                            ////////////// Nivel 1: Elementos transversales ///////////////////
                            ///////////////////////////////////////////////////////////////////

                            .WithInner(Tag.Create("h1").WithInner(index[5].Title).WithId("Apartado6"))
                            .WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header1TraversalElements), addCommonTextTags)
                            .WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header1TraversalElements), addCommonTextTags)
                            .WithInner(Tag.Create("h2").WithInner("Fomento de la lectura y tecnologías de la información y de comunicación").WithId("Apartado6-1"))
                            .WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2TraversalReadingAndTIC), addCommonTextTags)
                            .WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2TraversalReadingAndTIC), addCommonTextTags)
                            .WithInner(Tag.Create("h2").WithInner("Comunicación audiovisual, emprendimiento, educación cívica y constitucional").WithId("Apartado6-2"))
                            .WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2TraversalCommunicationEntrepreneurshipAndEducation), addCommonTextTags)
                            .WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2TraversalCommunicationEntrepreneurshipAndEducation), addCommonTextTags)
                            .WithInner(pageBreak)



                            //////////////////////////////////////////////////////////////////////////////
                            ////////////// Nivel 1: Recursos didácticos y organizativos //////////////////
                            //////////////////////////////////////////////////////////////////////////////

                            .WithInner(Tag.Create("h1").WithInner(index[6].Title).WithId("Apartado7"))
                            .WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header1Resources), addCommonTextTags)
                            .WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header1Resources), addCommonTextTags)                            
                            .WithInner(Tag.Create("h2").WithInner("Espacios requeridos").WithId("Apartado7-1"))
                            .WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2ResourcesSpaces), addCommonTextTags)
                            .WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2ResourcesSpaces), addCommonTextTags)                            
                            .WithInnerForeach<CommonText>(Subject.SpaceResources.ToList(),
                                (c, i, l) =>
                                {
                                    l.Add(Tag.Create("h3").WithInner(c.Title).WithId(String.Format("Apartado7-1-{0}", i + 1)));
                                    l.Add(Tag.Create("div").WithInner(c.Description));
                                }
                             )

                            .WithInner(Tag.Create("h2").WithInner("Materiales y herramientas").WithId("Apartado7-2"))
                            .WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2ResourcesMaterialAndTools), addCommonTextTags)
                            .WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2ResourcesMaterialAndTools), addCommonTextTags) 
                            .WithInnerForeach<CommonText>(Subject.MaterialResources.ToList(),
                                (c, i, l) =>
                                {
                                    l.Add(Tag.Create("h3").WithInner(c.Title).WithId(String.Format("Apartado7-2-{0}", i + 1)));
                                    l.Add(Tag.Create("div").WithInner(c.Description));
                                }
                             )
                            .WithInner(pageBreak)

                            ///////////////////////////////////////////////////////////////////////////////
                            ////////////// Nivel 1: Programación del módulo profesional ///////////////////
                            ///////////////////////////////////////////////////////////////////////////////
                            
                            .WithInner(Tag.Create("h1").WithInner(index[7].Title).WithId("Apartado8"))
                            .WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header1SubjectProgramming), addCommonTextTags)
                            .WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header1SubjectProgramming), addCommonTextTags) 
                            .WithInner(Tag.Create("h2").WithInner("Resultados de aprendizaje, criterios de evaluación y contenidos").WithId("Apartado8-1"))
                            .WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2LearningResultsAndContents), addCommonTextTags)
                            .WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2LearningResultsAndContents), addCommonTextTags) 
                            .WithInner(Tag.Create("h3").WithInner("Resultados de aprendizaje y criterios de evaluación").WithId("Apartado8-1-1"))
                            .WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header3LearningResults), addCommonTextTags)
                            .WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header3LearningResults), addCommonTextTags) 
                            .WithInnerForeach<LearningResult>(subjectTemplate.LearningResults.ToList(),
                                (r, i, l) =>
                                {
                                        l.Add(Tag.Create("div").WithInner(String.Format("RA{0}: ", i + 1) + r.Description));
                                        l.Add(Tag.Create("div").WithInner("Criterios"));

                                        l.Add(Tag.Create("div")
                                        .WithInnerForeach<CommonText>(subjectTemplate.LearningResults.ToList()[i].Criterias.ToList(),
                                            (c, j, l) =>
                                            {
                                                l.Add(Tag.Create("div").WithInner(String.Format("{0}.{1}: ", i + 1, j + 1) + c.Description));
                                            }
                                        ));
                                }
                             )
                            .WithInner(Tag.Create("h3").WithInner("Contenidos").WithId("Apartado8-1-2"))
                            .WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header3Contents), addCommonTextTags)
                            .WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header3Contents), addCommonTextTags) 
                            .WithInnerForeach<Content>(subjectTemplate.Contents.ToList(),
                                (c, i, l) =>
                                {
                                l.Add(Tag.Create("div").WithInner(String.Format("{0}: ", i + 1) + c.Description));
                                l.Add(Tag.Create("div")
                                        .WithInnerForeach<CommonText>(subjectTemplate.Contents.ToList()[i].Points.ToList(),
                                            (p, j, l) =>
                                            {
                                                l.Add(Tag.Create("div").WithInner(String.Format("{0}.{1}: ", i + 1, j + 1) + p.Description));
                                            }
                                        )
                                    );
                                }
                            )
                            .WithInner(Tag.Create("h2").WithInner("Bloques de enseñanza y aprendizaje").WithId("Apartado8-2"))
                            .WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2Blocks), addCommonTextTags)
                            .WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2Blocks), addCommonTextTags) 
                            .WithInner(
                                Table.Create()
                                    .WithRow()
                                        .WithCell(String.Format("{0}: {1}", subjectTemplate.SubjectCode, subjectTemplate.SubjectName), 1, 5).WithCellClass("tableHeader1")
                                        .WithCell(String.Format("Horas: {0}", subjectTemplate.GradeClassroomHours + subjectTemplate.GradeCompanyHours)).WithCellClass("tableHeader1")
                                    .WithRow()
                                        .WithCell("Bloque de enseñanza-aprendizaje", 2, 2).WithCellClass("tableHeader2")
                                        .WithCell("RA", 2, 1).WithCellClass("tableHeader2")
                                        .WithCell("Contenidos", 2, 1).WithCellClass("tableHeader2")
                                        .WithCell("Evaluación", 1, 2).WithCellClass("tableHeader2")
                                    .WithRow()
                                        .WithCell("CE").WithCellClass("tableHeader2")
                                        .WithCell("Actividades evaluables").WithCellClass("tableHeader2")

                                    .WithRowForeach<Block>(Subject.Blocks.ToList(),
                                        (b, i, t) =>
                                        {
                                            t.WithCell(String.Format("<b>Bloque {0}</b>", i + 1));
                                            t.WithCell(String.Format(CultureInfo.InvariantCulture, "{0} horas", Subject.QueryBlockDuration(i)));

                                            string raText = "";
                                            bool first = true;
                                            Subject.QueryBlockReferencedLearningResultIndexes(i).ForEach(
                                                r => { raText += (first?"":", ") + "RA" + (r + 1); first = false;
                                            });

                                            string contentText = "";
                                            first = true;
                                            Subject.QueryBlockReferencedContentIndexes(i).ForEach(
                                                c =>
                                                {
                                                    contentText += (first ? "" : ", ") + (c + 1); first = false;
                                                });

                                            string criteriaText = "";
                                            first = true;
                                            Subject.QueryBlockReferencedLearningResultCriteriaIndexes(i).ForEach(
                                                cIndex =>
                                                {
                                                    criteriaText += (first ? "" : ", ") + (cIndex.learningResultIndex + 1) + "." + (cIndex.criteriaIndex + 1);
                                                    first = false;
                                                }
                                            );

                                            string evaluableActivitiesText = "";
                                            first = true;
                                            Subject.QueryBlockEvaluableActivityIndexes(i).ForEach(
                                                (aIndex) =>
                                                {
                                                    evaluableActivitiesText += (first ? "" : "<br>") + Utils.FormatEvaluableActivity(i, aIndex.evaluationType, aIndex.activityTypeIndex);
                                                    first = false;
                                                }
                                            );

                                            t.WithCell(raText, 2, 1);
                                            t.WithCell(contentText, 2, 1);
                                            t.WithCell(criteriaText, 2, 1);
                                            t.WithCell(evaluableActivitiesText, 2, 1);
                                            t.WithRow().WithCell(b.Description, 1, 2);
                                        }
                                    )
                            )
                            .WithInner(Tag.Create("h2").WithInner("Programación de actividades de enseñanza-aprendizaje").WithId("Apartado8-3"))
                            .WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2Activities), addCommonTextTags)
                            .WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2Activities), addCommonTextTags) 
                            .WithInnerForeach<Block>(Subject.Blocks.ToList(),
                                (b, i, l) =>
                                {
                                    l.Add(Tag.Create("h3").WithInner(String.Format("Bloque {0}", i + 1)).WithId(String.Format("Apartado8-3-{0}", i + 1)));

                                    List<Activity> activities = b.Activities.ToList();
                                    foreach(Activity a in activities)
                                    {
                                        l.Add(
                                            Table.Create()
                                                    .WithRow()
                                                        .WithCell(a.Title, 1, 4).WithCellClass("tableHeader1")
                                                    .WithRow()
                                                        .WithCell(a.Description, 1, 4)
                                                    .WithRow()
                                                        .WithCell("Metodología").WithCellClass("tableHeader2")
                                                        .WithCell("Duración").WithCellClass("tableHeader2")
                                                        .WithCell("Fecha de inicio").WithCellClass("tableHeader2")
                                                        .WithCell("Fecha de fin").WithCellClass("tableHeader2")
                                                    .WithRow()
                                                        .WithCell(a.Metodology.Title)
                                                        .WithCell(String.Format(CultureInfo.InvariantCulture, "{0:0}h ({1} sesiones)", a.Duration, GetSessionsCountText(a, schedule)))
                                                        .WithCell(Utils.FormatStartDayHour(schedule.Find(_a => _a.activity.StorageId == a.StorageId).start, Subject.WeekSchedule))
                                                        .WithCell(Utils.FormatEndDayHour(schedule.Find(_a => _a.activity.StorageId == a.StorageId).end, Subject.WeekSchedule))
                                                    .WithRow()
                                                        .WithCell("Espacios").WithCellClass("tableHeader2")
                                                        .WithCell("Materiales").WithCellClass("tableHeader2")
                                                        .WithCell("Contenidos").WithCellClass("tableHeader2")
                                                        .WithCell("Capacidades clave").WithCellClass("tableHeader2")
                                                     .WithRow()
                                                        .WithCell(GetSpacesText(a))
                                                        .WithCell(GetMaterialsText(a))
                                                        .WithCell(GetContentsText(i, a))
                                                        .WithCell(GetKeyCapacitiesText(a))
                                                     .WithRowIf(a.EvaluationType != ActivityEvaluationType.NotEvaluable)
                                                        .WithCell("Código de actividad evaluable").WithCellClass("tableHeader2")
                                                        .WithCell("Instrumento de evaluación").WithCellClass("tableHeader2")
                                                        .WithCell("Peso en los resultados de aprendizaje").WithCellClass("tableHeader2")
                                                        .WithCell("Criterios de evaluación").WithCellClass("tableHeader2")
                                                     .WithRowIf(a.EvaluationType != ActivityEvaluationType.NotEvaluable)
                                                        .WithCell(a.EvaluationType != ActivityEvaluationType.NotEvaluable ? Utils.FormatEvaluableActivity(i, a.EvaluationType, Subject.QueryEvaluableActivityTypeIndex(i, a)) : "")
                                                        .WithCell(a.EvaluationType != ActivityEvaluationType.NotEvaluable ? a.EvaluationInstrumentType.Title : "")
                                                        .WithCell(GetReferencedLearningResultsWeightsText(i, a))
                                                        .WithCell(GetReferencedCriteriasText(i, a))
                                        );
                                    }
                                }
                            )
                            .WithInner(pageBreak)

                            //////////////////////////////////////////////////////////////////////
                            ////////////// Nivel 1: Referencias bibliográficas ///////////////////
                            //////////////////////////////////////////////////////////////////////
                            
                            .WithInner(Tag.Create("h1").WithInner(index[8].Title).WithId("Apartado9"))
                            .WithInner(Tag.Create("div")
                                .WithInnerForeach<CommonText>(Subject.Citations.ToList(),
                                    (c, i, l) =>
                                    {
                                        l.Add(Tag.Create("div").WithInner(String.Format("{0}- {1}", i + 1, c.Description)));
                                    }
                                )
                             )
                            .WithInner(pageBreak)

                            //////////////////////////////////////////////////
                            ////////////// Nivel 1: Anexos ///////////////////
                            //////////////////////////////////////////////////
                            
                            .WithInner(Tag.Create("h1").WithInner(index[9].Title).WithId("Apartado10"))
                            .WithInner(Tag.Create("h2").WithInner("Cuadro de distribución de pesos").WithId("Apartado10-1"))
                            .WithInner(Table.Create()
                                .WithRow()
                                    .WithCell("&nbsp;", 1, 2).WithCellClass("weightsTable")
                                    .WithCellForeach<LearningResult>(subjectTemplate.LearningResults.ToList(),
                                        (learningResult, i, table) =>
                                        {
                                            table.WithCellInner(String.Format("RA{0}", i + 1)).WithCellClass("weightsTableHeader1");
                                        }
                                    )
                                .WithRow()
                                    .WithCell("Bloque").WithCellClass("weightsTableHeader1")
                                    .WithCell("Peso&nbsp;RA").WithCellClass("weightsTableHeader1")
                                    .WithCellForeach<SubjectLearningResultIndexesWeight>(Subject.QueryLearningResultsIndexesWeights(),
                                        (resultWeight, i, table) =>
                                        {
                                            table.WithCellInner(String.Format("{0:0}%", resultWeight.weight)).WithCellClass("weightsTableHeader2");
                                        }
                                    )
                                .WithRowForeach<Block>(Subject.Blocks.ToList(),
                                   (block, i, table) =>
                                   {
                                       List<EvaluableActivityIndex> evaluableActivityIndexes = Subject.QueryBlockEvaluableActivityIndexes(i);

                                       table.WithCell(String.Format("Bloque&nbsp;{0}", i + 1), evaluableActivityIndexes.Count, 1).WithCellClass("weightsTableHeader1");

                                       bool first = true;

                                       foreach(EvaluableActivityIndex activityInfo in evaluableActivityIndexes)
                                       {
                                           if(!first) { table.WithRow(); }
                                           table.WithCell(Utils.FormatEvaluableActivity(i, activityInfo.evaluationType, activityInfo.activityTypeIndex)).WithCellClass("weightsTableHeader2");

                                           table.WithCellForeach<SubjectLearningResultIndexesWeight>(Subject.QueryActivityLearningResultsIndexesWeight(i, activityInfo.activityIndex),
                                               (resultWeight, j, table) =>
                                               {
                                                   table.WithCellInner(resultWeight.weight > 0 ? String.Format("{0:0}%", resultWeight.weight) : "&nbsp;").WithCellClass("weightsTable");
                                               }

                                           );

                                           first = false;
                                       }
                                   }
                                )
                                    
                            )
                )
                .ToString();

            return html;
        }

    }
}
