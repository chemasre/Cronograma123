using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Programacion123
{
    internal class Unit : Entity
    {
        internal int Id { get; set; }
        internal int Hours { get; set; }

        public Unit()
        {
            Id = 1;
            Hours = 1;

            StorageClassId = "unit";
        }

        public override void Save()
        {
            base.Save();

            UnitData data = new();
            data.Id = Id;
            data.Hours = Hours;

            Storage.SaveData<UnitData>(StorageId, StorageClassId, data);
        }

        public void Load(string storageId)
        {

            base.Load(storageId);

            UnitData data = Storage.LoadData<UnitData>(storageId, StorageClassId);

            Id = data.Id;
            Hours = data.Hours;

        }


        public override ValidationResult Validate()
        {
           if(Hours <= 0) { return ValidationResult.oneHourMinimum; }
           else { return ValidationResult.success; }
        }
    };
}
