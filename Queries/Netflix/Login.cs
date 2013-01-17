using FlixSharp.Constants.Netflix;
using FlixSharp.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp.Queries.Netflix
{
    public class NetflixLogin
    {
        public static String ConsumerKey { get { return consumerkey; } }
        private static String consumerkey;
        public static String SharedSecret { get { return sharedsecret; } }
        private static String sharedsecret;
        public static String ApplicationName { get { return applicationname; } }
        private static String applicationname;

        public static Boolean InformationSet { get { return informationset; } }
        private static Boolean informationset = false;

        /// <summary>
        /// Set your Netflix API information before using any api queries
        /// </summary>
        /// <param name="ConsumerKey"></param>
        /// <param name="SharedSecret"></param>
        /// <param name="ApplicationName">[Optional] If you are not sure what Netflix 
        /// thinks your application name is, you may set it upon retrieving it from 
        /// the result of the GetLoginUrl request - your callback will receive it as a parameter</param>
        public NetflixLogin SetCredentials(String ConsumerKey, String SharedSecret, String ApplicationName = "")
        {
            consumerkey = ConsumerKey;
            sharedsecret = SharedSecret;
            applicationname = ApplicationName;

            informationset = true;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ApplicationName">The Application Name as given by Netflix from the API</param>
        public void SetApplicationName(String ApplicationName)
        {
            applicationname = ApplicationName;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public String GetRequestUrl()
        {
            CheckInformationSet();
            return OAuthHelpers.GetOAuthRequestUrl(SharedSecret, ConsumerKey, NetflixConstants.RequestUrl, "GET");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public String GetLoginUrl(String Token, String Callback)
        {
            CheckInformationSet();
            Dictionary<String, String> extraParams = new Dictionary<String, String>();
            extraParams.Add("application_name", ApplicationName);
            return OAuthHelpers.GetOAuthLoginUrl(ConsumerKey, Token, Callback, NetflixConstants.LoginUrl, extraParams);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenSecret"></param>
        /// <returns></returns>
        public String GetAccessUrl(String Token, String TokenSecret)
        {
            CheckInformationSet();
            return OAuthHelpers.GetOAuthAccessUrl(SharedSecret, ConsumerKey, NetflixConstants.AccessUrl, Token, TokenSecret);
        }

        internal static void CheckInformationSet()
        {
            if (!informationset)
                throw new ArgumentException("Netflix API key/shared secret/application name have not been set.");
        }
    }

}
