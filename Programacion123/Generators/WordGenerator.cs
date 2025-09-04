using Microsoft.Office.Interop.Word;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Programacion123
{

    public partial class WordGenerator : Generator
    {
        public const string SettingsId = "DocGenerator";

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

            Application app = new();

            WordDocument doc = new(app);

            doc.WithMargins(style.Margins)
               .WithOrientation(style.Orientation)
               .WithTextStyle(DocumentTextElementId.NormalText, style.TextElementStyles[DocumentTextElementId.NormalText])
               .WithTextStyle(DocumentTextElementId.Header1, style.TextElementStyles[DocumentTextElementId.Header1])
               .WithTextStyle(DocumentTextElementId.Header2, style.TextElementStyles[DocumentTextElementId.Header2])
               .WithTextStyle(DocumentTextElementId.Header3, style.TextElementStyles[DocumentTextElementId.Header3])
               .WithTextStyle(DocumentTextElementId.Header4, style.TextElementStyles[DocumentTextElementId.Header4])
               .WithParagraph("Hola")
               .WithParagraph("Qué tal")
               .WithHeader1("Qué tal")
               .WithParagraph("Qué tal")
               .WithHeader2("Qué tal")
               .WithParagraph("Qué tal")
               .WithHeader3("Qué tal")
               .WithParagraph("Qué tal")

               
                //////////////////////////////////////////////////////////////////
                ///////////// Nivel 1: Organización del módulo ///////////////////
                //////////////////////////////////////////////////////////////////

                //.WithInner(Tag.Create("h1").WithInner(index[0].Title).WithId("Apartado1"))
                //.WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header1ModuleOrganization), addCommonTextTags)
                //.WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header1ModuleOrganization), addCommonTextTags)

               .WithHeader1(index[0].Title)
               .Foreach<string>(GetGradeCommonText(CommonTextId.header1ModuleOrganization), addParagraph)
               .Foreach<string>(GetSubjectCommonText(CommonTextId.header1ModuleOrganization), addParagraph)
               
                /////////////////////////////////////////////////////////////////////////////////////
                ///////////// Nivel 1: Justificación de la importancia del módulo ///////////////////
                /////////////////////////////////////////////////////////////////////////////////////

                //.WithInner(Tag.Create("h1").WithInner(index[1].Title).WithId("Apartado2"))
                //.WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header1ImportanceJustification), addCommonTextTags)
                //.WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header1ImportanceJustification), addCommonTextTags)
               
               .WithHeader1(index[1].Title)
               .Foreach<string>(GetGradeCommonText(CommonTextId.header1ImportanceJustification), addParagraph)
               .Foreach<string>(GetSubjectCommonText(CommonTextId.header1ImportanceJustification), addParagraph)

                /////////////////////////////////////////////////////////////////
                ///////////// Nivel 1: Elementos curriculares ///////////////////
                /////////////////////////////////////////////////////////////////

                //.WithInner(Tag.Create("h1").WithInner(index[2].Title).WithId("Apartado3"))
                //.WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header1CurricularElements), addCommonTextTags)
                //.WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header1CurricularElements), addCommonTextTags)
                //.WithInner(Tag.Create("h2").WithInner(index[2].Subitems[0].Title).WithId("Apartado3-1"))
                //.WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2GeneralObjectives), addCommonTextTags)
                //.WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2GeneralObjectives), addCommonTextTags)
                //.WithInner(Tag.Create("div")
                //    .WithInnerForeach<CommonText>(subjectTemplate.GeneralObjectives.ToList(),
                //        (o, i, l) =>
                //        {
                //            int index = subjectTemplate.GradeTemplate.GeneralObjectives.ToList().FindIndex(_o => _o.StorageId == o.StorageId);
                //            l.Add(Tag.Create("div").WithInner(String.Format("{0}. {1}", Utils.FormatLetterPrefixLowercase(index), o.Description)));
                //        }
                //    )
                // )
                //.WithInner(Tag.Create("h2").WithInner(index[2].Subitems[1].Title).WithId("Apartado3-2"))
                //.WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2GeneralCompetences), addCommonTextTags)
                //.WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2GeneralCompetences), addCommonTextTags)
                //.WithInner(Tag.Create("div")
                //    .WithInnerForeach<CommonText>(subjectTemplate.GeneralCompetences.ToList(),
                //        (c, i, l) =>
                //        {
                //            int index = subjectTemplate.GradeTemplate.GeneralCompetences.ToList().FindIndex(_c => _c.StorageId == c.StorageId);
                //            l.Add(Tag.Create("div").WithInner(String.Format("{0}. {1}", Utils.FormatLetterPrefixLowercase(index), c.Description)));
                //        }
                //    )
                // )
                //.WithInner(Tag.Create("h2").WithInner(index[2].Subitems[2].Title).WithId("Apartado3-3"))
                //.WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2KeyCompetences), addCommonTextTags)
                //.WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2KeyCompetences), addCommonTextTags)
                //.WithInnerForeach<int>(Subject.QueryReferencedKeyCompetencesIndexes(),
                //    (c, i, l) =>
                //    {
                //        l.Add(Tag.Create("h3").WithInner(gradeTemplate.KeyCapacities[c].Title));
                //        l.Add(Tag.Create("div").WithInner(gradeTemplate.KeyCapacities[c].Description));
                //    }
                // )
                //.WithInner(pageBreak)

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

                //.WithInner(Tag.Create("h1").WithInner(index[3].Title).WithId("Apartado4"))
                //.WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header1MetodologyAndDidacticOrientations), addCommonTextTags)
                //.WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header1MetodologyAndDidacticOrientations), addCommonTextTags)
                //.WithInner(Tag.Create("h2").WithInner("Metodología general y específica de la materia").WithId("Apartado4-1"))
                //.WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2Metodology), addCommonTextTags)
                //.WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2Metodology), addCommonTextTags)
                //.WithInnerForeach<CommonText>(Subject.Metodologies.ToList(),
                //    (c, i, l) =>
                //    {
                //        l.Add(Tag.Create("h3").WithInner(c.Title).WithId(String.Format("Apartado4-1-{0}", i + 1)));
                //        l.Add(Tag.Create("div").WithInner(c.Description));
                //    }
                //)
                //.WithInner(Tag.Create("h2").WithInner("Medidas de atención al alumnado con necesidad específica de apoyo educativo o con necesidad de compensación educativa: atención a la diversidad").WithId("Apartado4-2"))
                //.WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2Diversity), addCommonTextTags)
                //.WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2Diversity), addCommonTextTags)
                //.WithInner(pageBreak)

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

                //.WithInner(Tag.Create("h1").WithInner(index[4].Title).WithId("Apartado5"))
                //.WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header1EvaluationSystem), addCommonTextTags)
                //.WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header1EvaluationSystem), addCommonTextTags)
                //.WithInner(Tag.Create("h2").WithInner("Instrumentos de evaluación").WithId("Apartado5-1"))
                //.WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2EvaluationInstruments), addCommonTextTags)
                //.WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2EvaluationInstruments), addCommonTextTags)
                //.WithInnerForeach<CommonText>(Subject.EvaluationInstrumentsTypes.ToList(),
                //    (c, i, l) =>
                //    {
                //        l.Add(Tag.Create("h3").WithInner(c.Title).WithId(String.Format("Apartado5-1-{0}", i + 1)));
                //        l.Add(Tag.Create("div").WithInner(c.Description));
                //    }
                //)
                //.WithInner(Tag.Create("h2").WithInner("Evaluación del funcionamiento de la programación").WithId("Apartado5-2"))
                //.WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2EvaluationOfProgramming), addCommonTextTags)
                //.WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2EvaluationOfProgramming), addCommonTextTags)
                //.WithInner(pageBreak)

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

                //.WithInner(Tag.Create("h1").WithInner(index[5].Title).WithId("Apartado6"))
                //.WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header1TraversalElements), addCommonTextTags)
                //.WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header1TraversalElements), addCommonTextTags)
                //.WithInner(Tag.Create("h2").WithInner("Fomento de la lectura y tecnologías de la información y de comunicación").WithId("Apartado6-1"))
                //.WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2TraversalReadingAndTIC), addCommonTextTags)
                //.WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2TraversalReadingAndTIC), addCommonTextTags)
                //.WithInner(Tag.Create("h2").WithInner("Comunicación audiovisual, emprendimiento, educación cívica y constitucional").WithId("Apartado6-2"))
                //.WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2TraversalCommunicationEntrepreneurshipAndEducation), addCommonTextTags)
                //.WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2TraversalCommunicationEntrepreneurshipAndEducation), addCommonTextTags)
                //.WithInner(pageBreak)

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

                //.WithInner(Tag.Create("h1").WithInner(index[6].Title).WithId("Apartado7"))
                //.WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header1Resources), addCommonTextTags)
                //.WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header1Resources), addCommonTextTags)                            
                //.WithInner(Tag.Create("h2").WithInner("Espacios requeridos").WithId("Apartado7-1"))
                //.WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2ResourcesSpaces), addCommonTextTags)
                //.WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2ResourcesSpaces), addCommonTextTags)                            
                //.WithInnerForeach<CommonText>(Subject.SpaceResources.ToList(),
                //    (c, i, l) =>
                //    {
                //        l.Add(Tag.Create("h3").WithInner(c.Title).WithId(String.Format("Apartado7-1-{0}", i + 1)));
                //        l.Add(Tag.Create("div").WithInner(c.Description));
                //    }
                //    )
                //.WithInner(Tag.Create("h2").WithInner("Materiales y herramientas").WithId("Apartado7-2"))
                //.WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2ResourcesMaterialAndTools), addCommonTextTags)
                //.WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2ResourcesMaterialAndTools), addCommonTextTags) 
                //.WithInnerForeach<CommonText>(Subject.MaterialResources.ToList(),
                //    (c, i, l) =>
                //    {
                //        l.Add(Tag.Create("h3").WithInner(c.Title).WithId(String.Format("Apartado7-2-{0}", i + 1)));
                //        l.Add(Tag.Create("div").WithInner(c.Description));
                //    }
                //    )
                //.WithInner(pageBreak)

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
                            
                //.WithInner(Tag.Create("h1").WithInner(index[7].Title).WithId("Apartado8"))
                //.WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header1SubjectProgramming), addCommonTextTags)
                //.WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header1SubjectProgramming), addCommonTextTags) 

                .WithHeader1(index[7].Title)
                .Foreach<string>(GetGradeCommonText(CommonTextId.header1SubjectProgramming), addParagraph)
                .Foreach<string>(GetSubjectCommonText(CommonTextId.header1SubjectProgramming), addParagraph)

                //.WithInner(Tag.Create("h2").WithInner("Resultados de aprendizaje, criterios de evaluación y contenidos").WithId("Apartado8-1"))
                //.WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2LearningResultsAndContents), addCommonTextTags)
                //.WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2LearningResultsAndContents), addCommonTextTags) 

                .WithHeader2(index[7].Subitems[0].Title)
                .Foreach<string>(GetGradeCommonText(CommonTextId.header2LearningResultsAndContents), addParagraph)
                .Foreach<string>(GetSubjectCommonText(CommonTextId.header2LearningResultsAndContents), addParagraph)

                //.WithInner(Tag.Create("h3").WithInner("Resultados de aprendizaje y criterios de evaluación").WithId("Apartado8-1-1"))
                //.WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header3LearningResults), addCommonTextTags)
                //.WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header3LearningResults), addCommonTextTags)
                
                .WithHeader3(index[7].Subitems[0].Subitems[0].Title)
                .Foreach<string>(GetGradeCommonText(CommonTextId.header3LearningResults), addParagraph)
                .Foreach<string>(GetSubjectCommonText(CommonTextId.header3LearningResults), addParagraph)

                //.WithInnerForeach<LearningResult>(subjectTemplate.LearningResults.ToList(),
                //    (r, i, l) =>
                //    {
                //            l.Add(Tag.Create("div").WithInner(String.Format("RA{0}: ", i + 1) + r.Description));
                //            l.Add(Tag.Create("div").WithInner("Criterios"));

                //            l.Add(Tag.Create("div")
                //            .WithInnerForeach<CommonText>(subjectTemplate.LearningResults.ToList()[i].Criterias.ToList(),
                //                (c, j, l) =>
                //                {
                //                    l.Add(Tag.Create("div").WithInner(String.Format("{0}.{1}: ", i + 1, j + 1) + c.Description));
                //                }
                //            ));
                //    }
                //    )
                
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

                //.WithInner(Tag.Create("h3").WithInner("Contenidos").WithId("Apartado8-1-2"))
                //.WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header3Contents), addCommonTextTags)
                //.WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header3Contents), addCommonTextTags) 

                .WithHeader3(index[7].Subitems[0].Subitems[1].Title)
                .Foreach<string>(GetGradeCommonText(CommonTextId.header3Contents), addParagraph)
                .Foreach<string>(GetSubjectCommonText(CommonTextId.header3Contents), addParagraph)


                //.WithInnerForeach<Content>(subjectTemplate.Contents.ToList(),
                //    (c, i, l) =>
                //    {
                //    l.Add(Tag.Create("div").WithInner(String.Format("{0}: ", i + 1) + c.Description));
                //    l.Add(Tag.Create("div")
                //            .WithInnerForeach<CommonText>(subjectTemplate.Contents.ToList()[i].Points.ToList(),
                //                (p, j, l) =>
                //                {
                //                    l.Add(Tag.Create("div").WithInner(String.Format("{0}.{1}: ", i + 1, j + 1) + p.Description));
                //                }
                //            )
                //        );
                //    }
                //)

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

                //.WithInner(Tag.Create("h2").WithInner("Bloques de enseñanza y aprendizaje").WithId("Apartado8-2"))
                //.WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2Blocks), addCommonTextTags)
                //.WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2Blocks), addCommonTextTags) 

                .WithHeader2(index[7].Subitems[1].Title)
                .Foreach<string>(GetGradeCommonText(CommonTextId.header2Blocks), addParagraph)
                .Foreach<string>(GetSubjectCommonText(CommonTextId.header2Blocks), addParagraph)

                //.WithInner(
                //    Table.Create()
                //        .WithRow()
                //            .WithCell(String.Format("{0}: {1}", subjectTemplate.SubjectCode, subjectTemplate.SubjectName), 1, 5).WithCellClass("tableHeader1")
                //            .WithCell(String.Format("Horas: {0}", subjectTemplate.GradeClassroomHours + subjectTemplate.GradeCompanyHours)).WithCellClass("tableHeader1")
                //        .WithRow()
                //            .WithCell("Bloque de enseñanza-aprendizaje", 2, 2).WithCellClass("tableHeader2")
                //            .WithCell("RA", 2, 1).WithCellClass("tableHeader2")
                //            .WithCell("Contenidos", 2, 1).WithCellClass("tableHeader2")
                //            .WithCell("Evaluación", 1, 2).WithCellClass("tableHeader2")
                //        .WithRow()
                //            .WithCell("CE").WithCellClass("tableHeader2")
                //            .WithCell("Actividades evaluables").WithCellClass("tableHeader2")

                //        .WithRowForeach<Block>(Subject.Blocks.ToList(),
                //            (b, i, t) =>
                //            {
                //                t.WithCell(String.Format("<b>Bloque {0}</b>", i + 1));
                //                t.WithCell(String.Format(CultureInfo.InvariantCulture, "{0} horas", Subject.QueryBlockDuration(i)));

                //                string raText = "";
                //                bool first = true;
                //                Subject.QueryBlockReferencedLearningResultIndexes(i).ForEach(
                //                    r => { raText += (first?"":", ") + "RA" + (r + 1); first = false;
                //                });

                //                string contentText = "";
                //                first = true;
                //                Subject.QueryBlockReferencedContentIndexes(i).ForEach(
                //                    c =>
                //                    {
                //                        contentText += (first ? "" : ", ") + (c + 1); first = false;
                //                    });

                //                string criteriaText = "";
                //                first = true;
                //                Subject.QueryBlockReferencedLearningResultCriteriaIndexes(i).ForEach(
                //                    cIndex =>
                //                    {
                //                        criteriaText += (first ? "" : ", ") + (cIndex.learningResultIndex + 1) + "." + (cIndex.criteriaIndex + 1);
                //                        first = false;
                //                    }
                //                );

                //                string evaluableActivitiesText = "";
                //                first = true;
                //                Subject.QueryBlockEvaluableActivityIndexes(i).ForEach(
                //                    (aIndex) =>
                //                    {
                //                        evaluableActivitiesText += (first ? "" : "<br>") + Utils.FormatEvaluableActivity(i, aIndex.evaluationType, aIndex.activityTypeIndex);
                //                        first = false;
                //                    }
                //                );

                //                t.WithCell(raText, 2, 1);
                //                t.WithCell(contentText, 2, 1);
                //                t.WithCell(criteriaText, 2, 1);
                //                t.WithCell(evaluableActivitiesText, 2, 1);
                //                t.WithRow().WithCell(b.Description, 1, 2);
                //            }
                //        )
                //)

                //.WithInner(Tag.Create("h2").WithInner("Programación de actividades de enseñanza-aprendizaje").WithId("Apartado8-3"))
                //.WithInnerForeach<string>(GetGradeCommonText(CommonTextId.header2Activities), addCommonTextTags)
                //.WithInnerForeach<string>(GetSubjectCommonText(CommonTextId.header2Activities), addCommonTextTags)
                
                .WithHeader2(index[7].Subitems[2].Title)
                .Foreach<string>(GetGradeCommonText(CommonTextId.header2Activities), addParagraph)
                .Foreach<string>(GetSubjectCommonText(CommonTextId.header2Activities), addParagraph)

                //.WithInnerForeach<Block>(Subject.Blocks.ToList(),
                //    (b, i, l) =>
                //    {
                //        l.Add(Tag.Create("h3").WithInner(String.Format("Bloque {0}", i + 1)).WithId(String.Format("Apartado8-3-{0}", i + 1)));

                //        List<Activity> activities = b.Activities.ToList();
                //        foreach(Activity a in activities)
                //        {
                //            l.Add(
                //                Table.Create()
                //                        .WithRow()
                //                            .WithCell(a.Title, 1, 4).WithCellClass("tableHeader1")
                //                        .WithRow()
                //                            .WithCell(a.Description, 1, 4)
                //                        .WithRow()
                //                            .WithCell("Metodología").WithCellClass("tableHeader2")
                //                            .WithCell("Duración").WithCellClass("tableHeader2")
                //                            .WithCell("Fecha de inicio").WithCellClass("tableHeader2")
                //                            .WithCell("Fecha de fin").WithCellClass("tableHeader2")
                //                        .WithRow()
                //                            .WithCell(a.Metodology.Title)
                //                            .WithCell(String.Format(CultureInfo.InvariantCulture, "{0:0}h ({1} sesiones)", a.Duration, GetSessionsCountText(a, schedule)))
                //                            .WithCell(Utils.FormatStartDayHour(schedule.Find(_a => _a.activity.StorageId == a.StorageId).start, Subject.WeekSchedule))
                //                            .WithCell(Utils.FormatEndDayHour(schedule.Find(_a => _a.activity.StorageId == a.StorageId).end, Subject.WeekSchedule))
                //                        .WithRow()
                //                            .WithCell("Espacios").WithCellClass("tableHeader2")
                //                            .WithCell("Materiales").WithCellClass("tableHeader2")
                //                            .WithCell("Contenidos").WithCellClass("tableHeader2")
                //                            .WithCell("Capacidades clave").WithCellClass("tableHeader2")
                //                            .WithRow()
                //                            .WithCell(GetSpacesText(a))
                //                            .WithCell(GetMaterialsText(a))
                //                            .WithCell(GetContentsText(i, a))
                //                            .WithCell(GetKeyCapacitiesText(a))
                //                            .WithRowIf(a.EvaluationType != ActivityEvaluationType.NotEvaluable)
                //                            .WithCell("Código de actividad evaluable").WithCellClass("tableHeader2")
                //                            .WithCell("Instrumento de evaluación").WithCellClass("tableHeader2")
                //                            .WithCell("Peso en los resultados de aprendizaje").WithCellClass("tableHeader2")
                //                            .WithCell("Criterios de evaluación").WithCellClass("tableHeader2")
                //                            .WithRowIf(a.EvaluationType != ActivityEvaluationType.NotEvaluable)
                //                            .WithCell(a.EvaluationType != ActivityEvaluationType.NotEvaluable ? Utils.FormatEvaluableActivity(i, a.EvaluationType, Subject.QueryEvaluableActivityTypeIndex(i, a)) : "")
                //                            .WithCell(a.EvaluationType != ActivityEvaluationType.NotEvaluable ? a.EvaluationInstrumentType.Title : "")
                //                            .WithCell(GetReferencedLearningResultsWeightsText(i, a))
                //                            .WithCell(GetReferencedCriteriasText(i, a))
                //            );
                //        }
                //    }
                //)
                //.WithInner(pageBreak)

                //////////////////////////////////////////////////////////////////////
                ////////////// Nivel 1: Referencias bibliográficas ///////////////////
                //////////////////////////////////////////////////////////////////////
                            
                //.WithInner(Tag.Create("h1").WithInner(index[8].Title).WithId("Apartado9"))
                //.WithInner(Tag.Create("div")
                //    .WithInnerForeach<CommonText>(Subject.Citations.ToList(),
                //        (c, i, l) =>
                //        {
                //            l.Add(Tag.Create("div").WithInner(String.Format("{0}- {1}", i + 1, c.Description)));
                //        }
                //    )
                //    )
                //.WithInner(pageBreak)

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
