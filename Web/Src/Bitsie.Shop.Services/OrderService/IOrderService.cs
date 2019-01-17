using Bitsie.Shop.Domain;
using Bitsie.Shop.Domain.Filters;
using System;
using System.Collections.Generic;

namespace Bitsie.Shop.Services
{
    public interface IOrderService
    {
        void SyncOrder(Order order, int blockHeight);
        void SyncAll();
        void CreateOrder(Order transaction, string callbackUrl);
        Order AddOfflineOrder(string address, string txId);
        Order GetOrderByAddress(string address);
        Order GetOrderById(int id);
        void UpdateInternalRecords(Order order);
        int GetNumOrders(int userId, int month, int year);
        Order GetOrderByNumber(string number);
        IList<Order> GetOpenOrders();
        void SyncTransaction(string address, BitcoinTransaction transaction, Order order = null);
        decimal UpdateTransactions(Order order, IList<BitcoinTransaction> transactions);
        void SendPaymentNotification(string recipient, User user, Order order);
        Payout ProcessPayouts(IList<Order> orders);
        IPagedList<Order> GetOrders(OrderFilter filter, int currentPage, int numPerPage);
        bool ValidateOrder(Order transaction, IValidationDictionary validationDictionary);
        OfflineAddress GetOfflineAddress(string address);
        void Save(Order order);
        void SendPartialPaymentNotification(Order order);
    }
}
