using System;
using XSockets.Client40.Common.Interfaces;

namespace XSockets.Client40.Common.Event.Arguments
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