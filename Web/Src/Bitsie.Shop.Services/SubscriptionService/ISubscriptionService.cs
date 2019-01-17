using Bitsie.Shop.Domain;
using Bitsie.Shop.Domain.Filters;
using System;
using System.Collections.Generic;
namespace Bitsie.Shop.Services
{
    public interface ISubscriptionService
    {
        Subscription GetSubscriptionById(int id);
        void Save(Subscription subscription);
        void SendSubscriptionReminders(DateTime expirationDate);
        void UpdateSubscriptionStatuses();
        void UnsuspendAccounts();
    }
}
