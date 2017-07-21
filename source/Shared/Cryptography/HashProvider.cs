using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Shared.Cryptography
{
    public class HashProvider
    {
        public static byte[] Md5(byte[] data)
        {
            return MD5.Create().ComputeHash(data);
        }

        public static string Sha256(string data)
        {
            return Sha256(Encoding.UTF8.GetBytes(data));
        }

        public static string Sha256(byte[] data)
        {
            byte[] digest = SHA256.Create().ComputeHash(data);
            return BitConverter.ToString(digest).Replace("-", "").ToLower();
        }

        public static string GenerateSalt()
        {
            return Sha256(Path.GetRandomFileName());
        }
    }
}
