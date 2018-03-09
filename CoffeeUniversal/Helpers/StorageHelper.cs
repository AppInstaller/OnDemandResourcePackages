using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;

namespace CoffeeUniversal.Helpers
{
	public sealed class StorageHelper
    {
        private static Mutex StorageMutex = new Mutex(false, "CoffeeStorageMutex");
        private static string storageFile = "CoffeeData.xml";

        private StorageHelper() { }

        public async static void SaveToStorage<T>(T data)
        {
            try
            {
                StorageMutex.WaitOne(TimeSpan.FromMilliseconds(1000));

                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile file = await localFolder.CreateFileAsync(storageFile, CreationCollisionOption.OpenIfExists);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                StringWriter textWriter = new StringWriter();
                xmlSerializer.Serialize(textWriter, data);
                await FileIO.WriteTextAsync(file, textWriter.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SaveToStorage: " +ex.ToString());
            }
            finally
            {
                StorageMutex.ReleaseMutex();
            }
        }

        public async static Task<T> ReadFromStorage<T>()
        {
            T data = default(T);
            StringReader textReader = null;

            try
            {
                StorageMutex.WaitOne(TimeSpan.FromMilliseconds(1000));

                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile file = await localFolder.GetFileAsync(storageFile);
                if (file != null)
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    string textData = await FileIO.ReadTextAsync(file);
                    textReader = new StringReader(textData);
                    data = (T)xmlSerializer.Deserialize(textReader);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ReadFromStorage: " +ex.ToString());
            }
            finally
            {
                if (textReader != null)
                {
                    textReader.Dispose();
                }
                StorageMutex.ReleaseMutex();
            }
            return data;
        }

    }
}
