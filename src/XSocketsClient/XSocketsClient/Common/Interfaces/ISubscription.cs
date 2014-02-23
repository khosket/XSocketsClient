using System;
using XSocketsClient.Common.Event.Interface;
using XSocketsClient.Model;

namespace XSocketsClient.Common.Interfaces
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