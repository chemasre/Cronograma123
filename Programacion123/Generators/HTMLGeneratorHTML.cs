using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        CoverGradeName,
        IndexLevel1,
        IndexLevel2,
        IndexLevel3
    }

    public enum DocumentTableElementId
    {
        TableNormalCell,
        TableHeader1Cell,
        TableHeader2Cell
    
    }

    struct IndexItem
    {
        public string Title;
        public List<IndexItem> Subitems;
    }

    public partial class HTMLGenerator : Generator
    {
        public string GenerateHTML()
        {
            SubjectTemplate? subjectTemplate = Subject.Template;
            GradeTemplate? gradeTemplate = subjectTemplate?.GradeTemplate;
            Func<GradeCommonTextId, string?> getGradeCommonText = (id) => { return gradeTemplate?.CommonTexts[id].Description; };
            Func<SubjectCommonTextId, string?> getSubjectCommonText = (id) => { return Subject.CommonTexts[id].Description; };

            string? gradeTypeName = (gradeTemplate.GradeType == GradeType.superior ?
                                    "Ciclo formativo de grado superior" : "Ciclo formativo de grado medio");

            List<ActivitySchedule>? schedule = null;
            
            if(Subject.CanScheduleActivities()) { schedule = Subject.ScheduleActivities(); }

            Func<Activity, string> getSpacesText =
                (Activity a) =>
                {
                    string spacesText = "";
                    bool first = true;
                    foreach(CommonText s in a.SpaceResources.ToList()) { spacesText += (first?"":"<br>") + s.Title; }
                    return spacesText;
                };

            Func<Activity, string> getMaterialsText =
                (Activity a) =>
                {
                    string materialsText = "";
                    bool first = true;
                    foreach(CommonText s in a.MaterialResources.ToList()) { materialsText += (first?"":"<br>") + s.Title; }
                    return materialsText.Length > 0 ? materialsText : "-";
                };

            List<IndexItem> indexMetodologies = new();
            Subject.Metodologies.ToList().ForEach(m => indexMetodologies.Add(new() { Title = m.Title, Subitems = new() }));

            List<IndexItem> indexInstrumentTypes= new();
            Subject.EvaluationInstrumentsTypes.ToList().ForEach(instrument => indexInstrumentTypes.Add(new() { Title = instrument.Title, Subitems = new() }));

            List<IndexItem> indexMaterialResources = new();
            Subject.MaterialResources.ToList().ForEach(resource => indexMaterialResources.Add(new() { Title = resource.Title, Subitems = new() }));

            List<IndexItem> indexSpaceResources = new();
            Subject.SpaceResources.ToList().ForEach(resource => indexSpaceResources.Add(new() { Title = resource.Title, Subitems = new() }));

            List<IndexItem> indexBlocks = new();
            int blockIndex = 0;
            Subject.Blocks.ToList().ForEach(
                b =>
                {
                    indexBlocks.Add(new() { Title = String.Format("Bloque {0}", blockIndex + 1), Subitems = new() });
                    blockIndex++;
                });

            List<IndexItem> indexItems = new()
            {
                new IndexItem(){ Title = "Organización del módulo", Subitems = new() {} },
                new IndexItem(){ Title = "Justificación de la importancia del módulo", Subitems = new () {} },
                new IndexItem(){ Title = "Elementos curriculares", Subitems = new ()
                    {
                        new IndexItem() { Title = "Objetivos generales relacionados con el módulo", Subitems = new (){ } },
                        new IndexItem() { Title = "Competencias profesionales, personales y sociales", Subitems = new (){ } },
                        new IndexItem() { Title = "Capacidades clave", Subitems = new() { } }
                    }
                },
                new IndexItem(){ Title = "Metodología. Orientaciones didácticas", Subitems = new ()
                    {
                        new IndexItem() { Title = "Metodología general y específica de la materia", Subitems = indexMetodologies },
                        new IndexItem() { Title = "Medidas de atención al alumnado con necesidad específica de apoyo educativo" +
                                                  " o con necesidad de compensación educativa: atención a la diversidad", Subitems = new ()
                            {
                                new IndexItem() { Title = "Medidas generales del centro", Subitems = new (){ } }
                            }
                        }
                    }
                },
                new IndexItem(){ Title = "Sistema de evaluación", Subitems = new ()
                    {
                        new IndexItem() { Title = "Instrumentos de evaluación", Subitems = indexInstrumentTypes },
                        new IndexItem() { Title = "Evaluación del funcionamiento de la programación", Subitems = new (){ } }
                    }
                },
                new IndexItem(){ Title = "Elementos transversales", Subitems = new ()
                    {
                        new IndexItem() { Title = "Fomento de la lectura y tecnologías de la información y de comunicación", Subitems = new (){ } },
                        new IndexItem() { Title = "Comunicación audiovisual, emprendimiento, educación cívica y constitucional", Subitems = new (){ } }
                    }
                },

                new IndexItem(){ Title = "Recursos didácticos y organizativos", Subitems = new ()
                    {
                        new IndexItem() { Title = "Espacios requeridos", Subitems = indexSpaceResources },
                        new IndexItem() { Title = "Materiales y herramientas", Subitems = indexMaterialResources }
                    }
                },
                new IndexItem(){ Title = "Programación del módulo profesional", Subitems = new ()
                    {
                        new IndexItem() { Title = "Resultados de aprendizaje, criterios de evaluación y contenidos", Subitems = new ()
                            {
                                new IndexItem() { Title = "Resultados de aprendizaje y criterios de evaluación", Subitems = new (){ } },
                                new IndexItem() { Title = "Contenidos", Subitems = new (){ } }
                            }
                        },
                        new IndexItem() { Title = "Bloques de enseñanza y aprendizaje", Subitems = new (){ } },
                        new IndexItem() { Title = "Programación de actividades de enseñanza-aprendizaje", Subitems = indexBlocks },
                    }

                },
                new IndexItem(){ Title = "Referencias bibliográficas del módulo", Subitems = new () { } },
                new IndexItem(){ Title = "Anexos", Subitems = new () { } }
            };

            // https://awkwardcoder.blogspot.com/2011/08/manipulating-web-browser-scroll.html

            string javascript = 
            @"function getVerticalScrollPosition() {
            return document.documentElement.scrollTop.toString();
            }
            function setVerticalScrollPosition(position) {
            document.documentElement.scrollTop = position;
            }";


            string html =
                "<!DOCTYPE html>" +
                Tag.Create("html").WithParam("lang", "es")
                    .WithInner(
                        Tag.Create("head")
                            .WithInner(Tag.Create("meta").WithParam("charset", "UTF-8"))
                            .WithInner(Tag.Create("title").WithInner("Programación didáctica del módulo " + subjectTemplate.SubjectName))
                            .WithInner(Tag.Create("style").WithInner(GenerateCSS()))
                            .WithInner(Tag.Create("script").WithInner(javascript))
                    )
                    .WithInner(
                        Tag.Create("body")
                            .WithInner(Tag.Create("div").WithClass("cover")
                               .WithInner(Tag.Create("img").WithClass("coverLogo").WithParam("src", "data:image/png;base64," + DocumentStyle.LogoBase64))
                               .WithInner(Tag.Create("div").WithClass("coverSubjectCode").WithInner("Módulo profesional " + subjectTemplate.SubjectCode))
                               .WithInner(Tag.Create("div").WithClass("coverSubjectName").WithInner(subjectTemplate.SubjectName))
                               .WithInner(Tag.Create("div").WithClass("coverGradeTypeName").WithInner(gradeTypeName))
                               .WithInner(Tag.Create("div").WithClass("coverGradeName").WithInner(gradeTemplate.GradeName))
                             )

                            //////////////////////////////////////////////////////////////////
                            ///////////////////// ÍNDICE DE CONTENIDOS  //////////////////////
                            //////////////////////////////////////////////////////////////////

                            .WithInner(Tag.Create("h1").WithInner("Contenidos").WithId("Indice").WithClass("index"))
                            .WithInnerForeach<IndexItem>(indexItems,
                                (item, i, l) =>
                                {
                                    l.Add(Tag.Create("div").WithInner(Tag.Create("a").WithParam("href", String.Format("#Apartado{0}", i + 1))
                                                            .WithInner(item.Title)).WithClass("indexLevel1"));

                                    int subitemIndex = 0;
                                    foreach(IndexItem subitem in item.Subitems)
                                    {
                                        l.Add(Tag.Create("div").WithInner(Tag.Create("a").WithParam("href", String.Format("#Apartado{0}-{1}", i + 1, subitemIndex + 1))
                                                                .WithInner(subitem.Title)).WithClass("indexLevel2"));
                                        int subSubitemIndex = 0;
                                        foreach(IndexItem subsubitem in subitem.Subitems)
                                        {
                                            l.Add(Tag.Create("div").WithInner(Tag.Create("a").WithParam("href", String.Format("#Apartado{0}-{1}-{2}", i + 1, subitemIndex + 1, subSubitemIndex + 1))
                                                                    .WithInner(subsubitem.Title)).WithClass("indexLevel3"));
                                            subSubitemIndex++;
                                        }

                                        subitemIndex++;
                                    }
                                }
                            )

                            //////////////////////////////////////////////////////////////////
                            ///////////// Nivel 1: Organización del módulo ///////////////////
                            //////////////////////////////////////////////////////////////////

                            .WithInner(Tag.Create("h1").WithInner(indexItems[0].Title).WithId("Apartado1"))
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
                                                            List<int> referencedResults = Subject.GetBlockReferencedLearningResultIndexes(i);
                                                            List<SubjectLearningResultCriteriaIndex> referencedCriterias = Subject.GetBlockReferencedLearningResultCriteriaIndexes(i);

                                                            string rasText = "";
                                                            bool first = true;
                                                            referencedResults.ForEach((i) => { rasText += (first?"":", ") + String.Format("RA{0}", i + 1); first = false; });

                                                            string criteriasText = "";
                                                            first = true;
                                                            referencedCriterias.ForEach((c) => { criteriasText += (first ? "" : ", ") + String.Format("{0}.{1}", c.learningResultIndex + 1, c.criteriaIndex + 1); first = false; });

                                                            float hours = Subject.GetBlockDuration(i);

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

                            /////////////////////////////////////////////////////////////////////////////////////
                            ///////////// Nivel 1: Justificación de la importancia del módulo ///////////////////
                            /////////////////////////////////////////////////////////////////////////////////////

                            .WithInner(Tag.Create("h1").WithInner(indexItems[1].Title).WithId("Apartado2"))
                            .WithInner(Tag.Create("div").WithInner(getSubjectCommonText.Invoke(SubjectCommonTextId.subjectImportanceJustification)))

                            /////////////////////////////////////////////////////////////////
                            ///////////// Nivel 1: Elementos curriculares ///////////////////
                            /////////////////////////////////////////////////////////////////

                            .WithInner(Tag.Create("h1").WithInner(indexItems[2].Title).WithId("Apartado3"))
                            .WithInner(Tag.Create("h2").WithInner(indexItems[2].Subitems[0].Title).WithId("Apartado3-1"))
                            .WithInner(Tag.Create("div")
                                .WithInnerForeach<CommonText>(subjectTemplate.GeneralObjectives.ToList(),
                                    (o, i, l) =>
                                    {
                                        int index = subjectTemplate.GradeTemplate.GeneralObjectives.ToList().FindIndex(_o => _o.StorageId == o.StorageId);
                                        l.Add(Tag.Create("div").WithInner(String.Format("{0}. {1}", Utils.FormatLetterPrefixLowercase(index), o.Description)));
                                    }
                                )
                             )
                            .WithInner(Tag.Create("h2").WithInner(indexItems[2].Subitems[1].Title).WithId("Apartado3-2"))
                            .WithInner(Tag.Create("div")
                                .WithInnerForeach<CommonText>(subjectTemplate.GeneralCompetences.ToList(),
                                    (c, i, l) =>
                                    {
                                        int index = subjectTemplate.GradeTemplate.GeneralCompetences.ToList().FindIndex(_c => _c.StorageId == c.StorageId);
                                        l.Add(Tag.Create("div").WithInner(String.Format("{0}. {1}", Utils.FormatLetterPrefixLowercase(index), c.Description)));
                                    }
                                )
                             )
                            .WithInner(Tag.Create("h2").WithInner(indexItems[2].Subitems[2].Title).WithId("Apartado3-3"))
                            .WithInnerForeach<int>(Subject.GetReferencedKeyCompetencesIndexes(),
                                (c, i, l) =>
                                {
                                    l.Add(Tag.Create("h3").WithInner(gradeTemplate.KeyCapacities[c].Title));
                                    l.Add(Tag.Create("div").WithInner(gradeTemplate.KeyCapacities[c].Description));
                                }
                             )

                            /////////////////////////////////////////////////////////////////////////////////
                            ///////////// Nivel 1: Metodología y orientaciones didácticas ///////////////////
                            /////////////////////////////////////////////////////////////////////////////////

                            .WithInner(Tag.Create("h1").WithInner(indexItems[3].Title).WithId("Apartado4"))
                            .WithInner(Tag.Create("div").WithInner(getGradeCommonText.Invoke(GradeCommonTextId.introductionToMetodologies)))
                            .WithInner(Tag.Create("h2").WithInner("Metodología general y específica de la materia").WithId("Apartado4-1"))
                            .WithInner(Tag.Create("div").WithInner(getGradeCommonText.Invoke(GradeCommonTextId.schoolPolicyMetodology)))
                            .WithInnerForeach<CommonText>(Subject.Metodologies.ToList(),
                                (c, i, l) =>
                                {
                                    l.Add(Tag.Create("h3").WithInner(c.Title).WithId(String.Format("Apartado4-1-{0}", i + 1)));
                                    l.Add(Tag.Create("div").WithInner(c.Description));
                                }
                            )
                            .WithInner(Tag.Create("h2").WithInner("Medidas de atención al alumnado con necesidad específica de apoyo educativo o con necesidad de compensación educativa: atención a la diversidad").WithId("Apartado4-2"))
                            .WithInner(Tag.Create("div").WithInner(getGradeCommonText.Invoke(GradeCommonTextId.introductionToDiversity)))
                            .WithInner(Tag.Create("h3").WithInner("Medidas generales del centro").WithId("Apartado4-2-1"))
                            .WithInner(Tag.Create("div").WithInner(getGradeCommonText.Invoke(GradeCommonTextId.schoolPolicyDiversity)))

                            ////////////////////////////////////////////////////////////////
                            ///////////// Nivel 1: Sistema de evaluación ///////////////////
                            ////////////////////////////////////////////////////////////////

                            .WithInner(Tag.Create("h1").WithInner(indexItems[4].Title).WithId("Apartado5"))
                            .WithInner(Tag.Create("h2").WithInner("Instrumentos de evaluación").WithId("Apartado5-1"))
                            .WithInnerForeach<CommonText>(Subject.EvaluationInstrumentsTypes.ToList(),
                                (c, i, l) =>
                                {
                                    l.Add(Tag.Create("h3").WithInner(c.Title).WithId(String.Format("Apartado5-1-{0}", i + 1)));
                                    l.Add(Tag.Create("div").WithInner(c.Description));
                                }
                            )
                            .WithInner(Tag.Create("h2").WithInner("Evaluación del funcionamiento de la programación").WithId("Apartado5-2"))

                            ///////////////////////////////////////////////////////////////////
                            ////////////// Nivel 1: Elementos transversales ///////////////////
                            ///////////////////////////////////////////////////////////////////

                            .WithInner(Tag.Create("h1").WithInner(indexItems[5].Title).WithId("Apartado6"))
                            .WithInner(Tag.Create("h2").WithInner("Fomento de la lectura y tecnologías de la información y de comunicación").WithId("Apartado6-1"))
                            .WithInner(Tag.Create("div").WithInner(getSubjectCommonText.Invoke(SubjectCommonTextId.subjectTraversalReadingAndTIC)))
                            .WithInner(Tag.Create("h2").WithInner("Comunicación audiovisual, emprendimiento, educación cívica y constitucional").WithId("Apartado6-2"))
                            .WithInner(Tag.Create("div").WithInner(getSubjectCommonText.Invoke(SubjectCommonTextId.subjectTraversalCommunicationEntrepreneurshipAndEducation)))



                            //////////////////////////////////////////////////////////////////////////////
                            ////////////// Nivel 1: Recursos didácticos y organizativos //////////////////
                            //////////////////////////////////////////////////////////////////////////////

                            .WithInner(Tag.Create("h1").WithInner(indexItems[6].Title).WithId("Apartado7"))
                            
                            .WithInner(Tag.Create("h2").WithInner("Espacios requeridos").WithId("Apartado7-1"))
                            .WithInnerForeach<CommonText>(Subject.SpaceResources.ToList(),
                                (c, i, l) =>
                                {
                                    l.Add(Tag.Create("h3").WithInner(c.Title).WithId(String.Format("Apartado7-1-{0}", i + 1)));
                                    l.Add(Tag.Create("div").WithInner(c.Description));
                                }
                             )

                            .WithInner(Tag.Create("h2").WithInner("Materiales y herramientas").WithId("Apartado7-2"))
                            .WithInnerForeach<CommonText>(Subject.MaterialResources.ToList(),
                                (c, i, l) =>
                                {
                                    l.Add(Tag.Create("h3").WithInner(c.Title).WithId(String.Format("Apartado7-2-{0}", i + 1)));
                                    l.Add(Tag.Create("div").WithInner(c.Description));
                                }
                             )

                            ///////////////////////////////////////////////////////////////////////////////
                            ////////////// Nivel 1: Programación del módulo profesional ///////////////////
                            ///////////////////////////////////////////////////////////////////////////////
                            
                            .WithInner(Tag.Create("h1").WithInner(indexItems[7].Title).WithId("Apartado8"))
                            .WithInner(Tag.Create("h2").WithInner("Resultados de aprendizaje, criterios de evaluación y contenidos").WithId("Apartado8-1"))
                            .WithInner(Tag.Create("h3").WithInner("Resultados de aprendizaje y criterios de evaluación").WithId("Apartado8-1-1"))
                            .WithInner(Tag.Create("div").WithInner(getGradeCommonText.Invoke(GradeCommonTextId.introductionToLearningResults)))
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
                            .WithInner(Tag.Create("div").WithInner(getGradeCommonText.Invoke(GradeCommonTextId.introductionToContents)))
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
                                            t.WithCell(String.Format(CultureInfo.InvariantCulture, "{0} horas", Subject.GetBlockDuration(i)));

                                            string raText = "";
                                            bool first = true;
                                            Subject.GetBlockReferencedLearningResultIndexes(i).ForEach(
                                                r => { raText += (first?"":", ") + "RA" + (r + 1); first = false;
                                            });

                                            string contentText = "";
                                            first = true;
                                            Subject.GetBlockReferencedContentIndexes(i).ForEach(
                                                c =>
                                                {
                                                    contentText += (first ? "" : ", ") + (c + 1); first = false;
                                                });

                                            string criteriaText = "";
                                            first = true;
                                            Subject.GetBlockReferencedLearningResultCriteriaIndexes(i).ForEach(
                                                cIndex =>
                                                {
                                                    criteriaText += (first ? "" : ", ") + (cIndex.learningResultIndex + 1) + "." + (cIndex.criteriaIndex + 1);
                                                    first = false;
                                                }
                                            );

                                            string evaluableActivitiesText = "";
                                            first = true;
                                            Subject.GetBlockEvaluableActivityIndexes(i).ForEach(
                                                (aIndex) =>
                                                {
                                                    evaluableActivitiesText += (first ? "" : "<br>") + Utils.FormatEvaluableActivity(i, aIndex);
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
                                                        .WithCell(a.Title, 1, 7).WithCellClass("tableHeader1")
                                                    .WithRow()
                                                        .WithCell(a.Description, 1, 7)
                                                    .WithRow()
                                                        .WithCell("Metodología").WithCellClass("tableHeader2")
                                                        .WithCell("Espacios").WithCellClass("tableHeader2")
                                                        .WithCell("Materiales").WithCellClass("tableHeader2")
                                                        .WithCell("Duración").WithCellClass("tableHeader2")
                                                        .WithCell("Fecha de inicio").WithCellClass("tableHeader2")
                                                        .WithCell("Fecha de fin").WithCellClass("tableHeader2")
                                                        .WithCell("Sesiones").WithCellClass("tableHeader2")
                                                    .WithRow()
                                                        .WithCell(a.Metodology.Title)
                                                        .WithCell(getSpacesText.Invoke(a))
                                                        .WithCell(getMaterialsText.Invoke(a))
                                                        .WithCell(String.Format(CultureInfo.InvariantCulture, "{0:0}h", a.Duration))
                                                        .WithCell(Utils.FormatStartDayHour(schedule.Find(_a => _a.activity.StorageId == a.StorageId).start, Subject.WeekSchedule))
                                                        .WithCell(Utils.FormatEndDayHour(schedule.Find(_a => _a.activity.StorageId == a.StorageId).end, Subject.WeekSchedule))
                                                        .WithCell(Subject.CountActivitySessions(schedule.Find(_a => _a.activity.StorageId == a.StorageId)).ToString())
                                        );
                                    }
                                }
                            )

                            //////////////////////////////////////////////////////////////////////
                            ////////////// Nivel 1: Referencias bibliográficas ///////////////////
                            //////////////////////////////////////////////////////////////////////
                            
                            .WithInner(Tag.Create("h1").WithInner(indexItems[8].Title).WithId("Apartado9"))
                            .WithInner(Tag.Create("div")
                                .WithInnerForeach<CommonText>(Subject.Citations.ToList(),
                                    (c, i, l) =>
                                    {
                                        l.Add(Tag.Create("div").WithInner(String.Format("{0}- {1}", i + 1, c.Description)));
                                    }
                                )
                             )

                            //////////////////////////////////////////////////
                            ////////////// Nivel 1: Anexos ///////////////////
                            //////////////////////////////////////////////////
                            
                            .WithInner(Tag.Create("h1").WithInner(indexItems[9].Title).WithId("Apartado10"))
                )
                .ToString();

            return html;
        }

    }
}
