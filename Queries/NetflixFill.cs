using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlixSharp.Holders;
using FlixSharp.Constants;
using FlixSharp.Async;
using System.Xml.Linq;
using FlixSharp.Helpers;

namespace FlixSharp.Queries
{
    public class NetflixFill
    {
        public async Task<Title> GetBaseTitle(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            if (NetflixId.Contains("http"))
                TitleType = GetNetflixType(NetflixId, TitleType);
            else
                throw new ArgumentException("NetflixId and TitleType combination is not valid: No definition Netflix Type was identifiable.");

            NetflixId = AsyncHelpers.GetIdFromUrl(NetflixId);

            switch (TitleType)
            {
                case NetflixType.Movie:
                    return await Movies.GetBaseTitle(NetflixId, null, OnUserBehalf);
                case NetflixType.Series:
                    throw new NotImplementedException();
                case NetflixType.SeriesSeason:
                    throw new NotImplementedException();
                case NetflixType.Programs:
                    throw new NotImplementedException();
                default: return null;
            }
        }

        public async Task<People> GetActors(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            if (NetflixId.Contains("http"))
                TitleType = GetNetflixType(NetflixId, TitleType);
            else
                throw new ArgumentException("NetflixId and TitleType combination is not valid: No definition Netflix Type was identifiable.");

            NetflixId = AsyncHelpers.GetIdFromUrl(NetflixId);

            switch (TitleType)
            {
                case NetflixType.Movie:
                    return await Movies.GetActors(NetflixId, null, OnUserBehalf);
                case NetflixType.Series:
                    throw new NotImplementedException();
                case NetflixType.SeriesSeason:
                    throw new NotImplementedException();
                case NetflixType.Programs:
                    throw new NotImplementedException();
                default: return null;
            }
        }
        public async Task<People> GetDirectors(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            if (NetflixId.Contains("http"))
                TitleType = GetNetflixType(NetflixId, TitleType);
            else
                throw new ArgumentException("NetflixId and TitleType combination is not valid: No definition Netflix Type was identifiable.");

            NetflixId = AsyncHelpers.GetIdFromUrl(NetflixId);

            switch (TitleType)
            {
                case NetflixType.Movie:
                    return await Movies.GetDirectors(NetflixId, null, OnUserBehalf);
                case NetflixType.Series:
                    throw new NotImplementedException();
                case NetflixType.SeriesSeason:
                    throw new NotImplementedException();
                case NetflixType.Programs:
                    throw new NotImplementedException();
                default: return null;
            }
        }

        public async Task<Title> GetCompleteTitle(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            if (NetflixId.Contains("http"))
                TitleType = GetNetflixType(NetflixId, TitleType);
            else
                throw new ArgumentException("NetflixId and TitleType combination is not valid: No definition Netflix Type was identifiable.");

            NetflixId = AsyncHelpers.GetIdFromUrl(NetflixId);

            switch (TitleType)
            {
                case NetflixType.Movie:
                    return await Movies.GetCompleteTitle(NetflixId, OnUserBehalf);
                case NetflixType.Series:
                    throw new NotImplementedException();
                case NetflixType.SeriesSeason:
                    throw new NotImplementedException();
                case NetflixType.Programs:
                    throw new NotImplementedException();
                default: return null;
            }
        }

        public async Task<List<Award>> GetAwards(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            if (NetflixId.Contains("http"))
                TitleType = GetNetflixType(NetflixId, TitleType);
            else
                throw new ArgumentException("NetflixId and TitleType combination is not valid: No definition Netflix Type was identifiable.");


            NetflixId = AsyncHelpers.GetIdFromUrl(NetflixId);

            switch (TitleType)
            {
                case NetflixType.Movie:
                    return await Movies.GetAwards(NetflixId, null, OnUserBehalf);
                case NetflixType.Series:
                    throw new NotImplementedException();
                case NetflixType.SeriesSeason:
                    throw new NotImplementedException();
                case NetflixType.Programs:
                    throw new NotImplementedException();
                default: return null;
            }
        }

        public async Task<List<FormatAvailability>> GetFormatAvailability(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            if (NetflixId.Contains("http"))
                TitleType = GetNetflixType(NetflixId, TitleType);
            else
                throw new ArgumentException("NetflixId and TitleType combination is not valid: No definition Netflix Type was identifiable.");


            NetflixId = AsyncHelpers.GetIdFromUrl(NetflixId);

            switch (TitleType)
            {
                case NetflixType.Movie:
                    return await Movies.GetFormatAvailability(NetflixId, null, OnUserBehalf);
                case NetflixType.Series:
                    throw new NotImplementedException();
                case NetflixType.SeriesSeason:
                    throw new NotImplementedException();
                case NetflixType.Programs:
                    throw new NotImplementedException();
                default: return null;
            }
        }

        public async Task<String> GetSynopsis(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            if (NetflixId.Contains("http"))
                TitleType = GetNetflixType(NetflixId, TitleType);
            else
                throw new ArgumentException("NetflixId and TitleType combination is not valid: No definition Netflix Type was identifiable.");


            NetflixId = AsyncHelpers.GetIdFromUrl(NetflixId);

            switch (TitleType)
            {
                case NetflixType.Movie:
                    return await Movies.GetSynopsis(NetflixId, null, OnUserBehalf);
                case NetflixType.Series:
                    throw new NotImplementedException();
                case NetflixType.SeriesSeason:
                    throw new NotImplementedException();
                case NetflixType.Programs:
                    throw new NotImplementedException();
                default: return null;
            }
        }

        public async Task<List<ScreenFormats>> GetScreenFormats(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            if (NetflixId.Contains("http"))
                TitleType = GetNetflixType(NetflixId, TitleType);
            else
                throw new ArgumentException("NetflixId and TitleType combination is not valid: No definition Netflix Type was identifiable.");


            NetflixId = AsyncHelpers.GetIdFromUrl(NetflixId);

            switch (TitleType)
            {
                case NetflixType.Movie:
                    return await Movies.GetScreenFormats(NetflixId, null, OnUserBehalf);
                case NetflixType.Series:
                    throw new NotImplementedException();
                case NetflixType.SeriesSeason:
                    throw new NotImplementedException();
                case NetflixType.Programs:
                    throw new NotImplementedException();
                default: return null;
            }
        }


        /// <summary>
        /// Make a catalog/titles similar titles request.
        /// </summary>
        /// <param name="NetflixId">Full ID url</param>
        /// <returns></returns>
        public async Task<List<Title>> GetSimilarTitles(String NetflixId, Int32 Limit = 10, Int32 Page = 0, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            if (NetflixId.Contains("http"))
                TitleType = GetNetflixType(NetflixId, TitleType);
            else
                throw new ArgumentException("NetflixId and TitleType combination is not valid: No definition Netflix Type was identifiable.");

            NetflixId = AsyncHelpers.GetIdFromUrl(NetflixId);

            switch (TitleType)
            {
                case NetflixType.Movie:
                    return await Movies.GetSimilarTitles(NetflixId, null, Limit, Page, OnUserBehalf);
                case NetflixType.Series:
                    throw new NotImplementedException();
                case NetflixType.SeriesSeason:
                    throw new NotImplementedException();
                case NetflixType.Programs:
                    throw new NotImplementedException();
            }
            return null;
        }

        public FillMovies Movies = new FillMovies();
        public FillSeries Series = new FillSeries();
        public FillSeriesSeason SeriesSeason = new FillSeriesSeason();
        public FillPrograms Programs = new FillPrograms();

        private static NetflixType? GetNetflixType(String NetflixId, NetflixType? TitleType)
        {
            if (TitleType == null)
            {
                
                if (NetflixId.Contains("movies"))
                    TitleType = NetflixType.Movie;
                else if (NetflixId.Contains("series"))
                {
                    if (NetflixId.Contains("season"))
                        TitleType = NetflixType.SeriesSeason;
                    else
                        TitleType = NetflixType.Series;
                }
                else
                    TitleType = NetflixType.Programs;
                
            }
            return TitleType;
        }

    }

    public class FillMovies
    {
        public async Task<Title> GetBaseTitle(String NetflixId, Account na = null, Boolean OnUserBehalf = true)
        {
            NetflixLogin.CheckInformationSet();

            Dictionary<String, String> extraParams = new Dictionary<String, String>();

            String tokenSecret = "";
            if (OnUserBehalf)
            {
                if(na == null)
                    na = Netflix.SafeReturnUserInfo();
                if (na != null)
                {
                    tokenSecret = na.TokenSecret;
                    extraParams.Add("oauth_token", na.Token);
                }
            }

            String completetitleurl = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
                NetflixLogin.ConsumerKey,
                String.Format(NetflixConstants.MoviesBaseInfo, NetflixId),
                "GET",
                tokenSecret,
                extraParams);

            XDocument doc = await AsyncHelpers.LoadXDocumentAsync(completetitleurl);

            return (from movie
                    in doc.Elements("catalog_title")
                    select new Title(TitleExpansion.Minimal)
                    {
                        IdUrl = movie.Element("id").Value,
                        Year = (Int32)movie.Element("release_year"),
                        FullTitle = (String)movie.Element("title").Attribute("regular"),
                        ShortTitle = (String)movie.Element("title").Attribute("short"),
                        BoxArtUrlSmall = (String)movie.Element("box_art").Attribute("small"),
                        BoxArtUrlLarge = (String)movie.Element("box_art").Attribute("large"),
                        Rating = new Rating((from mpaa
                                            in movie.Elements("category")
                                             where mpaa.Attribute("scheme").Value == NetflixConstants.Schemas.CategoryMpaaRating
                                             select mpaa) ??
                                            (from tv
                                            in movie.Elements("category")
                                             where tv.Attribute("scheme").Value == NetflixConstants.Schemas.CategoryTvRating
                                             select tv)),
                        AverageRating = (Single)movie.Element("average_rating"),
                        RunTime = (Int32?)movie.Element("runtime"),
                        Genres = new List<String>(from genres
                                                  in movie.Elements("category")
                                                  where (String)genres.Attribute("scheme") == NetflixConstants.Schemas.CategoryGenre
                                                  select (String)genres.Attribute("term"))
                        
                    }).SingleOrDefault();
        }

        public async Task<Title> GetCompleteTitle(String NetflixId, Boolean OnUserBehalf = true)
        {
            NetflixLogin.CheckInformationSet();

            Dictionary<String, String> extraParams = new Dictionary<String, String>();

            String tokenSecret = "";
            Account na = null;
            if (OnUserBehalf)
            {
                na = Netflix.SafeReturnUserInfo();
                if (na != null)
                {
                    tokenSecret = na.TokenSecret;
                    extraParams.Add("oauth_token", na.Token);
                }
            }

            Title nfm = await GetBaseTitle(NetflixId, na);

            ///4) get synopsis
            var synopsis = GetSynopsis(NetflixId, na);

            ///6) get similar titles (add those to database in basic format, similar to AsyncHelpers.GetDatabaseMovies
            var similartitles = GetSimilarTitles(NetflixId, na);

            ///8) get awards
            var awards = GetAwards(NetflixId, na);

            ///9) screen format / title format
            var screenformats = GetScreenFormats(NetflixId, na);

            ///10) format availability
            var formatavailability = GetFormatAvailability(NetflixId, na);

            ///12) Actors
            var actors = GetActors(NetflixId, na);

            ///13) Directors
            var directors = GetDirectors(NetflixId, na);

            nfm.Synopsis = await synopsis;
            nfm.SimilarTitles = await similartitles;
            nfm.Awards = await awards;
            nfm.ScreenFormats = await screenformats;
            nfm.Formats = await formatavailability;
            nfm.Actors = await actors;
            nfm.Directors = await directors;
            nfm.completeness = TitleExpansion.Complete;

            return nfm;
        }
        
        public async Task<People> GetDirectors(String NetflixId, Account na = null, Boolean OnUserBehalf = true)
        {
            NetflixLogin.CheckInformationSet();

            Dictionary<String, String> extraParams = new Dictionary<String, String>();

            String tokenSecret = "";
            if (OnUserBehalf)
            {
                if (na == null)
                    na = Netflix.SafeReturnUserInfo();
                if (na != null)
                {
                    tokenSecret = na.TokenSecret;
                    extraParams.Add("oauth_token", na.Token);
                }
            }

            NetflixId = AsyncHelpers.GetIdFromUrl(NetflixId);

            String url = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
                NetflixLogin.ConsumerKey,
                String.Format(NetflixConstants.MoviesDirectors, NetflixId),
                "GET",
                tokenSecret,
                extraParams);

            var doc = AsyncHelpers.LoadXDocumentAsync(url);
            People people = new People();
            people.AddRange(from person
                            in (await doc).Element("people").Elements("person")
                            select new Person(PersonExpansion.Minimal)
                            {
                                IdUrl = person.Element("id").Value,
                                Name = person.Element("name").Value,
                                Bio = (String)person.Element("bio")
                            });
            return people;
        }
        
        public async Task<People> GetActors(String NetflixId, Account na = null, Boolean OnUserBehalf = true)
        {
            NetflixLogin.CheckInformationSet();

            Dictionary<String, String> extraParams = new Dictionary<String, String>();

            String tokenSecret = "";
            if (OnUserBehalf)
            {
                if (na == null)
                    na = Netflix.SafeReturnUserInfo();
                if (na != null)
                {
                    tokenSecret = na.TokenSecret;
                    extraParams.Add("oauth_token", na.Token);
                }
            }

            NetflixId = AsyncHelpers.GetIdFromUrl(NetflixId);

            String url = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
                NetflixLogin.ConsumerKey,
                String.Format(NetflixConstants.MoviesCast, NetflixId),
                "GET",
                tokenSecret,
                extraParams);

            var doc = AsyncHelpers.LoadXDocumentAsync(url);
            People people = new People();
            people.AddRange(from person
                            in (await doc).Element("people").Elements("person")
                            select new Person(PersonExpansion.Minimal)
                            {
                                IdUrl = person.Element("id").Value,
                                Name = person.Element("name").Value,
                                Bio = (String)person.Element("bio")   
                            });
            return people;
        }

        public async Task<List<Title>> GetSimilarTitles(String NetflixId, Account na = null, Int32 Limit = 10, Int32 Page = 0, Boolean OnUserBehalf = true)
        {
            NetflixLogin.CheckInformationSet();

            Dictionary<String, String> extraParams = new Dictionary<String, String>();
            extraParams.Add("start_index", Page.ToString());
            extraParams.Add("max_results", Limit.ToString());
            
            String tokenSecret = "";
            if (OnUserBehalf)
            {
                if (na == null)
                    na = Netflix.SafeReturnUserInfo();
                if (na != null)
                {
                    tokenSecret = na.TokenSecret;
                    extraParams.Add("oauth_token", na.Token);
                }
            }
        
            NetflixId = AsyncHelpers.GetIdFromUrl(NetflixId);

            String url = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
                NetflixLogin.ConsumerKey,
                String.Format(NetflixConstants.MoviesSimilars, NetflixId),
                "GET",
                tokenSecret,
                extraParams);

            var doc = AsyncHelpers.LoadXDocumentAsync(url);
            List<Title> movies = new List<Title>(Limit);
            movies.AddRange(from movie
                            in (await doc).Element("similars").Elements("similars_item")
                            select new Title(TitleExpansion.Minimal)
                            {
                                IdUrl = movie.Element("id").Value,
                                Year = (Int32)movie.Element("release_year"),
                                FullTitle = (String)movie.Element("title").Attribute("regular"),
                                AverageRating = (Single)movie.Element("average_rating"),
                                ShortTitle = (String)movie.Element("title").Attribute("short"),
                                BoxArtUrlSmall = (String)movie.Element("box_art").Attribute("small"),
                                BoxArtUrlLarge = (String)movie.Element("box_art").Attribute("large"),
                                NetflixType = (movie.Element("id").Value.Contains("movie") ? NetflixType.Movie :
                                    movie.Element("id").Value.Contains("programs") ? NetflixType.Programs :
                                    movie.Element("id").Value.Contains("series") && movie.Element("id").Value.Contains("season") ?
                                        NetflixType.SeriesSeason : NetflixType.Series)
                            });
            return movies;
        }

        public async Task<List<Award>> GetAwards(String NetflixId, Account na = null, Boolean OnUserBehalf = true)
        {
            NetflixLogin.CheckInformationSet();

            Dictionary<String, String> extraParams = new Dictionary<String, String>();

            String tokenSecret = "";
            if (OnUserBehalf)
            {
                if (na == null)
                    na = Netflix.SafeReturnUserInfo();
                if (na != null)
                {
                    tokenSecret = na.TokenSecret;
                    extraParams.Add("oauth_token", na.Token);
                }
            }

            NetflixId = AsyncHelpers.GetIdFromUrl(NetflixId);

            String url = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
                NetflixLogin.ConsumerKey,
                String.Format(NetflixConstants.MoviesAwards, NetflixId),
                "GET",
                tokenSecret,
                extraParams);

            XDocument doc = await AsyncHelpers.LoadXDocumentAsync(url);
            
            var awardnominees = from awards
                         in doc.Element("awards").Elements("award_nominee")
                         select new Award()
                         {///get all the nominees first?
                             Year = (Int32?)awards.Attribute("year"),
                             AwardName = (String)awards.Element("category").Attribute("term"),
                             PersonId = (awards.Element("link") != null ?
                                (String)awards.Element("link").Attribute("href") : null),
                             Type = (AwardType)Enum.Parse(typeof(AwardType), 
                                awards.Element("category").Attribute("scheme").Value.
                                Split(new[]{"/"}, StringSplitOptions.RemoveEmptyEntries)[awards.Element("category").Attribute("scheme").Value.
                                    Split(new[]{"/"}, StringSplitOptions.RemoveEmptyEntries).Length - 1], true),
                             Winner = false
                         };
            var awardwinners = from awards
                         in doc.Element("awards").Elements("award_winner")
                         select new Award()
                         {
                             Year = (Int32?)awards.Attribute("year"),
                             AwardName = (String)awards.Element("category").Attribute("term"),
                             PersonId = (awards.Element("link") != null ? 
                                (String)awards.Element("link").Attribute("href") : null),
                             Type = (AwardType)Enum.Parse(typeof(AwardType),
                                awards.Element("category").Attribute("scheme").Value.
                                Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries)[awards.Element("category").Attribute("scheme").Value.
                                    Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries).Length - 1], true),
                             Winner = true
                         };
            List<Award> a = new List<Award>();
            a.AddRange(awardnominees);
            a.AddRange(awardwinners);
            return a;
        }

        public async Task<List<FormatAvailability>> GetFormatAvailability(String NetflixId, Account na = null, Boolean OnUserBehalf = true)
        {
            NetflixLogin.CheckInformationSet();

            Dictionary<String, String> extraParams = new Dictionary<String, String>();

            String tokenSecret = "";
            if (OnUserBehalf)
            {
                if (na == null)
                    na = Netflix.SafeReturnUserInfo();
                if (na != null)
                {
                    tokenSecret = na.TokenSecret;
                    extraParams.Add("oauth_token", na.Token);
                }
            }

            NetflixId = AsyncHelpers.GetIdFromUrl(NetflixId);

            String url = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
                NetflixLogin.ConsumerKey,
                String.Format(NetflixConstants.MoviesFormatAvailability, NetflixId),
                "GET",
                tokenSecret,
                extraParams);

            XDocument doc = await AsyncHelpers.LoadXDocumentAsync(url);

            var formatavailability = from formats
                                in doc.Element("delivery_formats").Elements("availability")
                                select new FormatAvailability()
                                {
                                    AvailableFrom = formats.Attribute("available_from") == null || (String)formats.Attribute("available_from") == "" ?
                                            null : (Nullable<DateTime>)GeneralHelpers.FromUnixTime(Int32.Parse((String)formats.Attribute("available_from"))),
                                    AvailableUntil = formats.Attribute("available_until") == null || (String)formats.Attribute("available_until") == "" ?
                                            null : (Nullable<DateTime>)GeneralHelpers.FromUnixTime(Int32.Parse((String)formats.Attribute("available_until"))),
                                    Format = (String)formats.Element("category").Attribute("term")
                                };

            List<FormatAvailability> fa = new List<FormatAvailability>();
            fa.AddRange(formatavailability);
            return fa;
        }

        public async Task<List<ScreenFormats>> GetScreenFormats(String NetflixId, Account na = null, Boolean OnUserBehalf = true)
        {
            NetflixLogin.CheckInformationSet();

            Dictionary<String, String> extraParams = new Dictionary<String, String>();

            String tokenSecret = "";
            if (OnUserBehalf)
            {
                if (na == null)
                    na = Netflix.SafeReturnUserInfo();
                if (na != null)
                {
                    tokenSecret = na.TokenSecret;
                    extraParams.Add("oauth_token", na.Token);
                }
            }

            NetflixId = AsyncHelpers.GetIdFromUrl(NetflixId);

            String url = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
                NetflixLogin.ConsumerKey,
                String.Format(NetflixConstants.MoviesScreenFormat, NetflixId),
                "GET",
                tokenSecret,
                extraParams);

            XDocument doc = await AsyncHelpers.LoadXDocumentAsync(url);

            var screenformats = from formats
                                in doc.Element("screen_formats").Elements("screen_format")
                                select new ScreenFormats()
                                {
                                    Format = (from format
                                            in formats.Elements("category")
                                            where (String)format.Attribute("scheme") == NetflixConstants.Schemas.LinkTitleFormat
                                            select (String)format.Attribute("term")).FirstOrDefault(),
                                    ScreenFormat = (from screenformat
                                            in formats.Elements("category")
                                            where (String)screenformat.Attribute("scheme") == NetflixConstants.Schemas.LinkScreenFormat
                                            select (String)screenformat.Attribute("term")).FirstOrDefault()
                                };

            List<ScreenFormats> sf = new List<ScreenFormats>();
            sf.AddRange(screenformats);
            return sf;
        }

        public async Task<String> GetSynopsis(String NetflixId, Account na = null, Boolean OnUserBehalf = true)
        {
            NetflixLogin.CheckInformationSet();

            Dictionary<String, String> extraParams = new Dictionary<String, String>();

            String tokenSecret = "";
            if (OnUserBehalf)
            {
                if (na == null)
                    na = Netflix.SafeReturnUserInfo();
                if (na != null)
                {
                    tokenSecret = na.TokenSecret;
                    extraParams.Add("oauth_token", na.Token);
                }
            }

            NetflixId = AsyncHelpers.GetIdFromUrl(NetflixId);

            String url = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
                NetflixLogin.ConsumerKey,
                String.Format(NetflixConstants.MoviesSynopsis, NetflixId),
                "GET",
                tokenSecret,
                extraParams);

            XDocument doc = await AsyncHelpers.LoadXDocumentAsync(url);

            return (String)doc.Element("synopsis");
        }
    }
    public class FillSeries
    {
    }

    public class FillSeriesSeason
    { 
    }
    public class FillPrograms
    {
    }
}
