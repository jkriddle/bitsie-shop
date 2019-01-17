using NBitcoin;
using NBitcoin.DataEncoders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            BitcoinExtPubKey b58pubkey = Network.Main.CreateBitcoinExtPubKey(ExtPubKey.Parse("xpub6Ezwm36bj3eTV4E5giu3eS1gf6Ji4E69jvS9LKTmLNBudqicqFJ1bcPtxMNZ3bxhrHGL5AaSFufqLEfvWcYS5MjbL3QW5BezTUR5NHx1Nqs"));

            // And try deriving a child
            var customerChildKey = b58pubkey.ExtPubKey.Derive(new KeyPath("1"));
            var customerChildPubKey = Network.Main.CreateBitcoinExtPubKey(customerChildKey);
            Console.WriteLine(customerChildPubKey.ExtPubKey.PubKey.GetAddress(Network.Main));
            Console.WriteLine("YAY!");

            Console.ReadKey();
        }
    }
}
