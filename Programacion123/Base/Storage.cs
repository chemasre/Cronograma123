using System.IO;
using System.Text.Json;

namespace Programacion123
{
    internal partial class Storage
    {
        static string basePath = "Storage\\";

        public static void Init()
        {
            if(!Directory.Exists(basePath)) { Directory.CreateDirectory(basePath); }
        }

        static string GetBasePath()
        {
            return (isArchiveOpen ? archiveExtractionPath : basePath);
        }

        public static string? FindParentStorageId(string storageId, string storageClassId)
        {
            string? found = null;
            string[] directories = Directory.GetDirectories(GetBasePath());
            int i = 0;
            while(i < directories.Length && found == null)
            {
                if(File.Exists(directories[i] + "\\" + storageId + "." + storageClassId)) { found = directories[i]; }
                else { i ++; }
            }

            if(found != null)
            {
                int index = found.LastIndexOf('\\');
                found = found.Substring(index + 1);
            }

            return found;
        }

        /// <summary>
        /// Finds an entity by its storageId. The entity must have a parent different from null.
        /// </summary>
        public static T? FindChildEntity<T>(string storageId) where T: Entity, new()
        {
            T entity = new T();

            string? parentStorageId = FindParentStorageId(storageId, entity.StorageClassId);

            if(parentStorageId == null)
            {
                return null;
            }
            else
            {
                entity.LoadOrCreate(storageId, parentStorageId);

                return entity;
            }


        }
        
        /// <summary>
        /// Finds an entity by its storageId. The entities must have parents different from null and not necesarily the same.
        /// </summary>
        public static List<T> FindChildEntities<T>(List<string> storageIds) where T:Entity, new()
        {
            List<T> result = new List<T?>();
            storageIds.ForEach(
                    e =>
                    {
                        T? entity = FindChildEntity<T>(e); 
                        if(entity != null) { result.Add(entity); }
                    }
                );
            return result;
        }

        /// <summary>
        /// Finds an entity by its storage id. The entity can have a parent or not.
        /// </summary>
        public static T? FindEntity<T>(string storageId) where T: Entity, new()
        {
            if(ExistsEntity<T>(storageId)) { return LoadOrCreateEntity<T>(storageId); }
            else { return FindChildEntity<T>(storageId); }            
        }

        /// <summary>
        /// Finds entities by their storage ids. The entities can have parents or not, and not necesarily the same.
        /// </summary>
        public static List<T?> FindEntities<T>(List<string> storageIds) where T:Entity, new()
        {
            List<T?> result = new List<T?>();
            storageIds.ForEach(e =>
                            {
                                T? entity = FindEntity<T>(e); 
                                if(entity != null) { result.Add(entity); } 
                            });
            return result;
        }

        /// <summary>
        /// Finds an entity with a specific storageId and parentStorageId
        /// </summary>
        public static T? FindEntity<T>(string storageId, string? parentStorageId) where T:Entity, new()
        {
            if(ExistsEntity<T>(storageId, parentStorageId)) { return LoadOrCreateEntity<T>(storageId, parentStorageId); }
            else { return null; }
        }

        public static bool ExistsData<T>(string storageId, string storageClassId, string? parentStorageId = null)  where T: StorageData
        {
            bool result = true;
            string folder;

            if(parentStorageId != null)
            {   folder = parentStorageId + "\\";
                if(!Directory.Exists(GetBasePath() + folder)) { result = false; }
            }
            else { folder = ""; }

            if(result)
            {
                if(!File.Exists(GetBasePath() + folder + storageId + "." + storageClassId)) { result = false; }
            }
            
            return result;

        }

        public static void SaveData<T>(string storageId, string storageClassId, T data, string? parentStorageId = null) where T: StorageData
        {
            string folder;

            if(parentStorageId != null)
            {   folder = parentStorageId + "\\";
                if(!Directory.Exists(GetBasePath() + folder)) { Directory.CreateDirectory(GetBasePath() + folder); }
            }
            else { folder = ""; }

            var stream = new FileStream(GetBasePath() + folder + storageId + "." + storageClassId, FileMode.Create, FileAccess.Write);
            var writer = new StreamWriter(stream);
            JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerOptions.Default);
            options.WriteIndented = true;
            writer.Write(JsonSerializer.Serialize<T>(data, options));
            writer.Close();
        }

        public static T LoadData<T>(string storageId, string storageClassId, string? parentStorageId = null) where T: StorageData
        {
            string folder = (parentStorageId != null ? parentStorageId + "\\" : "");

            var stream = new FileStream(GetBasePath() + folder + storageId + "." + storageClassId, FileMode.Open, FileAccess.Read);
            var reader = new StreamReader(stream);
            string text = reader.ReadToEnd();
            T data = JsonSerializer.Deserialize<T>(text);
            reader.Close();

            return data;
        }

        public static void DeleteData(string storageId, string storageClassId, string? parentStorageId = null)
        {
            string folder = (parentStorageId != null ? parentStorageId + "\\" : "");

            File.Delete(GetBasePath() + folder + storageId + "." + storageClassId);

            if(Directory.Exists(GetBasePath() + folder + storageId)) { Directory.Delete(GetBasePath() + folder + storageId); }
        }

        public static List<T> LoadDatas<T>(string storageClassId, string? parentStorageId = null) where T : StorageData
        {
            string folder = (parentStorageId != null ? parentStorageId + "\\" : "");

            List<T> result = new();
            string[] files = Directory.GetFiles(GetBasePath() + folder + "", "*." + storageClassId);

            Array.ForEach<string>(files, 
            (string e) =>
            {
                T data = LoadData<T>(Path.GetFileNameWithoutExtension(e), storageClassId);
                result.Add(data);
            });

            return result;
        }

        public static List<T> LoadOrCreateEntities<T>(List<string> storageIds, string? parentStorageId = null) where T : Entity, new()
        {
            List<T> result = new();

            storageIds.ForEach(
                e =>
                {   T entity = new T();
                    entity.LoadOrCreate(e, parentStorageId);
                    result.Add(entity);
                });

            return result;
        }

        public static List<string> GetStorageIds<T>(List<T> entities) where T: Entity
        {
            List<string> result = new();

            entities.ForEach( e => result.Add(e.StorageId) );

            return result;
        }

        public static bool ExistsEntity<T>(string storageId, string? parentStorageId = null) where T: Entity, new()
        {
            T entity = new T();
            return entity.Exists(storageId, parentStorageId);
        }

        public static T LoadOrCreateEntity<T>(string storageId, string? parentStorageId = null) where T: Entity, new()
        {
            T entity = new T();
            entity.LoadOrCreate(storageId, parentStorageId);
            return entity;
        }

        public static List<T> LoadAllEntities<T>(string? parentStorageId = null) where T : Entity, new()
        {
            string folder = (parentStorageId != null ? parentStorageId: "");

            List<T> result = new();
            T entity = new T();

            if(Directory.Exists(GetBasePath() + folder))
            {
                string[] files = Directory.GetFiles(GetBasePath() + folder, "*." + entity.StorageClassId);
               
                Array.ForEach<string>(files,
                    (e) =>
                    {
                        T entity = new();
                        entity.LoadOrCreate(Path.GetFileNameWithoutExtension(e), parentStorageId);
                        result.Add(entity);
                    });
            }

            return result;

        }

        public static void SaveEntities<T>(List<T> entities, string? parentStorageId = null) where T : Entity
        {
            entities.ForEach((e) => { e.Save(parentStorageId); } );
        }

        public static void Reset()
        {
            ResetRecursively(GetBasePath());
        }

        static void ResetRecursively(string directory, int depth = 0)
        {
            string[] files = Directory.GetFiles(directory);
            Array.ForEach(files, (f) => File.Delete(f));

            string[] directories = Directory.GetDirectories(directory);
            Array.ForEach(directories, (d) => ResetRecursively(d, depth + 1));

            if(depth > 0)
            {
                Directory.Delete(directory);
            }

        }
    }
}
