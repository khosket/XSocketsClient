using System;
using XSocketsClient.Common.Interfaces;

namespace XSocketsClient.Model
{
    public class ClientInfo : IClientInfo
    {
        public Guid ClientGuid { get; set; }
        public Guid StorageGuid { get; set; }
    }
}