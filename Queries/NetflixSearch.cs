using FlixSharp.Constants;
using FlixSharp.Holders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FlixSharp.Queries
{
    public class NetflixSearch
    {
        /// <summary>
        /// Make a catalog/titles search request
        /// </summary>
        /// <param name="title"></param>
        /// <param name="max"></param>
        /// <param name="onuserbehalf">Make the request on the user's behalf if a 
        /// GetCurrentUserNetflixUserInfo delegate was provided during creation.</param>
        /// <returns></returns>
        public Movies SearchTitle(String Title, Int32 Limit = 10, Boolean OnUserBehalf = true,
            TitleExpansion ExpansionLevel = TitleExpansion.Minimal)
        {
            Netflix.Login.CheckInformationSet();
            return null;
        }

        public IEnumerable<String> AutoCompleteTitle(String Title, Int32 Limit = 10)
        {
            Netflix.Login.CheckInformationSet();

            String url = NetflixConstants.CatalogAutoCompleteUrl + 
                "?oauth_consumer_key=" + Netflix.Login.ConsumerKey +
                "&term=" + Title;

            XDocument doc = XDocument.Load(url);

            var titles = from someelement
                         in doc.Descendants("title")
                         select someelement.Attribute("short").Value;

            return titles.Take(Limit);
        }
    }
}
