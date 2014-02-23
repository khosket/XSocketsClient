using System.Collections.Generic;

namespace XSocketsClient.Protocol
{
    public interface IXFrameHandler
    {
        void Receive(IEnumerable<byte> data);
    }
}