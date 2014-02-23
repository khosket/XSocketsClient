using System;
using System.Collections.Generic;
using XSocketsClient.Common.Event.Arguments;
using XSocketsClient.Common.Event.Interface;

namespace XSocketsClient.Common.Interfaces
{
    public interface IXSocketController
    {
        int BufferSize { get; set; }
        string Alias { get; set; }
        //PluginRange PluginRange { get; set; }         
        ISocketWrapper Socket { get; set; }
        bool CorrectController { get; set; }
        bool IsChannel { get; set; }
        //IDictionary<string, ISet<IPluginCustomEventInfo>> CustomEvents { get; }
        //IXSocketProtocol ProtocolInstance { get; set; }
        //Guid ApiKey { get; set; }
        IXSocketJsonSerializer JsonSerializer { get; set; }        
        Guid ClientGuid { get; set; }
        Guid StorageGuid { get; set; }


        event EventHandler<OnClientConnectArgs> OnOpen;
        event EventHandler<OnClientDisconnectArgs> OnClose;
        event EventHandler<OnErrorArgs> OnError;
        
        IXSocketController NewInstance();

        void OnMessage(ITextArgs textArgs);
        void OnMessage(IBinaryArgs binaryArgs);

        bool Available();

        void Close();
        void Open();
        
        void Send(byte[] data);

        bool SubscribesTo(string @event);

        IList<string> Subscriptions { get; set; }
    }
}