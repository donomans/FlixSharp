using FlixSharp.Helpers.OAuth;
using FlixSharp.Holders.RottenTomatoes;
using FlixSharp.Queries.RottenTomatoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp.Helpers.RottenTomatoes
{
    internal class UrlBuilder
    {
        public static String SearchUrl(String SearchTerm, Int32 Limit)
        {
            return String.Format(Constants.SearchUrl, Login.ConsumerKey, OAuthHelpers.Encode(SearchTerm));
        }
    }
}
