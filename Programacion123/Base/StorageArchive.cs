using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion123
{
    internal partial class Storage
    {
        static bool isArchiveOpen;
        static string archiveExtractionPath;

        public static void Archive_Open(string archivePath)
        {
            archiveExtractionPath = Path.GetTempPath() + "Programacion123" + Guid.NewGuid().ToString() + "\\";
            Directory.CreateDirectory(archiveExtractionPath);
            ZipFile.ExtractToDirectory(archivePath, archiveExtractionPath);
            isArchiveOpen = true;
        }

        public static void Archive_Close()
        {
            Archive_DeleteDirectory(archiveExtractionPath);
            isArchiveOpen = false;
        }

        static void Archive_CopyStorageIdToBase(string rootStorageId)
        {
            Archive_CopyStorageIdToBaseRecursive(rootStorageId, null);
        }

        static void Archive_CopyStorageIdToBaseRecursive(string storageId, string? parentStorageId)
        {
            string sourceFile = Directory.GetFiles(archiveExtractionPath + (parentStorageId != null ? parentStorageId : ""), storageId + ".*")[0];
            string destinationFile = basePath + (parentStorageId != null ? parentStorageId + "\\" : "") + Path.GetFileName(sourceFile);

            if(Directory.Exists(archiveExtractionPath + storageId))
            {
                Directory.CreateDirectory(basePath + storageId);

                string[] files = Directory.GetFiles(archiveExtractionPath + storageId);

                foreach(string f in files)
                {
                    Archive_CopyStorageIdToBaseRecursive(Path.GetFileNameWithoutExtension(f), storageId);
                }
            }

            File.Copy(sourceFile, destinationFile);
        }

        static void Archive_DeleteStorageIdFromBase(string rootStorageId)
        {
            Archive_DeleteStorageIdFromBaseRecursive(rootStorageId, null);
        }

        static void Archive_DeleteStorageIdFromBaseRecursive(string storageId, string? parentStorageId = null)
        {
            string file = Directory.GetFiles(basePath + (parentStorageId != null ? parentStorageId : ""), storageId + ".*")[0];

            if(Directory.Exists(basePath + storageId))
            {
                string[] directoryFiles = Directory.GetFiles(basePath + storageId, "*.*");

                foreach(string f in directoryFiles)
                {
                    Archive_DeleteStorageIdFromBaseRecursive(Path.GetFileNameWithoutExtension(f), storageId);
                }

                Directory.Delete(basePath + storageId);
            }

            File.Delete(file);

        }

        static bool Archive_ExistsStorageIdInBase(string rootStorageId)
        {
            string[] files = Directory.GetFiles(basePath);

            if(Array.Find<string>(files, (f => Path.GetFileNameWithoutExtension(f) == rootStorageId)) != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        static void Archive_DeleteDirectory(string directory)
        {
            string[] files = Directory.GetFiles(directory);
            Array.ForEach(files, f => File.Delete(f));

            string[] directories = Directory.GetDirectories(directory);
            Array.ForEach(directories, d => Archive_DeleteDirectory(d));

            Directory.Delete(directory);
        }



        public static void Archive_CopyStorageIdsToBase(List<string> rootStorageIds)
        {
            foreach(string s in rootStorageIds)
            {
                if(Archive_ExistsStorageIdInBase(s)) { Archive_DeleteStorageIdFromBase(s); }

                Archive_CopyStorageIdToBase(s);
            }
        }

        public static void Archive_Create(List<string> rootStorageIds, string archivePath)
        {
            using(ZipArchive zip = ZipFile.Open(archivePath, ZipArchiveMode.Update))
            {
                ArchiveCreate_Recursive(basePath, rootStorageIds, zip);
            }
        }

        static void ArchiveCreate_Recursive(string path, List<string> storageIds, ZipArchive zip)
        {
            Dictionary<string, string> storageIdToFilePath = new();
            string[] files = Directory.GetFiles(path, "*.*");
            Array.ForEach<string>(files, (f) => storageIdToFilePath.Add(Path.GetFileNameWithoutExtension(f), f));

            foreach(string s in storageIds)
            {
                zip.CreateEntryFromFile(storageIdToFilePath[s], storageIdToFilePath[s].Substring(basePath.Length));

                string directory = basePath + s;
                if(Directory.Exists(directory))
                {
                    List<string> directoryStorageIds = new();
                    Array.ForEach<string>(Directory.GetFiles(directory, "*.*"), (f) => directoryStorageIds.Add(Path.GetFileNameWithoutExtension(f) ));

                    ArchiveCreate_Recursive(directory, directoryStorageIds, zip); 
                }
            }
        }
    }
}
