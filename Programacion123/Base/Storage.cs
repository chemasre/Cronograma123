using System.IO;
using System.Text.Json;

namespace Programacion123
{
    internal class Storage
    {
        const string basePath = "Storage\\";

        public static void Init()
        {
            if(!Directory.Exists(basePath)) { Directory.CreateDirectory(basePath); }
        }

        public static bool ExistsData<T>(string storageId, string storageClassId, string? parentStorageId = null)  where T: StorageData
        {
            bool result = true;
            string folder;

            if(parentStorageId != null)
            {   folder = parentStorageId + "\\";
                if(!Directory.Exists(basePath + folder)) { result = false; }
            }
            else { folder = ""; }

            if(result)
            {
                if(!File.Exists(basePath + folder + storageId + "." + storageClassId)) { result = false; }
            }
            
            return result;

        }

        public static void SaveData<T>(string storageId, string storageClassId, T data, string? parentStorageId = null) where T: StorageData
        {
            string folder;

            if(parentStorageId != null)
            {   folder = parentStorageId + "\\";
                if(!Directory.Exists(basePath + folder)) { Directory.CreateDirectory(basePath + folder); }
            }
            else { folder = ""; }

            var stream = new FileStream(basePath + folder + storageId + "." + storageClassId, FileMode.Create, FileAccess.Write);
            var writer = new StreamWriter(stream);
            JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerOptions.Default);
            options.WriteIndented = true;
            writer.Write(JsonSerializer.Serialize<T>(data, options));
            writer.Close();
        }

        public static T LoadData<T>(string storageId, string storageClassId, string? parentStorageId = null) where T: StorageData
        {
            string folder = (parentStorageId != null ? parentStorageId + "\\" : "");

            var stream = new FileStream(basePath + folder + storageId + "." + storageClassId, FileMode.Open, FileAccess.Read);
            var reader = new StreamReader(stream);
            string text = reader.ReadToEnd();
            T data = JsonSerializer.Deserialize<T>(text);
            reader.Close();

            return data;
        }

        public static void DeleteData(string storageId, string storageClassId, string? parentStorageId = null)
        {
            string folder = (parentStorageId != null ? parentStorageId + "\\" : "");

            File.Delete(basePath + folder + storageId + "." + storageClassId);

            if(Directory.Exists(basePath + folder + storageId)) { Directory.Delete(basePath + folder + storageId); }
        }

        public static List<T> LoadDatas<T>(string storageClassId, string? parentStorageId = null) where T : StorageData
        {
            string folder = (parentStorageId != null ? parentStorageId + "\\" : "");

            List<T> result = new();
            string[] files = Directory.GetFiles(basePath + folder + "", "*." + storageClassId);

            Array.ForEach<string>(files, 
            (string e) =>
            {
                T data = LoadData<T>(Path.GetFileNameWithoutExtension(e), storageClassId);
                result.Add(data);
            });

            return result;
        }

        public static List<T> LoadEntities<T>(List<string> storageIds, string? parentStorageId = null) where T : Entity, new()
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

        public static T LoadOrCreateEntity<T>(string storageId, string? parentStorageId = null) where T: Entity, new()
        {
            T entity = new T();
            entity.LoadOrCreate(storageId, parentStorageId);
            return entity;
        }

        public static List<T> LoadEntities<T>(string? parentStorageId = null) where T : Entity, new()
        {
            string folder = (parentStorageId != null ? parentStorageId: "");

            List<T> result = new();
            T entity = new T();

            if(Directory.Exists(Directory.GetCurrentDirectory() + "\\" + basePath + folder))
            {
                string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\" + basePath + folder, "*." + entity.StorageClassId);
               
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

    }
}
