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

    public partial class Subject : Entity
    {
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

        int FindCriteriaIndex(LearningResult learningResult, CommonText criteria)
        {
            return learningResult.Criterias.ToList().FindIndex(c => criteria.StorageId == c.StorageId);
        }

        int FindCriteriaLearningResultIndex(CommonText criteria)
        {
            return Template.LearningResults.ToList().FindIndex(r => FindCriteriaIndex(r, criteria) >= 0);
        }
    }
}
