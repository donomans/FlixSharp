using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FlixSharp.Helpers;
using System.Net;
using FlixSharp.Holders.Netflix;
using FlixSharp.Holders;
using FlixSharp.Helpers.OAuth;
using FlixSharp.Helpers.Async;

namespace FlixSharp.Queries.Netflix
{
    public class Fill
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
                    else if (NetflixId.Contains("people"))
                        TitleType = NetflixType.People;
                    else if (NetflixId.Contains("discs"))
                        TitleType = NetflixType.Discs;
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
                Account NetflixAccount = FlixSharp.Netflix.SafeReturnUserInfo();
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

        internal static async Task<List<Title>> GetBaseTitleInfo(Task<XDocument> doc, String nodename)
        {
            List<Title> movies = new List<Title>(25);
            
            movies.AddRange(from movie
                            in (await doc).Descendants(nodename)
                            select new Title(TitleExpansion.Minimal)
                            {
                                IdUrl = movie.Element("id").Value,
                                Year = movie.Element("release_year") != null 
                                    && (String)movie.Element("release_year") != "" ?
                                    (Int32)movie.Element("release_year") : 0,
                                FullTitle = (String)movie.Element("title").Attribute("regular"),
                                ShortTitle = (String)movie.Element("title").Attribute("short"),
                                BoxArtUrlSmall = (String)movie.Element("box_art").Attribute("small"),
                                BoxArtUrlLarge = (String)movie.Element("box_art").Attribute("large"),
                                Rating = new Rating((from mpaa
                                                    in movie.Elements("category")
                                                        where (String)mpaa.Attribute("scheme") == Constants.Schemas.CategoryMpaaRating
                                                        select mpaa) ??
                                                    (from tv
                                                    in movie.Elements("category")
                                                        where (String)tv.Attribute("scheme") == Constants.Schemas.CategoryTvRating
                                                        select tv)),
                                AverageRating = movie.Element("average_rating") != null 
                                    && movie.Element("average_rating").ToString() != "" ?
                                    (Single)movie.Element("average_rating") : 0,
                                RunTime = movie.Element("runtime") != null 
                                    && movie.Element("runtime").ToString() != "" ?
                                    (Int32)movie.Element("runtime") : 0,
                                Genres = new List<String>(from genres
                                                            in movie.Elements("category")
                                                            where (String)genres.Attribute("scheme") == Constants.Schemas.CategoryGenre
                                                            select (String)genres.Attribute("term")),
                                NetflixSiteUrl = (from webpage
                                                    in movie.Elements("link")
                                                    where (String)webpage.Attribute("title") == "web page"
                                                    select (String)webpage.Attribute("href")).FirstOrDefault(),
                                OfficialWebsite = (from webpage
                                                    in movie.Elements("link")
                                                    where (String)webpage.Attribute("rel") == Constants.Schemas.TitleOfficialUrl
                                                    select (String)webpage.Attribute("href")).FirstOrDefault(),
                                HasAwards = (from webpage
                                                in movie.Elements("link")
                                                where (String)webpage.Attribute("rel") == Constants.Schemas.LinkAwards
                                                select true).FirstOrDefault(),
                                HasBonusMaterials = (from webpage
                                                        in movie.Elements("link")
                                                        where (String)webpage.Attribute("rel") == Constants.Schemas.LinkBonusMaterials
                                                        select true).FirstOrDefault(),
                                HasDiscs = (from webpage
                                            in movie.Elements("link")
                                            where (String)webpage.Attribute("rel") == Constants.Schemas.LinkDiscs
                                            select true).FirstOrDefault(),
                                HasLanguages = (from webpage
                                                in movie.Elements("link")
                                                where (String)webpage.Attribute("rel") == Constants.Schemas.LinkLanguagesAndAudio
                                                select true).FirstOrDefault(),
                                HasEpisodes = (from webpage
                                                in movie.Elements("link")
                                                where (String)webpage.Attribute("rel") == Constants.Schemas.LinkTitlesPrograms
                                                select true).FirstOrDefault()
                            });
            
            return movies;
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
            Login.CheckInformationSet();

            Account NetflixAccount = null;
            if (OnUserBehalf)
                NetflixAccount = FlixSharp.Netflix.SafeReturnUserInfo();

            TitleType = Fill.GetNetflixType(NetflixId, TitleType);

            ///Need to come up with a better way to reduce the regex work that happens on these calls
            ///Should only do it once and then pass the result in
            //var id = GeneralHelpers.GetIdFromUrl(NetflixId);

            var nfm = GetBaseTitle(NetflixId, NetflixAccount, OnUserBehalf, TitleType);
            //var nfm = GetBaseTitle(id, NetflixAccount, OnUserBehalf, TitleType);

            ///4) get synopsis
            var synopsis = GetSynopsis(NetflixId, NetflixAccount, OnUserBehalf, TitleType);

            ///6) get similar titles (add those to database in basic format, similar to AsyncHelpers.GetDatabaseMovies
            var similartitles = GetSimilarTitles(NetflixId, NetflixAccount, OnUserBehalf, TitleType: TitleType);

            ///9) screen format / title format
            var screenformats = GetScreenFormats(NetflixId, NetflixAccount, OnUserBehalf, TitleType);

            ///10) format availability
            var formatavailability = GetFormatAvailability(NetflixId, NetflixAccount, OnUserBehalf, TitleType);

            ///12) Actors
            var actors = GetActors(NetflixId, NetflixAccount, OnUserBehalf, TitleType);

            ///13) Directors
            Task<People> directors = null;
            if(TitleType != NetflixType.Discs)
                directors = GetDirectors(NetflixId, NetflixAccount, OnUserBehalf, TitleType);
            
            if (OnUserBehalf)
            {
                ///14) User Rating for title (if on user behalf)
                //var userrating = Netflix.Users.Titles.GetUserRating(NetflixId, NetflixAccount, OnUserBehalf, TitleType);
            }
            Title title = await nfm;
            ///8) get awards
            Task<List<Award>> awards = null;
            if(title.HasAwards)
                awards = GetAwards(NetflixId, NetflixAccount, OnUserBehalf, TitleType);
            ///14) bonus
            Task<List<String>> bonus = null;            
            if(title.HasBonusMaterials)
                bonus = GetBonusMaterials(NetflixId, NetflixAccount, OnUserBehalf, TitleType);
            ///15) discs
            Task<List<Title>> discs = null;
            if (title.HasDiscs)
                discs = GetDiscs(NetflixId, NetflixAccount, OnUserBehalf, TitleType);

            ///16) languages and audio...
            if (title.HasLanguages)
            { }

            title.Synopsis = await synopsis;
            title.ScreenFormats = await screenformats;
            title.Formats = await formatavailability;
            title.Actors = await actors;
            if (TitleType != NetflixType.Discs)
                title.Directors = await directors;
            else
                title.Directors = new People();
            title.SimilarTitles = await similartitles;

            if (title.HasDiscs)
                title.Discs = await discs;
            if (title.HasBonusMaterials)
                title.BonusMaterials = await bonus;
            if (title.HasAwards)
                title.Awards = await awards;
            title.completeness = TitleExpansion.Complete;

            return title;
        }

        public async Task<Title> GetCompleteTitleFromExpanded(String NetflixIdUrl, Boolean OnUserBehalf)
        {
            return await GetCompleteTitleFromExpanded(NetflixIdUrl, null, OnUserBehalf);
        }
        public async Task<Title> GetCompleteTitleFromExpanded(String NetflixId, NetflixType? TitleType = null, Boolean OnUserBehalf = true)
        {
            Login.CheckInformationSet();

            Account NetflixAccount = null;
            if (OnUserBehalf)
                NetflixAccount = FlixSharp.Netflix.SafeReturnUserInfo();

            TitleType = Fill.GetNetflixType(NetflixId, TitleType);

            //NetflixId = GeneralHelpers.GetIdFromUrl(NetflixId);

            var nfm = GetBaseTitle(NetflixId, NetflixAccount, OnUserBehalf, TitleType);

            var similartitles = GetSimilarTitles(NetflixId, NetflixAccount, OnUserBehalf, TitleType: TitleType);

            if (OnUserBehalf)
            {
                ///14) User Rating for title (if on user behalf)
                //var userrating = Netflix.Users.Titles.GetUserRating(NetflixId, NetflixAccount, OnUserBehalf, TitleType);
            }
            Title title = await nfm;
            Task<List<Award>> awards = null;
            if (title.HasAwards)
                awards = GetAwards(NetflixId, NetflixAccount, OnUserBehalf, TitleType);
            Task<List<String>> bonus = null;
            if (title.HasBonusMaterials)
                bonus = GetBonusMaterials(NetflixId, NetflixAccount, OnUserBehalf, TitleType);
            Task<List<Title>> discs = null;
            if (title.HasDiscs)
                discs = GetDiscs(NetflixId, NetflixAccount, OnUserBehalf, TitleType);

            title.SimilarTitles = await similartitles;
            if (title.HasDiscs)
                title.Discs = await discs;
            if (title.HasBonusMaterials)
                title.BonusMaterials = await bonus;
            if (title.HasAwards)
                title.Awards = await awards;
            title.completeness = TitleExpansion.Complete;

            return title;
        }

        public async Task<Title> GetExpandedTitle(String NetflixIdUrl, Boolean OnUserBehalf)
        {
            return await GetExpandedTitle(NetflixIdUrl, null, OnUserBehalf);
        }
        public async Task<Title> GetExpandedTitle(String NetflixId, NetflixType? TitleType = null, Boolean OnUserBehalf = true)
        {
            Login.CheckInformationSet();

            Account NetflixAccount = null;
            if (OnUserBehalf)
                NetflixAccount = FlixSharp.Netflix.SafeReturnUserInfo();

            TitleType = Fill.GetNetflixType(NetflixId, TitleType);

            //NetflixId = GeneralHelpers.GetIdFromUrl(NetflixId);
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
            Task<People> directors = null;
            if (TitleType != NetflixType.Discs)
                directors = GetDirectors(NetflixId, NetflixAccount, OnUserBehalf, TitleType);

            Title title = await nfm;
            title.Synopsis = await synopsis;
            title.ScreenFormats = await screenformats;
            title.Formats = await formatavailability;
            title.Actors = await actors;
            if (TitleType != NetflixType.Discs)
                title.Directors = await directors;
            else
                title.Directors = new People();
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
            Login.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = Fill.GetTokens(OnUserBehalf, NetflixAccount, out TokenSecret);

            return await GetBaseTitle(NetflixId, extraParams, TokenSecret, TitleType);
        }
        public async Task<Title> GetBaseTitle(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            Login.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = Fill.GetTokens(OnUserBehalf, out TokenSecret);

            return await GetBaseTitle(NetflixId, extraParams, TokenSecret, TitleType);
        }
        private async Task<Title> GetBaseTitle(String NetflixId, Dictionary<String, String> ExtraParams, String TokenSecret, NetflixType? TitleType = null)
        {
            TitleType = Fill.GetNetflixType(NetflixId, TitleType);
            var idtup = GeneralHelpers.GetIdFromUrl(NetflixId);

            String url = "";
            switch (TitleType)
            {
                case NetflixType.Movie:
                    url = String.Format(Constants.MoviesBaseInfo, idtup.Id);
                    break;
                case NetflixType.Series:
                    url = String.Format(Constants.SeriesBaseInfo, idtup.Id);
                    break;
                case NetflixType.Discs:
                    url = String.Format(Constants.DiscsBaseInfo, idtup.Id);
                    break;
                case NetflixType.SeriesSeason:
                    url = String.Format(Constants.SeriesSeasonsBaseInfo, idtup.Id, idtup.SeasonId);
                    break;
                default: throw new Exception("Invalid request for TitleType: " + TitleType);
            }

            url = OAuthHelpers.GetOAuthRequestUrl(Login.SharedSecret,
                Login.ConsumerKey,
                url,
                "GET",
                TokenSecret,
                ExtraParams);


            var doc = AsyncHelpers.NetflixLoadXDocumentAsync(url);
            return (await Fill.GetBaseTitleInfo(doc, "catalog_title")).SingleOrDefault();
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
            Login.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = Fill.GetTokens(OnUserBehalf, NetflixAccount, out TokenSecret);

            return await GetActors(NetflixId, extraParams, TokenSecret, TitleType);
        }
        public async Task<People> GetActors(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            Login.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = Fill.GetTokens(OnUserBehalf, out TokenSecret);

            return await GetActors(NetflixId, extraParams, TokenSecret, TitleType);
        }
        private async Task<People> GetActors(String NetflixId, Dictionary<String, String> ExtraParams, String TokenSecret, NetflixType? TitleType = null)
        {
            TitleType = Fill.GetNetflixType(NetflixId, TitleType);
            var idtup = GeneralHelpers.GetIdFromUrl(NetflixId);

            String url = "";
            switch (TitleType)
            {
                case NetflixType.Movie:
                    url = String.Format(Constants.MoviesCast, idtup.Id);
                    break;
                case NetflixType.Series:
                    url = String.Format(Constants.SeriesCast, idtup.Id);
                    break;
                case NetflixType.Discs:
                    url = String.Format(Constants.DiscsCast, idtup.Id);
                    break;
                case NetflixType.SeriesSeason:
                    url = String.Format(Constants.SeriesSeasonsCast, idtup.Id, idtup.SeasonId);
                    break;
                default: throw new Exception("Invalid request for TitleType: " + TitleType);
            }

            url = OAuthHelpers.GetOAuthRequestUrl(Login.SharedSecret,
                Login.ConsumerKey,
                url,
                "GET",
                TokenSecret,
                ExtraParams);


            var doc = await AsyncHelpers.NetflixLoadXDocumentAsync(url);
            People people = new People();
        
            people.AddRange(from person
                                in doc.Element("people").Elements("person")
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
            Login.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = Fill.GetTokens(OnUserBehalf, NetflixAccount, out TokenSecret);

            return await GetDirectors(NetflixId, extraParams, TokenSecret, TitleType);
        }
        public async Task<People> GetDirectors(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            Login.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = Fill.GetTokens(OnUserBehalf, out TokenSecret);

            return await GetDirectors(NetflixId, extraParams, TokenSecret, TitleType);
        }
        private async Task<People> GetDirectors(String NetflixId, Dictionary<String, String> ExtraParams, String TokenSecret, NetflixType? TitleType = null)
        {
            TitleType = Fill.GetNetflixType(NetflixId, TitleType);
            var idtup = GeneralHelpers.GetIdFromUrl(NetflixId);

            String url = "";
            switch (TitleType)
            {
                case NetflixType.Movie:
                    url = String.Format(Constants.MoviesDirectors, idtup.Id);
                    break;
                case NetflixType.Series:
                    url = String.Format(Constants.SeriesDirectors, idtup.Id);
                    break;
                case NetflixType.SeriesSeason:
                    url = String.Format(Constants.SeriesSeasonsDirectors, idtup.Id, idtup.SeasonId);
                    break;
                default: throw new Exception("Invalid request for TitleType: " + TitleType);
            }

            url = OAuthHelpers.GetOAuthRequestUrl(Login.SharedSecret,
                Login.ConsumerKey,
                url,
                "GET",
                TokenSecret,
                ExtraParams);

            var doc = await AsyncHelpers.NetflixLoadXDocumentAsync(url);
            People people = new People();
            people.AddRange(from person
                            in doc.Element("people").Elements("person")
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
            Login.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = Fill.GetTokens(OnUserBehalf, NetflixAccount, out TokenSecret);

            return await GetAwards(NetflixId, extraParams, TokenSecret, TitleType);
        }
        public async Task<List<Award>> GetAwards(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            Login.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = Fill.GetTokens(OnUserBehalf, out TokenSecret);

            return await GetAwards(NetflixId, extraParams, TokenSecret, TitleType);
        }
        private async Task<List<Award>> GetAwards(String NetflixId, Dictionary<String, String> ExtraParams, String TokenSecret, NetflixType? TitleType = null)
        {
            TitleType = Fill.GetNetflixType(NetflixId, TitleType);
            var idtup = GeneralHelpers.GetIdFromUrl(NetflixId);

            String url = "";
            switch (TitleType)
            {
                case NetflixType.Movie:
                    url = String.Format(Constants.MoviesAwards, idtup.Id);
                    break;
                case NetflixType.Series:
                    url = String.Format(Constants.SeriesAwards, idtup.Id);
                    break;
                case NetflixType.SeriesSeason:
                    url = String.Format(Constants.SeriesSeasonsAwards, idtup.Id, idtup.SeasonId);
                    break;
                default: throw new Exception("Invalid request for TitleType: " + TitleType);
            }

            url = OAuthHelpers.GetOAuthRequestUrl(Login.SharedSecret,
                Login.ConsumerKey,
                url,
                "GET",
                TokenSecret,
                ExtraParams);


            var doc = await AsyncHelpers.NetflixLoadXDocumentAsync(url);
            List<Award> a = new List<Award>();
            var awardnominees = from awards
                         in doc.Element("awards").Elements("award_nominee")
                                select new Award()
                                {
                                    Year = awards.Attribute("year") == null || (String)awards.Attribute("year") == "" ? null : (Int32?)awards.Attribute("year"),
                                    AwardName = (String)awards.Element("category").Attribute("term"),
                                    PersonIdUrl = (awards.Element("link") != null ?
                                       (String)awards.Element("link").Attribute("href") : null),
                                    Type = (AwardType)Enum.Parse(typeof(AwardType),
                                       awards.Element("category").Attribute("scheme").Value.Replace("_", "").
                                       Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries)[awards.Element("category").Attribute("scheme").Value.
                                           Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries).Length - 1].Replace("_", ""), true),
                                    Winner = false
                                };
            var awardwinners = from awards
                         in doc.Element("awards").Elements("award_winner")
                               select new Award()
                               {
                                   Year = awards.Attribute("year") == null || (String)awards.Attribute("year") == "" ? null : (Int32?)awards.Attribute("year"),
                                   AwardName = (String)awards.Element("category").Attribute("term"),
                                   PersonIdUrl = (awards.Element("link") != null ?
                                      (String)awards.Element("link").Attribute("href") : null),
                                   Type = (AwardType)Enum.Parse(typeof(AwardType),
                                      awards.Element("category").Attribute("scheme").Value.
                                      Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries)[awards.Element("category").Attribute("scheme").Value.
                                          Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries).Length - 1].Replace("_", ""), true),
                                   Winner = true
                               };

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
            Login.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = Fill.GetTokens(OnUserBehalf, NetflixAccount, out TokenSecret);

            return await GetFormatAvailability(NetflixId, extraParams, TokenSecret, TitleType);
        }
        public async Task<List<FormatAvailability>> GetFormatAvailability(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            Login.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = Fill.GetTokens(OnUserBehalf, out TokenSecret);

            return await GetFormatAvailability(NetflixId, extraParams, TokenSecret, TitleType);
        }
        private async Task<List<FormatAvailability>> GetFormatAvailability(String NetflixId, Dictionary<String, String> ExtraParams, String TokenSecret, NetflixType? TitleType = null)
        {
            TitleType = Fill.GetNetflixType(NetflixId, TitleType);
            var idtup = GeneralHelpers.GetIdFromUrl(NetflixId);

            String url = "";
            switch (TitleType)
            {
                case NetflixType.Movie:
                    url = String.Format(Constants.MoviesFormatAvailability, idtup.Id);
                    break;
                case NetflixType.Series:
                    url = String.Format(Constants.SeriesFormatAvailability, idtup.Id);
                    break;
                case NetflixType.Discs:
                    url = String.Format(Constants.DiscsFormatAvailability, idtup.Id);
                    break;
                case NetflixType.SeriesSeason:
                    url = String.Format(Constants.SeriesSeasonsFormatAvailability, idtup.Id, idtup.SeasonId);
                    break;
                default: throw new Exception("Invalid request for TitleType: " + TitleType);
            }

            url = OAuthHelpers.GetOAuthRequestUrl(Login.SharedSecret,
                Login.ConsumerKey,
                url,
                "GET",
                TokenSecret,
                ExtraParams);

            var doc = AsyncHelpers.NetflixLoadXDocumentAsync(url);

            var formatavailability = from formats
                                    in (await doc).Element("delivery_formats").Elements("availability")
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
            Login.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = Fill.GetTokens(OnUserBehalf, NetflixAccount, out TokenSecret);

            return await GetSynopsis(NetflixId, extraParams, TokenSecret, TitleType);
        }
        public async Task<String> GetSynopsis(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            Login.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = Fill.GetTokens(OnUserBehalf, out TokenSecret);

            return await GetSynopsis(NetflixId, extraParams, TokenSecret, TitleType);
        }
        private async Task<String> GetSynopsis(String NetflixId, Dictionary<String, String> ExtraParams, String TokenSecret, NetflixType? TitleType = null)
        {
            TitleType = Fill.GetNetflixType(NetflixId, TitleType);
            var idtup = GeneralHelpers.GetIdFromUrl(NetflixId);

            String url = "";
            switch (TitleType)
            {
                case NetflixType.Movie:
                    url = String.Format(Constants.MoviesSynopsis, idtup.Id);
                    break;
                case NetflixType.Series:
                    url = String.Format(Constants.SeriesSynopsis, idtup.Id);
                    break;
                case NetflixType.SeriesSeason:
                    url = String.Format(Constants.SeriesSeasonsSynopsis, idtup.Id, idtup.SeasonId);
                    break;
                case NetflixType.Discs:
                    url = String.Format(Constants.DiscsSynopsis, idtup.Id);
                    break;
                default: throw new Exception("Invalid request for TitleType: " + TitleType);
            }

            url = OAuthHelpers.GetOAuthRequestUrl(Login.SharedSecret,
                Login.ConsumerKey,
                url,
                "GET",
                TokenSecret,
                ExtraParams);

            var doc = AsyncHelpers.NetflixLoadXDocumentAsync(url);

            return (String)(await doc).Element("synopsis");

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
            Login.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = Fill.GetTokens(OnUserBehalf, NetflixAccount, out TokenSecret);

            return await GetScreenFormats(NetflixId, extraParams, TokenSecret, TitleType);
        }
        public async Task<List<ScreenFormats>> GetScreenFormats(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            Login.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = Fill.GetTokens(OnUserBehalf, out TokenSecret);

            return await GetScreenFormats(NetflixId, extraParams, TokenSecret, TitleType);
        }
        private async Task<List<ScreenFormats>> GetScreenFormats(String NetflixId, Dictionary<String, String> ExtraParams, String TokenSecret, NetflixType? TitleType = null)
        {
            TitleType = Fill.GetNetflixType(NetflixId, TitleType);
            var idtup = GeneralHelpers.GetIdFromUrl(NetflixId);

            String url = "";
            switch (TitleType)
            {
                case NetflixType.Movie:
                    url = String.Format(Constants.MoviesScreenFormat, idtup.Id);
                    break;
                case NetflixType.Series:
                    url = String.Format(Constants.SeriesScreenFormat, idtup.Id);
                    break;
                case NetflixType.SeriesSeason:
                    url = String.Format(Constants.SeriesSeasonsScreenFormat, idtup.Id, idtup.SeasonId);
                    break;
                case NetflixType.Discs:
                    url = String.Format(Constants.DiscsScreenFormat, idtup.Id);
                    break;
                default: throw new Exception("Invalid request for TitleType: " + TitleType);
            }

            url = OAuthHelpers.GetOAuthRequestUrl(Login.SharedSecret,
                Login.ConsumerKey,
                url,
                "GET",
                TokenSecret,
                ExtraParams);

            XDocument doc = await AsyncHelpers.NetflixLoadXDocumentAsync(url);


            var screenformats = from formats
                                in doc.Element("screen_formats").Elements("screen_format")
                                select new ScreenFormats()
                                {
                                    Format = (from format
                                            in formats.Elements("category")
                                              where (String)format.Attribute("scheme") == Constants.Schemas.LinkTitleFormat
                                              select (String)format.Attribute("term")).FirstOrDefault(),
                                    ScreenFormat = (from screenformat
                                            in formats.Elements("category")
                                                    where (String)screenformat.Attribute("scheme") == Constants.Schemas.LinkScreenFormat
                                                    select (String)screenformat.Attribute("term")).FirstOrDefault()
                                };

            return screenformats.ToList();
        }

        public async Task<List<Title>> GetSimilarTitles(String NetflixIdUrl, Boolean OnUserBehalf, Int32 Limit = 25, Int32 Page = 0)
        {
            return await GetSimilarTitles(NetflixIdUrl, OnUserBehalf, Limit, Page, null);
        }
        public async Task<List<Title>> GetSimilarTitles(String NetflixId, NetflixType TitleType, Boolean OnUserBehalf = true, Int32 Limit = 25, Int32 Page = 0)
        {
            return await GetSimilarTitles(NetflixId, OnUserBehalf, Limit, Page, TitleType);
        }
        public async Task<List<Title>> GetSimilarTitles(String NetflixId, Account NetflixAccount, Boolean OnUserBehalf = true, Int32 Limit = 25, Int32 Page = 0, NetflixType? TitleType = null)
        {
            Login.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = Fill.GetTokens(OnUserBehalf, NetflixAccount, out TokenSecret);
            
            return await GetSimilarTitles(NetflixId, extraParams, TokenSecret, Limit, Page, TitleType);
        }
        public async Task<List<Title>> GetSimilarTitles(String NetflixId, Boolean OnUserBehalf = true, Int32 Limit = 25, Int32 Page = 0, NetflixType? TitleType = null)
        {
            Login.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = Fill.GetTokens(OnUserBehalf, out TokenSecret);

            return await GetSimilarTitles(NetflixId, extraParams, TokenSecret, Limit, Page, TitleType);
        }
        private async Task<List<Title>> GetSimilarTitles(String NetflixId, Dictionary<String, String> ExtraParams, String TokenSecret, Int32 Limit = 25, Int32 Page = 0, NetflixType? TitleType = null)
        {
            TitleType = Fill.GetNetflixType(NetflixId, TitleType);
            var idtup = GeneralHelpers.GetIdFromUrl(NetflixId);

            ExtraParams.Add("start_index", Page.ToString());
            ExtraParams.Add("max_results", Limit.ToString());

            String url = "";
            switch (TitleType)
            {
                case NetflixType.Movie:
                    url = String.Format(Constants.MoviesSimilars, idtup.Id);
                    break;
                case NetflixType.Series:
                    url = String.Format(Constants.SeriesSimilars, idtup.Id);
                    break;
                case NetflixType.Discs:
                    url = String.Format(Constants.DiscsSimilars, idtup.Id);
                    break;
                case NetflixType.SeriesSeason:
                    url = String.Format(Constants.SeriesSeasonsSimilars, idtup.Id, idtup.SeasonId);
                    break;
                default: throw new Exception("Invalid request for TitleType: " + TitleType);
            }

            url = OAuthHelpers.GetOAuthRequestUrl(Login.SharedSecret,
                Login.ConsumerKey,
                url,
                "GET",
                TokenSecret,
                ExtraParams);

            var doc = AsyncHelpers.NetflixLoadXDocumentAsync(url);

            return await Fill.GetBaseTitleInfo(doc, "similars_item");
        }

        public async Task<List<String>> GetBonusMaterials(String NetflixIdUrl, Boolean OnUserBehalf)
        {
            return await GetBonusMaterials(NetflixIdUrl, OnUserBehalf, null);
        }
        public async Task<List<String>> GetBonusMaterials(String NetflixId, NetflixType TitleType, Boolean OnUserBehalf = true)
        {
            return await GetBonusMaterials(NetflixId, OnUserBehalf, TitleType);
        }
        public async Task<List<String>> GetBonusMaterials(String NetflixId, Account NetflixAccount, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            Login.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = Fill.GetTokens(OnUserBehalf, NetflixAccount, out TokenSecret);

            return await GetBonusMaterials(NetflixId, extraParams, TokenSecret, TitleType);
        }
        public async Task<List<String>> GetBonusMaterials(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            Login.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = Fill.GetTokens(OnUserBehalf, out TokenSecret);

            return await GetBonusMaterials(NetflixId, extraParams, TokenSecret, TitleType);
        }
        private async Task<List<String>> GetBonusMaterials(String NetflixId, Dictionary<String, String> ExtraParams, String TokenSecret, NetflixType? TitleType = null)
        {
            TitleType = Fill.GetNetflixType(NetflixId, TitleType);
            var idtup = GeneralHelpers.GetIdFromUrl(NetflixId);

            String url = "";
            switch (TitleType)
            {
                case NetflixType.Movie:
                    url = String.Format(Constants.MoviesBonusMaterials, idtup.Id);
                    break;
                case NetflixType.Series:
                    url = String.Format(Constants.SeriesBonusMaterials, idtup.Id);
                    break;
                case NetflixType.SeriesSeason:
                    url = String.Format(Constants.SeriesSeasonsBonusMaterials, idtup.Id, idtup.SeasonId);
                    break;
                default: throw new Exception("Invalid request for TitleType: " + TitleType);
            }

            url = OAuthHelpers.GetOAuthRequestUrl(Login.SharedSecret,
                Login.ConsumerKey,
                url,
                "GET",
                TokenSecret,
                ExtraParams);
            try
            {
                var doc = AsyncHelpers.NetflixLoadXDocumentAsync(url);
                
                var bonus = from movie
                            in (await doc).Elements("bonus_materials")
                            select movie.Element("link") != null ? (String)movie.Element("link").Attribute("href") : "";
                return bonus.ToList();
            }
            catch (ApiException ex)
            {
                ///bonus materials aren't terribly common
                throw new NetflixApiException("No bonus materials found", ex);
            }
        }

        public async Task<List<Title>> GetDiscs(String NetflixIdUrl, Boolean OnUserBehalf)
        {
            return await GetDiscs(NetflixIdUrl, OnUserBehalf, null);
        }
        public async Task<List<Title>> GetDiscs(String NetflixId, NetflixType TitleType, Boolean OnUserBehalf = true)
        {
            return await GetDiscs(NetflixId, OnUserBehalf, TitleType);
        }
        public async Task<List<Title>> GetDiscs(String NetflixId, Account NetflixAccount, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            Login.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = Fill.GetTokens(OnUserBehalf, NetflixAccount, out TokenSecret);

            return await GetDiscs(NetflixId, extraParams, TokenSecret, TitleType);
        }
        public async Task<List<Title>> GetDiscs(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            Login.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = Fill.GetTokens(OnUserBehalf, out TokenSecret);

            return await GetDiscs(NetflixId, extraParams, TokenSecret, TitleType);
        }
        private async Task<List<Title>> GetDiscs(String NetflixId, Dictionary<String, String> ExtraParams, String TokenSecret, NetflixType? TitleType = null)
        {
            TitleType = Fill.GetNetflixType(NetflixId, TitleType);
            var idtup = GeneralHelpers.GetIdFromUrl(NetflixId);

            String url = "";
            switch (TitleType)
            {
                case NetflixType.Movie:
                    url = String.Format(Constants.MoviesDiscs, idtup.Id);
                    break;
                case NetflixType.Series:
                    url = String.Format(Constants.SeriesDiscs, idtup.Id);
                    break;
                case NetflixType.SeriesSeason:
                    url = String.Format(Constants.SeriesSeasonsDiscs, idtup.Id, idtup.SeasonId);
                    break;
                default: throw new Exception("Invalid request for TitleType: " + TitleType);
            }

            url = OAuthHelpers.GetOAuthRequestUrl(Login.SharedSecret,
                Login.ConsumerKey,
                url,
                "GET",
                TokenSecret,
                ExtraParams);
            
            var doc = AsyncHelpers.NetflixLoadXDocumentAsync(url);
            return await Fill.GetBaseTitleInfo(doc, "catalog_title");
            
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
            Login.CheckInformationSet();

            Account NetflixAccount = null;
            if (OnUserBehalf)
                NetflixAccount = FlixSharp.Netflix.SafeReturnUserInfo();

            TitleType = Fill.GetNetflixType(NetflixId, TitleType);

            var idtup = GeneralHelpers.GetIdFromUrl(NetflixId);
            NetflixId = idtup.Id;

            ///1) get base
            var nfp = GetBasePerson(NetflixId, NetflixAccount, OnUserBehalf, TitleType);

            ///2) get filmography
            var filmography = GetFilmography(NetflixId, NetflixAccount, OnUserBehalf, TitleType);

            Person person = await nfp;
            person.completeness = PersonExpansion.Complete;
            person.Filmography = await filmography;
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
            Login.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = Fill.GetTokens(OnUserBehalf, NetflixAccount, out TokenSecret);

            return await GetBasePerson(NetflixId, extraParams, TokenSecret, TitleType);
        }
        public async Task<Person> GetBasePerson(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            Login.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = Fill.GetTokens(OnUserBehalf, out TokenSecret);

            return await GetBasePerson(NetflixId, extraParams, TokenSecret, TitleType);
        }
        private async Task<Person> GetBasePerson(String NetflixId, Dictionary<String, String> ExtraParams, String TokenSecret, NetflixType? TitleType = null)
        {
            TitleType = Fill.GetNetflixType(NetflixId, TitleType);
            var idtup = GeneralHelpers.GetIdFromUrl(NetflixId);
            NetflixId = idtup.Id;

            String url = "";
            switch (TitleType)
            {
                case NetflixType.People:
                    url = String.Format(Constants.PeopleBaseInfo, NetflixId);
                    break;
                default: return null;
            }

            url = OAuthHelpers.GetOAuthRequestUrl(Login.SharedSecret,
                Login.ConsumerKey,
                url,
                "GET",
                TokenSecret,
                ExtraParams);

            var doc = AsyncHelpers.NetflixLoadXDocumentAsync(url);

            Person p = (from person
                            in (await doc).Elements("person")
                        select new Person(PersonExpansion.Minimal)
                        {
                            IdUrl = person.Element("id").Value,
                            Name = person.Element("name").Value,
                            Bio = (String)person.Element("bio"),
                            NetflixSiteUrl = (from webpage
                                    in person.Elements("link")
                                              where (String)webpage.Attribute("title") == "web page"
                                              select (String)webpage.Attribute("href")).FirstOrDefault()
                        }).SingleOrDefault();
            return p;

        }

        public async Task<List<Title>> GetFilmography(String NetflixIdUrl, Boolean OnUserBehalf)
        {
            return await GetFilmography(NetflixIdUrl, OnUserBehalf, null);
        }
        public async Task<List<Title>> GetFilmography(String NetflixId, NetflixType TitleType, Boolean OnUserBehalf = true)
        {
            return await GetFilmography(NetflixId, OnUserBehalf, TitleType);
        }
        public async Task<List<Title>> GetFilmography(String NetflixId, Account NetflixAccount, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            Login.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = Fill.GetTokens(OnUserBehalf, NetflixAccount, out TokenSecret);

            return await GetFilmography(NetflixId, extraParams, TokenSecret, TitleType);
        }
        public async Task<List<Title>> GetFilmography(String NetflixId, Boolean OnUserBehalf = true, NetflixType? TitleType = null)
        {
            Login.CheckInformationSet();
            String TokenSecret;
            Dictionary<String, String> extraParams = Fill.GetTokens(OnUserBehalf, out TokenSecret);

            return await GetFilmography(NetflixId, extraParams, TokenSecret, TitleType);
        }
        private async Task<List<Title>> GetFilmography(String NetflixId, Dictionary<String, String> ExtraParams, String TokenSecret, NetflixType? TitleType = null)
        {
            TitleType = Fill.GetNetflixType(NetflixId, TitleType);
            var idtup = GeneralHelpers.GetIdFromUrl(NetflixId);
            NetflixId = idtup.Id;

            String url = "";
            switch (TitleType)
            {
                case NetflixType.People:
                    url = String.Format(Constants.PeopleFilmography, NetflixId);
                    break;
                default: return null;
            }

            url = OAuthHelpers.GetOAuthRequestUrl(Login.SharedSecret,
                Login.ConsumerKey,
                url,
                "GET",
                TokenSecret,
                ExtraParams);

            var doc = AsyncHelpers.NetflixLoadXDocumentAsync(url);
            return await Fill.GetBaseTitleInfo(doc, "filmography_item");
            
        }

    }

}
