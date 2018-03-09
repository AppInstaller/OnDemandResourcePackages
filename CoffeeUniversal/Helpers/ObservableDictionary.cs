using System.Collections.Generic;
using System.Linq;
using Windows.Foundation.Collections;

namespace CoffeeUniversal.Helpers
{
	public class ObservableDictionary : IObservableMap<string, object>
    {
        private class ObservableDictionaryChangedEventArgs : IMapChangedEventArgs<string>
        {
            public ObservableDictionaryChangedEventArgs(CollectionChange change, string key)
            {
                CollectionChange = change;
                Key = key;
            }

            public CollectionChange CollectionChange { get; private set; }
            public string Key { get; private set; }
        }

        private Dictionary<string, object> dictionary = new Dictionary<string, object>();
        public event MapChangedEventHandler<string, object> MapChanged;

        private void InvokeMapChanged(CollectionChange change, string key)
        {
            var eventHandler = MapChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new ObservableDictionaryChangedEventArgs(change, key));
            }
        }

        public void Add(string key, object value)
        {
            dictionary.Add(key, value);
            InvokeMapChanged(CollectionChange.ItemInserted, key);
        }

        public void Add(KeyValuePair<string, object> item)
        {
            Add(item.Key, item.Value);
        }

        public bool Remove(string key)
        {
            if (dictionary.Remove(key))
            {
                InvokeMapChanged(CollectionChange.ItemRemoved, key);
                return true;
            }
            return false;
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            object currentValue;
            if (dictionary.TryGetValue(item.Key, out currentValue) &&
                object.Equals(item.Value, currentValue) && dictionary.Remove(item.Key))
            {
                InvokeMapChanged(CollectionChange.ItemRemoved, item.Key);
                return true;
            }
            return false;
        }

        public object this[string key]
        {
            get
            {
                return dictionary[key];
            }
            set
            {
                dictionary[key] = value;
                InvokeMapChanged(CollectionChange.ItemChanged, key);
            }
        }

        public void Clear()
        {
            string[] priorKeys = dictionary.Keys.ToArray();
            dictionary.Clear();
            foreach (var key in priorKeys)
            {
                InvokeMapChanged(CollectionChange.ItemRemoved, key);
            }
        }

        public ICollection<string> Keys
        {
            get { return dictionary.Keys; }
        }

        public bool ContainsKey(string key)
        {
            return dictionary.ContainsKey(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public ICollection<object> Values
        {
            get { return dictionary.Values; }
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return dictionary.Contains(item);
        }

        public int Count
        {
            get { return dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            int arraySize = array.Length;
            foreach (var pair in dictionary)
            {
                if (arrayIndex >= arraySize) break;
                array[arrayIndex++] = pair;
            }
        }
    }
}
