using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Office.Interop.Word;

namespace Programacion123
{
    public struct HTMLGeneratorConfiguration
    {
        public string style;
    }

    public class HTMLGenerator : DocGenerator
    {
        struct InnerContent
        {
            internal Tag? tag;
            internal string?text;
        }

        class Tag
        {
            string tag;

            List<InnerContent>innerElements;
            
            List<Tuple<string, string>> parameters;

            internal static Tag Create(string _tag) { return new Tag(){ tag = _tag, parameters = new(), innerElements = new() }; }
            internal Tag WithInner(string text) { innerElements.Add(new() { text = text }); return this; }
            internal Tag WithInner(Tag tag) { innerElements.Add(new() { tag = tag}); return this; }
            internal Tag WithInnerList(List<Tag> tags) { tags.ForEach(t => innerElements.Add(new(){ tag = t }) ); return this; }
            internal Tag WithInnerForeach<TForeachElement>(List<TForeachElement> list, Action<TForeachElement, int, List<Tag> > action)
            {
                List<Tag> tagList = new();
                int i = 0;
                list.ForEach((e) => { action.Invoke(e, i, tagList); i++; } );
                
                return WithInnerList(tagList);
            }
            internal Tag WithParam(string param, string value) { parameters.Add(new(param, value)); return this; }
            internal Tag WithClass(string className) { parameters.Add(new("class", className)); return this; }
            internal Tag WithId(string id) { parameters.Add(new("id", id)); return this; }

            public override string ToString()
            {
                string parameterText = "";

                parameters.ForEach(p => parameterText += " " + p.Item1 + "=" + "'" + p.Item2 + "'");

                string open = "<" + tag + parameterText + ">";
                
                string inner = "";
                
                innerElements.ForEach( e => inner += (e.tag != null ? e.tag.ToString() : e.text != null ? e.text : "") );

                string close = innerElements.Count > 0 ? "</" + tag + ">" : "";

                return open + inner + close + "\n";
            }

        }

        class Table : Tag
        {
            List< List<Tag> > rows;

            internal static Table Create()
            {
                Table t = new Table(){ rows = new() };
                return t;
            }

            internal Table WithRowForeach<TElement>(List<TElement> input, Action<TElement, int, Table> action)
            {
                for(int i = 0; i < input.Count; i ++)
                {
                    rows.Add(new());
                    action.Invoke(input[i], i, this);
                }

                return this;
            }

            internal Table WithRow()
            {
                rows.Add(new());

                return this;
            }

            internal Table WithCell(Tag content, int rowspan = 1, int colspan = 1)
            {
                WithCell(new InnerContent() { tag = content }, rowspan, colspan);            

                return this;
            }

            internal Table WithCellClass(string className)
            {
                List<Tag> rowTags = rows[rows.Count - 1];
                rowTags[rowTags.Count - 1].WithClass(className);

                return this;
            }

            internal Table WithCell(string content, int rowspan = 1, int colspan = 1)
            {
                WithCell(new InnerContent() { text = content }, rowspan, colspan);
                
                return this;
            }

            Table WithCell(InnerContent content, int rowspan, int colspan)
            {
                List<Tag> row = rows[rows.Count - 1];
                Tag cell = Tag.Create("td");
                if(content.tag != null) { cell.WithInner(content.tag); }
                else if(content.text != null) { cell.WithInner(content.text); }
                if(rowspan > 1) { cell.WithParam("rowspan", rowspan.ToString()); }
                if(colspan > 1) { cell.WithParam("colspan", colspan.ToString()); }
                row.Add(cell);  

                return this;
            }

            public override string ToString()
            {
                List<Tag> rowTags = new();

                foreach(List<Tag> r in rows)
                {
                    Tag rowTag = Tag.Create("tr").WithInnerList(r);
                    rowTags.Add(rowTag);
                }
                
                return Tag.Create("table").WithInnerList(rowTags).ToString();

            }


        }

        public override void Generate(Subject _subject, string path)
        {
            FileStreamOptions options = new() { Access = FileAccess.Write, Mode = FileMode.Create };
            StreamWriter writer = new(path, Encoding.UTF8, options);

            Subject subject = _subject;
            SubjectTemplate? subjectTemplate = _subject.Template;
            GradeTemplate? gradeTemplate = subjectTemplate?.GradeTemplate;
            Func<GradeCommonTextId, string?> getGradeCommonText = (id) => { return gradeTemplate?.CommonTexts[id].Description; };

            string? gradeTypeName = (gradeTemplate.GradeType == GradeType.superior ?
                                    "Ciclo formativo de grado superior" : "Ciclo formativo de grado medio");

            List<ActivitySchedule>? schedule = null;
            
            if(subject.CanScheduleActivities()) { schedule = subject.ScheduleActivities(); }

            string css = ":root {  --paragraphSeparation:30pt; --backgroundColor1: #0070C0; --backgroundColor2: #92D050; --textColor1: #000000; --textColor2: #FFFFFF; --textColor3: #0070C0; --borderColor1: #000000}\n" +
                         "body { width: 21cm; padding: 0.5cm; text-align: justify; font-family:sans-serif; color: var(--textColor1) }\n" +
                         "table { width: 100%; border-style: solid; border-width:1pt; border-color:var(--borderColor1); border-spacing: 0pt; margin-bottom: var(--paragraphSeparation) }\n" +
                         "td { border-style: solid; border-width:1pt; border-color:var(--borderColor1); padding: 5pt }\n" +
                         ".tableHeader1 { background-color: var(--backgroundColor1); font-weight:bold; color: var(--textColor2) }\n" +
                         ".tableHeader2 { background-color: var(--backgroundColor2); color: var(--textColor1) }\n" +
                         "h1 { color: var(--textColor3) }";


            string html =
                
                Tag.Create("html").WithInner(
                        Tag.Create("header")
                            .WithInner(Tag.Create("title").WithInner("Programación didáctica del módulo " + subjectTemplate.SubjectName))
                            .WithInner(Tag.Create("style").WithInner(css)
                            )
                    )
                    .WithInner(
                        Tag.Create("body")
                            .WithInner(Tag.Create("h2").WithInner("Módulo profesional " + subjectTemplate.SubjectCode))
                            .WithInner(Tag.Create("h1").WithInner(subjectTemplate.SubjectName))
                            .WithInner(Tag.Create("h3").WithInner(gradeTypeName)
                            .WithInner(Tag.Create("h2").WithInner(gradeTemplate.GradeName))
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
                                              .WithRowForeach<Block>(subject.Blocks.ToList(),
                                                        (b, i, t) =>
                                                        {
                                                            List<int> referencedResults = subject.GetBlockReferencedLearningResultIndexes(i);
                                                            List<SubjectLearningResultCriteriaIndex> referencedCriterias = subject.GetBlockReferencedLearningResultCriteriaIndexes(i);

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
                            .WithInnerForeach<CommonText>(subject.Metodologies.ToList(),
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
                            .WithInnerForeach<CommonText>(subject.EvaluationInstrumentsTypes.ToList(),
                                (c, i, l) =>
                                {
                                    l.Add(Tag.Create("h3").WithInner(c.Title));
                                    l.Add(Tag.Create("div").WithInner(c.Description));
                                }
                            )
                            .WithInner(Tag.Create("h1").WithInner("Evaluación del funcionamiento de la programación"))
                            .WithInner(Tag.Create("h1").WithInner("Recursos didácticos y organizativos"))
                            .WithInner(Tag.Create("h2").WithInner("Espacios requeridos"))
                            .WithInnerForeach<CommonText>(subject.SpaceResources.ToList(),
                                (c, i, l) =>
                                {
                                    l.Add(Tag.Create("h3").WithInner(c.Title));
                                    l.Add(Tag.Create("div").WithInner(c.Description));
                                }
                             )
                            .WithInner(Tag.Create("h2").WithInner("Materiales y herramientas"))
                            .WithInnerForeach<CommonText>(subject.MaterialResources.ToList(),
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
                            .WithInnerForeach<Block>(subject.Blocks.ToList(),
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

            writer.Write(html);
            writer.Close();

        }

        public override void LoadConfiguration(string path)
        {
        }

        public override void SaveConfiguration(string path)
        {
        }

    }
}
