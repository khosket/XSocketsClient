using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using XSockets.Client40.Common.Event.Arguments;
using XSockets.Client40.Common.Event.Interface;
using XSockets.Client40.Common.Interfaces;
using XSockets.Client40.Globals;
using XSockets.Client40.Helpers;
using XSockets.Client40.Model;
using XSockets.Client40.Protocol;
using XSockets.Client40.Protocol.Handshake.Builder;
using XSockets.Client40.Wrapper;

namespace XSockets.Client40
{
    /// <summary>
    /// A client for communicating with XSockets over pub/sub
    /// </summary>
    public partial class XSocketClient : IXSocketClient
    {
        public IXSocketJsonSerializer Serializer { get; set; }

        public IClientInfo ClientInfo { get; set; }

        /// <summary>        
        /// Will fire in two scenarios
        /// - The binding that fires is primitive
        /// - The IsPrimitive property is true when passed into the constructor
        /// </summary>
        public event EventHandler<TextArgs> OnMessage;

        /// <summary>
        /// Fires when a binary message arrives
        /// </summary>
        public event EventHandler<BinaryArgs> OnBlob;

        /// <summary>
        /// Fires when the handshake is done and everything is setup on the server
        /// </summary>
        public event EventHandler OnOpen;

        /// <summary>
        /// Fires if the connection is closed
        /// </summary>
        public event EventHandler OnClose;

        /// <summary>
        /// Fires when an error message is received
        /// </summary>
        public event EventHandler<TextArgs> OnError;

        public event EventHandler<BinaryArgs> OnPing;
        public event EventHandler<BinaryArgs> OnPong;

        /// <summary>
        /// Fires when the socket is open
        /// </summary>
        internal event EventHandler OnSocketOpen;

        private bool IsPrimitive { get; set; }

        /// <summary>
        /// Return true if the socket is connected and the handshake is done
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return this.Socket != null && this.Socket.Socket.Connected && this.IsHandshakeDone;
            }
        }

        /// <summary>
        /// True if the handshake with the server is done
        /// </summary>
        public bool IsHandshakeDone { get; private set; }

        public bool FireOnMessageForUnboundEvents { get; set; }

        public ISocketWrapper Socket { get; private set; }

        public string Url { get; private set; }

        private EndPoint _remoteEndPoint;

        private IXFrameHandler _frameHandler;

        private readonly string _origin;
        private bool _isSecure;
        private readonly X509Certificate2 _certificate;

        public XSocketClient(string url, string origin, bool connect = false, bool isPrimitive = false)
        {
            this.ClientInfo = new ClientInfo();

            this.Serializer = new XSocketJsonSerializer();

            this.Bindings = new List<ISubscription>();

            this.IsPrimitive = isPrimitive;

            this.AddDefaultSubscriptions();

            this.Url = url;
            this._origin = origin;
            if (connect)
                Open();
        }

        public XSocketClient(string url, string origin, X509Certificate2 certificate, bool connect = false, bool isPrimitive = false)
            : this(url, origin, connect, isPrimitive)
        {
            this._isSecure = true;
            this._certificate = certificate;
        }

        private void Error(ITextArgs textArgs)
        {
            if (this.OnError != null)
                this.OnError.Invoke(this, textArgs as TextArgs);
        }

        public void Open()
        {
            if (this.IsConnected) return;

            var connectionstring = GetConnectionstring();

            var handshake = new Rfc6455Handshake(connectionstring, this._origin);

            SetRemoteEndpoint();

            ConnectSocket();

            DoHandshake(handshake);
        }

        private string GetConnectionstring()
        {
            var connectionstring = this.Url;

            if (this.ClientInfo.StorageGuid != Guid.Empty)
                connectionstring += "?" + Constants.Connection.Parameters.XSocketsClientStorageGuid + "=" +
                                    this.ClientInfo.StorageGuid;
            return connectionstring;
        }

        private void SetRemoteEndpoint()
        {
            var url = new Uri(this.Url);

            //check for secure connection (via wss)
            if (url.Scheme.ToLower() == "wss")
            {
                _isSecure = true;
            }

            IPAddress ipAddress;
            if (!IPAddress.TryParse(url.Host, out ipAddress))
            {
                var addr = Dns.GetHostAddresses(url.Host);
                if (addr.Any(p => p.AddressFamily == AddressFamily.InterNetwork))
                    _remoteEndPoint = new IPEndPoint(addr.First(p => p.AddressFamily == AddressFamily.InterNetwork), url.Port);
            }
            else
            {
                _remoteEndPoint = new IPEndPoint(ipAddress, url.Port);
            }
        }

        private void ConnectSocket()
        {
            var socket = new Socket(_remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(_remoteEndPoint);

            if (this._isSecure)
                Socket = new SocketWrapper(socket, this._certificate);
            else
                Socket = new SocketWrapper(socket);
        }

        private void DoHandshake(Rfc6455Handshake handshake)
        {
            var buffer = new byte[1024];
            Socket.Send(Encoding.UTF8.GetBytes(handshake.ToString()), () => Socket.Receive(buffer, r =>
            {
                //check if only a partial handshake received (noticed with/ IIS8 hosts)
                if (r < 2) 
                { 
                    //read the rest of the handshake
                    ReadHandshakeContinue(buffer, r); 
                }
                IsHandshakeDone = true;
                
                Receive();
                
                if (!this.IsPrimitive)
                {
                    BindUnboundBindings();
                }

                if (this.OnSocketOpen != null)
                    this.OnSocketOpen.Invoke(this, null);
            }, err => FireOnClose()),
                        err => FireOnClose());
        }

        private void ReadHandshakeContinue(byte[] buffer, int offset)
        {
            Socket.Receive(buffer, r =>
            {
                int totalOffset = offset + r;
                if (r < 2)
                {
                    ReadHandshakeContinue(buffer, totalOffset);
                }

                return;
            },
            ex => FireOnClose(),
            offset);
        }

        public ITextArgs AsTextArgs(object o, string eventname)
        {
            return new TextArgs { @event = eventname.ToLower(), data = this.Serializer.SerializeToString(o) };
        }

        private void Opened(ITextArgs textArgs)
        {
            this.ClientInfo = this.Serializer.DeserializeFromString<ClientInfo>(textArgs.data);
            FireOnOpen();
        }

        private void ThrowIfPrimitive()
        {
            if (this.IsPrimitive)
            {
                throw new Exception("You can't use binding if having a primitive connection");
            }
        }

        protected virtual void FireOnBlob(IBinaryArgs args)
        {
            if (this.OnBlob != null) this.OnBlob.Invoke(this, args as BinaryArgs);
        }

        protected virtual void FireOnOpen()
        {
            if (this.OnOpen != null) this.OnOpen.Invoke(this, null);
        }


        /// <summary>
        /// Fires the OnClose event
        /// </summary>
        protected virtual void FireOnClose()
        {
            this.Close();
            if (this.OnClose != null) this.OnClose.Invoke(this, null);
        }

        /// <summary>
        /// A message was received
        /// </summary>
        /// <param name="args"></param>
        protected virtual void FireOnMessage(ITextArgs args)
        {
            try
            {
                //Is this a primitive instance (pub/sub not used)
                if (this.IsPrimitive)
                {
                    if (this.OnMessage != null) this.OnMessage.Invoke(this, args as TextArgs);
                    return;
                }

                //Get binding
                var binding = this.GetBindings().FirstOrDefault(b => b.Event.Equals(args.@event) && b.IsBound);
                //If binding is missing and we allow unbound events, fire onmessage
                if (binding == null)
                {
                    if (FireOnMessageForUnboundEvents)
                    {
                        if (this.OnMessage != null) this.OnMessage.Invoke(this, args as TextArgs);
                    }
                    return;
                }

                //The binding is primitive = fire onmessage
                if (binding.IsPrimitive)
                {
                    //fire primitive event onmessage
                    if (this.OnMessage != null) this.OnMessage.Invoke(this, args as TextArgs);
                }
                //Pub/Sub is used, fire the callback with the argument
                else
                {
                    this.FireBoundMethod(binding, args);
                }
            }
            catch (Exception) // Will dispatch to OnError on exception
            {
                if (this.OnError != null) this.OnError.Invoke(this, args as TextArgs);
            }
        }

        private void FireBoundMethod(ISubscription binding, ITextArgs args)
        {
            binding.Callback(args);
            binding.Counter++;
            if (binding.SubscriptionType == SubscriptionType.One)
                this.UnBind(binding.Event);
            else if (binding.SubscriptionType == SubscriptionType.Many && binding.Counter == binding.Limit)
            {
                this.UnBind(binding.Event);
            }
        }

        /// <summary>
        ///     Close the current connection
        /// </summary>
        public void Close()
        {
            var frame = GetDataFrame(FrameType.Close, Encoding.UTF8.GetBytes(""));

            Socket.Send(frame.ToBytes(), () => this.Socket.Dispose(), err => { });

            this.IsHandshakeDone = false;
        }

        public void Ping(byte[] data)
        {
            this.SendControlFrame(FrameType.Ping, data);
        }
        public void Pong(byte[] data)
        {
            this.SendControlFrame(FrameType.Pong, data);
        }
    }
}