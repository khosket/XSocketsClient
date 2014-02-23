using System.Collections.Generic;

namespace XSocketsClient.Protocol
{
    public interface IReadState
    {
        List<byte> Data { get; }
        FrameType? FrameType { get; set; }
        void Clear();
    }
}