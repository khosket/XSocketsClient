using System;
using ServiceStack.Text;
using XSockets.Client40.Common.Interfaces;

namespace XSockets.Client40.Helpers
{
    public class XSocketJsonSerializer : IXSocketJsonSerializer
    {
        public XSocketJsonSerializer()
        {
            JsConfig.ExcludeTypeInfo = true;
            JsConfig.IncludeTypeInfo = false;
        }

        public string SerializeToString<T>(T obj)
        {
            return JsonSerializer.SerializeToString(obj);
        }

        public string SerializeToString(object obj, Type type)
        {
            return JsonSerializer.SerializeToString(obj, type);
        }

        public T DeserializeFromString<T>(string json)
        {
            return JsonSerializer.DeserializeFromString<T>(json);
        }

        public object DeserializeFromString(string json, Type type)
        {
            return JsonSerializer.DeserializeFromString(json, type);
        }
    }
    //public class XSocketJsonSerializer : IXSocketJsonSerializer
    //{
    //    private readonly JsonSerializerSettings _jsonSerializerSettings;

    //    public XSocketJsonSerializer()
    //    {
    //        this._jsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
    //    }

    //    public string SerializeToString<T>(T obj)
    //    {
    //        return JsonConvert.SerializeObject(obj, this._jsonSerializerSettings);
    //    }

    //    public string SerializeToString(object obj, Type type)
    //    {
    //        return JsonConvert.SerializeObject(obj, type, Formatting.None, this._jsonSerializerSettings);
    //    }

    //    public T DeserializeFromString<T>(string json)
    //    {
    //        return JsonConvert.DeserializeObject<T>(json);
    //    }

    //    public object DeserializeFromString(string json, Type type)
    //    {
    //        return JsonConvert.DeserializeObject(json, type);
    //    }
    //}
}
