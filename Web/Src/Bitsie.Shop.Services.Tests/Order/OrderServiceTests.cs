using System;
using System.Collections.Generic;
using System.Linq;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Domain.Filters;
using Bitsie.Shop.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Bitsie.Shop.Api;

namespace Bitsie.Shop.Services.Tests
{
    [TestClass]
    public class OrderServiceTests
    {

        private IOrderService CreateOrderService(IOrderRepository orderRepository)
        {
            var configService = new Mock<IConfigService>();
            var walletApi = new Mock<IWalletApi>();
            //configService.Setup(c => c.AppSettings("NotificationTemplatePath")).Returns("/fake/path");
            var payoutRepository = new Mock<IPayoutRepository>();
            var transactionRepository = new Mock<ITransactionRepository>();
            var notificationService = new Mock<INotificationService>();
            var offlineAddressRepository = new Mock<IOfflineAddressRepository>();
            var messageApi = new Mock<IMessageApi>();
            var systemService = new Mock<ISystemService>();
            var bitcoinService = new Mock<IBitcoinService>();
            var logService = new Mock<ILogService>();
            var invoiceService = new Mock<IInvoiceService>();
            var invoiceRepository = new Mock<IInvoiceRepository>();
            var userRepository = new Mock<IUserRepository>();
            var subscriptionRepository = new Mock<ISubscriptionRepository>();
            var hdWalletService = new Mock<IHdWalletService>();
            return new OrderService(configService.Object, orderRepository, 
                walletApi.Object, payoutRepository.Object,
                transactionRepository.Object, bitcoinService.Object, 
                invoiceService.Object,
                offlineAddressRepository.Object,
                messageApi.Object,
                notificationService.Object,
                invoiceRepository.Object,
                logService.Object,
                systemService.Object,
                userRepository.Object,
                subscriptionRepository.Object,
                hdWalletService.Object);
        }

        [TestMethod]
        public void Order_Status_Unchanged_If_No_Payment_Sent()
        {
            // Arrange
            var orderRepository = new Mock<IOrderRepository>();
            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Paid,
                Total = 10.00m
            };
            var bitcoinAddress = new BitcoinAddress()
            {
                Address = "1BitcoinAddress",
                Balance = 0,
                Transactions = new List<BitcoinTransaction>()
            };
            var orderService = CreateOrderService(orderRepository.Object);

            // Act
            orderService.SyncAll();

            // Assert
            Assert.AreEqual("foobar", order.Status);
        }

        [TestMethod]
        public void Order_Status_Expired_If_Too_Old()
        {
            // Arrange
            var orderRepository = new Mock<IOrderRepository>();
            var order = new Order
            {
                OrderDate = DateTime.UtcNow.AddMinutes(-16),
                Status = OrderStatus.Paid,
                Total = 10.00m
            };
            var bitcoinAddress = new BitcoinAddress()
            {
                Address = "1BitcoinAddress",
                Balance = 0,
                Transactions = new List<BitcoinTransaction>()
            };
            var orderService = CreateOrderService(orderRepository.Object);

            // Act
            orderService.SyncAll();

            // Assert
            Assert.AreEqual("expired", order.Status);
        }

        [TestMethod]
        public void Order_Status_Marked_Paid_If_Equal_Payment_Sent()
        {
            // Arrange
            var orderRepository = new Mock<IOrderRepository>();
            var order = new Order
            {
                OrderDate = DateTime.UtcNow.AddMinutes(-1),
                Status = OrderStatus.Pending,
                Total = 10.00m,
                BtcTotal = 0.0001m
            };
            var bitcoinAddress = new BitcoinAddress()
            {
                Address = "1BitcoinAddress",
                Balance = 0.0001m,
                Transactions = new List<BitcoinTransaction>() {
                    new BitcoinTransaction {
                        TransactionDate = DateTime.UtcNow,
                        TxId = "TX1",
                        Value = 0.0001m
                    }
                }
            };
            var orderService = CreateOrderService(orderRepository.Object);

            // Act
            orderService.SyncAll();

            // Assert
            Assert.AreEqual("paid", order.Status);
        }

        [TestMethod]
        public void Order_Status_Unchanged_If_Equal_Payment_Sent_Before_Order_Placed()
        {
            // Arrange
            var orderRepository = new Mock<IOrderRepository>();
            var order = new Order
            {
                OrderDate = DateTime.UtcNow.AddMinutes(-1),
                Status = OrderStatus.Paid,
                Total = 10.00m,
                BtcTotal = 0.0001m
            };
            var bitcoinAddress = new BitcoinAddress()
            {
                Address = "1BitcoinAddress",
                Balance = 0.0001m,
                Transactions = new List<BitcoinTransaction>() {
                    new BitcoinTransaction {
                        TransactionDate = DateTime.UtcNow.AddMinutes(-2),
                        TxId = "TX1",
                        Value = 0.0001m
                    }
                }
            };
            var orderService = CreateOrderService(orderRepository.Object);

            // Act
            orderService.SyncAll();

            // Assert
            Assert.AreEqual("foobar", order.Status);
        }

        [TestMethod]
        public void Order_Status_Marked_Partial_If_Partial_Payment_Sent()
        {
            // Arrange
            var orderRepository = new Mock<IOrderRepository>();
            var order = new Order
            {
                OrderDate = DateTime.UtcNow.AddMinutes(-1),
                Status = OrderStatus.Pending,
                Total = 10.00m,
                BtcTotal = 0.0001m
            };
            var bitcoinAddress = new BitcoinAddress()
            {
                Address = "1BitcoinAddress",
                Balance = 0.0001m,
                Transactions = new List<BitcoinTransaction>() {
                    new BitcoinTransaction {
                        TransactionDate = DateTime.UtcNow,
                        TxId = "TX1",
                        Value = 0.00005m
                    }
                }
            };
            var orderService = CreateOrderService(orderRepository.Object);

            // Act
            orderService.SyncAll();

            // Assert
            Assert.AreEqual("partial", order.Status);
        }

    }
}
