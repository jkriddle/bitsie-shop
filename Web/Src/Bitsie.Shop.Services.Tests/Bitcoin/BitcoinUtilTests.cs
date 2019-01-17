using Bitsie.Shop.Domain.Bitcoin;
using Bitsie.Shop.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bitsie.Shop.Services.Tests
{
    [TestClass]
    public class BitcoinUtilTests
    {
        [TestMethod]
        public void Can_Decrypt_Address()
        {
            // Arrange
            string privKey = "5JGmpBFDxNmeQZ1ZgXYNNW6nBAbhe7M2DVfuLmUrJc6VryijNgN";
            string address = "1E39B2avrNNzucrR8cqm2QHc34kdpFvsKX";

            // Act
            var result = new KeyPair(privKey);

            // Assert
            Assert.AreEqual(result.AddressBase58, address);
        }
    }
}
