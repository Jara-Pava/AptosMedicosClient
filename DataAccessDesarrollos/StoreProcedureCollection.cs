
using System.Configuration;

namespace DataAccessDesarrollos
{
    public class StoreProcedureCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new StoreProcedure();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((StoreProcedure)element).Code;
        }

        public StoreProcedure this[int index] => (StoreProcedure)BaseGet(index);

        public new StoreProcedure this[string code] => (StoreProcedure)BaseGet(code);

        public void Add(StoreProcedure sp) => BaseAdd(sp);

        public void Remove(string code)
        {
            if (BaseGet(code) != null)
                BaseRemove(code);
        }

        public void Clear() => BaseClear();
    }
}