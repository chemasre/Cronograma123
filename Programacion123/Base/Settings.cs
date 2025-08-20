using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Programacion123
{

    public class Settings
    {
        internal static string basePath = "Settings\\";

        static bool isArchiveOpen;
        static string archiveExtractionPath;

        public static void Init()
        {
            if (!Directory.Exists(basePath)) { Directory.CreateDirectory(basePath); }

            HTMLGenerator generator = new();
            generator.LoadOrCreateSettings();
        }

        static string GetBasePath()
        {
            return (isArchiveOpen ? archiveExtractionPath + "_" + basePath: basePath);
        }

        static void DeleteAllFiles()
        {
            string[] files = Directory.GetFiles(GetBasePath(), "*.json");

            foreach (string f in files) { File.Delete(f);  }

        }

        public static void Reset()
        {
            if (!Directory.Exists(GetBasePath())) { Directory.CreateDirectory(GetBasePath()); }
            
            DeleteAllFiles();

            HTMLGenerator generator = new();
            generator.LoadOrCreateSettings();
        }

        public static T LoadOrCreateSettings<T>(string settingsId) where T: new()
        {
            T settings;

            if (!File.Exists(GetBasePath() + settingsId + ".json"))
            {
                settings = new T();

                SaveSettings<T>(settingsId, settings);
            }
            else
            {
                var stream = new FileStream(GetBasePath() + settingsId + ".json", FileMode.Open, FileAccess.Read);
                var reader = new StreamReader(stream);
                string text = reader.ReadToEnd();
                settings = JsonSerializer.Deserialize<T>(text);
                reader.Close();

            }

            return settings;
        }

        public static void SaveSettings<T>(string settingsId, T settings)
        {
            var stream = new FileStream(GetBasePath() + settingsId + ".json", FileMode.Create, FileAccess.Write);
            var writer = new StreamWriter(stream);
            JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerOptions.Default);
            options.WriteIndented = true;
            writer.Write(JsonSerializer.Serialize<T>(settings, options));
            writer.Close();

        }

        public static bool ExistSettings<T>(string settingsId)
        {
            return File.Exists(GetBasePath() + settingsId + ".json");
        }

        public static void Archive_Open(string archivePath)
        {
            archiveExtractionPath = Path.GetTempPath() + "Programacion123" + Guid.NewGuid().ToString() + "\\";
            Directory.CreateDirectory(archiveExtractionPath);
            ZipFile.ExtractToDirectory(archivePath, archiveExtractionPath);
            isArchiveOpen = true;
        }

        public static void Archive_Close()
        {
            DeleteAllFiles();
            Directory.Delete(GetBasePath());
            isArchiveOpen = false;
        }

        public static void Archive_Add(string archivePath)
        {
            using (ZipArchive zip = ZipFile.Open(archivePath, ZipArchiveMode.Update))
            {
                zip.CreateEntryFromFile(basePath + HTMLGenerator.SettingsId + ".json", "_" + basePath + HTMLGenerator.SettingsId + ".json");
            }

        }

        public static void Archive_CopyToBase()
        {
            if(File.Exists(archiveExtractionPath + "_" + basePath + HTMLGenerator.SettingsId + ".json"))
            {
                string sourceFile = archiveExtractionPath + "_" + basePath + HTMLGenerator.SettingsId + ".json";
                string destinationFile = basePath + HTMLGenerator.SettingsId + ".json";

                if(File.Exists(destinationFile)) { File.Delete(destinationFile);  }

                File.Copy(sourceFile, destinationFile);
            }


        }
    }
}
