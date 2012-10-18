using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FlixSharp.OAuth;
using FlixSharp.Constants;
using FlixSharp.Holders;
using FlixSharp.Queries;
using System.Xml.Linq;

namespace FlixSharp
{
    public class Netflix
    {
        public static class Login
        {
            public static String ConsumerKey { get { return consumerkey; } }
            private static String consumerkey;
            public static String SharedSecret { get { return sharedsecret; } }
            private static String sharedsecret;
            public static String ApplicationName { get { return applicationname;} }
            private static String applicationname;

            public static Boolean InformationSet { get { return informationset;} }
            private static Boolean informationset = false;

            /// <summary>
            /// Set your Netflix API information before using any api queries
            /// </summary>
            /// <param name="ConsumerKey"></param>
            /// <param name="SharedSecret"></param>
            /// <param name="ApplicationName">[Optional] If you are not sure what Netflix 
            /// thinks your application name is, you may set it upon retrieving it from 
            /// the result of the GetLoginUrl request - your callback will receive it as a parameter</param>
            public static void SetCredentials(String ConsumerKey, String SharedSecret, String ApplicationName = "")
            {
                consumerkey = ConsumerKey;
                sharedsecret = SharedSecret;
                applicationname = ApplicationName;

                informationset = true;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="ApplicationName">The Application Name as given by Netflix from the API</param>
            public static void SetApplicationName(String ApplicationName)
            {
                applicationname = ApplicationName;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public static String GetRequestUrl()
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
            public static String GetLoginUrl(String Token, String Callback)
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
            public static String GetAccessUrl(String Token, String TokenSecret)
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

        /// <summary>
        /// Method to provide the current user's Netflix account Token, Token Secret, and User Id.
        /// Used for Protected Netflix API requests.  Can return null if there is no Netflix account linked
        /// for the currently logged in user for this web request.
        /// </summary>
        /// <returns></returns>
        public delegate Account GetCurrentUserNetflixUserInfo();
        
        private static GetCurrentUserNetflixUserInfo _GetUserInfo = null;

        internal static Account SafeReturnUserInfo()
        {
            if (Netflix._GetUserInfo != null)
            {
                try
                {
                    Account na;
                    na = Netflix._GetUserInfo();
                    return na;
                }
                catch (Exception ex) 
                {
                }
            }
            return null;
        }


        public static void SetMethodForGettingCurrentUserAccount(GetCurrentUserNetflixUserInfo GetUserInfo)
        {
            _GetUserInfo = GetUserInfo;
        }
        /// <summary>
        /// Instantiate a new Netflix client to send requests.
        /// [Note] Although GetUserInfo method is only required the first time,
        /// it may be provided in every instantiation (or can be changed for each 
        /// instantiation if so desired)
        /// </summary>
        /// <param name="GetUserInfo">A static method that returns a Netflix Account 
        /// (presumably for the current logged in user to make Protected requests)</param>
        public Netflix(GetCurrentUserNetflixUserInfo GetUserInfo = null)
        {
            Login.CheckInformationSet();
            if (_GetUserInfo == null)
                _GetUserInfo = GetUserInfo;
        }

        public NetflixSearch Search { get { return search; } }
        private NetflixSearch search = new NetflixSearch();


        /// <summary>
        /// Make a catalog/titles similar titles request.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Movies GetSimilarTitles(String Id, Int32 Limit = 10, Boolean OnUserBehalf = true)
        {
            Login.CheckInformationSet();
            return null;
        }
    }
}
