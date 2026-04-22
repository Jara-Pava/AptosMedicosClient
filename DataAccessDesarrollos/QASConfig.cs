using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessDesarrollos
{
    public class QASConfig : ConfigurationSection
    {
        [ConfigurationProperty("ReloadTime", DefaultValue = 600000L, IsRequired = false, IsKey = false)]
        public long ReloadTime
        {
            get { return (long)this["ReloadTime"]; }
            set { this["ReloadTime"] = value; }
        }

        [ConfigurationProperty("InitialDateTime", IsRequired = false, IsKey = false)]
        public DateTime InitialDateTime
        {
            get { return (DateTime)(this["InitialDateTime"] ?? DateTime.MinValue); }
            set { this["InitialDateTime"] = value; }
        }

        [ConfigurationProperty("StoreProcedures", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(StoreProcedureCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public StoreProcedureCollection StoreProcedures
        {
            get { return (StoreProcedureCollection)base["StoreProcedures"]; }
        }
    }
}