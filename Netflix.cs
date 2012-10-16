using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FlixSharp.OAuth;
using FlixSharp.Constants;
using FlixSharp.Holders;

namespace FlixSharp
{

    public class Netflix
    {
        public class Login
        {
            internal String consumerkey;
            internal String sharedsecret;
            internal String applicationname;

            public Login(String ConsumerKey, String SharedSecret, String ApplicationName)
            {
                consumerkey = ConsumerKey;
                sharedsecret = SharedSecret;
                applicationname = ApplicationName;
            }

            public String GetRequestUrl()
            {
                return OAuthHelpers.GetOAuthRequestUrl(sharedsecret, consumerkey, NetflixConstants.RequestUrl, "GET");
            }

            public String GetLoginUrl(String Token, String Callback, Dictionary<String, String> ExtraParams)
            {
                return OAuthHelpers.GetOAuthLoginUrl(consumerkey, Token, Callback, NetflixConstants.LoginUrl, ExtraParams);
            }


            public String GetAccessUrl(String Token, String TokenSecret)
            {
                return OAuthHelpers.GetOAuthAccessUrl(sharedsecret, consumerkey, NetflixConstants.AccessUrl, Token, TokenSecret);
            }
        }

        /// <summary>
        /// Method to provider the current user's Netflix account Token, Token Secret, and User Id.
        /// Used for Protected Netflix API requests.
        /// </summary>
        /// <returns></returns>
        public delegate NetflixAccount GetCurrentUserNetflixUserInfo();
        
        private GetCurrentUserNetflixUserInfo getuserinfo;

        /// <summary>
        /// Instantiate a new Netflix client to send requests
        /// </summary>
        /// <param name="GetUserInfo"></param>
        public Netflix(GetCurrentUserNetflixUserInfo GetUserInfo = null)
        {
            getuserinfo = GetUserInfo;
        }

        /// <summary>
        /// Make a catalog/titles search request
        /// </summary>
        /// <param name="title"></param>
        /// <param name="max"></param>
        /// <param name="onuserbehalf">Make the request on the user's behalf if a 
        /// GetCurrentUserNetflixUserInfo delegate was provided during creation.</param>
        /// <returns></returns>
        public NetflixMovies SearchTitle(String Title, Int32 Limit = 10, Boolean OnUserBehalf = true)
        {   
            return null;
        }

        /// <summary>
        /// Make a catalog/titles similar titles request.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public NetflixMovies GetSimilarTitles(String Id, Int32 Limit = 10, Boolean OnUserBehalf = true)
        {
            return null;
        }
    }
}
