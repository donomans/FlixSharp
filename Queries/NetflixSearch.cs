using FlixSharp.Async;
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
        /// <param name="SearchTerm"></param>
        /// <param name="Limit"></param>
        /// <param name="OnUserBehalf">Make the request on the user's behalf if a 
        /// GetCurrentUserNetflixUserInfo delegate was provided during creation.</param>
        /// <returns></returns>
        public async Task<SearchResults> Search(String SearchTerm, Int32 Limit = 20, Boolean OnUserBehalf = true,
            TitleExpansion TitleExpansionLevel = TitleExpansion.Minimal,
            PersonExpansion PersonExpansionLevel = PersonExpansion.Minimal)
        {
            Netflix.Login.CheckInformationSet();

            Dictionary<String, String> extraParams = new Dictionary<String, String>();
            extraParams.Add("term", OAuth.OAuthHelpers.Encode(SearchTerm));
            extraParams.Add("max_results", Limit.ToString());

            String tokenSecret = "";
            if (OnUserBehalf)
            {
                Account na = Netflix.SafeReturnUserInfo();
                if (na != null)
                {
                    tokenSecret = na.TokenSecret;
                    extraParams.Add("oauth_token", na.Token);
                }
            }
            
            String titleurl = OAuth.OAuthHelpers.GetOAuthRequestUrl(Netflix.Login.SharedSecret,
                Netflix.Login.ConsumerKey,
                NetflixConstants.CatalogTitleUrl,
                "GET",
                tokenSecret,
                extraParams);

            XDocument moviedoc = await AsyncHelpers.LoadXDocumentAsync(titleurl);
            
            String personurl = OAuth.OAuthHelpers.GetOAuthRequestUrl(Netflix.Login.SharedSecret,
                Netflix.Login.ConsumerKey,
                NetflixConstants.CatalogPeopleUrl,
                "GET",
                tokenSecret,
                extraParams);

            XDocument persondoc = await AsyncHelpers.LoadXDocumentAsync(personurl);
            
            Movies movies = new Movies();

            switch (TitleExpansionLevel)
            {
                case TitleExpansion.Minimal:
                    movies.AddRange(from movie
                         in moviedoc.Descendants("catalog_title")
                                    select new Movie(TitleExpansion.Minimal)
                                    {
                                        IdUrl = movie.Element("id").Value,
                                        Year = (Int32)movie.Element("release_year"),
                                        Title = (String)movie.Element("title").Attribute("regular"),
                                        ShortTitle = (String)movie.Element("title").Attribute("short"),
                                        BoxArtUrlSmall = (String)movie.Element("box_art").Attribute("small"),
                                        BoxArtUrlLarge = (String)movie.Element("box_art").Attribute("large")
                                    });
                    break;
                case TitleExpansion.Basic:
                    movies.AddRange(from movie
                                    in moviedoc.Descendants("catalog_title")
                                    select new Movie(TitleExpansion.Minimal)
                                    {
                                        IdUrl = movie.Element("id").Value,
                                        Year = (Int32)movie.Element("release_year"),
                                        Title = (String)movie.Element("title").Attribute("regular"),
                                        ShortTitle = (String)movie.Element("title").Attribute("short"),
                                        BoxArtUrlSmall = (String)movie.Element("box_art").Attribute("small"),
                                        BoxArtUrlLarge = (String)movie.Element("box_art").Attribute("large"),
                                        Rating = new Rating((from mpaa
                                                            in movie.Elements("category")
                                                             where mpaa.Attribute("scheme").Value == "http://api-public.netflix.com/categories/mpaa_ratings"
                                                             select mpaa) ??
                                                            (from tv
                                                            in movie.Elements("category")
                                                             where tv.Attribute("scheme").Value == "http://api-public.netflix.com/categories/tv_ratings"
                                                             select tv)),
                                        AverageRating = (Single)movie.Element("average_rating"),
                                        RunTime = (Int32?)movie.Element("runtime")
                                    });
                    foreach (Movie m in movies)
                    {
                        m.Synopsis = await AsyncHelpers.GetSynopsis(m.Id);
                    }
                    break;
                case TitleExpansion.Expanded:
                    movies.AddRange(await AsyncHelpers.GetExpandedMovieDetails(moviedoc));
                    break;
                case TitleExpansion.Complete:
                    movies.AddRange(await AsyncHelpers.GetCompleteMovieDetails(moviedoc));
                    break;
            }

            

            People people = new People();

            switch (PersonExpansionLevel)
            {
                case PersonExpansion.Minimal:
                    people.AddRange(from person
                                    in persondoc.Descendants("person")
                                    select new Person(PersonExpansion.Minimal)
                                    {
                                        IdUrl = person.Element("id").Value,
                                        Name = person.Element("name").Value,
                                        Bio = (String)person.Element("bio")
                                    });
                    break;
                case PersonExpansion.Complete:
                    people.AddRange(await AsyncHelpers.GetCompletePersonDetails(persondoc));
                    break;
            }


            SearchResults sr = new SearchResults();
            sr.MovieResults = movies;
            sr.PeopleResults = people;
            sr.SearchTerm = SearchTerm;
            return sr;
        }

        public async Task<Movies> SearchTitles(String Title, Int32 Limit = 10, Boolean OnUserBehalf = true,
            TitleExpansion ExpansionLevel = TitleExpansion.Minimal)
        {
            Netflix.Login.CheckInformationSet();

            Dictionary<String, String> extraParams = new Dictionary<String, String>();
            extraParams.Add("term", OAuth.OAuthHelpers.Encode(Title));
            extraParams.Add("max_results", Limit.ToString());

            String tokenSecret = "";
            //String token = "";
            if (OnUserBehalf)
            {
                Account na = Netflix.SafeReturnUserInfo();
                if (na != null)
                {
                    tokenSecret = na.TokenSecret;
                    extraParams.Add("oauth_token", na.Token);
                }
            }

            String titleurl = OAuth.OAuthHelpers.GetOAuthRequestUrl(Netflix.Login.SharedSecret,
                Netflix.Login.ConsumerKey,
                NetflixConstants.CatalogTitleUrl,
                "GET",
                tokenSecret,
                extraParams);

            XDocument moviedoc = await AsyncHelpers.LoadXDocumentAsync(titleurl);

            Movies movies = new Movies();
            
            switch (ExpansionLevel)
            {
                case TitleExpansion.Minimal:
                    movies.AddRange(from movie
                         in moviedoc.Descendants("catalog_title")
                                    select new Movie(TitleExpansion.Minimal)
                                    {
                                        IdUrl = movie.Element("id").Value,
                                        Year = (Int32)movie.Element("release_year"),
                                        Title = (String)movie.Element("title").Attribute("regular"),
                                        ShortTitle = (String)movie.Element("title").Attribute("short"),
                                        BoxArtUrlSmall = (String)movie.Element("box_art").Attribute("small"),
                                        BoxArtUrlLarge = (String)movie.Element("box_art").Attribute("large")
                                    });
                    break;
                case TitleExpansion.Basic:
                    movies.AddRange(from movie
                                    in moviedoc.Descendants("catalog_title")
                                    select new Movie(TitleExpansion.Minimal)
                                    {
                                        IdUrl = movie.Element("id").Value,
                                        Year = (Int32)movie.Element("release_year"),
                                        Title = (String)movie.Element("title").Attribute("regular"),
                                        ShortTitle = (String)movie.Element("title").Attribute("short"),
                                        BoxArtUrlSmall = (String)movie.Element("box_art").Attribute("small"),
                                        BoxArtUrlLarge = (String)movie.Element("box_art").Attribute("large"),
                                        Rating = new Rating((from mpaa
                                                            in movie.Elements("category")
                                                             where mpaa.Attribute("scheme").Value == "http://api-public.netflix.com/categories/mpaa_ratings"
                                                             select mpaa) ??
                                                            (from tv
                                                            in movie.Elements("category")
                                                             where tv.Attribute("scheme").Value == "http://api-public.netflix.com/categories/tv_ratings"
                                                             select tv)),
                                        AverageRating = (Single)movie.Element("average_rating"),
                                        RunTime = (Int32?)movie.Element("runtime")
                                    });
                    foreach (Movie m in movies)
                    {
                        m.Synopsis = await AsyncHelpers.GetSynopsis(m.Id);
                    }
                    break;
                case TitleExpansion.Expanded:
                    movies.AddRange(await AsyncHelpers.GetExpandedMovieDetails(moviedoc));
                    break;
                case TitleExpansion.Complete:
                    movies.AddRange(await AsyncHelpers.GetCompleteMovieDetails(moviedoc));
                    break;
            }

            
            return movies;
        }

        public async Task<People> SearchPeople(String Name, Int32 Limit = 10, Boolean OnUserBehalf = true,
           PersonExpansion ExpansionLevel = PersonExpansion.Minimal)
        {
            Netflix.Login.CheckInformationSet();

            Dictionary<String, String> extraParams = new Dictionary<String, String>();
            extraParams.Add("term", OAuth.OAuthHelpers.Encode(Name));
            extraParams.Add("max_results", Limit.ToString());

            String tokenSecret = "";
            //String token = "";
            if (OnUserBehalf)
            {
                Account na = Netflix.SafeReturnUserInfo();
                if (na != null)
                {
                    tokenSecret = na.TokenSecret;
                    extraParams.Add("oauth_token", na.Token);
                }
            }

            String personurl = OAuth.OAuthHelpers.GetOAuthRequestUrl(Netflix.Login.SharedSecret,
                Netflix.Login.ConsumerKey,
                NetflixConstants.CatalogPeopleUrl,
                "GET",
                tokenSecret,
                extraParams);

            XDocument persondoc = await AsyncHelpers.LoadXDocumentAsync(personurl);

            People people = new People();

            switch (ExpansionLevel)
            {
                case PersonExpansion.Minimal:
                    people.AddRange(from person
                                    in persondoc.Descendants("person")
                                    select new Person(PersonExpansion.Minimal)
                                    {
                                        IdUrl = person.Element("id").Value,
                                        Name = person.Element("name").Value,
                                        Bio = (String)person.Element("bio")
                                    });
                    break;
                case PersonExpansion.Complete:
                    people.AddRange(await AsyncHelpers.GetCompletePersonDetails(persondoc));
                    break;
            }


            return people;
        }

        public async Task<IEnumerable<String>> AutoCompleteTitle(String Title, Int32 Limit = 10)
        {
            Netflix.Login.CheckInformationSet();

            String url = NetflixConstants.CatalogTitleAutoCompleteUrl + 
                "?oauth_consumer_key=" + Netflix.Login.ConsumerKey +
                "&term=" + Title;

            XDocument doc = await AsyncHelpers.LoadXDocumentAsync(url);

            var titles = from someelement
                         in doc.Descendants("title")
                         select someelement.Attribute("short").Value;

            return titles.Take(Limit);
        }
    }
}
