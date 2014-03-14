using System;

namespace XSockets.Client40.Common.Interfaces
{
    public interface IClientInfo
    {
        Guid ClientGuid { get; set; }
        Guid StorageGuid { get; set; }
    }
}