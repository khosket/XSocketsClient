using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XSocketsClient.Common.Event.Interface;
using XSocketsClient.Common.Interfaces;
using XSocketsClient.Globals;
using XSocketsClient.Model;

namespace XSocketsClient
{
    /// <summary>
    /// A client for communicating with XSockets over pub/sub
    /// </summary>
    public partial class XSocketClient
    {
        private IList<ISubscription> Bindings { get; set; }

        private void AddDefaultSubscriptions()
        {
            var onError = new Subscription(Constants.Events.OnError, Error) { IsBound = true };
            this.AddBinding(onError);
            var onOpen = new Subscription(Constants.Events.Connections.Opened, Opened) { IsBound = true };
            this.AddBinding(onOpen);
        }

        public IList<ISubscription> GetBindings()
        {
            lock (this.Bindings)
            {            
                return this.Bindings.ToList();
            }
        }
        private void AddBinding(ISubscription subscription)
        {
            lock (this.Bindings)
            {
                this.Bindings.Add(subscription);
            }
        }

        private void RemoveBinding(ISubscription subscription)
        {
            lock (this.Bindings)
            {
                this.Bindings.Remove(subscription);
            }
        }

        private void BindUnboundBindings()
        {
            var unboundBindings = this.GetBindings().Where(p => p.IsBound == false).ToList();

            if (!unboundBindings.Any()) return;
            foreach (var unboundBinding in unboundBindings)
            {
                var binding = unboundBinding;
                new Task(
                    () =>
                    Send(this.AsTextArgs(new XSubscriptions {Event = binding.Event.ToLower(), Confirm = binding.Confirm}
                                         , Constants.Events.PubSub.Subscribe), () =>
                                             {
                                                 var b = this.GetBindings().Single(p => p.Event == binding.Event);
                                                 b.IsBound = true;
                                             })).RunSynchronously();
            }
        }

        public void Bind(string name)
        {
            this.Subscribe(name, SubscriptionType.All);          
        }
              
        public void Bind(string name, Action<ITextArgs> callback)
        {
            this.Subscribe(name, callback, SubscriptionType.All);         
        }

        public void Bind(string name, Action<ITextArgs> callback, Action<ITextArgs> confirmCallback)
        {
            this.Subscribe(name, callback, confirmCallback, SubscriptionType.All);
        }

        public void One(string name, Action<ITextArgs> callback)
        {
            this.Subscribe(name, callback, SubscriptionType.One);    
        }

        public void One(string name, Action<ITextArgs> callback, Action<ITextArgs> confirmCallback)
        {
            this.Subscribe(name, callback, confirmCallback, SubscriptionType.One);
        }

        public void Many(string name, uint limit, Action<ITextArgs> callback)
        {
            this.Subscribe(name, callback, SubscriptionType.Many,limit);
        }

        public void Many(string name, uint limit, Action<ITextArgs> callback, Action<ITextArgs> confirmCallback)
        {
            this.Subscribe(name,callback,confirmCallback, SubscriptionType.Many, limit);
        }

        private void AddConfirmCallback(Action<ITextArgs> confirmCallback, string @event)
        {
            var e = string.Format("__{0}", @event);
            if (this.GetBindings().Any(p => p.Event == e)) return;

            var confirm = new Subscription(e, confirmCallback);
            this.AddBinding(confirm);
            confirm.IsBound = this.IsConnected;
        }

        /// <summary>
        /// Remove the subscription from the list
        /// </summary>
        /// <param name="name"></param>
        public void UnBind(string name)
        {
            ISubscription subscription = this.GetBindings().FirstOrDefault(b => b.Event.Equals(name.ToLower()));
            if (subscription == null) return;

            if (this.IsConnected)
            {
                //Unbind on server
                Send(this.AsTextArgs(new XSubscriptions {Event = name.ToLower()}, Constants.Events.PubSub.Unsubscribe));
            }

            this.RemoveBinding(subscription);
        }

        private void Subscribe(string name, SubscriptionType subscriptionType, uint limit = 0)
        {
            ThrowIfPrimitive();
            var subscription = new Subscription(name, subscriptionType, limit);
            this.AddBinding(subscription);

            if (this.IsConnected)
            {
                Send(this.AsTextArgs(new XSubscriptions
                {
                    Event = name.ToLower(),
                    Confirm = true
                }, Constants.Events.PubSub.Subscribe), () => { subscription.IsBound = true; });
            }
        }
        private void Subscribe(string name, Action<ITextArgs> callback, SubscriptionType subscriptionType, uint limit = 0)
        {
            ThrowIfPrimitive();
            var subscription = new Subscription(name, callback, subscriptionType, limit);
            this.AddBinding(subscription);

            if (this.IsConnected)
            {                
                Send(this.AsTextArgs(new XSubscriptions
                {
                    Event = name.ToLower(),
                    Confirm = true
                }, Constants.Events.PubSub.Subscribe), () => { subscription.IsBound = true; });
            }
        }
        private void Subscribe(string name, Action<ITextArgs> callback, Action<ITextArgs> confirmCallback, SubscriptionType subscriptionType, uint limit = 0)
        {
            ThrowIfPrimitive();
            var subscription = new Subscription(name.ToLower(), callback, subscriptionType, limit, true);
            this.AddBinding(subscription);
            AddConfirmCallback(confirmCallback, subscription.Event);
            if (this.IsConnected)
            {                
                Send(this.AsTextArgs(new XSubscriptions { Event = name.ToLower() }, Constants.Events.PubSub.Subscribe), () => subscription.IsBound = true);
            }
        }        
        //Sending methods

        /// <summary>
        ///     Send message
        /// </summary>
        /// <param name="payload">ITextArgs</param>
        /// <param name="callback"> </param>
        public void Send(ITextArgs payload, Action callback)
        {
            if (!this.IsConnected)
                throw new Exception("You cant send messages when not conencted to the server");
            var frame = GetDataFrame(payload);
            Socket.Send(frame.ToBytes(), callback.Invoke, err => FireOnClose());
        }

        /// <summary>
        ///     Send a binary message)
        /// </summary>
        /// <param name="payload">IBinaryArgs</param>
        public void Send(IBinaryArgs payload)
        {
            if (!this.IsConnected)
                throw new Exception("You cant send messages when not conencted to the server");
            var frame = GetDataFrame(payload);
            Socket.Send(frame.ToBytes(), () => { }, ex => { });
        }

        public void Send(string payload)
        {
            if (!this.IsConnected)
                throw new Exception("You cant send messages when not conencted to the server");
            var frame = GetDataFrame(payload);
            Socket.Send(frame.ToBytes(), () => { }, err => FireOnClose());
        }

        public void Send(string payload, Action callback)
        {
            if (!this.IsConnected)
                throw new Exception("You cant send messages when not conencted to the server");
            var frame = GetDataFrame(payload);
            Socket.Send(frame.ToBytes(), callback.Invoke, err => FireOnClose());
        }

        public void Send(ITextArgs payload)
        {
            if (!this.IsConnected)
                throw new Exception("You cant send messages when not conencted to the server");
            var frame = GetDataFrame(payload);
            Socket.Send(frame.ToBytes(), () => { }, err => FireOnClose());
        }

        public void Send(object obj, string @event)
        {
            this.Send(this.AsTextArgs(obj, @event));
        }

        public void Send(object obj, string @event, Action callback)
        {
            this.Send(this.AsTextArgs(obj, @event), callback);
        }

        public void Trigger(ITextArgs payload)
        {
            this.Send(payload);
        }

        public void Trigger(ITextArgs payload, Action callback)
        {
            this.Send(payload, callback);
        }

        public void Trigger(object obj, string @event)
        {
            this.Send(this.AsTextArgs(obj,@event));
        }

        public void Trigger(object obj, string @event, Action callback)
        {
            this.Send(this.AsTextArgs(obj,@event), callback);
        }

        public void Trigger(IBinaryArgs payload)
        {
            this.Send(payload);
        }
    }
}