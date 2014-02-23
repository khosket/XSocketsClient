using System;

namespace XSocketsClient.Common.Event.Arguments
{
    public class OnErrorArgs : EventArgs
    {
        public OnErrorArgs(string message)
        {
            Message = message;
            Exception = null;
        }

        public OnErrorArgs(Exception ex, string message)
        {
            Message = message;
            Exception = ex;
        }

        public OnErrorArgs(Exception ex)
        {
            Exception = ex;
        }

        #region Properties

        public string Message { get; private set; }
        public Exception Exception { get; private set; }

        #endregion Properties
    }
}