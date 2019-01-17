using Bitsie.Shop.Domain;
using Bitsie.Shop.Domain.Filters;
using Bitsie.Shop.Infrastructure;
using System.Linq;
using System;
namespace Bitsie.Shop.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;

        public SubscriptionService(ISubscriptionRepository subscriptionRepository,
            INotificationService notificationService,
            IUserRepository userRepository)
        {
            _subscriptionRepository = subscriptionRepository;
            _notificationService = notificationService;
            _userRepository = userRepository;
        }

        public Subscription GetSubscriptionById(int id)
        {
            return _subscriptionRepository.FindOne(id);
        }

        public void Save(Subscription subscription)
        {
            _subscriptionRepository.Save(subscription);
        }

        public void SendSubscriptionReminders(DateTime expirationDate)
        {
            int totalRecords = 0;
            var subscriptions = _subscriptionRepository.Search(new SubscriptionFilter
            {
                ExpirationDate = expirationDate,
                Status = SubscriptionStatus.Active
            }, 1, 10000, out totalRecords);

            int days = (expirationDate.Date - DateTime.Now.Date).Days;

            string expireDays = "in " + days + " days";
            if (days == 1) expireDays = "tomorrow";
            if (days == 0) expireDays = "today";

            foreach (var subscription in subscriptions)
            {
                _notificationService.Notify(subscription.User.Email, "Your Bitsie Shop subscription has expired", "SubscriptionExpiring", new
                {
                    Days = expireDays,
                    Plan = subscription.Type
                });
            }
        }

        public void UpdateSubscriptionStatuses()
        {
            int totalRecords = 0;

            // Expire subscription
            var subscriptions = _subscriptionRepository.Search(new SubscriptionFilter
            {
                ExpiresBefore = DateTime.UtcNow,
                Status = SubscriptionStatus.Active
            }, 1, 10000, out totalRecords);

            foreach (var subscription in subscriptions)
            {
                subscription.Status = SubscriptionStatus.Expired;
                _subscriptionRepository.Save(subscription);
                _notificationService.Notify(subscription.User.Email, "Your Bitsie Shop subscription has expired", "SubscriptionExpired", new
                {
                    Plan = subscription.Type
                });
            }
        }

        /// <summary>
        /// Remove suspension from user accounts.
        /// </summary>
        public void UnsuspendAccounts()
        {
            var users = _userRepository.FindAll().Where(u => u.Status == UserStatus.Suspended);
            foreach (var u in users)
            {
                u.Status = UserStatus.Active;
                _userRepository.Save(u);
            }
        }
    }
}
