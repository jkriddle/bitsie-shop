using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Domain
{
    public static class GuidHelper
    {
        private static GuidGenerator _instance;
        private static GuidGenerator Instance
        {
            get
            {
                if (_instance == null) _instance = new GuidGenerator();
                return _instance;
            }
        }

        public static string Create(int size) {
            return Instance.Create(size);
        }

        private class GuidGenerator {
            private readonly Random _rand;

            public GuidGenerator() {
                _rand = new Random((int)DateTime.Now.Ticks);
            }

            /// <summary>
            /// Generate a new random string
            /// </summary>
            /// <param name="size"></param>
            /// <returns></returns>
            public string Create(int size)
            {
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                return new string(
                    Enumerable.Repeat(chars, size)
                              .Select(s => s[_rand.Next(s.Length)])
                              .ToArray());
            }
        }

        
    }
}
