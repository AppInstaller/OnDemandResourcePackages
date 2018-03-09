using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CoffeeUniversal.Helpers
{
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        public virtual void WriteXml(XmlWriter writer)
        {
            XmlSerializerForDictionary.WriteXml(writer, this);
        }

        public virtual void ReadXml(XmlReader reader)
        {
            XmlSerializerForDictionary.ReadXml(reader, this);
        }

        public virtual XmlSchema GetSchema()
        {
            return null;
        }
    }
}
