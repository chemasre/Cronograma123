namespace Programacion123
{
    public class Constants
    {
        public const string appName = "Programabara";
        public const string contactName = "Chema";
        public const string contactEmail = "chema.sre@gmail.com";
        public const string version = "0.9.12";

        public const float resetTaskMinDuration = 2.0f;
        public const float setupTaskMinDuration = 2.0f;
        public const float validationTaskMinDuration = 0.5f;

        public const string tutorialUrl = "https://youtube.com";
        public const string licenseUrl = "https://creativecommons.org/licenses/by-nc-nd/4.0/";
        public const string projectsUrl = "https://sinestesiagamedesign.es/teaching/projects";
        public const string otherApp1Url = "https://sinestesiagamedesign.es/teaching/projects/turtlesandbox";
        public const string otherApp2Url = "https://sinestesiagamedesign.es/teaching/projects/miniboycolor";

        public const float buttonNotAvailableOpacity = 0.25f;
        public const string buttonAvailableEffect = "EffectDropShadow";

        public const string configFileName = "Config.json";

        public const string storageBasePath = "Storage\\";

        public const string packagesBasePath = "Packages\\";

        public const string tutorialPackageName = "Ejemplos.zip";
        public static readonly List<Tuple<string, string, string> > tutorialPackageInfo = new() {
                new("gradeTemplate", "b917baf5-dd9f-4f0e-89b3-ba55a1dfc963", "078331D2B148FD545B018721BDD21C0E"),
                new("subjecttemplate", "874c5a67-3bae-424f-b58e-732993426265", "AE6A9AB4F54A5A356C782D4B94C6B854"),
                new ("calendar", "198dadc6-4763-41c9-a4a4-6974a99df247", "44BE47D2D812782F0249166F6667DE38"),
                new ("weekschedule", "8e18fc0c-1b8e-48cb-b0fc-1860430b8b64", "AAD553ACB20B1C5C2289934980C7AB19"),
                new("subject", "89065e34-6721-4bdc-8068-5569a41203a6", "557AB10E860139D3D9509D058BE4A706")
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
