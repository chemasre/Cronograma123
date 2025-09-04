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

            List<InnerContent> innerElements;
            
            List<Tuple<string, string>> parameters;

            bool ifCondition;

            internal static Tag Create(string _tag) { return new Tag(){ tag = _tag, parameters = new(), innerElements = new(), ifCondition = true  }; }
            internal Tag WithInner(string text) { ifCondition = true; innerElements.Add(new() { text = text }); return this; }
            internal Tag WithInner(Tag tag) { ifCondition = true; innerElements.Add(new() { tag = tag}); return this; }
            internal Tag WithInnerList(List<Tag> tags) { ifCondition = true; tags.ForEach(t => innerElements.Add(new(){ tag = t }) ); return this; }
            internal Tag WithInnerIf(bool condition, string text) { ifCondition = condition; if(ifCondition) { WithInner(text); } return this; }
            internal Tag WithInnerIf(bool condition, Tag tag) { ifCondition = condition; if(ifCondition) { WithInner(tag); } return this; }
            internal Tag WithInnerForeach<TForeachElement>(List<TForeachElement> list, Action<TForeachElement, int, List<Tag> > action)
            {
                List<Tag> tagList = new();
                int i = 0;
                list.ForEach((e) => { action.Invoke(e, i, tagList); i++; } );
                
                return WithInnerList(tagList);
            }
            internal Tag WithParam(string param, string value)
            {
                int paramIndex = parameters.FindIndex(p => p.Item1 == param);
                if(paramIndex >= 0) { parameters[paramIndex] = new(param, value); }
                else { parameters.Add(new(param, value)); }

                return this;
            }
            internal Tag WithClass(string className)
            {
                parameters.Add(new("class", className)); return this;
            }
            internal Tag WithId(string id)
            {
                parameters.Add(new("id", id)); return this;
            }

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

            bool rowCondition;

            internal static Table Create()
            {
                Table t = new Table(){ rows = new(), rowCondition = true };

                return t;
            }

            internal Table WithRowIf(bool condition)
            {
                rowCondition = condition;
                
                if(condition) { rows.Add(new()); }

                return this;
            }

            internal Table WithRowForeach<TElement>(List<TElement> input, Action<TElement, int, Table> action)
            {
                rowCondition = true;

                for(int i = 0; i < input.Count; i ++)
                {
                    rows.Add(new());
                    action.Invoke(input[i], i, this);
                }

                return this;
            }

            internal Table WithCellForeach<TElement>(List<TElement> input, Action<TElement, int, Table> action)
            {
                if(!rowCondition) { return this; }

                for(int i = 0; i < input.Count; i ++)
                {
                    rows[rows.Count - 1].Add(Tag.Create("td"));
                    action.Invoke(input[i], i, this);
                }

                return this;
            }

            internal Table WithRow()
            {
                rowCondition = true;

                rows.Add(new());

                return this;
            }

            internal Table WithCell(Tag content, int rowspan = 1, int colspan = 1)
            {
                if(!rowCondition) { return this; }

                WithCell(new InnerContent() { tag = content }, rowspan, colspan);            

                return this;
            }

            internal Table WithCell()
            {
                if(!rowCondition) { return this; }

                rows[rows.Count - 1].Add(Tag.Create("td"));

                return this;
            }

            internal Table WithCellInner(Tag content)
            {
                if(!rowCondition) { return this; }

                List<Tag> rowTags = rows[rows.Count - 1];
                rowTags[rowTags.Count - 1].WithInner(content);

                return this;
            }

            internal Table WithCellInner(string content)
            {
                if(!rowCondition) { return this; }

                List<Tag> rowTags = rows[rows.Count - 1];
                rowTags[rowTags.Count - 1].WithInner(content);

                return this;
            }

            internal Table WithCellSpan(int rowspan, int colspan)
            {
                if(!rowCondition) { return this; }

                List<Tag> rowTags = rows[rows.Count - 1];
                rowTags[rowTags.Count - 1].WithParam("rowspan", rowspan.ToString());
                rowTags[rowTags.Count - 1].WithParam("colspan", colspan.ToString());

                return this;
            }

            internal Table WithCellClass(string className)
            {
                if(!rowCondition) { return this; }

                List<Tag> rowTags = rows[rows.Count - 1];
                rowTags[rowTags.Count - 1].WithClass(className);

                return this;
            }

            internal Table WithCell(string content, int rowspan = 1, int colspan = 1)
            {
                if(!rowCondition) { return this; }

                WithCell(new InnerContent() { text = content }, rowspan, colspan);
                
                return this;
            }

            Table WithCell(InnerContent content, int rowspan, int colspan)
            {
                if(!rowCondition) { return this; }

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
