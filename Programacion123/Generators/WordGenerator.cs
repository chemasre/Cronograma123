using Microsoft.Office.Interop.Word;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Globalization;

namespace Programacion123
{

    public partial class WordGenerator : Generator
    {
        public const string SettingsId = "DocGenerator";

        public WordGenerator()
        {
            LineBreak = "\n";
            NonBreakingSpace = " ";

        }

        public override void Generate(string path)
        {
            Debug.Assert(Style.HasValue);
            Debug.Assert(Subject != null);
            Debug.Assert(Subject.Template != null);
            Debug.Assert(Subject.Template.GradeTemplate != null);

            DocumentStyle style = Style.Value;
            SubjectTemplate subjectTemplate = Subject.Template;

            GradeTemplate gradeTemplate = Subject.Template.GradeTemplate;

            List<DocumentIndexItem> index = BuildIndex();

            Action<string, int, WordDocument> addParagraph = ((s, i, d) => d.WithParagraph(s));

            List<ActivitySchedule> schedule = Subject.ScheduleActivities();

            Application app = new();

            WordDocument.Create(app)
               .WithMargins(style.Margins)
               .WithOrientation(style.Orientation)
               .WithTextStyle(DocumentTextElementId.NormalText, style.TextElementStyles[DocumentTextElementId.NormalText])
               .WithTextStyle(DocumentTextElementId.Header1, style.TextElementStyles[DocumentTextElementId.Header1])
               .WithTextStyle(DocumentTextElementId.Header2, style.TextElementStyles[DocumentTextElementId.Header2])
               .WithTextStyle(DocumentTextElementId.Header3, style.TextElementStyles[DocumentTextElementId.Header3])
               .WithTextStyle(DocumentTextElementId.Header4, style.TextElementStyles[DocumentTextElementId.Header4])
               //.WithParagraph("Hola")
               //.WithParagraph("Qué tal")
               //.WithHeader1("Qué tal")
               //.WithParagraph("Qué tal")
               //.WithHeader2("Qué tal")
               //.WithParagraph("Qué tal")
               //.WithHeader3("Qué tal")
               //.WithParagraph("Qué tal")
               //.WithTable(2, 3)
               //.WithCellSpan(1, 1, 2, 1)
               //.WithCellContent(1, 1, "Test11")
               //.WithCellContent(1, 2, "Test12")
               //.WithCellContent(1, 3, "Test13")
               //.WithCellContent(2, 2, "Test22")
               //.WithCellContent(2, 3, "Test23")

                //////////////////////////////////////////////////////////////////
                ///////////// Nivel 1: Organización del módulo ///////////////////
                //////////////////////////////////////////////////////////////////

               .WithHeader1(index[0].Title)
               .Foreach<string>(GetGradeCommonText(CommonTextId.header1ModuleOrganization), addParagraph)
               .Foreach<string>(GetSubjectCommonText(CommonTextId.header1ModuleOrganization), addParagraph)

                .WithTable(5, 3)
                .WithCellSpan(1, 1, 1, 3).WithCellContent(1, 1, GetGradeTypeName() + " - " + gradeTemplate.GradeName)
                .WithCellSpan(2, 1, 1, 3).WithCellContent(2, 1, "Módulo profesional:MP" + subjectTemplate.SubjectCode + " - " + subjectTemplate.SubjectName)
                .WithCellContent(3, 1, "Horas centro educativo: " +  subjectTemplate.GradeClassroomHours)
                .WithCellContent(3, 2, "Horas empresa: " + subjectTemplate.GradeCompanyHours)
                .WithCellContent(3, 3, "Horas totales: " + (subjectTemplate.GradeClassroomHours + subjectTemplate.GradeCompanyHours))
                .WithCellSpan(4, 1, 1, 2).WithCellContent(4, 1, "Modalidad: Presencial")
                .WithCellContent(4, 2, "Régimen: Anual")
                .WithCellSpan(5, 1, 1, 3).WithCellContent(5, 1, "Familia profesional: " + gradeTemplate.GradeFamilyName)

                .WithTable(2 + Subject.Blocks.Count, 6)
                .WithCellSpan(1, 1, 1, 6).WithCellContent(1, 1, subjectTemplate.SubjectCode + ": " + subjectTemplate.SubjectName)
                .WithCellContent(2, 1, "Bloques de enseñanza").WithCellContent(2, 2, "RAs").WithCellContent(2, 3, "CEs")
                .WithCellContent(2, 4, "Duración").WithCellContent(2, 5, "Fecha de inicio").WithCellContent(2, 6, "Fecha de fin")
                .Foreach<Block>(Subject.Blocks.ToList(),
                    (b, i, d) =>
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


                        d.WithCellContent(3 + i, 1, String.Format("Bloque {0}: {1}", i + 1, b.Title));
                        d.WithCellContent(3 + i, 2, rasText);
                        d.WithCellContent(3 + i, 3, criteriasText);
                        d.WithCellContent(3 + i, 4, String.Format("{0}h", hours));
                        d.WithCellContent(3 + i, 5, startActivitySchedule.HasValue ? Utils.FormatDate(startActivitySchedule.Value.start.day) : "");
                        d.WithCellContent(3 + i, 6, endActivitySchedule.HasValue ? Utils.FormatDate(endActivitySchedule.Value.start.day) : "");

                    }
                 )


                /////////////////////////////////////////////////////////////////////////////////////
                ///////////// Nivel 1: Justificación de la importancia del módulo ///////////////////
                /////////////////////////////////////////////////////////////////////////////////////

               .WithHeader1(index[1].Title)
               .Foreach<string>(GetGradeCommonText(CommonTextId.header1ImportanceJustification), addParagraph)
               .Foreach<string>(GetSubjectCommonText(CommonTextId.header1ImportanceJustification), addParagraph)

                /////////////////////////////////////////////////////////////////
                ///////////// Nivel 1: Elementos curriculares ///////////////////
                /////////////////////////////////////////////////////////////////

               .WithHeader1(index[2].Title)
               .Foreach<string>(GetGradeCommonText(CommonTextId.header1CurricularElements), addParagraph)
               .Foreach<string>(GetSubjectCommonText(CommonTextId.header1CurricularElements), addParagraph)

               .WithHeader2(index[2].Subitems[0].Title)
               .Foreach<string>(GetGradeCommonText(CommonTextId.header2GeneralObjectives), addParagraph)
               .Foreach<string>(GetSubjectCommonText(CommonTextId.header2GeneralObjectives), addParagraph)

               .WithHeader2(index[2].Subitems[1].Title)
               .Foreach<string>(GetGradeCommonText(CommonTextId.header2GeneralCompetences), addParagraph)
               .Foreach<string>(GetSubjectCommonText(CommonTextId.header2GeneralCompetences), addParagraph)

               .WithHeader2(index[2].Subitems[2].Title)
               .Foreach<string>(GetGradeCommonText(CommonTextId.header2KeyCompetences), addParagraph)
               .Foreach<string>(GetSubjectCommonText(CommonTextId.header2KeyCompetences), addParagraph)

                /////////////////////////////////////////////////////////////////////////////////
                ///////////// Nivel 1: Metodología y orientaciones didácticas ///////////////////
                /////////////////////////////////////////////////////////////////////////////////

               .WithHeader1(index[3].Title)
               .Foreach<string>(GetGradeCommonText(CommonTextId.header1MetodologyAndDidacticOrientations), addParagraph)
               .Foreach<string>(GetSubjectCommonText(CommonTextId.header1MetodologyAndDidacticOrientations), addParagraph)

               .Foreach<CommonText>(Subject.Metodologies.ToList(),
                    (c, i, d) =>
                    {
                        d.WithHeader3(c.Title)
                        .WithParagraph(c.Description);
                    }               
               )

               .WithHeader2(index[3].Subitems[0].Title)
               .Foreach<string>(GetGradeCommonText(CommonTextId.header2Metodology), addParagraph)
               .Foreach<string>(GetSubjectCommonText(CommonTextId.header2Metodology), addParagraph)

               .WithHeader2(index[3].Subitems[1].Title)
               .Foreach<string>(GetGradeCommonText(CommonTextId.header2Diversity), addParagraph)
               .Foreach<string>(GetSubjectCommonText(CommonTextId.header2Diversity), addParagraph)

                ////////////////////////////////////////////////////////////////
                ///////////// Nivel 1: Sistema de evaluación ///////////////////
                ////////////////////////////////////////////////////////////////

               .WithHeader1(index[4].Title)
               .Foreach<string>(GetGradeCommonText(CommonTextId.header1EvaluationSystem), addParagraph)
               .Foreach<string>(GetSubjectCommonText(CommonTextId.header1EvaluationSystem), addParagraph)

               .WithHeader2(index[4].Subitems[0].Title)
               .Foreach<string>(GetGradeCommonText(CommonTextId.header2EvaluationInstruments), addParagraph)
               .Foreach<string>(GetSubjectCommonText(CommonTextId.header2EvaluationInstruments), addParagraph)

               .Foreach<CommonText>(Subject.EvaluationInstrumentsTypes.ToList(),
                    (c, i, d) =>
                    {
                        d.WithHeader3(c.Title)
                        .WithParagraph(c.Description);
                    }
               )

               .WithHeader2(index[4].Subitems[1].Title)
               .Foreach<string>(GetGradeCommonText(CommonTextId.header2EvaluationOfProgramming), addParagraph)
               .Foreach<string>(GetSubjectCommonText(CommonTextId.header2EvaluationOfProgramming), addParagraph)

                ///////////////////////////////////////////////////////////////////
                ////////////// Nivel 1: Elementos transversales ///////////////////
                ///////////////////////////////////////////////////////////////////

               .WithHeader1(index[5].Title)
               .Foreach<string>(GetGradeCommonText(CommonTextId.header1TraversalElements), addParagraph)
               .Foreach<string>(GetSubjectCommonText(CommonTextId.header1TraversalElements), addParagraph)

               .WithHeader2(index[5].Subitems[0].Title)
               .Foreach<string>(GetGradeCommonText(CommonTextId.header2TraversalReadingAndTIC), addParagraph)
               .Foreach<string>(GetSubjectCommonText(CommonTextId.header2TraversalReadingAndTIC), addParagraph)

               .WithHeader2(index[5].Subitems[1].Title)
               .Foreach<string>(GetGradeCommonText(CommonTextId.header2TraversalCommunicationEntrepreneurshipAndEducation), addParagraph)
               .Foreach<string>(GetSubjectCommonText(CommonTextId.header2TraversalCommunicationEntrepreneurshipAndEducation), addParagraph)

                //////////////////////////////////////////////////////////////////////////////
                ////////////// Nivel 1: Recursos didácticos y organizativos //////////////////
                //////////////////////////////////////////////////////////////////////////////

               .WithHeader1(index[6].Title)
               .Foreach<string>(GetGradeCommonText(CommonTextId.header1Resources), addParagraph)
               .Foreach<string>(GetSubjectCommonText(CommonTextId.header1Resources), addParagraph)

               .WithHeader2(index[6].Subitems[0].Title)
               .Foreach<string>(GetGradeCommonText(CommonTextId.header2ResourcesSpaces), addParagraph)
               .Foreach<string>(GetSubjectCommonText(CommonTextId.header2ResourcesSpaces), addParagraph)

               .Foreach<CommonText>(Subject.SpaceResources.ToList(),
                    (c, i, d) =>
                    {
                        d.WithHeader3(c.Title)
                        .WithParagraph(c.Description);
                    }
               )

               .WithHeader2(index[6].Subitems[1].Title)
               .Foreach<string>(GetGradeCommonText(CommonTextId.header2ResourcesMaterialAndTools), addParagraph)
               .Foreach<string>(GetSubjectCommonText(CommonTextId.header2ResourcesMaterialAndTools), addParagraph)

               .Foreach<CommonText>(Subject.MaterialResources.ToList(),
                    (c, i, d) =>
                    {
                        d.WithHeader3(c.Title)
                        .WithParagraph(c.Description);
                    }
               )

                ///////////////////////////////////////////////////////////////////////////////
                ////////////// Nivel 1: Programación del módulo profesional ///////////////////
                ///////////////////////////////////////////////////////////////////////////////
                            
                .WithHeader1(index[7].Title)
                .Foreach<string>(GetGradeCommonText(CommonTextId.header1SubjectProgramming), addParagraph)
                .Foreach<string>(GetSubjectCommonText(CommonTextId.header1SubjectProgramming), addParagraph)

                .WithHeader2(index[7].Subitems[0].Title)
                .Foreach<string>(GetGradeCommonText(CommonTextId.header2LearningResultsAndContents), addParagraph)
                .Foreach<string>(GetSubjectCommonText(CommonTextId.header2LearningResultsAndContents), addParagraph)

                .WithHeader3(index[7].Subitems[0].Subitems[0].Title)
                .Foreach<string>(GetGradeCommonText(CommonTextId.header3LearningResults), addParagraph)
                .Foreach<string>(GetSubjectCommonText(CommonTextId.header3LearningResults), addParagraph)
                
                .Foreach<LearningResult>(subjectTemplate.LearningResults.ToList(),
                    (r, i, d) =>
                    {
                            d.WithParagraph(String.Format("RA{0}: ", i + 1) + r.Description)
                            .WithParagraph("Criterios")
                            .Foreach<CommonText>(subjectTemplate.LearningResults.ToList()[i].Criterias.ToList(),
                                (c, j, d) =>
                                {
                                    d.WithParagraph(String.Format("{0}.{1}: ", i + 1, j + 1) + c.Description);
                                }
                            );
                    }
                )

                .WithHeader3(index[7].Subitems[0].Subitems[1].Title)
                .Foreach<string>(GetGradeCommonText(CommonTextId.header3Contents), addParagraph)
                .Foreach<string>(GetSubjectCommonText(CommonTextId.header3Contents), addParagraph)

                .Foreach<Content>(subjectTemplate.Contents.ToList(),
                    (c, i, d) =>
                    {
                        d.WithParagraph(String.Format("{0}: ", i + 1) + c.Description);
                        d.Foreach<CommonText>(subjectTemplate.Contents.ToList()[i].Points.ToList(),
                            (p, j, d) =>
                            {
                                d.WithParagraph(String.Format("{0}.{1}: ", i + 1, j + 1) + p.Description);
                            }
                        );
                    }
                )

                .WithHeader2(index[7].Subitems[1].Title)
                .Foreach<string>(GetGradeCommonText(CommonTextId.header2Blocks), addParagraph)
                .Foreach<string>(GetSubjectCommonText(CommonTextId.header2Blocks), addParagraph)

                .WithTable(2 * Subject.Blocks.Count + 3, 6)
                .WithCellSpan(1, 1, 1, 5).WithCellContent(1, 1, String.Format("{0}: {1}", subjectTemplate.SubjectCode, subjectTemplate.SubjectName))
                .WithCellContent(1, 2, String.Format("Horas: {0}", subjectTemplate.GradeClassroomHours + subjectTemplate.GradeCompanyHours))
                .WithCellContent(2, 1, "Bloque de enseñanza-aprendizaje")
                .WithCellContent(2, 3, "RA")
                .WithCellContent(2, 4, "Contenidos")
                .WithCellContent(2, 5, "Evaluación")
                .WithCellContent(3, 5, "CE")
                .WithCellContent(3, 6, "Actividades evaluables")
                .WithCellSpan(2, 1, 2, 2).WithCellSpan(2, 2, 2, 1).WithCellSpan(2, 3, 2, 1).WithCellSpan(2, 4, 1, 2)
                .Foreach<Block>(Subject.Blocks.ToList(),
                    (b, i, d) =>
                    {
                        d.WithCellContent(4 + i * 2, 1, String.Format("Bloque {0}", i + 1));
                        d.WithCellContent(4 + i * 2, 2, String.Format(CultureInfo.InvariantCulture, "{0} horas", Subject.QueryBlockDuration(i)));

                        string raText = "";
                        bool first = true;
                        Subject.QueryBlockReferencedLearningResultIndexes(i).ForEach(
                            r =>
                            {
                                raText += (first ? "" : ", ") + "RA" + (r + 1); first = false;
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
                                evaluableActivitiesText += (first ? "" : "\n") + Utils.FormatEvaluableActivity(i, aIndex.evaluationType, aIndex.activityTypeIndex);
                                first = false;
                            }
                        );

                        d.WithCellContent(4 + i * 2 + 0, 3, raText);
                        d.WithCellContent(4 + i * 2 + 0, 4, contentText);
                        d.WithCellContent(4 + i * 2 + 0, 5, criteriaText);
                        d.WithCellContent(4 + i * 2 + 0, 6, evaluableActivitiesText);

                        d.WithCellContent(4 + i * 2 + 1, 1, b.Description);

                        d.WithCellSpan(4 + i * 2 + 0, 3, 2, 1);
                        d.WithCellSpan(4 + i * 2 + 0, 4, 2, 1);
                        d.WithCellSpan(4 + i * 2 + 0, 5, 2, 1);
                        d.WithCellSpan(4 + i * 2 + 0, 6, 2, 1);
                        d.WithCellSpan(4 + i * 2 + 1, 1, 1, 2);

                    }
                )

                .WithHeader2(index[7].Subitems[2].Title)
                .Foreach<string>(GetGradeCommonText(CommonTextId.header2Activities), addParagraph)
                .Foreach<string>(GetSubjectCommonText(CommonTextId.header2Activities), addParagraph)

                .Foreach<Block>(Subject.Blocks.ToList(),
                    (b, i, d) =>
                    {
                        d.WithHeader3(String.Format("Bloque {0}", i + 1));

                        d.Foreach<Activity>(b.Activities.ToList(),
                            (a, j, d) =>
                            {
                                Debug.Assert(a.Metodology != null);

                                int rows = 6 + (a.EvaluationType != ActivityEvaluationType.NotEvaluable ? 2 : 0);

                                d.WithTable(rows, 4)

                                .WithCellContent(1, 1, a.Title)

                                .WithCellContent(2, 1, a.Description)

                                .WithCellContent(3, 1, "Metodología")
                                .WithCellContent(3, 2, "Duración")
                                .WithCellContent(3, 3, "Fecha de inicio")
                                .WithCellContent(3, 4, "Fecha de fin")

                                .WithCellContent(4, 1, a.Metodology.Title)
                                .WithCellContent(4, 2, String.Format(CultureInfo.InvariantCulture, "{0:0}h ({1} sesiones)", a.Duration, GetSessionsCountText(a, schedule)))
                                .WithCellContent(4, 3, Utils.FormatStartDayHour(schedule.Find(_a => _a.activity.StorageId == a.StorageId).start, Subject.WeekSchedule))
                                .WithCellContent(4, 4, Utils.FormatEndDayHour(schedule.Find(_a => _a.activity.StorageId == a.StorageId).end, Subject.WeekSchedule))

                                .WithCellContent(5, 1, "Espacios")
                                .WithCellContent(5, 2, "Materiales")
                                .WithCellContent(5, 3, "Contenidos")
                                .WithCellContent(5, 4, "Capacidades clave")

                                .WithCellContent(6, 1, GetSpacesText(a))
                                .WithCellContent(6, 2, GetMaterialsText(a))
                                .WithCellContent(6, 3, GetContentsText(i, a))
                                .WithCellContent(6, 4, GetKeyCapacitiesText(a))

                                .If(a.EvaluationType != ActivityEvaluationType.NotEvaluable,
                                
                                    (d) =>
                                    {
                                        Debug.Assert(a.EvaluationInstrumentType != null);

                                        d.WithCellContent(7, 1, "Código de actividad evaluable")
                                        .WithCellContent(7, 2, "Instrumento de evaluación")
                                        .WithCellContent(7, 3, "Peso en los resultados de aprendizaje")
                                        .WithCellContent(7, 4, "Criterios de evaluación")

                                        .WithCellContent(8, 1, a.EvaluationType != ActivityEvaluationType.NotEvaluable ? Utils.FormatEvaluableActivity(i, a.EvaluationType, Subject.QueryEvaluableActivityTypeIndex(i, a)) : "")
                                        .WithCellContent(8, 2, a.EvaluationType != ActivityEvaluationType.NotEvaluable ? a.EvaluationInstrumentType.Title : "")
                                        .WithCellContent(8, 3, GetReferencedLearningResultsWeightsText(i, a))
                                        .WithCellContent(8, 4, GetReferencedCriteriasText(i, a));

                                    }
                                );

                                d.WithCellSpan(1, 1, 1, 4)
                                .WithCellSpan(2, 1, 1, 4);
                            }
                        );
                    }
                )


                //////////////////////////////////////////////////////////////////////
                ////////////// Nivel 1: Referencias bibliográficas ///////////////////
                //////////////////////////////////////////////////////////////////////
                            
                .WithHeader1(index[8].Title)
                .Foreach<CommonText>(Subject.Citations.ToList(),
                    (c, i, d) =>
                    {
                        d.WithParagraph(String.Format("{0}- {1}", i + 1, c.Description));
                    }
                )

                //////////////////////////////////////////////////
                ////////////// Nivel 1: Anexos ///////////////////
                //////////////////////////////////////////////////
                            
                //.WithInner(Tag.Create("h1").WithInner(index[9].Title).WithId("Apartado10"))
                //.WithInner(Tag.Create("h2").WithInner("Cuadro de distribución de pesos").WithId("Apartado10-1"))

                .WithHeader1(index[9].Title)
                .WithHeader2(index[9].Subitems[0].Title)

                .WithTable(2 + Subject.QueryEvaluableActivityIndexes().Count, 2 + subjectTemplate.LearningResults.ToList().Count)
                .WithCellContent(1, 1, NonBreakingSpace).WithCellContent(1, 2, NonBreakingSpace)
                .Foreach<LearningResult>(subjectTemplate.LearningResults.ToList(),
                    (r, i, d) => { d.WithCellContent(1, 3 + i, String.Format("RA{0}", i + 1)); }
                )
                .WithCellContent(2, 1, "Bloque").WithCellContent(2, 2, "Peso" + NonBreakingSpace + "RA")
                .Foreach<SubjectLearningResultIndexesWeight>(Subject.QueryLearningResultsIndexesWeights(),
                    (r, i, d) =>
                    {
                        d.WithCellContent(2, 3 + i, String.Format("{0:0}%", r.weight));
                    }
                )
                .Do( 
                    (d) =>
                    {
                        List<EvaluableActivityIndex> activityIndexes = Subject.QueryEvaluableActivityIndexes();
        
                        List<int> blockIndexes = new();
                        Dictionary<int, int> blockActivityCount = new();
                        Dictionary<int, int> blockActivityStart = new();

                        int lastBlock = -1;

                        for(int i = 0; i < activityIndexes.Count; i++)
                        {
                            EvaluableActivityIndex activityIndex = activityIndexes[i];

                            if(lastBlock != activityIndex.blockIndex)
                            {
                                d.WithCellContent(3 + i, 1, String.Format("Bloque" + NonBreakingSpace + "{0}", activityIndex.blockIndex + 1));

                                blockIndexes.Add(activityIndex.blockIndex);
                                blockActivityCount[activityIndex.blockIndex] = 1;
                                blockActivityStart[activityIndex.blockIndex] = i;
                            }
                            else
                            {
                                blockActivityCount[activityIndex.blockIndex] ++;
                            }

                            lastBlock = activityIndex.blockIndex;

                            d.WithCellContent(3 + i, 2, Utils.FormatEvaluableActivity(activityIndex.blockIndex, activityIndex.evaluationType, activityIndex.activityTypeIndex));

                            List<SubjectLearningResultIndexesWeight> resultsWeights = Subject.QueryActivityLearningResultsIndexesWeight(activityIndex.blockIndex, activityIndex.activityIndex);
                            
                            for(int j = 0; j < resultsWeights.Count; j ++)
                            {
                                d.WithCellContent(3 + i, 3 + j, resultsWeights[j].weight > 0 ? String.Format("{0:0}%", resultsWeights[j].weight) : NonBreakingSpace);
                            }
                        }

                        for(int i = 0; i < blockIndexes.Count; i++)
                        {
                            int blockIndex = blockIndexes[i];
                            d.WithCellSpan(3 + i + (i > 0 ? blockActivityCount[blockIndexes[i - 1]] - 1 : 0) , 1, blockActivityCount[blockIndex], 1);
                        }

                    }
                )

                //.WithInner(Table.Create()
                //    .WithRow()
                //        .WithCell("&nbsp;", 1, 2)
                //        .WithCellForeach<LearningResult>(subjectTemplate.LearningResults.ToList(),
                //            (learningResult, i, table) =>
                //            {
                //                table.WithCellInner(String.Format("RA{0}", i + 1)).WithCellClass("tableHeader1");
                //            }
                //        )
                //    .WithRow()
                //        .WithCell("Bloque").WithCellClass("tableHeader1")
                //        .WithCell("Peso&nbsp;RA").WithCellClass("tableHeader1")
                //        .WithCellForeach<SubjectLearningResultIndexesWeight>(Subject.QueryLearningResultsIndexesWeights(),
                //            (resultWeight, i, table) =>
                //            {
                //                table.WithCellInner(String.Format("{0:0}%", resultWeight.weight)).WithCellClass("tableHeader2");
                //            }
                //        )
                //    .WithRowForeach<Block>(Subject.Blocks.ToList(),
                //        (block, i, table) =>
                //        {
                //            List<EvaluableActivityIndex> evaluableActivityIndexes = Subject.QueryBlockEvaluableActivityIndexes(i);

                //            table.WithCell(String.Format("Bloque&nbsp;{0}", i + 1), evaluableActivityIndexes.Count, 1).WithCellClass("tableHeader1");

                //            bool first = true;

                //            foreach(EvaluableActivityIndex activityInfo in evaluableActivityIndexes)
                //            {
                //                if(!first) { table.WithRow(); }
                //                table.WithCell(Utils.FormatEvaluableActivity(i, activityInfo.evaluationType, activityInfo.activityTypeIndex)).WithCellClass("tableHeader2");

                //                table.WithCellForeach<SubjectLearningResultIndexesWeight>(Subject.QueryActivityLearningResultsIndexesWeight(i, activityInfo.activityIndex),
                //                    (resultWeight, j, table) =>
                //                    {
                //                        table.WithCellInner(resultWeight.weight > 0 ? String.Format("{0:0}%", resultWeight.weight) : "&nbsp;");
                //                    }

                //                );

                //                first = false;
                //            }
                //        }
                //    )
                                    
                //)


               .Save(path)
               .Close();
            
            object missingValue = Missing.Value;
    
            app.Quit(ref missingValue, ref missingValue, ref missingValue);
        }

        public override void LoadOrCreateSettings()
        {

        }

        public override void SaveSettings()
        {

        }

        public override GeneratorValidationResult Validate()
        {
            GeneratorValidationResult result = new() { code = GeneratorValidationCode.success };

            return result;
        }
    }
}
