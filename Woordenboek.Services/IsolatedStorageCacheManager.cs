using System;
using System.Net;
using System.Runtime.Serialization;
using System.IO;
using Windows.Storage;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace mpost.WP7.Client.Storage
{
    public static class IsolatedStorageCacheManager
    {
        public static async Task SaveData(string filename, object objectGraph, bool overwriteIfNull = true)
        {
            string json = null;
            if (objectGraph != null)
                json = SerializeObjectGraph(objectGraph);
            if (json != null || overwriteIfNull)
            {
                await WriteFile(filename, json);
            }
        }

        private static string SerializeObjectGraph(object graph)
        {
            if (graph == null) return null;
            DataContractJsonSerializer ser = new DataContractJsonSerializer(graph.GetType());
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, graph);
            var bytes = ms.ToArray();
            return UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        public static async Task<T> LoadData<T>(string filename)
        {
            var json = await ReadFile(filename);
            MemoryStream ms = new MemoryStream(UTF8Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            T result = (T)ser.ReadObject(ms);
            return result;
        }

        public static async Task<string> ReadFile(string filename)
        {
            var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var folder = localFolder;
            var file = await folder.GetFileAsync(filename);
            var fs = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            var inStream = fs.GetInputStreamAt(0);
            Windows.Storage.Streams.DataReader reader = new Windows.Storage.Streams.DataReader(inStream);
            await reader.LoadAsync((uint)fs.Size);
            string data = reader.ReadString((uint)fs.Size);
            reader.DetachStream();
            return data;
        }

        public static async Task WriteFile(string filename, string contents)
        {
            var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var folder = localFolder;
            var file = await folder.CreateFileAsync(filename, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            var fs = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
            var outStream = fs.GetOutputStreamAt(0);
            var dataWriter = new Windows.Storage.Streams.DataWriter(outStream);
            dataWriter.WriteString(contents);
            await dataWriter.StoreAsync();
            dataWriter.DetachStream();
            await outStream.FlushAsync();
            dataWriter.Dispose();
            outStream.Dispose();
            fs.Dispose();
        }

    }
}
