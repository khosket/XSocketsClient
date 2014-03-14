using System.Runtime.Serialization;

namespace XSockets.Client40.Common.Event.Interface
{
    public interface IArgs
    {
        [DataMember(Name = "event", IsRequired = true)]
        string @event { get; set; }
    }
}
