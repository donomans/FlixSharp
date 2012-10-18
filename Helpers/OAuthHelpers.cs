using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FlixSharp.OAuth
{
    internal class OAuthHelpers
    {
        public static String GenerateTimestamp()
        {
            return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString("F0");
        }
        public static String GenerateNonce()
        {
            Int64 nonce = 0;
            Random r = new Random();
            for (Int32 i = 0; i < 15; i++)
                nonce += (nonce << i) * r.Next(0, 9) + i;
            return Math.Abs(nonce).ToString("F0");
        }
        
        public static String Encode(String source)
        {
            List<Char> encoded = new List<Char>(source.Length);
            foreach (Char c in source)
            {
                if (Char.IsLetterOrDigit(c))
                    encoded.Add(c);
                else if (char.IsWhiteSpace(c))
                    encoded.AddRange(new[] { '%', '2', '0' });
                else
                    encoded.AddRange(HttpUtility.UrlEncode(c.ToString()).ToUpper().ToArray());
            }
            return String.Join("", encoded);
        }
        public static String CalculateParameterString(Dictionary<String, String> parameters)
        {
            var q = from entry in parameters
                    let encodedKey = entry.Key
                    let encodedValue = entry.Value
                    let encodedEntry = encodedKey + "=" + encodedValue
                    orderby encodedEntry ascending
                    select encodedEntry;
            return String.Join("&", q.ToArray());
        }
        public static String GetSigningKey(String ConsumerSecret, String TokenSecret = null)
        {
            return ConsumerSecret + "&" + (TokenSecret != null ? TokenSecret : "");
        }
        public static String Sign(String signatureBaseString, String signingKey)
        {
            Byte[] keyBytes = System.Text.Encoding.ASCII.GetBytes(signingKey);
            using (var myhmacsha1 = new System.Security.Cryptography.HMACSHA1(keyBytes))
            {
                Byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(signatureBaseString);
                Byte[] signedValue = myhmacsha1.ComputeHash(byteArray);//stream);
                return Convert.ToBase64String(signedValue);
            }
        }

        public static String GetOAuthRequestUrl(String consumerSecret, String consumerKey, 
            String uri, String httpMethod, String tokenSecret = null,
            Dictionary<String, String> extraParameters = null)
        {
            Dictionary<String, String> parameters = new Dictionary<String, String>();
            parameters.Add("oauth_nonce", GenerateNonce());
            parameters.Add("oauth_consumer_key", consumerKey);
            parameters.Add("oauth_signature_method", "HMAC-SHA1");
            parameters.Add("oauth_timestamp", GenerateTimestamp());
            parameters.Add("oauth_version", "1.0");

            if(extraParameters != null)
                parameters.AddRange(extraParameters);

            ///theBody: sorted list of parameters
            ///  oauth_consumer_key=consumerKey
            ///  oauth_nonce=GenerateNonce()
            ///  oauth_signature_method=HMAC-SHA1
            ///  oauth_timestamp=GenerateTimestamp()
            ///  oauth_version=1.0
            String theBody = CalculateParameterString(parameters); 

            ///theHead: httpMethod + & + HttpUtility.UrlEncode(uri) + &
            String theHead = httpMethod.ToUpper() + "&" + Encode(uri) + "&";
            String sigBase = theHead + Encode(theBody);

            String signingKey = GetSigningKey(consumerSecret, tokenSecret);
            String signature = Sign(sigBase, signingKey);

            return uri + "?" + theBody + "&oauth_signature=" + Encode(signature);
        }

        public static String GetOAuthLoginUrl(String consumerKey, String token, String callback, String loginUrl,
            Dictionary<String, String> extraParameters)
        {
            String url = loginUrl;
            url += "?oauth_token=" + token;
            
            if (extraParameters != null)
                url += "&" + CalculateParameterString(extraParameters);

            url += "&oauth_callback=" + Encode(callback) + "&oauth_consumer_key=" + consumerKey;

            return url;
        }

        public static String GetOAuthAccessUrl(String consumerSecret, String consumerKey, 
            String uri, String token, String tokenSecret)
        {
            Dictionary<String, String> extraParams = new Dictionary<String, String>();
            extraParams.Add("oauth_token", token);

            return GetOAuthRequestUrl(consumerSecret, consumerKey, uri, "GET", tokenSecret, extraParams);
        }
    }

    public static class OAuthExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> values)
        {
            foreach (T v in values)
                if (!collection.Contains(v))
                    collection.Add(v);
        }
    }
}
