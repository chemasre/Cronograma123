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

            string html =
                
                Tag.Create("html").WithInner(
                    Tag.Create("header").WithInner(
                        Tag.Create("title").WithInner("Programación didáctica del módulo " + _subject.Template.SubjectName)
                        ).WithInner(
                            Tag.Create("style").WithInner("body { color = 'red'; }")
                        )
                    ).WithInner(
                        Tag.Create("body")
                            .WithInner(Tag.Create("h2").WithInner("Módulo profesional " + _subject.Template.SubjectCode))
                            .WithInner(Tag.Create("h1").WithInner(_subject.Template.SubjectName))
                            .WithInner(Tag.Create("h3").WithInner(_subject.Template.GradeTemplate.GradeType == GradeType.superior ?
                                                                  "Ciclo formativo de grado superior" : "Ciclo formativo de grado medio"))
                            .WithInner(Tag.Create("h2").WithInner(_subject.Template.GradeTemplate.GradeName))
                            .WithInner(Tag.Create("h1").WithInner("Organización del módulo"))
                            .WithInner(Tag.Create("h1").WithInner("Justificación de la importancia del módulo"))
                            .WithInner(Tag.Create("h1").WithInner("Elementos curriculares"))
                            .WithInner(Tag.Create("h2").WithInner("Objetivos generales relacionados con el módulo"))
                            .WithInner(Tag.Create("h2").WithInner("Competencias profesionales, personales y sociales"))
                            .WithInner(Tag.Create("h2").WithInner("Capacidades clave"))
                            .WithInnerForeach<CommonText>(_subject.Template.KeyCapacities.ToList(),
                                (c, i, l) =>
                                {
                                    l.Add(Tag.Create("h3").WithInner(c.Title));
                                    l.Add(Tag.Create("div").WithInner(c.Description));
                                }
                             )                                
                            .WithInner(Tag.Create("h1").WithInner("Metodología. Orientaciones didácticas"))
                            .WithInner(Tag.Create("h2").WithInner("Metodología general y específica de la materia"))
                            .WithInnerForeach<CommonText>(_subject.Metodologies.ToList(),
                                (c, i, l) =>
                                {
                                    l.Add(Tag.Create("h3").WithInner(c.Title));
                                    l.Add(Tag.Create("div").WithInner(c.Description));
                                }
                            )
                            .WithInner(Tag.Create("h2").WithInner("Medidas de atención al alumnado con necesidad específica de apoyo educativo o con necesidad de compensación educativa: atención a la diversidad"))
                            .WithInner(Tag.Create("h3").WithInner("Medidas generales del centro"))
                            .WithInner(Tag.Create("h1").WithInner("Sistema de evaluación"))
                            .WithInner(Tag.Create("h1").WithInner("Instrumentos de evaluación"))
                            .WithInnerForeach<CommonText>(_subject.EvaluationInstrumentsTypes.ToList(),
                                (c, i, l) =>
                                {
                                    l.Add(Tag.Create("h3").WithInner(c.Title));
                                    l.Add(Tag.Create("div").WithInner(c.Description));
                                }
                            )
                            .WithInner(Tag.Create("h1").WithInner("Evaluación del funcionamiento de la programación"))
                            .WithInner(Tag.Create("h1").WithInner("Recursos didácticos y organizativos"))
                            .WithInner(Tag.Create("h2").WithInner("Espacios requeridos"))
                            .WithInnerForeach<CommonText>(_subject.SpaceResources.ToList(),
                                (c, i, l) =>
                                {
                                    l.Add(Tag.Create("h3").WithInner(c.Title));
                                    l.Add(Tag.Create("div").WithInner(c.Description));
                                }
                             )
                            .WithInner(Tag.Create("h2").WithInner("Materiales y herramientas"))
                            .WithInnerForeach<CommonText>(_subject.MaterialResources.ToList(),
                                (c, i, l) =>
                                {
                                    l.Add(Tag.Create("h3").WithInner(c.Title));
                                    l.Add(Tag.Create("div").WithInner(c.Description));
                                }
                             )
                            .WithInner(Tag.Create("h1").WithInner("Programación del módulo profesional"))
                            .WithInner(Tag.Create("h2").WithInner("Resultados de aprendizaje, criterios de evaluación y contenidos"))
                            .WithInner(Tag.Create("h3").WithInner("Resultados de aprendizaje y criterios de evaluación"))
                            .WithInnerForeach<LearningResult>(_subject.Template.LearningResults.ToList(),
                                (r, i, l) =>
                                {
                                    l.Add(Tag.Create("h4").WithInner(String.Format("RA{0}: ", i + 1) + r.Description)
                                        .WithInnerForeach<CommonText>(_subject.Template.LearningResults.ToList()[i].Criterias.ToList(),
                                            (c, j, l) =>
                                            {
                                                l.Add(Tag.Create("h5").WithInner(String.Format("{0}.{1}: ", i + 1, j + 1) + c.Description));
                                            }
                                        )
                                    );
                                }
                             )
                            .WithInner(Tag.Create("h3").WithInner("Contenidos"))
                            .WithInnerForeach<Content>(_subject.Template.Contents.ToList(),
                                (c, i, l) =>
                                {
                                    l.Add(Tag.Create("h4").WithInner(String.Format("{0}: ", i + 1) + c.Description)
                                        .WithInnerForeach<CommonText>(_subject.Template.Contents.ToList()[i].Points.ToList(),
                                            (p, j, l) =>
                                            {
                                                l.Add(Tag.Create("h5").WithInner(String.Format("{0}.{1}: ", i + 1, j + 1) + p.Description));
                                            }
                                        )
                                    );
                                }
                            )
                            .WithInner(Tag.Create("h3").WithInner("Bloques de enseñanza y aprendizaje"))
                            .WithInnerForeach<Block>(_subject.Blocks.ToList(),
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
