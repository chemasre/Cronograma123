using System.IO;
using System.Text.Json;

namespace Programacion123
{
    internal class Storage
    {
        public static void SaveData<T>(string storageId, string storageClassId, T data, string? parentStorageId = null) where T: StorageData
        {
            string folder;

            if(parentStorageId != null)
            {   folder = parentStorageId + "\\";
                if(!Directory.Exists(folder)) { Directory.CreateDirectory(folder); }
            }
            else { folder = ""; }

            var stream = new FileStream(folder + storageId + "." + storageClassId, FileMode.Create, FileAccess.Write);
            var writer = new StreamWriter(stream);
            JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerOptions.Default);
            options.WriteIndented = true;
            writer.Write(JsonSerializer.Serialize<T>(data, options));
            writer.Close();
        }

        public static T LoadData<T>(string storageId, string storageClassId, string? parentStorageId = null) where T: StorageData
        {
            string folder = (parentStorageId != null ? parentStorageId + "\\" : "");

            var stream = new FileStream(folder + storageId + "." + storageClassId, FileMode.Open, FileAccess.Read);
            var reader = new StreamReader(stream);
            string text = reader.ReadToEnd();
            T data = JsonSerializer.Deserialize<T>(text);
            reader.Close();

            return data;
        }

        public static void DeleteData(string storageId, string storageClassId, string? parentStorageId = null)
        {
            string folder = (parentStorageId != null ? parentStorageId + "\\" : "");

            File.Delete(folder + storageId + "." + storageClassId);
        }

        public static List<T> LoadDatas<T>(string storageClassId, string? parentStorageId = null) where T : StorageData
        {
            string folder = (parentStorageId != null ? parentStorageId + "\\" : "");

            List<T> result = new();
            string[] files = Directory.GetFiles(folder + "", "*." + storageClassId);

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
                    entity.Load(e, parentStorageId);
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

        public static T LoadEntity<T>(string storageId, string? parentStorageId = null) where T: Entity, new()
        {
            T entity = new T();
            entity.Load(storageId, parentStorageId);
            return entity;
        }

        public static List<T> LoadEntities<T>(string? parentStorageId = null) where T : Entity, new()
        {
            string folder = (parentStorageId != null ? "\\" + parentStorageId: "");

            List<T> result = new();
            T entity = new T();

            if(Directory.Exists(Directory.GetCurrentDirectory() + folder))
            {
                string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + folder, "*." + entity.StorageClassId);
               
                Array.ForEach<string>(files,
                    (e) =>
                    {
                        T entity = new();
                        entity.Load(Path.GetFileNameWithoutExtension(e), parentStorageId);
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
