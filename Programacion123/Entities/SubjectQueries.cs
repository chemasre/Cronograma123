using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public partial class Subject : Entity
    {
        public int CountActivitySessions(ActivitySchedule schedule)
        {
            return CountActivitySessions(schedule.start.day, schedule.end.day);
        }

        public int CountActivitySessions(DateTime startDay, DateTime endDay)
        {
            int count = 0;
            for(DateTime d = startDay; d <= endDay; d = d.AddDays(1))
            {
                if(Utils.IsSchoolDay(d, Calendar, WeekSchedule)) { count++; }
            }

            return count;
        }


        public List<int> GetBlockEvaluableActivityIndexes(int blockIndex)
        {
            List<int> result = new();

            int index = 0;
            Blocks[blockIndex].Activities.ToList().ForEach(a => { if(a.IsEvaluable) { result.Add(index); index ++; } });
            
            return result;
        }

        public List<EvaluableActivityIndex> GetEvaluableActivityIndexes()
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

        public float GetBlockDuration(int blockIndex)
        {
            float hours = 0;
            Blocks[blockIndex].Activities.ToList().ForEach(a => hours += a.Duration);
            return hours;

        }

        public List<int> GetBlockReferencedContentIndexes(int blockIndex)
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

        public List<int> GetBlockReferencedKeyCompetencesIndexes(int blockIndex)
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


        public List<int> GetBlockReferencedLearningResultIndexes(int blockIndex)
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

        public List<SubjectLearningResultCriteriaIndex> GetBlockReferencedLearningResultCriteriaIndexes(int blockIndex)
        {
            List<SubjectLearningResultCriteriaIndex> referenced = new();

            List<int> referencedLearningResults = GetBlockReferencedLearningResultIndexes(blockIndex);

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

        public List<int> GetReferencedKeyCompetencesIndexes()
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
