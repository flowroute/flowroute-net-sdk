using System.Security.Cryptography;
using System.Text;

namespace Flowroute.PhoneNumbers
{
    public class PhoneNumbersMessageString
    {
        private string _body;
        private string _bodyHash;
        public string Timestamp { get; set; }

        public string HttpMethod { get; set; }

        public string Body
        {
            get { return _body; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _body = string.Empty;
                    return;
                }

                _body = value;

                var md5 = MD5.Create();
                var bodyAsBytes = Encoding.UTF8.GetBytes(value);
                var computedAsBytes = md5.ComputeHash(bodyAsBytes);
                _bodyHash = HexStringFromBytes(computedAsBytes);
            }
        }

        public string Canonical { get; set; }

        public override string ToString()
        {
            var formattableString = $"{Timestamp}\n{HttpMethod}\n{_bodyHash}\n{Canonical}";
            return formattableString;
        }

        public static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }
    }
}