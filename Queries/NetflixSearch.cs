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
            NetflixLogin.CheckInformationSet();

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

            String personurl = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
                NetflixLogin.ConsumerKey,
                NetflixConstants.CatalogPeopleSearcUrl,
                "GET",
                tokenSecret,
                extraParams);
            var persondoc = AsyncHelpers.LoadXDocumentAsync(personurl);

            String titleurl = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
                NetflixLogin.ConsumerKey,
                NetflixConstants.CatalogTitleSearchUrl,
                "GET",
                tokenSecret,
                extraParams);
            var moviedoc = AsyncHelpers.LoadXDocumentAsync(titleurl);
            
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
                    people.AddRange(await AsyncHelpers.GetCompletePersonDetails(await persondoc));
                    break;
            }

            Titles movies = new Titles();
            switch (TitleExpansionLevel)
            {
                case TitleExpansion.Minimal:
                    movies.AddRange(await NetflixFill.GetBaseTitleInfo(moviedoc, "catalog_title"));
                    //movies.AddRange(from movie
                    //                in (await moviedoc).Descendants("catalog_title")
                    //                select new Title(TitleExpansion.Minimal)
                    //                {
                    //                    IdUrl = movie.Element("id").Value,
                    //                    Year = (Int32?)movie.Element("release_year") ?? 0,
                    //                    FullTitle = (String)movie.Element("title").Attribute("regular"),
                    //                    ShortTitle = (String)movie.Element("title").Attribute("short"),
                    //                    BoxArtUrlSmall = (String)movie.Element("box_art").Attribute("small"),
                    //                    BoxArtUrlLarge = (String)movie.Element("box_art").Attribute("large"),
                    //                    Rating = new Rating((from mpaa
                    //                                        in movie.Elements("category")
                    //                                         where (String)mpaa.Attribute("scheme") == NetflixConstants.Schemas.CategoryMpaaRating
                    //                                         select mpaa) ??
                    //                                        (from tv
                    //                                        in movie.Elements("category")
                    //                                         where (String)tv.Attribute("scheme") == NetflixConstants.Schemas.CategoryTvRating
                    //                                         select tv)),
                    //                    AverageRating = (Single?)movie.Element("average_rating") ?? 0,
                    //                    RunTime = (Int32?)movie.Element("runtime"),
                    //                    Genres = new List<String>(from genres
                    //                                              in movie.Elements("category")
                    //                                              where (String)genres.Attribute("scheme") == NetflixConstants.Schemas.CategoryGenre
                    //                                              select (String)genres.Attribute("term")),
                    //                    NetflixSiteUrl = (from webpage
                    //                                      in movie.Elements("link")
                    //                                      where (String)webpage.Attribute("title") == "web page"
                    //                                      select (String)webpage.Attribute("href")).FirstOrDefault(),
                    //                    OfficialWebsite = (from webpage
                    //                                       in movie.Elements("link")
                    //                                       where (String)webpage.Attribute("rel") == NetflixConstants.Schemas.TitleOfficialUrl
                    //                                       select (String)webpage.Attribute("href")).FirstOrDefault(),
                    //                    HasAwards = (from webpage
                    //                                 in movie.Elements("link")
                    //                                 where (String)webpage.Attribute("rel") == NetflixConstants.Schemas.LinkAwards
                    //                                 select true).FirstOrDefault(),
                    //                    HasBonusMaterials = (from webpage
                    //                                         in movie.Elements("link")
                    //                                         where (String)webpage.Attribute("rel") == NetflixConstants.Schemas.LinkBonusMaterials
                    //                                         select true).FirstOrDefault(),
                    //                    HasDiscs = (from webpage
                    //                                in movie.Elements("link")
                    //                                where (String)webpage.Attribute("rel") == NetflixConstants.Schemas.LinkDiscs
                    //                                select true).FirstOrDefault(),
                    //                    HasLanguages = (from webpage
                    //                                    in movie.Elements("link")
                    //                                    where (String)webpage.Attribute("rel") == NetflixConstants.Schemas.LinkLanguagesAndAudio
                    //                                    select true).FirstOrDefault()
                    //                });
                    break;
                case TitleExpansion.Expanded:
                    movies.AddRange(await AsyncHelpers.GetExpandedMovieDetails(await moviedoc));
                    break;
                case TitleExpansion.Complete:
                    movies.AddRange(await AsyncHelpers.GetCompleteMovieDetails(await moviedoc));
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
            NetflixLogin.CheckInformationSet();

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

            String titleurl = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
                NetflixLogin.ConsumerKey,
                NetflixConstants.CatalogTitleSearchUrl,
                "GET",
                tokenSecret,
                extraParams);

            var moviedoc = AsyncHelpers.LoadXDocumentAsync(titleurl);

            Titles movies = new Titles();

            switch (ExpansionLevel)
            {
                case TitleExpansion.Minimal:
                    movies.AddRange(await NetflixFill.GetBaseTitleInfo(moviedoc, "catalog_title"));
                    //movies.AddRange(from movie
                    //     in (await moviedoc).Element("catalog_titles").Elements("catalog_title")
                    //                select new Title(TitleExpansion.Minimal)
                    //                {
                    //                    IdUrl = movie.Element("id").Value,
                    //                    Year = (Int32?)movie.Element("release_year") ?? 0,
                    //                    FullTitle = (String)movie.Element("title").Attribute("regular"),
                    //                    ShortTitle = (String)movie.Element("title").Attribute("short"),
                    //                    BoxArtUrlSmall = (String)movie.Element("box_art").Attribute("small"),
                    //                    BoxArtUrlLarge = (String)movie.Element("box_art").Attribute("large"),
                    //                    Rating = new Rating((from mpaa
                    //                                        in movie.Elements("category")
                    //                                         where (String)mpaa.Attribute("scheme") == NetflixConstants.Schemas.CategoryMpaaRating
                    //                                         select mpaa) ??
                    //                                        (from tv
                    //                                        in movie.Elements("category")
                    //                                         where (String)tv.Attribute("scheme") == NetflixConstants.Schemas.CategoryTvRating
                    //                                         select tv)),
                    //                    AverageRating = (Single?)movie.Element("average_rating") ?? 0,
                    //                    RunTime = (Int32?)movie.Element("runtime"),
                    //                    Genres = new List<String>(from genres
                    //                                              in movie.Elements("category")
                    //                                              where (String)genres.Attribute("scheme") == NetflixConstants.Schemas.CategoryGenre
                    //                                              select (String)genres.Attribute("term")),
                    //                    NetflixSiteUrl = (from webpage
                    //                                      in movie.Elements("link")
                    //                                      where (String)webpage.Attribute("title") == "web page"
                    //                                      select (String)webpage.Attribute("href")).FirstOrDefault(),
                    //                    OfficialWebsite = (from webpage
                    //                                       in movie.Elements("link")
                    //                                       where (String)webpage.Attribute("rel") == NetflixConstants.Schemas.TitleOfficialUrl
                    //                                       select (String)webpage.Attribute("href")).FirstOrDefault(),
                    //                    HasAwards = (from webpage
                    //                                 in movie.Elements("link")
                    //                                 where (String)webpage.Attribute("rel") == NetflixConstants.Schemas.LinkAwards
                    //                                 select true).FirstOrDefault(),
                    //                    HasBonusMaterials = (from webpage
                    //                                         in movie.Elements("link")
                    //                                         where (String)webpage.Attribute("rel") == NetflixConstants.Schemas.LinkBonusMaterials
                    //                                         select true).FirstOrDefault(),
                    //                    HasDiscs = (from webpage
                    //                                in movie.Elements("link")
                    //                                where (String)webpage.Attribute("rel") == NetflixConstants.Schemas.LinkDiscs
                    //                                select true).FirstOrDefault(),
                    //                    HasLanguages = (from webpage
                    //                                    in movie.Elements("link")
                    //                                    where (String)webpage.Attribute("rel") == NetflixConstants.Schemas.LinkLanguagesAndAudio
                    //                                    select true).FirstOrDefault()
                    //                });
                    break;
                case TitleExpansion.Expanded:
                    movies.AddRange(await AsyncHelpers.GetExpandedMovieDetails(await moviedoc));
                    break;
                case TitleExpansion.Complete:
                    movies.AddRange(await AsyncHelpers.GetCompleteMovieDetails(await moviedoc));
                    break;
            }


            return movies;

        }

        public async Task<People> SearchPeople(String Name, Int32 Limit = 10, Boolean OnUserBehalf = true,
           PersonExpansion ExpansionLevel = PersonExpansion.Minimal)
        {
            NetflixLogin.CheckInformationSet();

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

            String personurl = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
                NetflixLogin.ConsumerKey,
                NetflixConstants.CatalogPeopleSearcUrl,
                "GET",
                tokenSecret,
                extraParams);

            var persondoc = AsyncHelpers.LoadXDocumentAsync(personurl);

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
                    people.AddRange(await AsyncHelpers.GetCompletePersonDetails(await persondoc));
                    break;
            }


            return people;
        }

        public async Task<IEnumerable<String>> AutoCompleteTitle(String Title, Int32 Limit = 10)
        {
            NetflixLogin.CheckInformationSet();

            String url = NetflixConstants.CatalogTitleAutoCompleteUrl + 
                "?oauth_consumer_key=" + NetflixLogin.ConsumerKey +
                "&term=" + Title;

            var doc = AsyncHelpers.LoadXDocumentAsync(url);

            var titles = from someelement
                         in (await doc).Descendants("title")
                         select someelement.Attribute("short").Value;

            return titles.Take(Limit);
        }
    }
}
