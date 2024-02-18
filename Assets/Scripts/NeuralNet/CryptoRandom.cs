using System;
using System.Security.Cryptography;

namespace NeuralNet
{
    public class CryptoRandom
    {
        public double RandomValue { get; set; }

        public CryptoRandom()
        {
            using (RNGCryptoServiceProvider p = new RNGCryptoServiceProvider())
            {
                Random r = new Random(p.GetHashCode());
                this.RandomValue = r.NextDouble();
            }
        }

        public int NextInt(int max)
        {
            return (int)(RandomValue * max);
        }

    }
}
