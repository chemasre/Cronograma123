namespace Programacion123
{
    public class Constants
    {
        public const string appName = "Programabara";
        public const string contactName = "Chema";
        public const string contactEmail = "chema.sre@gmail.com";
        public const string version = "0.9.14";

        public const float resetTaskMinDuration = 2.0f;
        public const float setupTaskMinDuration = 2.0f;
        public const float validationTaskMinDuration = 0.5f;

        public const string tutorialUrl = "https://www.youtube.com/watch?v=VaI0sMlEeZ4&list=PLACmNd6XmIJ0snX7z9p2fWkYNfvidA2-v";
        public const string licenseUrl = "https://creativecommons.org/licenses/by-nc-nd/4.0/";
        public const string projectsUrl = "https://sinestesiagamedesign.es/teaching/projects";
        public const string otherApp1Url = "https://sinestesiagamedesign.es/teaching/projects/turtlesandbox";
        public const string otherApp2Url = "https://sinestesiagamedesign.es/teaching/projects/miniboycolor";

        public const float buttonNotAvailableOpacity = 0.25f;
        public const string buttonAvailableEffect = "EffectDropShadow";

        public const string appFolderName = "Programabara";

        public const string configFileName = "Config.json";

        public const string storageBasePath = "Storage\\";

        public const string packagesBasePath = "Packages\\";

        public const string tutorialPackageName = "Ejemplos.zip";
        public static readonly List<Tuple<string, string, string> > tutorialPackageInfo = new() {
                new("gradeTemplate", "b917baf5-dd9f-4f0e-89b3-ba55a1dfc963", "7DEBF10CD3859FAEF5FB6C45E02DE762"),
                new("subjecttemplate", "874c5a67-3bae-424f-b58e-732993426265", "AC08083E3B3AF216DD6A8A241423F455"),
                new ("calendar", "198dadc6-4763-41c9-a4a4-6974a99df247", "ED57F14BC4B45C1625ED5414E88F9C4B"),
                new ("weekschedule", "8e18fc0c-1b8e-48cb-b0fc-1860430b8b64", "29A3FC23C55968FF57A810262FB8E5C1"),
                new("subject", "89065e34-6721-4bdc-8068-5569a41203a6", "6CE471C682A29CB4358254E15B7B37A1")
        };

        public const string logFileName = "Log.txt";
        public const int logLineBlockSize = 1000;
        public const int logMaxLineBlocks = 3;

        public const int christmasStartDay = 20;
        public const int christmasStartMonth = 12;

        public const int christmasEndDay = 7;
        public const int christmasEndMonth = 1;
    }

}
