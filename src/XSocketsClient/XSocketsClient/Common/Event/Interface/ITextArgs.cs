using System.Runtime.Serialization;

namespace XSocketsClient.Common.Event.Interface
{
    public interface ITextArgs : IArgs
    {        
        [DataMember(Name = "data", IsRequired = true)]
        string data { get; set; }
        [DataMember(Name = "controller", IsRequired = true)]
        string controller { get; set; }
    }
}