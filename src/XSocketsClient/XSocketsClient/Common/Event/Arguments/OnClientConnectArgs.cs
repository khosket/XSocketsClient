using System;
using XSocketsClient.Common.Interfaces;

namespace XSocketsClient.Common.Event.Arguments
{
    public class OnClientConnectArgs : EventArgs
    {
        public OnClientConnectArgs(IXSocketController controller)
        {
            Controller = controller;
        }

        #region Properties

        public IXSocketController Controller { get; private set; }

        #endregion Properties
    }
}