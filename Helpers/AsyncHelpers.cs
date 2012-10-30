using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using FlixSharp.Constants;
using FlixSharp.Holders;
using FlixSharp.Queries;

namespace FlixSharp.Async
{
    internal class AsyncHelpers
    {
        public static async Task<XDocument> LoadXDocumentAsync(String url)
        {
            using (WebClient wc = new WebClient())
            {
                String xml = await wc.DownloadStringTaskAsync(url);
                return XDocument.Parse(xml);
            }
        }

        public static async Task<IEnumerable<Title>> GetExpandedMovieDetails(XDocument doc, Boolean OnUserBehalf = true)
        {
            List<Task<Title>> titles = new List<Task<Title>>();

            var movieids = from movie
                    in doc.Element("catalog_titles").Elements("catalog_title")
                    select movie.Element("id").Value;

            foreach (String id in movieids)
            {
                ///loop through and do a NetflixFill.whatever to fill the titles
                titles.Add(Netflix.Fill.Titles.GetExpandedTitle(id, OnUserBehalf));
            }

            List<Title> awaitedtitles = new List<Title>();
            foreach (Task<Title> t in titles)
                awaitedtitles.Add(await t);
            return awaitedtitles;
        }

        public static async Task<IEnumerable<Title>> GetCompleteMovieDetails(XDocument doc, Boolean OnUserBehalf = true)
        {
            List<Task<Title>> titles = new List<Task<Title>>();
            var movieids = from movie
                    in doc.Element("catalog_titles").Elements("catalog_title")
                    select movie.Element("id").Value;
   
            foreach (String id in movieids)
            {
                titles.Add(Netflix.Fill.Titles.GetCompleteTitle(id, OnUserBehalf));
            }

            List<Title> awaitedtitles = new List<Title>();
            foreach (Task<Title> t in titles)
                awaitedtitles.Add(await t);
            return awaitedtitles;
        }

        public static async Task<IEnumerable<Person>> GetCompletePersonDetails(XDocument doc, Boolean OnUserBehalf = true)
        {
            List<Task<Person>> people = new List<Task<Person>>();
            var personids = from person
                            in doc.Element("people").Elements("person")
                            select person.Element("id").Value;

            foreach (String id in personids)
            {
                people.Add(Netflix.Fill.People.GetCompletePerson(id, OnUserBehalf));
            }

            List<Person> awaitedpeople = new List<Person>();
            foreach (Task<Person> p in people)
                awaitedpeople.Add(await p);
            return awaitedpeople;
        }
    }
}
