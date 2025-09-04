using System.Diagnostics;

namespace Programacion123
{
    public struct GeneratorValidationResult
    {
        public GeneratorValidationCode code;
        public int index;

        public static GeneratorValidationResult Create(GeneratorValidationCode _code) { return new GeneratorValidationResult() { code = _code }; }
        public GeneratorValidationResult WithIndex(int _index) { index = _index; return this; }

        public override string ToString()
        {
            if (code == GeneratorValidationCode.success) { return "No se detectan problemas."; }
            else if(code == GeneratorValidationCode.subjectIsNull) { return "No se ha seleccionado una programación de módulo"; }
            else // code == GeneratorValidationCode.subjectNotValid)
            { return String.Format("La programación del módulo presenta algún problema."); }

        }
    };

    public enum GeneratorValidationCode
    {
        success,
        subjectIsNull,
        subjectNotValid
    };

    public struct DocumentIndexItem
    {
        public string Title;
        public List<DocumentIndexItem> Subitems;
    }


    public abstract class Generator
    {
        public Subject? Subject { get; set; }
        public DocumentStyle? Style { get; set; }

        public abstract void Generate(string path);
        public abstract void SaveSettings();
        public abstract void LoadOrCreateSettings();
        public abstract GeneratorValidationResult Validate();

        public List<string> GetGradeCommonText(CommonTextId id)
        {
            Debug.Assert(Subject != null);
            Debug.Assert(Subject.Template != null);
            Debug.Assert(Subject.Template.GradeTemplate != null);

            return Subject.Template.GradeTemplate.CommonTexts[id].Description.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList<string>();
        }

        public string GetGradeTypeName()
        {
            Debug.Assert(Subject != null);
            Debug.Assert(Subject.Template != null);
            Debug.Assert(Subject.Template.GradeTemplate != null);

            return (Subject.Template.GradeTemplate.GradeType == GradeType.superior ?
                                    "Ciclo formativo de grado superior" : "Ciclo formativo de grado medio");

        }

        public List<string> GetSubjectCommonText(CommonTextId id)
        {
            Debug.Assert(Subject != null);
            Debug.Assert(Subject.Template != null);
            Debug.Assert(Subject.Template.GradeTemplate != null);

            return Subject.CommonTexts[id].Description.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList<string>();
        }

        public string GetSpacesText(Activity a)
        {
            string spacesText = "";
            bool first = true;
            foreach(CommonText s in a.SpaceResources.ToList()) { spacesText += (first?"":"<br>") + s.Title; }
            return spacesText;
        }

        public string GetMaterialsText(Activity a)
        {
            string materialsText = "";
            bool first = true;
            foreach(CommonText s in a.MaterialResources.ToList()) { materialsText += (first?"":"<br>") + s.Title; }
            return materialsText.Length > 0 ? materialsText : "-";
        }

        public string GetSessionsCountText(Activity a, List<ActivitySchedule> schedule)
        {
            return Subject.QueryActivitySessionsCount(schedule.Find(_a => _a.activity.StorageId == a.StorageId)).ToString();
        }

        public string GetContentsText(int blockIndex, Activity a)
        {
            int activityIndex = Subject.QueryActivityIndex(blockIndex, a);
            List<ContentPointIndex> contentPoints = Subject.QueryActivityContentPointsIndexes(blockIndex, activityIndex);

            string contentsText = "";
            bool first = true;
            foreach(ContentPointIndex c in contentPoints.ToList()) { contentsText += (first?"":", ") + Utils.FormatContentPoint(c.contentIndex, c.pointIndex); first = false; }

            return contentsText;
        }

        public string GetKeyCapacitiesText(Activity a)
        {
            string capacitiesText = "";
            bool first = true;
            foreach(CommonText capacity in a.KeyCompetences.ToList()) { capacitiesText += (first?"":"<br>") + capacity.Title; first = false; }

            return capacitiesText.Length > 0 ? capacitiesText : "-";
        }

        public string GetReferencedLearningResultsWeightsText(int blockIndex, Activity a)
        {
            string resultsWeightsText = "";
            bool first = true;

            int activityIndex = Subject.QueryActivityIndex(blockIndex, a);
            List<SubjectLearningResultIndexesWeight> resultWeights = Subject.QueryActivityLearningResultsIndexesWeight(blockIndex, activityIndex);
            foreach(SubjectLearningResultIndexesWeight resultWeight in resultWeights)
            {
                if(resultWeight.weight > 0)
                {
                    resultsWeightsText += (first?"":", ") + String.Format("RA{0}&nbsp;({1}%)", resultWeight.learningResultIndex + 1, resultWeight.weight);
                    first = false;
                }
            }

            return resultsWeightsText.Length > 0 ? resultsWeightsText : "-";
        }

        public string GetReferencedCriteriasText(int blockIndex, Activity a)
        {
            string criteriasText = "";
            bool first = true;

            int activityIndex = Subject.QueryActivityIndex(blockIndex, a);
            List<SubjectLearningResultCriteriaIndex> criterias = Subject.QueryActivityReferencedLearningResultCriteriaIndexes(blockIndex, activityIndex);
            foreach(SubjectLearningResultCriteriaIndex criteria in criterias)
            {
                string criteriaPrefix = Utils.FormatLearningResultCriteria(criteria.learningResultIndex, criteria.criteriaIndex);
                criteriasText += (first?"":", ") + String.Format("{0}", criteriaPrefix);
                first = false;
            }

            return criteriasText.Length > 0 ? criteriasText : "-";
        }

        protected List<DocumentIndexItem> BuildIndex()
        {
            List<DocumentIndexItem> indexMetodologies = new();
            Subject.Metodologies.ToList().ForEach(m => indexMetodologies.Add(new() { Title = m.Title, Subitems = new() }));

            List<DocumentIndexItem> indexInstrumentTypes= new();
            Subject.EvaluationInstrumentsTypes.ToList().ForEach(instrument => indexInstrumentTypes.Add(new() { Title = instrument.Title, Subitems = new() }));

            List<DocumentIndexItem> indexMaterialResources = new();
            Subject.MaterialResources.ToList().ForEach(resource => indexMaterialResources.Add(new() { Title = resource.Title, Subitems = new() }));

            List<DocumentIndexItem> indexSpaceResources = new();
            Subject.SpaceResources.ToList().ForEach(resource => indexSpaceResources.Add(new() { Title = resource.Title, Subitems = new() }));

            List<DocumentIndexItem> indexBlocks = new();
            int blockIndex = 0;
            Subject.Blocks.ToList().ForEach(
                b =>
                {
                    indexBlocks.Add(new() { Title = String.Format("Bloque {0}", blockIndex + 1), Subitems = new() });
                    blockIndex++;
                });

            List<DocumentIndexItem> indexItems = new()
            {
                new DocumentIndexItem(){ Title = "Organización del módulo", Subitems = new() {} },
                new DocumentIndexItem(){ Title = "Justificación de la importancia del módulo", Subitems = new () {} },
                new DocumentIndexItem(){ Title = "Elementos curriculares", Subitems = new ()
                    {
                        new DocumentIndexItem() { Title = "Objetivos generales relacionados con el módulo", Subitems = new (){ } },
                        new DocumentIndexItem() { Title = "Competencias profesionales, personales y sociales", Subitems = new (){ } },
                        new DocumentIndexItem() { Title = "Capacidades clave", Subitems = new() { } }
                    }
                },
                new DocumentIndexItem(){ Title = "Metodología. Orientaciones didácticas", Subitems = new ()
                    {
                        new DocumentIndexItem() { Title = "Metodología general y específica de la materia", Subitems = indexMetodologies },
                        new DocumentIndexItem() { Title = "Medidas de atención al alumnado con necesidad específica de apoyo educativo" +
                                                  " o con necesidad de compensación educativa: atención a la diversidad", Subitems = new () { } }
                    }
                },
                new DocumentIndexItem(){ Title = "Sistema de evaluación", Subitems = new ()
                    {
                        new DocumentIndexItem() { Title = "Instrumentos de evaluación", Subitems = indexInstrumentTypes },
                        new DocumentIndexItem() { Title = "Evaluación del funcionamiento de la programación", Subitems = new (){ } }
                    }
                },
                new DocumentIndexItem(){ Title = "Elementos transversales", Subitems = new ()
                    {
                        new DocumentIndexItem() { Title = "Fomento de la lectura y tecnologías de la información y de comunicación", Subitems = new (){ } },
                        new DocumentIndexItem() { Title = "Comunicación audiovisual, emprendimiento, educación cívica y constitucional", Subitems = new (){ } }
                    }
                },

                new DocumentIndexItem(){ Title = "Recursos didácticos y organizativos", Subitems = new ()
                    {
                        new DocumentIndexItem() { Title = "Espacios requeridos", Subitems = indexSpaceResources },
                        new DocumentIndexItem() { Title = "Materiales y herramientas", Subitems = indexMaterialResources }
                    }
                },
                new DocumentIndexItem(){ Title = "Programación del módulo profesional", Subitems = new ()
                    {
                        new DocumentIndexItem() { Title = "Resultados de aprendizaje, criterios de evaluación y contenidos", Subitems = new ()
                            {
                                new DocumentIndexItem() { Title = "Resultados de aprendizaje y criterios de evaluación", Subitems = new (){ } },
                                new DocumentIndexItem() { Title = "Contenidos", Subitems = new (){ } }
                            }
                        },
                        new DocumentIndexItem() { Title = "Bloques de enseñanza y aprendizaje", Subitems = new (){ } },
                        new DocumentIndexItem() { Title = "Programación de actividades de enseñanza-aprendizaje", Subitems = indexBlocks },
                    }

                },
                new DocumentIndexItem(){ Title = "Referencias bibliográficas del módulo", Subitems = new () { } },
                new DocumentIndexItem(){ Title = "Anexos", Subitems = new ()
                    {
                        new DocumentIndexItem() { Title = "Cuadro de distribución de pesos", Subitems = new() { } }
                    }
                }
            };

            return indexItems;

        }

    }

}
