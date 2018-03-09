using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace CoffeeUniversal.Helpers
{
    public class XmlSerializerForDictionary
    {
        public struct Pair<TKey, TValue>
        {
            public TKey Key;
            public TValue Value;
            public Pair(KeyValuePair<TKey, TValue> pair)
            {
                Key = pair.Key;
                Value = pair.Value;
            }
        }

        public static void WriteXml<TKey, TValue>(XmlWriter writer, IDictionary<TKey, TValue> dict)
        {
            var list = new List<Pair<TKey, TValue>>(dict.Count);
            foreach (var pair in dict)
            {
                list.Add(new Pair<TKey, TValue>(pair));
            }

            var serializer = new XmlSerializer(list.GetType());
            serializer.Serialize(writer, list);
        }

        public static void ReadXml<TKey, TValue>(XmlReader reader, IDictionary<TKey, TValue> dict)
        {
            reader.Read();
            var serializer = new XmlSerializer(typeof(List<Pair<TKey, TValue>>));
            var list = (List<Pair<TKey, TValue>>)serializer.Deserialize(reader);

            foreach (var pair in list)
            {
                dict.Add(pair.Key, pair.Value);
            }

            reader.Read();
        }
    }
}
