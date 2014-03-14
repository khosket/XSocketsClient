using System;
using XSockets.Client40.Common.Event.Interface;
using XSockets.Client40.Common.Interfaces;

namespace XSockets.Client40.Model
{
    internal class Subscription : ISubscription
    {
        public Subscription(string eventName, SubscriptionType subscriptionType = SubscriptionType.All, uint limit = 0, bool confirm = false)
        {
            this.Event = eventName.ToLower();            
            this.SubscriptionType = subscriptionType;
            this.Counter = 0;
            this.Limit = limit;
            this.IsPrimitive = true;
            this.Confirm = confirm;
        }
        public Subscription(string eventName, Action<ITextArgs> action, SubscriptionType subscriptionType = SubscriptionType.All, uint limit = 0, bool confirm = false)
        {
            this.Event = eventName.ToLower();
            this.Callback = action;
            this.SubscriptionType = subscriptionType;
            this.Counter = 0;
            this.Limit = limit;
            this.Confirm = confirm;
        }

        public string Event { get; set; }
        public bool IsPrimitive { get; set; }
        public bool IsBound { get; set; }
        public bool Confirm { get; set; }
        public uint Counter { get; set; }
        public uint Limit { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
        public Action<ITextArgs> Callback { get; set; }
    }
}