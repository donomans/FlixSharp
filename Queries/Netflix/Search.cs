using FlixSharp.Helpers.Async;
using FlixSharp.Helpers.Netflix.Async;
using FlixSharp.Holders;
using FlixSharp.Holders.Netflix;
using FlixSharp.Helpers.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FlixSharp.Queries.Netflix
{
    public class Search
    {
        /// <summary>
        /// Make a catalog/titles search request
        /// </summary>
        /// <param name="SearchTerm"></param>
        /// <param name="Limit"></param>
        /// <param name="OnUserBehalf">Make the request on the user's behalf if a 
        /// GetCurrentUserNetflixUserInfo delegate was provided during creation.</param>
        /// <returns></returns>
        public async Task<SearchResults> SearchEverything(String SearchTerm, Int32 Limit = 20, Boolean OnUserBehalf = true,
            TitleExpansion TitleExpansionLevel = TitleExpansion.Minimal,
            PersonExpansion PersonExpansionLevel = PersonExpansion.Minimal)
        {
            Login.CheckInformationSet();

            Dictionary<String, String> extraParams = new Dictionary<String, String>();
            extraParams.Add("term", OAuthHelpers.Encode(SearchTerm));
            extraParams.Add("max_results", Limit.ToString());

            String tokenSecret = "";
            if (OnUserBehalf)
            {
                Account na = FlixSharp.Netflix.SafeReturnUserInfo();
                if (na != null)
                {
                    tokenSecret = na.TokenSecret;
                    extraParams.Add("oauth_token", na.Token);
                }
            }

            String personurl = OAuthHelpers.GetOAuthRequestUrl(Login.SharedSecret,
                Login.ConsumerKey,
                Constants.CatalogPeopleSearcUrl,
                "GET",
                tokenSecret,
                extraParams);
            var persondoc = AsyncHelpers.NetflixLoadXDocumentAsync(personurl);

            String titleurl = OAuthHelpers.GetOAuthRequestUrl(Login.SharedSecret,
                Login.ConsumerKey,
                Constants.CatalogTitleSearchUrl,
                "GET",
                tokenSecret,
                extraParams);
            var moviedoc = AsyncHelpers.NetflixLoadXDocumentAsync(titleurl);
            
            People people = new People();
            switch (PersonExpansionLevel)
            {
                case PersonExpansion.Minimal:
                    people.AddRange(from person
                                    in (await persondoc).Descendants("person")
                                    select new Person(PersonExpansion.Minimal)
                                    {
                                        IdUrl = person.Element("id").Value,
                                        Name = person.Element("name").Value,
                                        Bio = (String)person.Element("bio")
                                    });
                    break;
                case PersonExpansion.Complete:
                    people.AddRange(await AsyncFiller.GetCompleteNetflixPersonDetails(await persondoc));
                    break;
            }

            Titles movies = new Titles();
            switch (TitleExpansionLevel)
            {
                case TitleExpansion.Minimal:
                    movies.AddRange(await Fill.GetBaseTitleInfo(moviedoc, "catalog_title"));
                    break;
                case TitleExpansion.Expanded:
                    movies.AddRange(await AsyncFiller.GetExpandedMovieDetails(await moviedoc));
                    break;
                case TitleExpansion.Complete:
                    movies.AddRange(await AsyncFiller.GetCompleteNetflixMovieDetails(await moviedoc));
                    break;
            }

            SearchResults sr = new SearchResults();
            sr.MovieResults = movies;
            sr.PeopleResults = people;
            sr.SearchTerm = SearchTerm;
            return sr;
        }

        public async Task<Titles> SearchTitles(String Title, Int32 Limit = 10, Boolean OnUserBehalf = true,
            TitleExpansion ExpansionLevel = TitleExpansion.Minimal)
        {
            Login.CheckInformationSet();

            Dictionary<String, String> extraParams = new Dictionary<String, String>();
            extraParams.Add("term", FlixSharp.Helpers.OAuth.OAuthHelpers.Encode(Title));
            extraParams.Add("max_results", Limit.ToString());

            String tokenSecret = "";
            //String token = "";
            if (OnUserBehalf)
            {
                Account na = FlixSharp.Netflix.SafeReturnUserInfo();
                if (na != null)
                {
                    tokenSecret = na.TokenSecret;
                    extraParams.Add("oauth_token", na.Token);
                }
            }

            String titleurl = OAuthHelpers.GetOAuthRequestUrl(Login.SharedSecret,
                Login.ConsumerKey,
                Constants.CatalogTitleSearchUrl,
                "GET",
                tokenSecret,
                extraParams);

            var moviedoc = AsyncHelpers.NetflixLoadXDocumentAsync(titleurl);

            Titles movies = new Titles();

            switch (ExpansionLevel)
            {
                case TitleExpansion.Minimal:
                    movies.AddRange(await Fill.GetBaseTitleInfo(moviedoc, "catalog_title"));
                    break;
                case TitleExpansion.Expanded:
                    movies.AddRange(await AsyncFiller.GetExpandedMovieDetails(await moviedoc));
                    break;
                case TitleExpansion.Complete:
                    movies.AddRange(await AsyncFiller.GetCompleteNetflixMovieDetails(await moviedoc));
                    break;
            }


            return movies;

        }

        public async Task<People> SearchPeople(String Name, Int32 Limit = 10, Boolean OnUserBehalf = true,
           PersonExpansion ExpansionLevel = PersonExpansion.Minimal)
        {
            Login.CheckInformationSet();

            Dictionary<String, String> extraParams = new Dictionary<String, String>();
            extraParams.Add("term", OAuthHelpers.Encode(Name));
            extraParams.Add("max_results", Limit.ToString());

            String tokenSecret = "";
            //String token = "";
            if (OnUserBehalf)
            {
                Account na = FlixSharp.Netflix.SafeReturnUserInfo();
                if (na != null)
                {
                    tokenSecret = na.TokenSecret;
                    extraParams.Add("oauth_token", na.Token);
                }
            }

            String personurl = OAuthHelpers.GetOAuthRequestUrl(Login.SharedSecret,
                Login.ConsumerKey,
                Constants.CatalogPeopleSearcUrl,
                "GET",
                tokenSecret,
                extraParams);

            var persondoc = AsyncHelpers.NetflixLoadXDocumentAsync(personurl);

            People people = new People();

            switch (ExpansionLevel)
            {
                case PersonExpansion.Minimal:
                    people.AddRange(from person
                                    in (await persondoc).Element("people").Elements("person")
                                    select new Person(PersonExpansion.Minimal)
                                    {
                                        IdUrl = person.Element("id").Value,
                                        Name = person.Element("name").Value,
                                        Bio = (String)person.Element("bio")
                                    });
                    break;
                case PersonExpansion.Complete:
                    people.AddRange(await AsyncFiller.GetCompleteNetflixPersonDetails(await persondoc));
                    break;
            }


            return people;
        }

        public async Task<IEnumerable<String>> AutoCompleteTitle(String Title, Int32 Limit = 10)
        {
            Login.CheckInformationSet();

            String url = Constants.CatalogTitleAutoCompleteUrl + 
                "?oauth_consumer_key=" + Login.ConsumerKey +
                "&term=" + Title;

            var doc = AsyncHelpers.NetflixLoadXDocumentAsync(url);

            var titles = from someelement
                         in (await doc).Descendants("title")
                         select someelement.Attribute("short").Value;

            return titles.Take(Limit);
        }
    }
}
