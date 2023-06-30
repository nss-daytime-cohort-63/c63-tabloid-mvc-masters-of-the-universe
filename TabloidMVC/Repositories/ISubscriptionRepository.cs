using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface ISubscriptionRepository
    {
        void Add(Subscription subscription);
        Subscription GetActiveSubByAuthAndSubscriber(int authorId, int subscriberId);
        List<Subscription> GetActiveSubscriptionsBySubscriberId(int subscriberId);
        void AddEndDate(int subscriptionId);
    }
}
