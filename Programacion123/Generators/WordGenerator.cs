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

            float? dpiX = null;
            float? dpiY = null;

            GetScreenDpi(out dpiX, out dpiY);

            WordDocument.Create(app)
               .If(dpiX != null && dpiY != null, d => d.WithReferenceDpi(dpiX.Value, dpiY.Value))
               .WithMargins(style.Margins)
               .WithOrientation(style.Orientation)
               .WithTextStyle(DocumentTextElementId.NormalText, style.TextElementStyles[DocumentTextElementId.NormalText])
               .WithTextStyle(DocumentTextElementId.Header1, style.TextElementStyles[DocumentTextElementId.Header1])
               .WithTextStyle(DocumentTextElementId.Header2, style.TextElementStyles[DocumentTextElementId.Header2])
               .WithTextStyle(DocumentTextElementId.Header3, style.TextElementStyles[DocumentTextElementId.Header3])
               .WithTextStyle(DocumentTextElementId.Header4, style.TextElementStyles[DocumentTextElementId.Header4])
               .WithTextStyle(DocumentTextElementId.TableText, style.TextElementStyles[DocumentTextElementId.TableText])
               .WithTextStyle(DocumentTextElementId.TableHeader1Text, style.TextElementStyles[DocumentTextElementId.TableHeader1Text])
               .WithTextStyle(DocumentTextElementId.TableHeader2Text, style.TextElementStyles[DocumentTextElementId.TableHeader2Text])
               .WithTextStyle(DocumentTextElementId.IndexLevel1, style.TextElementStyles[DocumentTextElementId.IndexLevel1])
               .WithTextStyle(DocumentTextElementId.IndexLevel2, style.TextElementStyles[DocumentTextElementId.IndexLevel2])
               .WithTextStyle(DocumentTextElementId.IndexLevel3, style.TextElementStyles[DocumentTextElementId.IndexLevel3])
               .WithTextStyle(DocumentTextElementId.IndexTitle, style.TextElementStyles[DocumentTextElementId.IndexTitle])
               .WithTextStyle(DocumentTextElementId.CoverSubjectCode, style.TextElementStyles[DocumentTextElementId.CoverSubjectCode])
               .WithTextStyle(DocumentTextElementId.CoverSubjectName, style.TextElementStyles[DocumentTextElementId.CoverSubjectName])
               .WithTextStyle(DocumentTextElementId.CoverGradeTypeName, style.TextElementStyles[DocumentTextElementId.CoverGradeTypeName])
               .WithTextStyle(DocumentTextElementId.CoverGradeName, style.TextElementStyles[DocumentTextElementId.CoverGradeName])
               .WithTextStyle(DocumentTextElementId.WeightsTableText, style.TextElementStyles[DocumentTextElementId.WeightsTableText])
               .WithTextStyle(DocumentTextElementId.WeightsTableHeader1Text, style.TextElementStyles[DocumentTextElementId.WeightsTableHeader1Text])
               .WithTextStyle(DocumentTextElementId.WeightsTableHeader2Text, style.TextElementStyles[DocumentTextElementId.WeightsTableHeader2Text])
               .WithTableElementStyle(DocumentTableElementId.TableNormalCell, style.TableElementStyles[DocumentTableElementId.TableNormalCell])
               .WithTableElementStyle(DocumentTableElementId.TableHeader1Cell, style.TableElementStyles[DocumentTableElementId.TableHeader1Cell])
               .WithTableElementStyle(DocumentTableElementId.TableHeader2Cell, style.TableElementStyles[DocumentTableElementId.TableHeader2Cell])
               .WithTableElementStyle(DocumentTableElementId.TableWeightsNormalCell, style.TableElementStyles[DocumentTableElementId.TableWeightsNormalCell])
               .WithTableElementStyle(DocumentTableElementId.TableWeightsHeader1Cell, style.TableElementStyles[DocumentTableElementId.TableWeightsHeader1Cell])
               .WithTableElementStyle(DocumentTableElementId.TableWeightsHeader2Cell, style.TableElementStyles[DocumentTableElementId.TableWeightsHeader2Cell])
               .WithCoverElementPosition(DocumentCoverElementId.Logo, style.CoverElementStyles[DocumentCoverElementId.Logo].Position)
               .WithCoverElementPosition(DocumentCoverElementId.Cover, style.CoverElementStyles[DocumentCoverElementId.Cover].Position)
               .WithCoverElementPosition(DocumentCoverElementId.GradeName, style.CoverElementStyles[DocumentCoverElementId.GradeName].Position)
               .WithCoverElementPosition(DocumentCoverElementId.GradeTypeName, style.CoverElementStyles[DocumentCoverElementId.GradeTypeName].Position)
               .WithCoverElementPosition(DocumentCoverElementId.SubjectCode, style.CoverElementStyles[DocumentCoverElementId.SubjectCode].Position)
               .WithCoverElementPosition(DocumentCoverElementId.SubjectName, style.CoverElementStyles[DocumentCoverElementId.SubjectName].Position)

                //////////////////////////////////////////////////////////////////
                ///////////// Nivel 1: Portada                 ///////////////////
                //////////////////////////////////////////////////////////////////

               .If(!String.IsNullOrEmpty(style.CoverBase64), (d) => d.WithCoverImageElement(style.CoverBase64, DocumentCoverElementId.Cover))
               .If(!String.IsNullOrEmpty(style.LogoBase64), (d) => d.WithCoverImageElement(style.LogoBase64, DocumentCoverElementId.Logo))
               .WithCoverTextElement("Módulo profesional " + subjectTemplate.SubjectCode, DocumentTextElementId.CoverSubjectCode, DocumentCoverElementId.SubjectCode)
               .WithCoverTextElement(subjectTemplate.SubjectName, DocumentTextElementId.CoverSubjectName, DocumentCoverElementId.SubjectName)
               .WithCoverTextElement(gradeTemplate.GradeName, DocumentTextElementId.CoverGradeName, DocumentCoverElementId.GradeName)
               .WithCoverTextElement(GetGradeTypeName(), DocumentTextElementId.CoverGradeTypeName, DocumentCoverElementId.GradeTypeName)
               .WithPageBreak()

                //////////////////////////////////////////////////////////////////
                ///////////// Nivel 1: Índice                  ///////////////////
                //////////////////////////////////////////////////////////////////

               .WithIndex()
               .WithPageBreak()
               
                //////////////////////////////////////////////////////////////////
                ///////////// Nivel 1: Organización del módulo ///////////////////
                //////////////////////////////////////////////////////////////////

               .WithHeader1(index[0].Title)
               .Foreach<string>(GetGradeCommonText(CommonTextId.header1ModuleOrganization), addParagraph)
               .Foreach<string>(GetSubjectCommonText(CommonTextId.header1ModuleOrganization), addParagraph)

                .WithTable(5, 3)
                .WithCellSpan(1, 1, 1, 3).WithCellHeader1(1, 1, GetGradeTypeName() + " - " + gradeTemplate.GradeName)
                .WithCellSpan(2, 1, 1, 3).WithCellHeader2(2, 1, "Módulo profesional:MP" + subjectTemplate.SubjectCode + " - " + subjectTemplate.SubjectName)
                .WithCell(3, 1, "Horas centro educativo: " +  subjectTemplate.GradeClassroomHours)
                .WithCell(3, 2, "Horas empresa: " + subjectTemplate.GradeCompanyHours)
                .WithCell(3, 3, "Horas totales: " + (subjectTemplate.GradeClassroomHours + subjectTemplate.GradeCompanyHours))
                .WithCellSpan(4, 1, 1, 2).WithCell(4, 1, "Modalidad: Presencial")
                .WithCell(4, 2, "Régimen: Anual")
                .WithCellSpan(5, 1, 1, 3).WithCell(5, 1, "Familia profesional: " + gradeTemplate.GradeFamilyName)

                .WithTable(2 + Subject.Blocks.Count, 6)
                .WithCellSpan(1, 1, 1, 6).WithCellHeader1(1, 1, subjectTemplate.SubjectCode + ": " + subjectTemplate.SubjectName)
                .WithCellHeader2(2, 1, "Bloques de enseñanza").WithCellHeader2(2, 2, "RAs").WithCellHeader2(2, 3, "CEs")
                .WithCellHeader2(2, 4, "Duración").WithCellHeader2(2, 5, "Fecha de inicio").WithCellHeader2(2, 6, "Fecha de fin")
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


                        d.WithCell(3 + i, 1, String.Format("Bloque {0}: {1}", i + 1, b.Title));
                        d.WithCell(3 + i, 2, rasText);
                        d.WithCell(3 + i, 3, criteriasText);
                        d.WithCell(3 + i, 4, String.Format("{0}h", hours));
                        d.WithCell(3 + i, 5, startActivitySchedule.HasValue ? Utils.FormatDate(startActivitySchedule.Value.start.day) : "");
                        d.WithCell(3 + i, 6, endActivitySchedule.HasValue ? Utils.FormatDate(endActivitySchedule.Value.start.day) : "");

                    }
                 )

                .WithPageBreak()

                /////////////////////////////////////////////////////////////////////////////////////
                ///////////// Nivel 1: Justificación de la importancia del módulo ///////////////////
                /////////////////////////////////////////////////////////////////////////////////////

               .WithHeader1(index[1].Title)
               .Foreach<string>(GetGradeCommonText(CommonTextId.header1ImportanceJustification), addParagraph)
               .Foreach<string>(GetSubjectCommonText(CommonTextId.header1ImportanceJustification), addParagraph)

                .WithPageBreak()

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

                .WithPageBreak()

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

                .WithPageBreak()

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

                .WithPageBreak()

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

                .WithPageBreak()

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

                .WithPageBreak()

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
                .WithCellSpan(1, 1, 1, 5).WithCellHeader1(1, 1, String.Format("{0}: {1}", subjectTemplate.SubjectCode, subjectTemplate.SubjectName))
                .WithCellHeader1(1, 2, String.Format("Horas: {0}", subjectTemplate.GradeClassroomHours + subjectTemplate.GradeCompanyHours))
                .WithCellHeader2(2, 1, "Bloque de enseñanza-aprendizaje")
                .WithCellHeader2(2, 2, "")
                .WithCellHeader2(2, 3, "RA")
                .WithCellHeader2(2, 4, "Contenidos")
                .WithCellHeader2(2, 5, "Evaluación")
                .WithCellHeader2(2, 6, "")
                .WithCellHeader2(3, 5, "CE")
                .WithCellHeader2(3, 6, "Actividades evaluables")
                .WithCellSpan(2, 1, 2, 2).WithCellSpan(2, 2, 2, 1).WithCellSpan(2, 3, 2, 1).WithCellSpan(2, 4, 1, 2)
                .Foreach<Block>(Subject.Blocks.ToList(),
                    (b, i, d) =>
                    {
                        d.WithCell(4 + i * 2, 1, String.Format("Bloque {0}", i + 1));
                        d.WithCell(4 + i * 2, 2, String.Format(CultureInfo.InvariantCulture, "{0} horas", Subject.QueryBlockDuration(i)));

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

                        d.WithCell(4 + i * 2 + 0, 3, raText);
                        d.WithCell(4 + i * 2 + 0, 4, contentText);
                        d.WithCell(4 + i * 2 + 0, 5, criteriaText);
                        d.WithCell(4 + i * 2 + 0, 6, evaluableActivitiesText);

                        d.WithCell(4 + i * 2 + 1, 1, b.Description);

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

                                .WithCellHeader1(1, 1, a.Title)
                                .WithCellHeader1(1, 2, "")
                                .WithCellHeader1(1, 3, "")
                                .WithCellHeader1(1, 4, "")

                                .WithCell(2, 1, a.Description)

                                .WithCellHeader2(3, 1, "Metodología")
                                .WithCellHeader2(3, 2, "Duración")
                                .WithCellHeader2(3, 3, "Fecha de inicio")
                                .WithCellHeader2(3, 4, "Fecha de fin")

                                .WithCell(4, 1, a.Metodology.Title)
                                .WithCell(4, 2, String.Format(CultureInfo.InvariantCulture, "{0:0}h ({1} sesiones)", a.Duration, GetSessionsCountText(a, schedule)))
                                .WithCell(4, 3, Utils.FormatStartDayHour(schedule.Find(_a => _a.activity.StorageId == a.StorageId).start, Subject.WeekSchedule))
                                .WithCell(4, 4, Utils.FormatEndDayHour(schedule.Find(_a => _a.activity.StorageId == a.StorageId).end, Subject.WeekSchedule))

                                .WithCellHeader2(5, 1, "Espacios")
                                .WithCellHeader2(5, 2, "Materiales")
                                .WithCellHeader2(5, 3, "Contenidos")
                                .WithCellHeader2(5, 4, "Capacidades clave")

                                .WithCell(6, 1, GetSpacesText(a))
                                .WithCell(6, 2, GetMaterialsText(a))
                                .WithCell(6, 3, GetContentsText(i, a))
                                .WithCell(6, 4, GetKeyCapacitiesText(a))

                                .If(a.EvaluationType != ActivityEvaluationType.NotEvaluable,
                                
                                    (d) =>
                                    {
                                        Debug.Assert(a.EvaluationInstrumentType != null);

                                        d.WithCellHeader2(7, 1, "Código de actividad evaluable")
                                        .WithCellHeader2(7, 2, "Instrumento de evaluación")
                                        .WithCellHeader2(7, 3, "Peso en los resultados de aprendizaje")
                                        .WithCellHeader2(7, 4, "Criterios de evaluación")

                                        .WithCell(8, 1, a.EvaluationType != ActivityEvaluationType.NotEvaluable ? Utils.FormatEvaluableActivity(i, a.EvaluationType, Subject.QueryEvaluableActivityTypeIndex(i, a)) : "")
                                        .WithCell(8, 2, a.EvaluationType != ActivityEvaluationType.NotEvaluable ? a.EvaluationInstrumentType.Title : "")
                                        .WithCell(8, 3, GetReferencedLearningResultsWeightsText(i, a))
                                        .WithCell(8, 4, GetReferencedCriteriasText(i, a));

                                    }
                                );

                                d.WithCellSpan(1, 1, 1, 4)
                                .WithCellSpan(2, 1, 1, 4);
                            }
                        );
                    }
                )

                .WithPageBreak()

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

                .WithPageBreak()

                //////////////////////////////////////////////////
                ////////////// Nivel 1: Anexos ///////////////////
                //////////////////////////////////////////////////
                            
                .WithHeader1(index[9].Title)
                .WithHeader2(index[9].Subitems[0].Title)

                .WithTable(2 + Subject.QueryEvaluableActivityIndexes().Count, 2 + subjectTemplate.LearningResults.ToList().Count)
                .WithEmptyCell(1, 1).WithCellBorders(false, true, false, false).WithEmptyCell(1, 2).WithCellBorders(false, true, false, true)
                .Foreach<LearningResult>(subjectTemplate.LearningResults.ToList(),
                    (r, i, d) => { d.WithCell(1, 3 + i, String.Format("RA{0}", i + 1), WordDocument.TextStyleWeightsTableHeader2, WordDocument.CellStyleWeightsHeader2); }
                )
                .WithCell(2, 1, "Bloque", WordDocument.TextStyleWeightsTableHeader1, WordDocument.CellStyleWeightsHeader1)
                .WithCell(2, 2, "Peso" + NonBreakingSpace + "RA", WordDocument.TextStyleWeightsTableHeader1, WordDocument.CellStyleWeightsHeader1)
                .Foreach<SubjectLearningResultIndexesWeight>(Subject.QueryLearningResultsIndexesWeights(),
                    (r, i, d) =>
                    {
                        d.WithCell(2, 3 + i, String.Format("{0:0}%", r.weight), WordDocument.TextStyleWeightsTableHeader2, WordDocument.CellStyleWeightsHeader2);
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
                                d.WithCell(3 + i, 1, String.Format("Bloque" + NonBreakingSpace + "{0}", activityIndex.blockIndex + 1), WordDocument.TextStyleWeightsTableHeader1, WordDocument.CellStyleWeightsHeader1);

                                blockIndexes.Add(activityIndex.blockIndex);
                                blockActivityCount[activityIndex.blockIndex] = 1;
                                blockActivityStart[activityIndex.blockIndex] = i;
                            }
                            else
                            {
                                blockActivityCount[activityIndex.blockIndex] ++;
                            }

                            lastBlock = activityIndex.blockIndex;

                            d.WithCell(3 + i, 2, Utils.FormatEvaluableActivity(activityIndex.blockIndex, activityIndex.evaluationType, activityIndex.activityTypeIndex), WordDocument.TextStyleWeightsTableHeader2, WordDocument.CellStyleWeightsHeader2);

                            List<SubjectLearningResultIndexesWeight> resultsWeights = Subject.QueryActivityLearningResultsIndexesWeight(activityIndex.blockIndex, activityIndex.activityIndex);
                            
                            for(int j = 0; j < resultsWeights.Count; j ++)
                            {
                                d.WithCell(3 + i, 3 + j, resultsWeights[j].weight > 0 ? String.Format("{0:0}%", resultsWeights[j].weight) : NonBreakingSpace, WordDocument.TextStyleWeightsTable, WordDocument.CellStyleWeightsNormal);
                            }
                        }

                        for(int i = 0; i < blockIndexes.Count; i++)
                        {
                            int blockIndex = blockIndexes[i];
                            d.WithCellSpan(3 + i + (i > 0 ? blockActivityCount[blockIndexes[i - 1]] - 1 : 0) , 1, blockActivityCount[blockIndex], 1);
                        }

                    }
                )

               .WithPageBreak()
               .WithPageNumbering()

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
