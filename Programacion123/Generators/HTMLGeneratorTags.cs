using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    public partial class HTMLGenerator : Generator
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
    }

}
