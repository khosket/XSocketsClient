using System;
using XSockets.Client40.Common.Interfaces;

namespace XSockets.Client40.Model
{
    public class ClientInfo : IClientInfo
    {
        public Guid ClientGuid { get; set; }
        public Guid StorageGuid { get; set; }
    }
}