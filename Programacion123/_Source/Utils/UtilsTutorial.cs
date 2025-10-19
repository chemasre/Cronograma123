using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    public partial class Utils
    {
        public static bool IsTutorialImported()
        {
            return Constants.tutorialPackageInfo.Any(
                (tuple) =>
                { return Storage.ExistsData(tuple.Item2, tuple.Item1); }
            );
        }

        public static bool IsTutorialComplete()
        {
            bool allFiles = Constants.tutorialPackageInfo.All(
                    (tuple) =>
                    { return Storage.ExistsData(tuple.Item2, tuple.Item1); }
                );

            bool sameFiles = false;

            if(allFiles)
            {
                sameFiles = Constants.tutorialPackageInfo.All(
                    (tuple) =>
                    { return Storage.Checksum_Calculate(tuple.Item2, tuple.Item1) == tuple.Item3; }
                );
            }

            return allFiles && sameFiles;
        }

        public static void ImportTutorial()
        {
            List<DocumentStyle> previousStyles = Storage.LoadAllEntities<DocumentStyle>();
            previousStyles.ForEach((s) => { s.Delete(); });

            Storage.Archive_Open(Constants.packagesBasePath + Constants.tutorialPackageName);

            List<string> gradeTemplatesIds = Storage.GetStorageIds<GradeTemplate>(Storage.LoadAllEntities<GradeTemplate>());
            List<string> subjectTemplatesIds = Storage.GetStorageIds<SubjectTemplate>(Storage.LoadAllEntities<SubjectTemplate>());
            List<string> calendarIds = Storage.GetStorageIds<Calendar>(Storage.LoadAllEntities<Calendar>());
            List<string> weekScheduleIds = Storage.GetStorageIds<WeekSchedule>(Storage.LoadAllEntities<WeekSchedule>());
            List<string> subjectIds = Storage.GetStorageIds<Subject>(Storage.LoadAllEntities<Subject>());
            List<string> styleIds = Storage.GetStorageIds<DocumentStyle>(Storage.LoadAllEntities<DocumentStyle>());

            List<string> storageIds = new();
            storageIds.AddRange(gradeTemplatesIds);
            storageIds.AddRange(subjectTemplatesIds);
            storageIds.AddRange(calendarIds);
            storageIds.AddRange(weekScheduleIds);
            storageIds.AddRange(subjectIds);
            storageIds.AddRange(styleIds);

            Storage.Archive_CopyStorageIdsToBase(storageIds);

            Storage.Archive_Close();

        }

    }
}
