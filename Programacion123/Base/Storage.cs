using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Programacion123
{
    internal class Storage
    {
        public static void SaveData<T>(string storageId, string storageClassId, T data) where T: StorageData
        {
            var stream = new FileStream(storageId + "." + storageClassId, FileMode.Create, FileAccess.Write);
            var writer = new StreamWriter(stream);
            JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerOptions.Default);
            options.WriteIndented = true;
            writer.Write(JsonSerializer.Serialize<T>(data, options));
            writer.Close();
        }

        public static T LoadData<T>(string storageId, string storageClassId) where T: StorageData
        {
            var stream = new FileStream(storageId + "." + storageClassId, FileMode.Open, FileAccess.Read);
            var reader = new StreamReader(stream);
            string text = reader.ReadToEnd();
            T data = JsonSerializer.Deserialize<T>(text);
            reader.Close();

            return data;
        }

        public static void DeleteData(string storageId, string storageClassId)
        {
            File.Delete(storageId + "." + storageClassId);
        }

        public static List<T> LoadData<T>(string storageClassId) where T : StorageData
        {
            List<T> result = new();
            string[] files = Directory.GetFiles("", "*." + storageClassId);

            Array.ForEach<string>(files, 
            (string e) =>
            {
                T data = LoadData<T>(Path.GetFileNameWithoutExtension(e), storageClassId);
                result.Add(data);
            });

            return result;
        }

        public static List<T> LoadEntities<T>(List<string> storageIds) where T : Entity, new()
        {
            List<T> result = new();

            storageIds.ForEach(
                e =>
                {   T entity = new T();
                    entity.Load(e);
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

        public static T LoadEntity<T>(string storageId) where T: Entity, new()
        {
            T entity = new T();
            entity.Load(storageId);
            return entity;
        }

        public static List<T> LoadEntities<T>() where T : Entity, new()
        {
            List<T> result = new();
            T entity = new T();

            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*." + entity.StorageClassId);
               

            Array.ForEach<string>(files,
                (e) =>
                {
                    T entity = new();
                    entity.Load(Path.GetFileNameWithoutExtension(e));
                    result.Add(entity);
                });

            return result;

        }

        public static void SaveEntities<T>(List<T> entities) where T : Entity
        {
            entities.ForEach((e) => { e.Save(); } );
        }

    }
}
