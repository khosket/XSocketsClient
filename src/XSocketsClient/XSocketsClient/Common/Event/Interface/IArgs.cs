using System.Runtime.Serialization;

namespace XSocketsClient.Common.Event.Interface
{
    public interface IArgs
    {
        [DataMember(Name = "event", IsRequired = true)]
        string @event { get; set; }
    }
}
