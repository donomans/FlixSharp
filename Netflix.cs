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
        /// 
        /// </summary>
        /// <returns></returns>
        public delegate NetflixAccount GetCurrentUserNetflixUserInfo();
        
        private GetCurrentUserNetflixUserInfo getuserinfo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="GetUserInfo"></param>
        public Netflix(GetCurrentUserNetflixUserInfo GetUserInfo = null)
        {
            getuserinfo = GetUserInfo;
        }

        /// <summary>
        /// make a catalog/titles search request
        /// </summary>
        /// <param name="title"></param>
        /// <param name="max"></param>
        /// <param name="onuserbehalf">Make the request on the user's behalf </param>
        /// <returns></returns>
        public NetflixMovies SearchTitle(String Title, Int32 Limit = 10, Boolean OnUserBehalf = true)
        {
            
            return null;
        }
    }
}
