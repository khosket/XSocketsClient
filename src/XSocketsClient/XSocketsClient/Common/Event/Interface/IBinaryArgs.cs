using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XSocketsClient.Common.Event.Interface
{
    public interface IBinaryArgs : IArgs
    {
        [DataMember(Name = "data", IsRequired = true)]
        IList<byte> data { get; set; }
    }
}