using System;
using XSocketsClient.Common.Interfaces;

namespace XSocketsClient.Common.Event.Arguments
{
    public class OnHandshakeCompleteArgs : EventArgs
    {
        public OnHandshakeCompleteArgs(IXSocketController controller, string handshake)
        {
            Controller = controller;
            Handshake = handshake;
        }

        #region Properties

        public IXSocketController Controller { get; private set; }
        public string Handshake { get; private set; }

        #endregion Properties
    }
}