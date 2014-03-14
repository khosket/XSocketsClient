using System;
using XSockets.Client40.Common.Event.Interface;
using XSockets.Client40.Model;

namespace XSockets.Client40.Common.Interfaces
{
    public interface ISubscription
    {
        string Event { get; set; }
        bool IsBound { get; set; }
        bool IsPrimitive { get; set; }
        uint Counter { get; set; }
        uint Limit { get; set; }
        bool Confirm { get; set; }
        SubscriptionType SubscriptionType { get; set; }
        Action<ITextArgs> Callback { get; set; }
    }
}