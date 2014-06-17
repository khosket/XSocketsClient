using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XSockets.Client40.Helpers
{
    public static class UriExtensions
    {

        /// <summary>
        /// Gets the websocket port to use for the URI.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>Port to use for connection.</returns>
        /// <remarks>
        /// Default port is not set correctly in windows xp (returns -1 as default port for wss URIs), use this helper class instead of <see cref="Uri.Port"/>
        /// </remarks>
        public static int GetWebSocketPort(this Uri uri)
        {
            //check for websocket port
            //default port not set correctly in windows xp (returns -1 as default port for wss URIs)
            int port = 0;
            if (uri.Port > 0)
            {
                port = uri.Port;
            }
            else if (uri.IsWebSocketSecure())
            {
                port = 443;     //default wss port
            }   
            else
            {
                port = 80;      //default ws port
            }                                 

            return port;
        }

        /// <summary>
        /// Check for a WSS (Secure WebSocket) uri.
        /// </summary>
        /// <param name="uri">Uri to validate.</param>
        /// <returns><c>True</c> if uri is WSS, <c>false</c> otherwise.</returns>
        public static bool IsWebSocketSecure(this Uri uri)
        {
            //check for secure connection (via wss)
            bool result = false;
            if (uri.Scheme.ToLower() == "wss")
            {
                result = true;
            }

            return result;
        }
    }
}
