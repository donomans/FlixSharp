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

        public static async Task<IEnumerable<Movie>> GetExpandedMovieDetails(XDocument doc)
        {
            return null;
        }

        public static async Task<IEnumerable<Movie>> GetCompleteMovieDetails(XDocument doc)
        {
            return null;
        }

        public static async Task<IEnumerable<Person>> GetCompletePersonDetails(XDocument doc)
        {
            return null;
        }

        public static async Task<String> GetSynopsis(String NetflixId)
        {
            NetflixId = GetIdFromUrl(NetflixId);
            XDocument doc = await LoadXDocumentAsync(String.Format(NetflixConstants.TitleSynopsisUrl, NetflixId));
            return (String)doc.Element("synopsis");
        }

        public static String GetIdFromUrl(String NetflixIdUrl)
        {
            return Regex.Match(NetflixIdUrl, "[0-9]{4,9}").Value;
        }
    }
}
