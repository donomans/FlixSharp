using FlixSharp.Holders.Netflix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FlixSharp.Helpers.Netflix.Async
{
    internal class AsyncFiller
    {
        public static async Task<IEnumerable<Title>> GetExpandedMovieDetails(XDocument doc, Boolean OnUserBehalf = true)
        {
            List<Task<Title>> titles = new List<Task<Title>>();

            var movieids = from movie
                    in doc.Element("catalog_titles").Elements("catalog_title")
                           select movie.Element("id").Value;

            foreach (String id in movieids)
            {
                ///loop through and do a NetflixFill.whatever to fill the titles
                titles.Add(FlixSharp.Netflix.Fill.Titles.GetExpandedTitle(id, OnUserBehalf));
            }

            List<Title> awaitedtitles = new List<Title>();
            foreach (Task<Title> t in titles)
                awaitedtitles.Add(await t);
            return awaitedtitles;
        }

        public static async Task<IEnumerable<Title>> GetCompleteNetflixMovieDetails(XDocument doc, Boolean OnUserBehalf = true)
        {
            List<Task<Title>> titles = new List<Task<Title>>();
            var movieids = from movie
                    in doc.Element("catalog_titles").Elements("catalog_title")
                           select movie.Element("id").Value;

            foreach (String id in movieids)
            {
                titles.Add(FlixSharp.Netflix.Fill.Titles.GetCompleteTitle(id, OnUserBehalf));
            }

            List<Title> awaitedtitles = new List<Title>();
            foreach (Task<Title> t in titles)
                awaitedtitles.Add(await t);
            return awaitedtitles;
        }

        public static async Task<IEnumerable<Person>> GetCompleteNetflixPersonDetails(XDocument doc, Boolean OnUserBehalf = true)
        {
            List<Task<Person>> people = new List<Task<Person>>();
            var personids = from person
                            in doc.Element("people").Elements("person")
                            select person.Element("id").Value;

            foreach (String id in personids)
            {
                people.Add(FlixSharp.Netflix.Fill.People.GetCompletePerson(id, OnUserBehalf));
            }

            List<Person> awaitedpeople = new List<Person>();
            foreach (Task<Person> p in people)
                awaitedpeople.Add(await p);
            return awaitedpeople;
        }
    }
   
}
