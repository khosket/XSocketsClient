using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XSockets.Client40.Common.Event.Interface
{
    public interface IBinaryArgs : IArgs
    {
        [DataMember(Name = "data", IsRequired = true)]
        IList<byte> data { get; set; }
    }
}