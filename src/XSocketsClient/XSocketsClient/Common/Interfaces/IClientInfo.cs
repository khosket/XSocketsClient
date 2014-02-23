using System;

namespace XSocketsClient.Common.Interfaces
{
    public interface IClientInfo
    {
        Guid ClientGuid { get; set; }
        Guid StorageGuid { get; set; }
    }
}