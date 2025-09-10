using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    public enum DocumentCoverElementId
    {
        Logo,
        SubjectCode,
        SubjectName,
        GradeTypeName,
        GradeName,
        Cover
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
        TableText,
        TableHeader1Text,
        TableHeader2Text,
        CoverSubjectCode,
        CoverSubjectName,
        CoverGradeTypeName,
        CoverGradeName,
        IndexLevel1,
        IndexLevel2,
        IndexLevel3,
        IndexTitle
    }

    public enum DocumentTableElementId
    {
        TableNormalCell,
        TableHeader1Cell,
        TableHeader2Cell
    
    }


}
