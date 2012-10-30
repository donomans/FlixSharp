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
        public FillTitles Titles = new FillTitles();
        public FillPeople People = new FillPeople();

        internal static NetflixType? GetNetflixType(String NetflixId, NetflixType? TitleType)
        {
            if (NetflixId.Contains("http") && TitleType == null)
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
            else if (TitleType == null)
                throw new ArgumentException("NetflixId and TitleType combination is not valid: No definitive Netflix Type was identifiable.");
            else
                return TitleType;
        }
        internal static Dictionary<String, String> GetTokens(Boolean OnUserBehalf, out String TokenSecret)
        {
            Dictionary<String, String> extraParams = new Dictionary<String, String>();
            TokenSecret = "";
            if (OnUserBehalf)
            {
                Account NetflixAccount = Netflix.SafeReturnUserInfo();
                if (NetflixAccount != null)
                {
                    TokenSecret = NetflixAccount.TokenSecret;
                    extraParams.Add("oauth_token", NetflixAccount.Token);
                }
            }
            return extraParams;
        }
        internal static Dictionary<String, String> GetTokens(Boolean OnUserBehalf, Account NetflixAccount, out String TokenSecret)
        {
            Dictionary<String, String> extraParams = new Dictionary<String, String>();
            TokenSecret = "";
            if (OnUserBehalf)
            {
                if (NetflixAccount != null)
                {
                    TokenSecret = NetflixAccount.TokenSecret;
                    extraParams.Add("oauth_token", NetflixAccount.Token);
                }
            }
            return extraParams;
        }

    }

    public class FillTitles
    {
        public async Task<Title> GetCompleteTitle(String NetflixIdUrl, Boolean OnUserBehalf)
        {
            return await GetCompleteTitle(NetflixIdUrl, null, OnUserBehalf);
        }
        public async Task<Title> GetCompleteTitle(String NetflixId, NetflixType? TitleType = null, Boolean OnUserBehalf = true)
        {
            NetflixLogin.CheckInformationSet();

            Account NetflixAccount = null;
            if (OnUserBehalf)
                NetflixAccount = Netflix.SafeReturnUserInfo();

            TitleType = NetflixFill.GetNetflixType(NetflixId, TitleType);

            NetflixId = GeneralHelpers.GetIdFromUrl(NetflixId);

            var nfm = GetBaseTitle(NetflixId, NetflixAccount, OnUserBehalf, TitleType);

            ///4) get synopsis
            var synopsis = GetSynopsis(NetflixId, NetflixAccount, OnUserBehalf, TitleType);

            ///6) get similar titles (add those to database in basic format, similar to AsyncHelpers.GetDatabaseMovies
            var similartitles = GetSimilarTitles(NetflixId, NetflixAccount, OnUserBehalf, 20, 0, TitleType);

            ///8) get awards
            var awards = GetAwards(NetflixId, NetflixAccount, OnUserBehalf, TitleType);

            ///9) screen format / title format
            var screenformats = GetScreenFormats(NetflixId, NetflixAccount, OnUserBehalf, TitleType);

            ///10) format availability
            var formatavailability = GetFormatAvailability(NetflixId, NetflixAccount, OnUserBehalf, TitleType);

            ///12) Actors
            var actors = GetActors(NetflixId, NetflixAccount, OnUserBehalf, TitleType);

            ///13) Directors
            var directors = GetDirectors(NetflixId, NetflixAccount, OnUserBehalf, TitleType);

            if (OnUserBehalf)
            {
                ///14) User Rating for title (if on user behalf)
                //var userrating = GetUserRating(NetflixId, NetflixAccount, OnUserBehalf, TitleType);
            }
            Title title = await nfm;
            title.Synopsis = await synopsis;
            title.Awards = await awards;
            title.ScreenFormats = await screenformats;
            title.Formats = await formatavailability;
            title.Actors = await actors;
            title.Directors = await directors;
            title.SimilarTitles = await similartitles;

            title.completeness = TitleExpansion.Complete;

            return title;
        }

        public async Task<Title> GetExpandedTitle(String NetflixIdUrl, Boolean OnUserBehalf)
        {
            return await GetExpandedTitle(NetflixIdUrl, null, OnUserBehalf);
        }
        public async Task<Title> GetExpandedTitle(String NetflixId, NetflixType? TitleType = null, Boolean OnUserBehalf = true)
        {
            NetflixLogin.CheckInformationSet();

            Account NetflixAccount = null;
            if (OnUserBehalf)
                NetflixAccount = Netflix.SafeReturnUserInfo();

            TitleType = NetflixFill.GetNetflixType(NetflixId, TitleType);

            NetflixId = GeneralHelpers.GetIdFromUrl(NetflixId);

            var nfm = GetBaseTitle(NetflixId, NetflixAccount, OnUserBehalf, TitleType);

            ///4) get synopsis
            var synopsis = GetSynopsis(NetflixId, NetflixAccount, OnUserBehalf, TitleType);

            ///9) screen format / title format
            var screenformats = GetScreenFormats(NetflixId, NetflixAccount, OnUserBehalf, TitleType);

            ///10) format availability
            var formatavailability = GetFormatAvailability(NetflixId, NetflixAccount, OnUserBehalf, TitleType);

            ///12) Actors
            var actors = GetActors(NetflixId, NetflixAccount, OnUserBehalf, TitleType);

            ///13) Directors
            var directors = GetDirectors(NetflixId, NetflixAccount, OnUserBehalf, TitleType);

            Title title = await nfm;
            title.Synopsis = await synopsis;
            title.ScreenFormats = await screenformats;
            title.Formats = await formatavailability;
            title.Actors = await actors;
            title.Directors = await directors;

            title.completeness = TitleExpansion.Expanded;

            return title;
        }

        public async Task<Title> GetBaseTitle(String NetflixIdUrl, Boolean OnUserBehalf)
        {
            return await GetBaseTitle(NetflixIdUrl, OnUserBehalf, null);
        }
        public async Task<Title> GetBaseTitle(String NetflixId, NetflixType TitleType, Boolean OnUserBehalf = true)
        {
            return await GetBaseTitle(NetflixId, OnUserBehalf, TitleType);
        }
        public async Task<Title> GetBaseTitle(String NetflixId, Account NetflixAccount, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            NetflixLogin.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = NetflixFill.GetTokens(OnUserBehalf, NetflixAccount, out TokenSecret);

            return await GetBaseTitle(NetflixId, extraParams, TokenSecret, TitleType);
        }
        public async Task<Title> GetBaseTitle(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            NetflixLogin.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = NetflixFill.GetTokens(OnUserBehalf, out TokenSecret);

            return await GetBaseTitle(NetflixId, extraParams, TokenSecret, TitleType);
        }
        private async Task<Title> GetBaseTitle(String NetflixId, Dictionary<String, String> ExtraParams, String TokenSecret, NetflixType? TitleType = null)
        {
            TitleType = NetflixFill.GetNetflixType(NetflixId, TitleType);
            NetflixId = GeneralHelpers.GetIdFromUrl(NetflixId);

            String url = "";
            switch (TitleType)
            {
                case NetflixType.Movie:
                    url = String.Format(NetflixConstants.MoviesBaseInfo, NetflixId);
                    break;
                case NetflixType.Series:
                    url = String.Format(NetflixConstants.SeriesBaseInfo, NetflixId);
                    break;
                case NetflixType.SeriesSeason:
                case NetflixType.Programs:
                default: return null;
            }

            url = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
                NetflixLogin.ConsumerKey,
                url,
                "GET",
                TokenSecret,
                ExtraParams);


            XDocument doc = await AsyncHelpers.LoadXDocumentAsync(url);

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

        public async Task<People> GetActors(String NetflixIdUrl, Boolean OnUserBehalf)
        {
            return await GetActors(NetflixIdUrl, OnUserBehalf, null);
        }
        public async Task<People> GetActors(String NetflixId, NetflixType TitleType, Boolean OnUserBehalf = true)
        {
            return await GetActors(NetflixId, OnUserBehalf, TitleType);
        }
        public async Task<People> GetActors(String NetflixId, Account NetflixAccount, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            NetflixLogin.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = NetflixFill.GetTokens(OnUserBehalf, NetflixAccount, out TokenSecret);

            return await GetActors(NetflixId, extraParams, TokenSecret, TitleType);
        }
        public async Task<People> GetActors(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            NetflixLogin.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = NetflixFill.GetTokens(OnUserBehalf, out TokenSecret);

            return await GetActors(NetflixId, extraParams, TokenSecret, TitleType);
        }
        private async Task<People> GetActors(String NetflixId, Dictionary<String, String> ExtraParams, String TokenSecret, NetflixType? TitleType = null)
        {
            TitleType = NetflixFill.GetNetflixType(NetflixId, TitleType);
            NetflixId = GeneralHelpers.GetIdFromUrl(NetflixId);

            String url = "";
            switch (TitleType)
            {
                case NetflixType.Movie:
                    url = String.Format(NetflixConstants.MoviesCast, NetflixId);
                    break;
                case NetflixType.Series:
                    url = String.Format(NetflixConstants.SeriesCast, NetflixId);
                    break;
                case NetflixType.SeriesSeason:
                case NetflixType.Programs:
                default: return null;
            }

            url = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
                NetflixLogin.ConsumerKey,
                url,
                "GET",
                TokenSecret,
                ExtraParams);


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

        public async Task<People> GetDirectors(String NetflixIdUrl, Boolean OnUserBehalf)
        {
            return await GetDirectors(NetflixIdUrl, OnUserBehalf, null);
        }
        public async Task<People> GetDirectors(String NetflixId, NetflixType TitleType, Boolean OnUserBehalf = true)
        {
            return await GetDirectors(NetflixId, OnUserBehalf, TitleType);
        }
        public async Task<People> GetDirectors(String NetflixId, Account NetflixAccount, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            NetflixLogin.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = NetflixFill.GetTokens(OnUserBehalf, NetflixAccount, out TokenSecret);

            return await GetDirectors(NetflixId, extraParams, TokenSecret, TitleType);
        }
        public async Task<People> GetDirectors(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            NetflixLogin.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = NetflixFill.GetTokens(OnUserBehalf, out TokenSecret);

            return await GetDirectors(NetflixId, extraParams, TokenSecret, TitleType);
        }
        private async Task<People> GetDirectors(String NetflixId, Dictionary<String, String> ExtraParams, String TokenSecret, NetflixType? TitleType = null)
        {
            TitleType = NetflixFill.GetNetflixType(NetflixId, TitleType);
            NetflixId = GeneralHelpers.GetIdFromUrl(NetflixId);

            String url = "";
            switch (TitleType)
            {
                case NetflixType.Movie:
                    url = String.Format(NetflixConstants.MoviesDirectors, NetflixId);
                    break;
                case NetflixType.Series:
                    url = String.Format(NetflixConstants.SeriesDirectors, NetflixId);
                    break;
                case NetflixType.SeriesSeason:
                case NetflixType.Programs:
                default: return null;
            }

            url = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
                NetflixLogin.ConsumerKey,
                url,
                "GET",
                TokenSecret,
                ExtraParams);

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

        public async Task<List<Award>> GetAwards(String NetflixIdUrl, Boolean OnUserBehalf)
        {
            return await GetAwards(NetflixIdUrl, OnUserBehalf, null);
        }
        public async Task<List<Award>> GetAwards(String NetflixId, NetflixType TitleType, Boolean OnUserBehalf = true)
        {
            return await GetAwards(NetflixId, OnUserBehalf, TitleType);
        }
        public async Task<List<Award>> GetAwards(String NetflixId, Account NetflixAccount, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            NetflixLogin.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = NetflixFill.GetTokens(OnUserBehalf, NetflixAccount, out TokenSecret);

            return await GetAwards(NetflixId, extraParams, TokenSecret, TitleType);
        }
        public async Task<List<Award>> GetAwards(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            NetflixLogin.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = NetflixFill.GetTokens(OnUserBehalf, out TokenSecret);

            return await GetAwards(NetflixId, extraParams, TokenSecret, TitleType);
        }
        private async Task<List<Award>> GetAwards(String NetflixId, Dictionary<String, String> ExtraParams, String TokenSecret, NetflixType? TitleType = null)
        {
            TitleType = NetflixFill.GetNetflixType(NetflixId, TitleType);
            NetflixId = GeneralHelpers.GetIdFromUrl(NetflixId);


            String url = "";
            switch (TitleType)
            {
                case NetflixType.Movie:
                    url = String.Format(NetflixConstants.MoviesAwards, NetflixId);
                    break;
                case NetflixType.Series:
                    url = String.Format(NetflixConstants.SeriesAwards, NetflixId);
                    break;
                case NetflixType.SeriesSeason:
                case NetflixType.Programs:
                default: return null;
            }

            url = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
                NetflixLogin.ConsumerKey,
                url,
                "GET",
                TokenSecret,
                ExtraParams);


            XDocument doc = await AsyncHelpers.LoadXDocumentAsync(url);

            var awardnominees = from awards
                         in doc.Element("awards").Elements("award_nominee")
                                select new Award()
                                {
                                    Year = awards.Attribute("year") == null || (String)awards.Attribute("year") == "" ? null : (Int32?)awards.Attribute("year"),
                                    AwardName = (String)awards.Element("category").Attribute("term"),
                                    PersonId = (awards.Element("link") != null ?
                                       (String)awards.Element("link").Attribute("href") : null),
                                    Type = (AwardType)Enum.Parse(typeof(AwardType),
                                       awards.Element("category").Attribute("scheme").Value.
                                       Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries)[awards.Element("category").Attribute("scheme").Value.
                                           Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries).Length - 1], true),
                                    Winner = false
                                };
            var awardwinners = from awards
                         in doc.Element("awards").Elements("award_winner")
                               select new Award()
                               {
                                   Year = awards.Attribute("year") == null || (String)awards.Attribute("year") == "" ? null : (Int32?)awards.Attribute("year"),
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

        public async Task<List<FormatAvailability>> GetFormatAvailability(String NetflixIdUrl, Boolean OnUserBehalf)
        {
            return await GetFormatAvailability(NetflixIdUrl, OnUserBehalf, null);
        }
        public async Task<List<FormatAvailability>> GetFormatAvailability(String NetflixId, NetflixType TitleType, Boolean OnUserBehalf = true)
        {
            return await GetFormatAvailability(NetflixId, OnUserBehalf, TitleType);
        }
        public async Task<List<FormatAvailability>> GetFormatAvailability(String NetflixId, Account NetflixAccount, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            NetflixLogin.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = NetflixFill.GetTokens(OnUserBehalf, NetflixAccount, out TokenSecret);

            return await GetFormatAvailability(NetflixId, extraParams, TokenSecret, TitleType);
        }
        public async Task<List<FormatAvailability>> GetFormatAvailability(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            NetflixLogin.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = NetflixFill.GetTokens(OnUserBehalf, out TokenSecret);

            return await GetFormatAvailability(NetflixId, extraParams, TokenSecret, TitleType);
        }
        private async Task<List<FormatAvailability>> GetFormatAvailability(String NetflixId, Dictionary<String, String> ExtraParams, String TokenSecret, NetflixType? TitleType = null)
        {
            TitleType = NetflixFill.GetNetflixType(NetflixId, TitleType);
            NetflixId = GeneralHelpers.GetIdFromUrl(NetflixId);

            String url = "";
            switch (TitleType)
            {
                case NetflixType.Movie:
                    url = String.Format(NetflixConstants.MoviesFormatAvailability, NetflixId);
                    break;
                case NetflixType.Series:
                    url = String.Format(NetflixConstants.SeriesFormatAvailability, NetflixId);
                    break;
                case NetflixType.SeriesSeason:
                case NetflixType.Programs:
                default: return null;
            }

            url = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
                NetflixLogin.ConsumerKey,
                url,
                "GET",
                TokenSecret,
                ExtraParams);

            XDocument doc = await AsyncHelpers.LoadXDocumentAsync(url);

            var formatavailability = from formats
                                in doc.Element("delivery_formats").Elements("availability")
                                     select new FormatAvailability()
                                     {
                                         AvailableFrom = formats.Attribute("available_from") == null || (String)formats.Attribute("available_from") == "" ?
                                                 null : (Nullable<DateTime>)GeneralHelpers.FromUnixTime(Int64.Parse((String)formats.Attribute("available_from"))),
                                         AvailableUntil = formats.Attribute("available_until") == null || (String)formats.Attribute("available_until") == "" ?
                                                 null : (Nullable<DateTime>)GeneralHelpers.FromUnixTime(Int64.Parse((String)formats.Attribute("available_until"))),
                                         Format = (String)formats.Element("category").Attribute("term")
                                     };

            return formatavailability.ToList();
        }

        public async Task<String> GetSynopsis(String NetflixIdUrl, Boolean OnUserBehalf)
        {
            return await GetSynopsis(NetflixIdUrl, OnUserBehalf, null);
        }
        public async Task<String> GetSynopsis(String NetflixId, NetflixType TitleType, Boolean OnUserBehalf = true)
        {
            return await GetSynopsis(NetflixId, OnUserBehalf, TitleType);
        }
        public async Task<String> GetSynopsis(String NetflixId, Account NetflixAccount, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            NetflixLogin.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = NetflixFill.GetTokens(OnUserBehalf, NetflixAccount, out TokenSecret);

            return await GetSynopsis(NetflixId, extraParams, TokenSecret, TitleType);
        }
        public async Task<String> GetSynopsis(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            NetflixLogin.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = NetflixFill.GetTokens(OnUserBehalf, out TokenSecret);

            return await GetSynopsis(NetflixId, extraParams, TokenSecret, TitleType);
        }
        private async Task<String> GetSynopsis(String NetflixId, Dictionary<String, String> ExtraParams, String TokenSecret, NetflixType? TitleType = null)
        {
            TitleType = NetflixFill.GetNetflixType(NetflixId, TitleType);
            NetflixId = GeneralHelpers.GetIdFromUrl(NetflixId);

            String url = "";
            switch (TitleType)
            {
                case NetflixType.Movie:
                    url = String.Format(NetflixConstants.MoviesSynopsis, NetflixId);
                    break;
                case NetflixType.Series:
                    url = String.Format(NetflixConstants.SeriesSynopsis, NetflixId);
                    break;
                case NetflixType.SeriesSeason:
                case NetflixType.Programs:
                default: return null;
            }

            url = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
                NetflixLogin.ConsumerKey,
                url,
                "GET",
                TokenSecret,
                ExtraParams);

            XDocument doc = await AsyncHelpers.LoadXDocumentAsync(url);

            return (String)doc.Element("synopsis");
        }

        public async Task<List<ScreenFormats>> GetScreenFormats(String NetflixIdUrl, Boolean OnUserBehalf)
        {
            return await GetScreenFormats(NetflixIdUrl, OnUserBehalf, null);
        }
        public async Task<List<ScreenFormats>> GetScreenFormats(String NetflixId, NetflixType TitleType, Boolean OnUserBehalf = true)
        {
            return await GetScreenFormats(NetflixId, OnUserBehalf, TitleType);
        }
        public async Task<List<ScreenFormats>> GetScreenFormats(String NetflixId, Account NetflixAccount, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            NetflixLogin.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = NetflixFill.GetTokens(OnUserBehalf, NetflixAccount, out TokenSecret);

            return await GetScreenFormats(NetflixId, extraParams, TokenSecret, TitleType);
        }
        public async Task<List<ScreenFormats>> GetScreenFormats(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            NetflixLogin.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = NetflixFill.GetTokens(OnUserBehalf, out TokenSecret);

            return await GetScreenFormats(NetflixId, extraParams, TokenSecret, TitleType);
        }
        private async Task<List<ScreenFormats>> GetScreenFormats(String NetflixId, Dictionary<String, String> ExtraParams, String TokenSecret, NetflixType? TitleType = null)
        {
            TitleType = NetflixFill.GetNetflixType(NetflixId, TitleType);
            NetflixId = GeneralHelpers.GetIdFromUrl(NetflixId);

            String url = "";
            switch (TitleType)
            {
                case NetflixType.Movie:
                    url = String.Format(NetflixConstants.MoviesScreenFormat, NetflixId);
                    break;
                case NetflixType.Series:
                    url = String.Format(NetflixConstants.SeriesScreenFormat, NetflixId);
                    break;
                case NetflixType.SeriesSeason:
                case NetflixType.Programs:
                default: return null;
            }

            url = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
                NetflixLogin.ConsumerKey,
                url,
                "GET",
                TokenSecret,
                ExtraParams);

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

            return screenformats.ToList();
        }

        public async Task<List<Title>> GetSimilarTitles(String NetflixIdUrl, Boolean OnUserBehalf, Int32 Limit = 10, Int32 Page = 0)
        {
            return await GetSimilarTitles(NetflixIdUrl, OnUserBehalf, Limit, Page, null);
        }
        public async Task<List<Title>> GetSimilarTitles(String NetflixId, NetflixType TitleType, Boolean OnUserBehalf = true, Int32 Limit = 10, Int32 Page = 0)
        {
            return await GetSimilarTitles(NetflixId, OnUserBehalf, Limit, Page, TitleType);
        }
        public async Task<List<Title>> GetSimilarTitles(String NetflixId, Account NetflixAccount, Boolean OnUserBehalf = true, Int32 Limit = 10, Int32 Page = 0, NetflixType? TitleType = null)
        {
            NetflixLogin.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = NetflixFill.GetTokens(OnUserBehalf, NetflixAccount, out TokenSecret);

            return await GetSimilarTitles(NetflixId, extraParams, TokenSecret, Limit, Page, TitleType);
        }
        public async Task<List<Title>> GetSimilarTitles(String NetflixId, Boolean OnUserBehalf = true, Int32 Limit = 10, Int32 Page = 0, NetflixType? TitleType = null)
        {
            NetflixLogin.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = NetflixFill.GetTokens(OnUserBehalf, out TokenSecret);

            return await GetSimilarTitles(NetflixId, extraParams, TokenSecret, Limit, Page, TitleType);
        }
        private async Task<List<Title>> GetSimilarTitles(String NetflixId, Dictionary<String, String> ExtraParams, String TokenSecret, Int32 Limit = 10, Int32 Page = 0, NetflixType? TitleType = null)
        {
            TitleType = NetflixFill.GetNetflixType(NetflixId, TitleType);
            NetflixId = GeneralHelpers.GetIdFromUrl(NetflixId);


            String url = "";
            switch (TitleType)
            {
                case NetflixType.Movie:
                    url = String.Format(NetflixConstants.MoviesSimilars, NetflixId);
                    break;
                case NetflixType.Series:
                    url = String.Format(NetflixConstants.SeriesSimilars, NetflixId);
                    break;
                case NetflixType.SeriesSeason:
                case NetflixType.Programs:
                default: return null;
            }

            url = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
                NetflixLogin.ConsumerKey,
                url,
                "GET",
                TokenSecret,
                ExtraParams);

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

        public async Task<List<Title>> GetRelatedTitles(String NetflixId, Int32 Limit = 10, Int32 Page = 0, Account NetflixAccount = null, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            throw new NotImplementedException();

            NetflixLogin.CheckInformationSet();

            Dictionary<String, String> extraParams = new Dictionary<String, String>();
            extraParams.Add("start_index", Page.ToString());
            extraParams.Add("max_results", Limit.ToString());

            String tokenSecret = "";
            if (OnUserBehalf)
            {
                if (NetflixAccount == null)
                    NetflixAccount = Netflix.SafeReturnUserInfo();
                if (NetflixAccount != null)
                {
                    tokenSecret = NetflixAccount.TokenSecret;
                    extraParams.Add("oauth_token", NetflixAccount.Token);
                }
            }

            TitleType = NetflixFill.GetNetflixType(NetflixId, TitleType);
            NetflixId = GeneralHelpers.GetIdFromUrl(NetflixId);

            String url = "";
            switch (TitleType)
            {
                case NetflixType.Movie:
                    url = String.Format(NetflixConstants.MoviesSynopsis, NetflixId);
                    break;
                case NetflixType.Series:
                    url = String.Format(NetflixConstants.SeriesSynopsis, NetflixId);
                    break;
                case NetflixType.SeriesSeason:
                case NetflixType.Programs:
                default: return null;
            }

            url = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
                NetflixLogin.ConsumerKey,
                url,
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
    }
    public class FillPeople
    {
        public async Task<Person> GetCompletePerson(String NetflixIdUrl, Boolean OnUserBehalf)
        {
            return await GetCompletePerson(NetflixIdUrl, null, OnUserBehalf);
        }
        public async Task<Person> GetCompletePerson(String NetflixId, NetflixType? TitleType = null, Boolean OnUserBehalf = true)
        {
            NetflixLogin.CheckInformationSet();

            Account NetflixAccount = null;
            if (OnUserBehalf)
                NetflixAccount = Netflix.SafeReturnUserInfo();

            TitleType = NetflixFill.GetNetflixType(NetflixId, TitleType);

            NetflixId = GeneralHelpers.GetIdFromUrl(NetflixId);

            var nfp = GetBasePerson(NetflixId, NetflixAccount, OnUserBehalf, TitleType);

            ///1) get filmography
            
            Person person = await nfp;
            person.completeness = PersonExpansion.Complete;

            return person;
        }

        public async Task<Person> GetBasePerson(String NetflixIdUrl, Boolean OnUserBehalf)
        {
            return await GetBasePerson(NetflixIdUrl, OnUserBehalf, null);
        }
        public async Task<Person> GetBasePerson(String NetflixId, NetflixType TitleType, Boolean OnUserBehalf = true)
        {
            return await GetBasePerson(NetflixId, OnUserBehalf, TitleType);
        }
        public async Task<Person> GetBasePerson(String NetflixId, Account NetflixAccount, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            NetflixLogin.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = NetflixFill.GetTokens(OnUserBehalf, NetflixAccount, out TokenSecret);

            return await GetBasePerson(NetflixId, extraParams, TokenSecret, TitleType);
        }
        public async Task<Person> GetBasePerson(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            NetflixLogin.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = NetflixFill.GetTokens(OnUserBehalf, out TokenSecret);

            return await GetBasePerson(NetflixId, extraParams, TokenSecret, TitleType);
        }
        private async Task<Person> GetBasePerson(String NetflixId, Dictionary<String, String> ExtraParams, String TokenSecret, NetflixType? TitleType = null)
        {
            TitleType = NetflixFill.GetNetflixType(NetflixId, TitleType);
            NetflixId = GeneralHelpers.GetIdFromUrl(NetflixId);

            String url = "";
            switch (TitleType)
            {
                case NetflixType.Movie:
                    url = String.Format(NetflixConstants.PeopleBaseInfo, NetflixId);
                    break;
                case NetflixType.Series:
                    url = String.Format(NetflixConstants.PeopleBaseInfo, NetflixId);
                    break;
                case NetflixType.SeriesSeason:
                case NetflixType.Programs:
                default: return null;
            }

            url = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
                NetflixLogin.ConsumerKey,
                url,
                "GET",
                TokenSecret,
                ExtraParams);

            var doc = AsyncHelpers.LoadXDocumentAsync(url);
            Person p = (from person
                            in (await doc).Element("people").Elements("person")
                        select new Person(PersonExpansion.Minimal)
                        {
                            IdUrl = person.Element("id").Value,
                            Name = person.Element("name").Value,
                            Bio = (String)person.Element("bio")
                        }).SingleOrDefault();
            return p;
        }
    }

    //public class FillMovies
    //{ 
    //    public async Task<List<Title>> GetRelatedTitles(String NetflixId, Account na = null, Int32 Limit = 10, Int32 Page = 0, Boolean OnUserBehalf = true)
    //    {
    //        NetflixLogin.CheckInformationSet();

    //        Dictionary<String, String> extraParams = new Dictionary<String, String>();
    //        extraParams.Add("start_index", Page.ToString());
    //        extraParams.Add("max_results", Limit.ToString());

    //        String tokenSecret = "";
    //        if (OnUserBehalf)
    //        {
    //            if (na == null)
    //                na = Netflix.SafeReturnUserInfo();
    //            if (na != null)
    //            {
    //                tokenSecret = na.TokenSecret;
    //                extraParams.Add("oauth_token", na.Token);
    //            }
    //        }

    //        NetflixId = AsyncHelpers.GetIdFromUrl(NetflixId);

    //        String url = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
    //            NetflixLogin.ConsumerKey,
    //            String.Format(NetflixConstants.MoviesRelated, NetflixId),
    //            "GET",
    //            tokenSecret,
    //            extraParams);

    //        var doc = AsyncHelpers.LoadXDocumentAsync(url);
    //        List<Title> movies = new List<Title>(Limit);
    //        movies.AddRange(from movie
    //                        in (await doc).Element("similars").Elements("similars_item")
    //                        select new Title(TitleExpansion.Minimal)
    //                        {
    //                            IdUrl = movie.Element("id").Value,
    //                            Year = (Int32)movie.Element("release_year"),
    //                            FullTitle = (String)movie.Element("title").Attribute("regular"),
    //                            AverageRating = (Single)movie.Element("average_rating"),
    //                            ShortTitle = (String)movie.Element("title").Attribute("short"),
    //                            BoxArtUrlSmall = (String)movie.Element("box_art").Attribute("small"),
    //                            BoxArtUrlLarge = (String)movie.Element("box_art").Attribute("large"),
    //                            NetflixType = (movie.Element("id").Value.Contains("movie") ? NetflixType.Movie :
    //                                movie.Element("id").Value.Contains("programs") ? NetflixType.Programs :
    //                                movie.Element("id").Value.Contains("series") && movie.Element("id").Value.Contains("season") ?
    //                                    NetflixType.SeriesSeason : NetflixType.Series)
    //                        });
    //        return movies;
    //    }

    //}
    //public class FillSeries
    //{
    //    public async Task<Title> GetBaseTitle(String NetflixId, Account na = null, Boolean OnUserBehalf = true)
    //    {
    //        NetflixLogin.CheckInformationSet();

    //        Dictionary<String, String> extraParams = new Dictionary<String, String>();

    //        String tokenSecret = "";
    //        if (OnUserBehalf)
    //        {
    //            if (na == null)
    //                na = Netflix.SafeReturnUserInfo();
    //            if (na != null)
    //            {
    //                tokenSecret = na.TokenSecret;
    //                extraParams.Add("oauth_token", na.Token);
    //            }
    //        }

    //        String completetitleurl = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
    //            NetflixLogin.ConsumerKey,
    //            String.Format(NetflixConstants.SeriesBaseInfo, NetflixId),
    //            "GET",
    //            tokenSecret,
    //            extraParams);

    //        XDocument doc = await AsyncHelpers.LoadXDocumentAsync(completetitleurl);

    //        return (from movie
    //                in doc.Elements("catalog_title")
    //                select new Title(TitleExpansion.Minimal)
    //                {
    //                    IdUrl = movie.Element("id").Value,
    //                    Year = (Int32)movie.Element("release_year"),
    //                    FullTitle = (String)movie.Element("title").Attribute("regular"),
    //                    ShortTitle = (String)movie.Element("title").Attribute("short"),
    //                    BoxArtUrlSmall = (String)movie.Element("box_art").Attribute("small"),
    //                    BoxArtUrlLarge = (String)movie.Element("box_art").Attribute("large"),
    //                    Rating = new Rating((from mpaa
    //                                        in movie.Elements("category")
    //                                         where mpaa.Attribute("scheme").Value == NetflixConstants.Schemas.CategoryMpaaRating
    //                                         select mpaa) ??
    //                                        (from tv
    //                                        in movie.Elements("category")
    //                                         where tv.Attribute("scheme").Value == NetflixConstants.Schemas.CategoryTvRating
    //                                         select tv)),
    //                    AverageRating = (Single)movie.Element("average_rating"),
    //                    RunTime = (Int32?)movie.Element("runtime"),
    //                    Genres = new List<String>(from genres
    //                                              in movie.Elements("category")
    //                                              where (String)genres.Attribute("scheme") == NetflixConstants.Schemas.CategoryGenre
    //                                              select (String)genres.Attribute("term"))

    //                }).SingleOrDefault();
    //    }
    //    public async Task<Title> GetCompleteTitle(String NetflixId, Boolean OnUserBehalf = true)
    //    {
    //        NetflixLogin.CheckInformationSet();

    //        Dictionary<String, String> extraParams = new Dictionary<String, String>();

    //        String tokenSecret = "";
    //        Account na = null;
    //        if (OnUserBehalf)
    //        {
    //            na = Netflix.SafeReturnUserInfo();
    //            if (na != null)
    //            {
    //                tokenSecret = na.TokenSecret;
    //                extraParams.Add("oauth_token", na.Token);
    //            }
    //        }

    //        Title nfm = await GetBaseTitle(NetflixId, na);

    //        ///4) get synopsis
    //        var synopsis = GetSynopsis(NetflixId, na);

    //        ///6) get similar titles (add those to database in basic format, similar to AsyncHelpers.GetDatabaseMovies
    //        var similartitles = GetSimilarTitles(NetflixId, na);

    //        ///8) get awards
    //        var awards = GetAwards(NetflixId, na);

    //        ///9) screen format / title format
    //        var screenformats = GetScreenFormats(NetflixId, na);

    //        ///10) format availability
    //        var formatavailability = GetFormatAvailability(NetflixId, na);

    //        ///12) Actors
    //        var actors = GetActors(NetflixId, na);

    //        ///13) Directors
    //        var directors = GetDirectors(NetflixId, na);

    //        nfm.Synopsis = await synopsis;
    //        nfm.SimilarTitles = await similartitles;
    //        nfm.Awards = await awards;
    //        nfm.ScreenFormats = await screenformats;
    //        nfm.Formats = await formatavailability;
    //        nfm.Actors = await actors;
    //        nfm.Directors = await directors;
    //        nfm.completeness = TitleExpansion.Complete;

    //        return nfm;
    //    }

    //    public async Task<People> GetDirectors(String NetflixId, Account na = null, Boolean OnUserBehalf = true)
    //    {
    //        NetflixLogin.CheckInformationSet();

    //        Dictionary<String, String> extraParams = new Dictionary<String, String>();

    //        String tokenSecret = "";
    //        if (OnUserBehalf)
    //        {
    //            if (na == null)
    //                na = Netflix.SafeReturnUserInfo();
    //            if (na != null)
    //            {
    //                tokenSecret = na.TokenSecret;
    //                extraParams.Add("oauth_token", na.Token);
    //            }
    //        }

    //        NetflixId = AsyncHelpers.GetIdFromUrl(NetflixId);

    //        String url = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
    //            NetflixLogin.ConsumerKey,
    //            String.Format(NetflixConstants.SeriesDirectors, NetflixId),
    //            "GET",
    //            tokenSecret,
    //            extraParams);

    //        var doc = AsyncHelpers.LoadXDocumentAsync(url);
    //        People people = new People();
    //        people.AddRange(from person
    //                        in (await doc).Element("people").Elements("person")
    //                        select new Person(PersonExpansion.Minimal)
    //                        {
    //                            IdUrl = person.Element("id").Value,
    //                            Name = person.Element("name").Value,
    //                            Bio = (String)person.Element("bio")
    //                        });
    //        return people;
    //    }

    //    public async Task<People> GetActors(String NetflixId, Account na = null, Boolean OnUserBehalf = true)
    //    {
    //        NetflixLogin.CheckInformationSet();

    //        Dictionary<String, String> extraParams = new Dictionary<String, String>();

    //        String tokenSecret = "";
    //        if (OnUserBehalf)
    //        {
    //            if (na == null)
    //                na = Netflix.SafeReturnUserInfo();
    //            if (na != null)
    //            {
    //                tokenSecret = na.TokenSecret;
    //                extraParams.Add("oauth_token", na.Token);
    //            }
    //        }

    //        NetflixId = AsyncHelpers.GetIdFromUrl(NetflixId);

    //        String url = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
    //            NetflixLogin.ConsumerKey,
    //            String.Format(NetflixConstants.SeriesCast, NetflixId),
    //            "GET",
    //            tokenSecret,
    //            extraParams);

    //        var doc = AsyncHelpers.LoadXDocumentAsync(url);
    //        People people = new People();
    //        people.AddRange(from person
    //                        in (await doc).Element("people").Elements("person")
    //                        select new Person(PersonExpansion.Minimal)
    //                        {
    //                            IdUrl = person.Element("id").Value,
    //                            Name = person.Element("name").Value,
    //                            Bio = (String)person.Element("bio")
    //                        });
    //        return people;
    //    }

    //    public async Task<List<Title>> GetSimilarTitles(String NetflixId, Account na = null, Int32 Limit = 10, Int32 Page = 0, Boolean OnUserBehalf = true)
    //    {
    //        NetflixLogin.CheckInformationSet();

    //        Dictionary<String, String> extraParams = new Dictionary<String, String>();
    //        extraParams.Add("start_index", Page.ToString());
    //        extraParams.Add("max_results", Limit.ToString());

    //        String tokenSecret = "";
    //        if (OnUserBehalf)
    //        {
    //            if (na == null)
    //                na = Netflix.SafeReturnUserInfo();
    //            if (na != null)
    //            {
    //                tokenSecret = na.TokenSecret;
    //                extraParams.Add("oauth_token", na.Token);
    //            }
    //        }

    //        NetflixId = AsyncHelpers.GetIdFromUrl(NetflixId);

    //        String url = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
    //            NetflixLogin.ConsumerKey,
    //            String.Format(NetflixConstants.SeriesSimilars, NetflixId),
    //            "GET",
    //            tokenSecret,
    //            extraParams);

    //        var doc = AsyncHelpers.LoadXDocumentAsync(url);
    //        List<Title> movies = new List<Title>(Limit);
    //        movies.AddRange(from movie
    //                        in (await doc).Element("similars").Elements("similars_item")
    //                        select new Title(TitleExpansion.Minimal)
    //                        {
    //                            IdUrl = movie.Element("id").Value,
    //                            Year = (Int32)movie.Element("release_year"),
    //                            FullTitle = (String)movie.Element("title").Attribute("regular"),
    //                            AverageRating = (Single)movie.Element("average_rating"),
    //                            ShortTitle = (String)movie.Element("title").Attribute("short"),
    //                            BoxArtUrlSmall = (String)movie.Element("box_art").Attribute("small"),
    //                            BoxArtUrlLarge = (String)movie.Element("box_art").Attribute("large"),
    //                            NetflixType = (movie.Element("id").Value.Contains("movie") ? NetflixType.Movie :
    //                                movie.Element("id").Value.Contains("programs") ? NetflixType.Programs :
    //                                movie.Element("id").Value.Contains("series") && movie.Element("id").Value.Contains("season") ?
    //                                    NetflixType.SeriesSeason : NetflixType.Series)
    //                        });
    //        return movies;
    //    }

    //    public async Task<List<Award>> GetAwards(String NetflixId, Account na = null, Boolean OnUserBehalf = true)
    //    {
    //        NetflixLogin.CheckInformationSet();

    //        Dictionary<String, String> extraParams = new Dictionary<String, String>();

    //        String tokenSecret = "";
    //        if (OnUserBehalf)
    //        {
    //            if (na == null)
    //                na = Netflix.SafeReturnUserInfo();
    //            if (na != null)
    //            {
    //                tokenSecret = na.TokenSecret;
    //                extraParams.Add("oauth_token", na.Token);
    //            }
    //        }

    //        NetflixId = AsyncHelpers.GetIdFromUrl(NetflixId);

    //        String url = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
    //            NetflixLogin.ConsumerKey,
    //            String.Format(NetflixConstants.SeriesAwards, NetflixId),
    //            "GET",
    //            tokenSecret,
    //            extraParams);

    //        XDocument doc = await AsyncHelpers.LoadXDocumentAsync(url);

    //        var awardnominees = from awards
    //                     in doc.Element("awards").Elements("award_nominee")
    //                            select new Award()
    //                            {///get all the nominees first?
    //                                Year = awards.Attribute("year") == null || (String)awards.Attribute("year") == "" ? null : (Int32?)awards.Attribute("year"),
    //                                AwardName = (String)awards.Element("category").Attribute("term"),
    //                                PersonId = (awards.Element("link") != null ?
    //                                   (String)awards.Element("link").Attribute("href") : null),
    //                                Type = (AwardType)Enum.Parse(typeof(AwardType),
    //                                   awards.Element("category").Attribute("scheme").Value.
    //                                   Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries)[awards.Element("category").Attribute("scheme").Value.
    //                                       Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries).Length - 1], true),
    //                                Winner = false
    //                            };
    //        var awardwinners = from awards
    //                     in doc.Element("awards").Elements("award_winner")
    //                           select new Award()
    //                           {
    //                               Year = awards.Attribute("year") == null || (String)awards.Attribute("year") == "" ? null : (Int32?)awards.Attribute("year"),
    //                               AwardName = (String)awards.Element("category").Attribute("term"),
    //                               PersonId = (awards.Element("link") != null ?
    //                                  (String)awards.Element("link").Attribute("href") : null),
    //                               Type = (AwardType)Enum.Parse(typeof(AwardType),
    //                                  awards.Element("category").Attribute("scheme").Value.
    //                                  Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries)[awards.Element("category").Attribute("scheme").Value.
    //                                      Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries).Length - 1], true),
    //                               Winner = true
    //                           };
    //        List<Award> a = new List<Award>();
    //        a.AddRange(awardnominees);
    //        a.AddRange(awardwinners);
    //        return a;
    //    }

    //    public async Task<List<FormatAvailability>> GetFormatAvailability(String NetflixId, Account na = null, Boolean OnUserBehalf = true)
    //    {
    //        NetflixLogin.CheckInformationSet();

    //        Dictionary<String, String> extraParams = new Dictionary<String, String>();

    //        String tokenSecret = "";
    //        if (OnUserBehalf)
    //        {
    //            if (na == null)
    //                na = Netflix.SafeReturnUserInfo();
    //            if (na != null)
    //            {
    //                tokenSecret = na.TokenSecret;
    //                extraParams.Add("oauth_token", na.Token);
    //            }
    //        }

    //        NetflixId = AsyncHelpers.GetIdFromUrl(NetflixId);

    //        String url = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
    //            NetflixLogin.ConsumerKey,
    //            String.Format(NetflixConstants.SeriesFormatAvailability, NetflixId),
    //            "GET",
    //            tokenSecret,
    //            extraParams);

    //        XDocument doc = await AsyncHelpers.LoadXDocumentAsync(url);

    //        var formatavailability = from formats
    //                            in doc.Element("delivery_formats").Elements("availability")
    //                                 select new FormatAvailability()
    //                                 {
    //                                     AvailableFrom = formats.Attribute("available_from") == null || (String)formats.Attribute("available_from") == "" ?
    //                                             null : (Nullable<DateTime>)GeneralHelpers.FromUnixTime(Int64.Parse((String)formats.Attribute("available_from"))),
    //                                     AvailableUntil = formats.Attribute("available_until") == null || (String)formats.Attribute("available_until") == "" ?
    //                                             null : (Nullable<DateTime>)GeneralHelpers.FromUnixTime(Int64.Parse((String)formats.Attribute("available_until"))),
    //                                     Format = (String)formats.Element("category").Attribute("term")
    //                                 };

    //        List<FormatAvailability> fa = new List<FormatAvailability>();
    //        fa.AddRange(formatavailability);
    //        return fa;
    //    }

    //    public async Task<List<ScreenFormats>> GetScreenFormats(String NetflixId, Account na = null, Boolean OnUserBehalf = true)
    //    {
    //        NetflixLogin.CheckInformationSet();

    //        Dictionary<String, String> extraParams = new Dictionary<String, String>();

    //        String tokenSecret = "";
    //        if (OnUserBehalf)
    //        {
    //            if (na == null)
    //                na = Netflix.SafeReturnUserInfo();
    //            if (na != null)
    //            {
    //                tokenSecret = na.TokenSecret;
    //                extraParams.Add("oauth_token", na.Token);
    //            }
    //        }

    //        NetflixId = AsyncHelpers.GetIdFromUrl(NetflixId);

    //        String url = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
    //            NetflixLogin.ConsumerKey,
    //            String.Format(NetflixConstants.SeriesScreenFormat, NetflixId),
    //            "GET",
    //            tokenSecret,
    //            extraParams);

    //        XDocument doc = await AsyncHelpers.LoadXDocumentAsync(url);

    //        var screenformats = from formats
    //                            in doc.Element("screen_formats").Elements("screen_format")
    //                            select new ScreenFormats()
    //                            {
    //                                Format = (from format
    //                                        in formats.Elements("category")
    //                                          where (String)format.Attribute("scheme") == NetflixConstants.Schemas.LinkTitleFormat
    //                                          select (String)format.Attribute("term")).FirstOrDefault(),
    //                                ScreenFormat = (from screenformat
    //                                        in formats.Elements("category")
    //                                                where (String)screenformat.Attribute("scheme") == NetflixConstants.Schemas.LinkScreenFormat
    //                                                select (String)screenformat.Attribute("term")).FirstOrDefault()
    //                            };

    //        List<ScreenFormats> sf = new List<ScreenFormats>();
    //        sf.AddRange(screenformats);
    //        return sf;
    //    }

    //    public async Task<String> GetSynopsis(String NetflixId, Account na = null, Boolean OnUserBehalf = true)
    //    {
    //        NetflixLogin.CheckInformationSet();

    //        Dictionary<String, String> extraParams = new Dictionary<String, String>();

    //        String tokenSecret = "";
    //        if (OnUserBehalf)
    //        {
    //            if (na == null)
    //                na = Netflix.SafeReturnUserInfo();
    //            if (na != null)
    //            {
    //                tokenSecret = na.TokenSecret;
    //                extraParams.Add("oauth_token", na.Token);
    //            }
    //        }

    //        NetflixId = AsyncHelpers.GetIdFromUrl(NetflixId);

    //        String url = OAuth.OAuthHelpers.GetOAuthRequestUrl(NetflixLogin.SharedSecret,
    //            NetflixLogin.ConsumerKey,
    //            String.Format(NetflixConstants.SeriesSynopsis, NetflixId),
    //            "GET",
    //            tokenSecret,
    //            extraParams);

    //        XDocument doc = await AsyncHelpers.LoadXDocumentAsync(url);

    //        return (String)doc.Element("synopsis");
    //    }
    //}

}
