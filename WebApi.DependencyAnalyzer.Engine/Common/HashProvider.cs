using System;
using System.Security.Cryptography;
using System.Text;

namespace WebApi.DependencyAnalyzer.Engine.Common
{
    public class HashProvider : IHashProvider<string>
    {
        public long GetHash(string input)
        {
            long hashCode = default(long);

            if (!string.IsNullOrEmpty(input))
            {
                byte[] byteContents = Encoding.Unicode.GetBytes(input);

                SHA256 hash = SHA256.Create();

                byte[] hashText = hash.ComputeHash(byteContents);

                long hashCodeStart = BitConverter.ToInt64(hashText, 0);
                long hashCodeMedium = BitConverter.ToInt64(hashText, 8);
                long hashCodeEnd = BitConverter.ToInt64(hashText, 24);

                hashCode = hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;
            }

            return (hashCode);
        }
    }
}