using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyBanGiay.Helpers
{
    public class VnPayLibrary
    {
        //Phiên bản API VNPAY
        public static string VERSION = "2.1.0";
        //Lưu trữ các tham số gửi đi
        private SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
        //Lưu trữ các tham số nhận về
        private SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        public string GetResponseData(string key)
        {
            return _responseData.ContainsKey(key) ? _responseData[key] : string.Empty;
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _responseData.Add(key, value);
            }
        }

        public string CreateRequestUrl(string baseUrl, string hashSecret)
        {
            var data = new StringBuilder();

            foreach (KeyValuePair<string, string> kv in _requestData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }

            string rawData = data.ToString();

            if (string.IsNullOrEmpty(rawData))
                return string.Empty;

            string rawDataForHash = rawData.Remove(rawData.Length - 1, 1);
            string vnpSecureHash = HmacSHA512(hashSecret, rawDataForHash);
            return baseUrl + "?" + rawDataForHash + "&vnp_SecureHash=" + vnpSecureHash;
        }

        public bool ValidateSignature(string inputHash, string hashSecret)
        {
            string rspRaw = GetHashData();
            string myHash = HmacSHA512(hashSecret, rspRaw);
            return myHash.Equals(inputHash, System.StringComparison.OrdinalIgnoreCase);
        }

        private string GetHashData()
        {
            var data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in _responseData)
            {
                if (!string.IsNullOrEmpty(kv.Value) && kv.Key != "vnp_SecureHash" && kv.Key != "vnp_SecureHashType")
                {
                    data.Append(kv.Key + "=" + kv.Value + "&");
                }
            }
            string rawData = data.ToString();

            if (rawData.Length > 0)
            {
                return rawData.Remove(rawData.Length - 1, 1);
            }
            return rawData;
        }

        public static string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(inputBytes);
                foreach (var b in hashBytes)
                {
                    hash.Append(b.ToString("X2"));
                }
            }
            return hash.ToString().ToLower();
        }
    }

    public class VnPayCompare : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            var vnpayCompare = System.Globalization.CompareInfo.GetCompareInfo("en-US");
            return vnpayCompare.Compare(x, y, System.Globalization.CompareOptions.Ordinal);
        }
    }
}
