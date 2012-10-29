using FlixSharp.Constants;
using FlixSharp.Holders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FlixSharp.Async
{
    internal static class AsyncHelpers
    {
        public static async Task<XDocument> LoadXDocumentAsync(String url)
        {
            using (WebClient wc = new WebClient())
            {
                String xml = wc.DownloadString(url);
                return XDocument.Parse(xml);
            }
        }

        public static async Task<IEnumerable<Title>> GetExpandedMovieDetails(XDocument doc)
        {
            ///loop through and do a NetflixFill.whatever to fill the titles
            return null;
        }

        public static async Task<IEnumerable<Title>> GetCompleteMovieDetails(XDocument doc)
        {
            return null;
        }

        public static async Task<IEnumerable<Person>> GetCompletePersonDetails(XDocument doc)
        {
            return null;
        }

        


        public static String GetIdFromUrl(String NetflixIdUrl)
        {
            return Regex.Match(NetflixIdUrl, "[0-9]{4,10}").Value; 
            ///Netflix IDs seem to be 5-9 characters, but there may be some 4 character or 10 character that I don't know about
        }
    }
}
