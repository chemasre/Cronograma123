namespace Programacion123
{
    public struct SubjectLearningResultCriteriaIndex
    {
        public int learningResultIndex;
        public int criteriaIndex;
    }

    public struct EvaluableActivityIndex
    {
        public int blockIndex;
        public int evaluableActivityIndex;
    }

    public struct SubjectLearningResultIndexesWeight
    {
        public int learningResultIndex;
        public float weight;
    }

    public struct ContentPointIndex()
    {
        public int contentIndex;
        public int pointIndex;
    }

    public partial class Subject : Entity
    {
        public int QueryActivitySessionsCount(ActivitySchedule schedule)
        {
            return QueryActivitySessionsCount(schedule.start.day, schedule.end.day);
        }

        public int QueryActivitySessionsCount(DateTime startDay, DateTime endDay)
        {
            int count = 0;
            for(DateTime d = startDay; d <= endDay; d = d.AddDays(1))
            {
                if(Utils.IsSchoolDay(d, Calendar, WeekSchedule)) { count++; }
            }

            return count;
        }


        public List<int> QueryBlockEvaluableActivityIndexes(int blockIndex)
        {
            List<int> result = new();

            int index = 0;
            Blocks[blockIndex].Activities.ToList().ForEach(a => { if(a.IsEvaluable) { result.Add(index); index ++; } });
            
            return result;
        }

        public List<EvaluableActivityIndex> QueryEvaluableActivityIndexes()
        {
            List<EvaluableActivityIndex> result = new();

            int blockIndex = 0;
            int activityIndex = 0;
            Blocks.ToList().ForEach(
                (b) =>
                {
                    activityIndex = 0;
                    b.Activities.ToList().ForEach(
                        (a) =>
                        {
                            if(a.IsEvaluable)
                            {
                                result.Add(new() { blockIndex = blockIndex, evaluableActivityIndex = activityIndex });
                                activityIndex ++;
                            }
                        }
                    );

                    blockIndex ++;
                }
            );

            return result;
        }

        public float QueryBlockDuration(int blockIndex)
        {
            float hours = 0;
            Blocks[blockIndex].Activities.ToList().ForEach(a => hours += a.Duration);
            return hours;

        }

        public List<int> QueryBlockReferencedContentIndexes(int blockIndex)
        {
            List<int> referencedList;

            HashSet<int> referencedSet = new();
            foreach (Activity a in Blocks[blockIndex].Activities.ToList())
            {
                a.ContentPoints.ToList().ForEach(p => referencedSet.Add(FindContentPointContentIndex(p)));
            }

            referencedList = referencedSet.ToList();
            referencedList.Sort();


            return referencedList;

        }

        public List<int> QueryBlockReferencedKeyCompetencesIndexes(int blockIndex)
        {
            List<int> referencedList;

            HashSet<int> referencedSet = new();
            foreach (Activity a in Blocks[blockIndex].Activities.ToList())
            {
                a.KeyCompetences.ToList().ForEach(c => referencedSet.Add(FindKeyCompetenceIndex(c)));
            }

            referencedList = referencedSet.ToList();
            referencedList.Sort();


            return referencedList;

        }


        public List<int> QueryBlockReferencedLearningResultIndexes(int blockIndex)
        {
            List<int> referencedList;

            HashSet<int> referencedSet = new();
            foreach (Activity a in Blocks[blockIndex].Activities.ToList())
            {
                if (a.IsEvaluable)
                {
                    a.Criterias.ToList().ForEach(c => referencedSet.Add(FindCriteriaLearningResultIndex(c)));
                }
            }

            referencedList = referencedSet.ToList();
            referencedList.Sort();


            return referencedList;
        }

        public List<int> QueryActivityReferencedLearningResultIndexes(int blockIndex, int activityIndex)
        {
            List<int> referencedList;

            HashSet<int> referencedSet = new();
            Activity a = Blocks[blockIndex].Activities[activityIndex];

            if (a.IsEvaluable)
            {
                a.Criterias.ToList().ForEach(c => referencedSet.Add(FindCriteriaLearningResultIndex(c)));
            }

            referencedList = referencedSet.ToList();
            referencedList.Sort();


            return referencedList;
        }


        public List<SubjectLearningResultCriteriaIndex> QueryActivityReferencedLearningResultCriteriaIndexes(int blockIndex, int activityIndex)
        {
            List<SubjectLearningResultCriteriaIndex> referenced = new();

            List<int> referencedLearningResults = QueryActivityReferencedLearningResultIndexes(blockIndex, activityIndex);

            Activity a = Blocks[blockIndex].Activities[activityIndex];

            Dictionary<int, HashSet<int>> referencedCriteriasByLearningResult = new();

            if (a.IsEvaluable)
            {
                foreach (var c in a.Criterias.ToList())
                {
                    referencedLearningResults.ForEach(
                        (learningResultIndex) =>
                        {
                            int criteriaIndex = FindCriteriaIndex(Template.LearningResults[learningResultIndex], c);
                            if (criteriaIndex >= 0)
                            {
                                if (!referencedCriteriasByLearningResult.ContainsKey(learningResultIndex))
                                { referencedCriteriasByLearningResult.Add(learningResultIndex, new HashSet<int>()); }
                                referencedCriteriasByLearningResult[learningResultIndex].Add(criteriaIndex);

                            }
                        }
                    );
                }
            }

            referencedLearningResults.ForEach(
                (learningResultIndex) =>
                {
                    List<int> criterias = referencedCriteriasByLearningResult[learningResultIndex].ToList<int>();
                    criterias.Sort();
                    criterias.ForEach((criteriaIndex) => referenced.Add(new SubjectLearningResultCriteriaIndex() { learningResultIndex = learningResultIndex, criteriaIndex = criteriaIndex }));
                }
            );

            return referenced;


        }

        public List<SubjectLearningResultCriteriaIndex> QueryBlockReferencedLearningResultCriteriaIndexes(int blockIndex)
        {
            List<SubjectLearningResultCriteriaIndex> referenced = new();

            List<int> referencedLearningResults = QueryBlockReferencedLearningResultIndexes(blockIndex);

            Dictionary<int, HashSet<int>> referencedCriteriasByLearningResult = new();

            foreach (Activity a in Blocks[blockIndex].Activities.ToList())
            {
                if (a.IsEvaluable)
                {
                    foreach (var c in a.Criterias.ToList())
                    {
                        referencedLearningResults.ForEach(
                            (learningResultIndex) =>
                            {
                                int criteriaIndex = FindCriteriaIndex(Template.LearningResults[learningResultIndex], c);
                                if (criteriaIndex >= 0)
                                {
                                    if (!referencedCriteriasByLearningResult.ContainsKey(learningResultIndex))
                                    { referencedCriteriasByLearningResult.Add(learningResultIndex, new HashSet<int>()); }
                                    referencedCriteriasByLearningResult[learningResultIndex].Add(criteriaIndex);

                                }
                            }
                        );
                    }
                }
            }

            referencedLearningResults.ForEach(
                (learningResultIndex) =>
                {
                    List<int> criterias = referencedCriteriasByLearningResult[learningResultIndex].ToList<int>();
                    criterias.Sort();
                    criterias.ForEach((criteriaIndex) => referenced.Add(new SubjectLearningResultCriteriaIndex() { learningResultIndex = learningResultIndex, criteriaIndex = criteriaIndex }));
                }
            );

            return referenced;
        }

        public List<int> QueryReferencedKeyCompetencesIndexes()
        {
            HashSet<int> referencesSet = new();
            List<int> sortedReferencesList = new();

            foreach(Block b in Blocks.ToList())
            {
                foreach(Activity a in b.Activities.ToList())
                {
                    foreach(CommonText c in a.KeyCompetences.ToList())
                    {
                        referencesSet.Add(FindKeyCompetenceIndex(c));
                    }
                }
            }

            sortedReferencesList = referencesSet.ToList();
            sortedReferencesList.Sort();

            return sortedReferencesList;
        }

        public List<SubjectLearningResultIndexesWeight> QueryLearningResultsIndexesWeights()
        {

            List<SubjectLearningResultIndexesWeight> result = new();

            List< KeyValuePair<LearningResult, float> > learningResultsWeights = LearningResultsWeights.ToList();
            List<LearningResult> learningResultsList = Template.LearningResults.ToList();

            for(int i = 0; i < learningResultsList.Count; i++)
            {
                int foundIndex;
                float weight;                    
                foundIndex = learningResultsWeights.FindIndex(r => r.Key.StorageId == learningResultsList[i].StorageId);
                if(foundIndex >= 0) { weight = learningResultsWeights[foundIndex].Value; }
                else { weight = 0; }

                result.Add(new SubjectLearningResultIndexesWeight() { learningResultIndex = i, weight = weight });
            }

            return result;

        }

        public List<SubjectLearningResultIndexesWeight> QueryActivityLearningResultsIndexesWeight(int blockIndex, int activityIndex)
        {
            List<SubjectLearningResultIndexesWeight> result = new();

            List<LearningResult> learningResultList = Template.LearningResults.ToList();
            List<KeyValuePair<LearningResult, float>> learningResultsWeightsList = Blocks[blockIndex].Activities[activityIndex].LearningResultsWeights.ToList();

            for(int i = 0; i < learningResultList.Count; i++)
            {
                float weight;
                int foundIndex = learningResultsWeightsList.FindIndex(r => r.Key.StorageId == learningResultList[i].StorageId);
                if(foundIndex >= 0)
                {
                    weight = learningResultsWeightsList[foundIndex].Value;
                }
                else
                {
                    weight = 0;
                }

                result.Add(new() {learningResultIndex = i, weight = weight });
            }

            return result;
        }

        public List<ContentPointIndex> QueryActivityContentPointsIndexes(int blockIndex, int activityIndex)
        {
            List<ContentPointIndex> result = new();
            List<CommonText> activityContentPoints = Blocks[blockIndex].Activities[activityIndex].ContentPoints.ToList();

            int contentIndex = 0;
            foreach(Content c in Template.Contents.ToList())
            {
                int pointIndex = 0;
                foreach(CommonText p in c.Points.ToList())
                {
                    if(activityContentPoints.FindIndex(activityPoint => activityPoint.StorageId == p.StorageId) >= 0)
                    {
                        result.Add(new ContentPointIndex() {contentIndex = contentIndex, pointIndex = pointIndex });
                    }
                    pointIndex ++;
                }

                contentIndex ++;

            }

            return result;
        }

        public int QueryActivityIndex(int blockIndex, Activity activity)
        {
            return Blocks[blockIndex].Activities.ToList().FindIndex(a => a.StorageId == activity.StorageId);
        }

        public int QueryEvaluableActivityIndex(int blockIndex, Activity activity)
        {
            List<Activity> evaluables = new List<Activity>(Blocks[blockIndex].Activities.ToList().Where(a => a.IsEvaluable));
            return evaluables.FindIndex(a => a.StorageId == activity.StorageId);
        }

        int FindCriteriaIndex(LearningResult learningResult, CommonText criteria)
        {
            return learningResult.Criterias.ToList().FindIndex(c => criteria.StorageId == c.StorageId);
        }

        int FindCriteriaLearningResultIndex(CommonText criteria)
        {
            return Template.LearningResults.ToList().FindIndex(r => FindCriteriaIndex(r, criteria) >= 0);
        }

        int FindContentPointIndex(Content content, CommonText contentPoint)
        {
            return content.Points.ToList().FindIndex(p => contentPoint.StorageId == p.StorageId);
        }

        int FindContentPointContentIndex(CommonText contentPoint)
        {
            return Template.Contents.ToList().FindIndex(c => FindContentPointIndex(c, contentPoint) >= 0);
        }

        int FindKeyCompetenceIndex(CommonText competence)
        {
            return Template.GradeTemplate.KeyCapacities.ToList().FindIndex(c => c.StorageId == competence.StorageId);
        }

    }
}
