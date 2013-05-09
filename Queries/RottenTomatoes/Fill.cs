using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlixSharp.Holders.RottenTomatoes;
using Newtonsoft.Json.Linq;
using FlixSharp.Holders;
using FlixSharp.Helpers.Async;
using FlixSharp.Helpers.RottenTomatoes;

namespace FlixSharp.Queries.RottenTomatoes
{
    public class Fill
    {
        public FillLists Lists = new FillLists();
        public FillTitles Titles = new FillTitles();
        public FillPeople People = new FillPeople();

        internal static async Task<List<Title>> GetBaseTitleInfo(Task<JObject> json)
        {
            //dynamic dynjson = await json;
            var jojson = await json;
            //List<Title> movies = new List<Title>();
            //if (dynjson.movies != null)
            //    foreach (var m in dynjson.movies)
            //        movies.Add(FillTitleInfo(m));
            //else
            //    movies.Add(FillTitleInfo(dynjson));

            List<Title> movies = new List<Title>();
            if (jojson["movies"] != null)
                foreach (JObject m in jojson["movies"])
                    movies.Add(FillTitleInfo(m));
            else
                movies.Add(FillTitleInfo(jojson));
            
            return movies;
        }
        private static Title FillTitleInfo(JObject m)
        {
            //System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
            //s.Start();
            Title t = new Title();
            t.Id = m["id"].ToString();
            t.FullTitle = m["title"].ToString();
            var year = m["year"];
            t.Year = year.ToString() != "" ? (Int32)year : 0;
            var mpaa_rating = m["mpaa_rating"].ToString().Replace("-", "");
            t.Rating = (MpaaRating)Enum.Parse(typeof(MpaaRating), mpaa_rating);
            var runtime = m["runtime"];
            t.RunTime = runtime != null && runtime.ToString() != "" ? (Int32)runtime : 0;
            //s.Stop();
            //var em = s.ElapsedMilliseconds;
            //s.Start();
            var ratings = m["ratings"];
            if (ratings != null)
            {
                var critics_score = ratings["critics_score"];
                if (critics_score != null)
                    t.Ratings.Add(new Rating()
                    {
                        Type = RottenRatingType.Critic,
                        RottenTomatoRating = ratings["critics_rating"] != null ?
                        (RottenRating)Enum.Parse(typeof(RottenRating), ratings["critics_rating"].ToString().Replace(" ", ""))
                        : RottenRating.None,
                        Score = (Int32)critics_score
                    });
                var audience_score = ratings["audience_score"];
                if (audience_score != null)
                    t.Ratings.Add(new Rating()
                    {
                        Type = RottenRatingType.Audience,
                        RottenTomatoRating = ratings["audience_rating"] != null ?
                        (RottenRating)Enum.Parse(typeof(RottenRating), ratings["audience_rating"].ToString().Replace(" ", ""))
                        : RottenRating.None,
                        Score = (Int32)audience_score
                    });
            }
            //s.Stop();
            //em = s.ElapsedMilliseconds;
            //s.Start();
            var release_dates = m["release_dates"];
            if (release_dates != null)
            {
                if (release_dates["theater"] != null)
                {
                    t.ReleaseDates.Add(new ReleaseDate()
                    {
                        Date = (DateTime?)release_dates["theater"],//DateTime.Parse(m.release_dates.theater),
                        ReleaseType = ReleaseDateType.Theater
                    });
                }
                if (release_dates["dvd"] != null)
                {
                    t.ReleaseDates.Add(new ReleaseDate()
                    {
                        Date = (DateTime?)release_dates["dvd"],//DateTime.Parse(m.release_dates.dvd),
                        ReleaseType = ReleaseDateType.DVD
                    });
                }
            }
            t.Synopsis = m["synopsis"].ToString();
            //s.Stop();
            //em = s.ElapsedMilliseconds;
            //s.Start();
            var alternate_ids = m["alternate_ids"];
            if (alternate_ids != null)
                t.AlternateIds.Add(new AlternateId()
                {
                    Id = (String)alternate_ids["imdb"],
                    Type = AlternateIdType.Imdb
                });
            t.RottenTomatoesSiteUrl = m["links"] != null ? 
                m["links"]["alternate"].ToString() 
                : "";
            t.Studio = (String)m["studio"];
            //s.Stop();
            //em = s.ElapsedMilliseconds;
            //s.Start();
            var posters = m["posters"];
            if (posters != null)
            {
                if (posters["thumbnail"] != null)
                    t.Posters.Add(new Poster()
                    {
                        Type = PosterType.Thumbnail,
                        Url = posters["thumbnail"].ToString()
                    });
                if (posters["profile"] != null)
                    t.Posters.Add(new Poster()
                    {
                        Type = PosterType.Profile,
                        Url = posters["profile"].ToString()
                    });
                if (posters["detailed"] != null)
                    t.Posters.Add(new Poster()
                    {
                        Type = PosterType.Detailed,
                        Url = posters["detailed"].ToString()
                    });
                if (posters["original"] != null)
                    t.Posters.Add(new Poster()
                    {
                        Type = PosterType.Original,
                        Url = posters["original"].ToString()
                    });
            }
            //s.Stop();
            //em = s.ElapsedMilliseconds;
            //s.Start();
            var abridged_cast = m["abridged_cast"];
            if (abridged_cast != null)
            {
                foreach (var actor in abridged_cast)
                {
                    t.Actors.Add(new Person()
                    {
                        Id = (String)actor["id"],
                        Name = (String)actor["name"],
                        Characters = actor["characters"] != null ?
                        new List<String>((actor["characters"] as JArray).Select(j => j.ToString()))
                        : new List<String>()
                    });
                }
            }
            //s.Stop();
            //em = s.ElapsedMilliseconds;
            //s.Start();
            var abridged_directors = m["abridged_directors"];
            if (abridged_directors != null)
            {
                foreach (var director in abridged_directors)
                    t.Directors.Add((String)director["name"]);
            }
            //s.Stop();
            //em = s.ElapsedMilliseconds;
            //s.Start();
            var genres = m["genres"];
            if (genres != null)
                t.Genres.AddRange((genres as JArray).Select(g => g.ToString()));
            //s.Stop();
            //em = s.ElapsedMilliseconds;
            return t;
        }
        /*
        private static Title FillTitleInfof(dynamic m)
        {
            System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
            s.Start();
            Title t = new Title();
            t.Id = m.id;
            t.FullTitle = m.title;
            t.Year = m.year != null && m.year.ToString() != "" ? m.year : 0;
            t.Rating = (MpaaRating)Enum.Parse(typeof(MpaaRating), m.mpaa_rating.ToString().Replace("-", ""));
            t.RunTime = m.runtime != null && m.runtime.ToString() != "" ? m.runtime : 0;
            t.CriticsConsensus = m.critics_consensus;
            s.Stop();
            var em = s.ElapsedMilliseconds;
            s.Start();
            if (m.ratings != null)
            {
                if (m.ratings.critics_score != null)
                    t.Ratings.Add(new Rating()
                    {
                        Type = RottenRatingType.Critic,
                        RottenTomatoRating = m.ratings.critics_rating != null ?
                        (RottenRating)Enum.Parse(typeof(RottenRating), m.ratings.critics_rating.ToString().Replace(" ", ""))
                        : RottenRating.None,
                        Score = m.ratings.critics_score
                    });
                if (m.ratings.audience_score != null)
                    t.Ratings.Add(new Rating()
                    {
                        Type = RottenRatingType.Audience,
                        RottenTomatoRating = m.ratings.audience_rating != null ?
                        (RottenRating)Enum.Parse(typeof(RottenRating), m.ratings.audience_rating.ToString().Replace(" ", ""))
                        : RottenRating.None,
                        Score = m.ratings.audience_score
                    });
            }
            s.Stop();
            em = s.ElapsedMilliseconds;
            s.Start(); 
            if (m.release_dates != null)
            {
                if (m.release_dates.theater != null)
                {
                    t.ReleaseDates.Add(new ReleaseDate()
                    {
                        Date = (DateTime?)m.release_dates.theater,//DateTime.Parse(m.release_dates.theater),
                        ReleaseType = ReleaseDateType.Theater
                    });
                }
                if (m.release_dates.dvd != null)
                {
                    t.ReleaseDates.Add(new ReleaseDate()
                    {
                        Date = (DateTime?)m.release_dates.dvd,//DateTime.Parse(m.release_dates.dvd),
                        ReleaseType = ReleaseDateType.DVD
                    });
                }
            }
            t.Synopsis = m.synopsis;
            s.Stop();
            em = s.ElapsedMilliseconds;
            s.Start(); 
            if (m.alternate_ids != null)
                t.AlternateIds.Add(new AlternateId()
                {
                    Id = m.alternate_ids.imdb,
                    Type = AlternateIdType.Imdb
                });
            t.RottenTomatoesSiteUrl = m.links != null ? m.links.alternate : "";
            t.Studio = m.studio;
            s.Stop();
            em = s.ElapsedMilliseconds;
            s.Start();
            if (m.posters != null)
            {
                if (m.posters.thumbnail != null)
                    t.Posters.Add(new Poster()
                    {
                        Type = PosterType.Thumbnail,
                        Url = m.posters.thumbnail
                    });
                if (m.posters.profile != null)
                    t.Posters.Add(new Poster()
                    {
                        Type = PosterType.Profile,
                        Url = m.posters.profile
                    });
                if (m.posters.detailed != null)
                    t.Posters.Add(new Poster()
                    {
                        Type = PosterType.Detailed,
                        Url = m.posters.detailed
                    });
                if (m.posters.original != null)
                    t.Posters.Add(new Poster()
                    {
                        Type = PosterType.Original,
                        Url = m.posters.original
                    });
            }
            s.Stop();
            em = s.ElapsedMilliseconds;
            s.Start();
            if (m.abridged_cast != null)
            {
                foreach (var actor in m.abridged_cast)
                {
                    //JArray chars = actor.characters;
                    //var watwat = wat.Select(j => j.ToString());
                    t.Actors.Add(new Person()
                    {
                        Id = actor.id,
                        Name = actor.name,
                        Characters = actor.characters != null ?
                        new List<String>((actor.characters as JArray).Select(j => j.ToString()))
                        : new List<String>()
                    });
                }
            }
            s.Stop();
            em = s.ElapsedMilliseconds;
            s.Start();
            if (m.abridged_directors != null)
            {
                foreach (var director in m.abridged_directors)
                    t.Directors.Add(director.name.ToString());
            }
            s.Stop();
            em = s.ElapsedMilliseconds;
            s.Start();
            if (m.genres != null)
                t.Genres.AddRange((m.genres as JArray).Select(g => g.ToString()));
            s.Stop();
            em = s.ElapsedMilliseconds;
            return t;
        }
        */
    }
    public class FillLists
    {
        #region Movie Lists
        public async Task<Titles> GetBoxOffice(String Country = "us", Int32 Limit = 10)
        {
            Login.CheckInformationSet();
            var moviejson = AsyncHelpers.RottenTomatoesLoadJObjectAsync(
                UrlBuilder.BoxOfficeUrl(Country, Limit));

            return new Titles(await Fill.GetBaseTitleInfo(moviejson));
        }

        public async Task<Titles> GetInTheaters(String Country = "us", Int32 Limit = 10, Int32 Page = 1)
        {
            Login.CheckInformationSet();
            var moviejson = AsyncHelpers.RottenTomatoesLoadJObjectAsync(
                UrlBuilder.InTheatersUrl(Country, Limit));

            return new Titles(await Fill.GetBaseTitleInfo(moviejson));
        }

        public async Task<Titles> GetOpeningMovies(String Country = "us", Int32 Limit = 10)
        {
            Login.CheckInformationSet();
            var moviejson = AsyncHelpers.RottenTomatoesLoadJObjectAsync(
                UrlBuilder.OpeningMoviesUrl(Country, Limit));

            return new Titles(await Fill.GetBaseTitleInfo(moviejson));
        }
        public async Task<Titles> GetUpcomingMovies(String Country = "us", Int32 Limit = 10, Int32 Page = 1)
        {
            Login.CheckInformationSet();
            var moviejson = AsyncHelpers.RottenTomatoesLoadJObjectAsync(
                UrlBuilder.UpcomingMoviesUrl(Country, Limit, Page));

            return new Titles(await Fill.GetBaseTitleInfo(moviejson));
        }
        #endregion

        #region DVD Lists
        public async Task<Titles> GetTopRentals(String Country = "us", Int32 Limit = 10)
        {
            Login.CheckInformationSet();
            var moviejson = AsyncHelpers.RottenTomatoesLoadJObjectAsync(
                UrlBuilder.TopRentalsUrl(Country, Limit));

            return new Titles(await Fill.GetBaseTitleInfo(moviejson));
        }

        public async Task<Titles> GetCurrentReleaseDVDs(String Country = "us", Int32 Limit = 10, Int32 Page = 1)
        {
            Login.CheckInformationSet();
            var moviejson = AsyncHelpers.RottenTomatoesLoadJObjectAsync(
                UrlBuilder.CurrentReleaseDVDsUrl(Country, Limit, Page));

            return new Titles(await Fill.GetBaseTitleInfo(moviejson));
        }

        public async Task<Titles> GetNewReleaseDVDs(String Country = "us", Int32 Limit = 10, Int32 Page = 1)
        {
            Login.CheckInformationSet();
            var moviejson = AsyncHelpers.RottenTomatoesLoadJObjectAsync(
                UrlBuilder.NewReleaseDVDsUrl(Country, Limit, Page));

            return new Titles(await Fill.GetBaseTitleInfo(moviejson));
        }

        public async Task<Titles> GetUpcomingDVDs(String Country = "us", Int32 Limit = 10, Int32 Page = 1)
        {
            Login.CheckInformationSet();
            var moviejson = AsyncHelpers.RottenTomatoesLoadJObjectAsync(
                UrlBuilder.UpcomingDVDsUrl(Country, Limit, Page));

            return new Titles(await Fill.GetBaseTitleInfo(moviejson));
        }
        #endregion
    }

    public class FillTitles
    {
        #region Detailed Info
        /// <summary>
        /// Full title information from a Rotten Tomatoes Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<Title> GetMoviesInfo(String Id)
        {
            Login.CheckInformationSet();

            var moviejson = AsyncHelpers.RottenTomatoesLoadJObjectAsync(
                UrlBuilder.MoviesInfoUrl(Id));

            return (await Fill.GetBaseTitleInfo(moviejson)).FirstOrDefault();
        }

        public async Task<List<Clip>> GetClips(String Id)
        {
            Login.CheckInformationSet();

            dynamic clipsjson = await AsyncHelpers.RottenTomatoesLoadJObjectAsync(
                UrlBuilder.ClipsUrl(Id));

            List<Clip> clips = new List<Clip>();

            foreach (var clip in clipsjson.clips)
            {
                Clip c = new Clip()
                {
                    Title = clip.title,
                    Duration = clip.duration,
                    ThumbnailUrl = clip.thumbnail,
                    SourceUrl = clip.links != null ? clip.links.alternate : ""
                };
                clips.Add(c);
            }
            return clips;
        }

        public async Task<List<Review>> GetReviews(String Id, ReviewType ReviewType, String Country = "us", Int32 Limit = 10, Int32 Page = 1)
        {
            Login.CheckInformationSet();

            dynamic reviewsjson = await AsyncHelpers.RottenTomatoesLoadJObjectAsync(
                UrlBuilder.ReviewsUrl(Id, ReviewType, Country, Limit, Page));

            List<Review> reviews = new List<Review>(reviewsjson.total);

            foreach (var review in reviewsjson.reviews)
            {
                Review r = new Review()
                {
                    Critic = review.critic,
                    Date = review.date != null ?
                    (DateTime?)DateTime.Parse(review.date) : null,
                    Freshness = review.freshness != null ?
                    (RottenRating)Enum.Parse(typeof(RottenRating), review.freshness, true) : RottenRating.None,
                    Publication = review.publication,
                    Quote = review.quote,
                    SourceUrl = review.links != null ? review.links.review : ""
                };
                reviews.Add(r);
            }
            return reviews;
        }

        public async Task<List<Title>> GetSimilarMovies(String Id, Int32 Limit = 5)
        {
            Login.CheckInformationSet();

            var similarjson = AsyncHelpers.RottenTomatoesLoadJObjectAsync(
                UrlBuilder.SimilarMoviesUrl(Id));

            return await Fill.GetBaseTitleInfo(similarjson);
        }
        #endregion

        public async Task<Title> GetTitleByAlias(String Id, AlternateIdType IdType = AlternateIdType.Imdb)
        {
            Login.CheckInformationSet();

            var moviejson = AsyncHelpers.RottenTomatoesLoadJObjectAsync(
                UrlBuilder.MovieAliasUrl(Id, IdType));

            return (await Fill.GetBaseTitleInfo(moviejson)).FirstOrDefault();
        }
    }

    public class FillPeople
    {
        public async Task<People> GetCast(String Id)
        {
            Login.CheckInformationSet();

            dynamic castjson = await AsyncHelpers.RottenTomatoesLoadJObjectAsync(
                UrlBuilder.CastInfoUrl(Id));

            People people = new People();
            foreach (var cast in castjson.cast)
            {
                Person p = new Person()
                {
                    Id = cast.id,
                    Name = cast.name,
                    Characters = cast.characters != null ?
                         new List<String>(cast.characters)
                         : new List<String>()
                };
                people.Add(p);
            }

            return people;
        }
    }
}
