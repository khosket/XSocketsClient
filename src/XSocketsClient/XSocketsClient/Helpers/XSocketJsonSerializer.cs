using System;
using Newtonsoft.Json;
using XSocketsClient.Common.Interfaces;

namespace XSocketsClient.Helpers
{
    public class XSocketJsonSerializer : IXSocketJsonSerializer
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public XSocketJsonSerializer()
        {
            this._jsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        }

        public string SerializeToString<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, this._jsonSerializerSettings);
        }

        public string SerializeToString(object obj, Type type)
        {
            return JsonConvert.SerializeObject(obj, type, Formatting.None, this._jsonSerializerSettings);
        }

        public T DeserializeFromString<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public object DeserializeFromString(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }
    }
}
