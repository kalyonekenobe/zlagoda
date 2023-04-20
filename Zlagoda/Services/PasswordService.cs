using System.Security.Cryptography;
using System.Text;

namespace Zlagoda.Services
{
    public class PasswordService
    {
        private readonly string _prefixSalt = "d2hpY2ggaW52b2x2ZXMgdGhlIGNhbGN1bGF0aW9uIG9mIGEgZGl2aXNvciB0aGF0IGlzIGNvbW1vbiB0byBib3RoIGNvbXBvbmVudHM=";
        private readonly string _suffixSalt = "IEZpbmFsbHksIGl0IHVwZGF0ZXMgdGhlIHJlYWwgYW5kIGltYWdpbmFyeSBjb21wb25lbnRzIG9mIGEgd2l0aCB0aGUgbmV3IHZhbHV=";

        public string Encrypt(string password)
        {
            string saltedPassword = $"{_prefixSalt}.{password}.{_suffixSalt}";
            using (var hash = SHA256.Create())
            {
                byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        public bool ComparePasswords(string passwordA, string passwordB)
        {
            return passwordA == passwordB;
        }
    }
}
