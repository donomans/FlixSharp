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
            dynamic dynjson = await json;
            List<Title> movies = new List<Title>(dynjson.movies.Count);
            foreach (var m in dynjson.movies)
            {
                Title t = new Title();
                t.Id = m.id;
                t.FullTitle = m.title;
                t.Year = m.year;
                t.Rating = (MpaaRating)Enum.Parse(typeof(MpaaRating), m.mpaa_rating.ToString().Replace("-", ""));
                t.RunTime = m.runtime != null && m.runtime.ToString() != "" ? m.runtime : 0;
                t.CriticsConsensus = m.critics_consensus;
                if (m.ratings != null)
                {
                    if (m.ratings.critics_score != null)
                        t.Ratings.Add(new Rating()
                        {
                            Type = RatingType.Critic,
                            RottenTomatoRating = m.ratings.critics_rating != null ? 
                            (RottenRating)Enum.Parse(typeof(RottenRating), m.ratings.critics_rating.ToString().Replace(" ", "")) 
                            : RottenRating.None,
                            Score = m.ratings.critics_score
                        });
                    if (m.ratings.audience_score != null)
                        t.Ratings.Add(new Rating()
                        {
                            Type = RatingType.Audience,
                            RottenTomatoRating = m.ratings.audience_rating != null ? 
                            (RottenRating)Enum.Parse(typeof(RottenRating), m.ratings.audience_rating.ToString().Replace(" ", "")) 
                            : RottenRating.None,
                            Score = m.ratings.audience_score
                        });
                }
                if(m.release_dates != null)
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
                if (m.alternate_ids != null)
                    t.AlternateIds.Add(new AlternateId()
                    {
                        Id = m.alternate_ids.imdb,
                        Type = AlternateIdType.Imdb
                    });
                t.RottenTomatoesSiteUrl = m.links != null ? m.links.alternate : "";
                t.Studio = m.studio;
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
                if (m.abridged_cast != null)
                {
                    foreach (var actor in m.abridged_cast)
                    {
                        JArray chars = actor.characters;
                        //var watwat = wat.Select(j => j.ToString());
                        t.Actors.Add(new Person()
                        {
                            Id = actor.id,
                            Name = actor.name,
                            Characters = actor.characters != null ? 
                            new List<String>(chars.Select(j => j.ToString()))
                            : new List<String>()
                        });
                    }
                }
                if (m.abridged_directors != null)
                {
                    foreach (var director in m.abridged_directors)
                        t.Directors.Add(director.name);
                }
                if (m.genres != null)
                    t.Genres.AddRange(m.genres);
                
                movies.Add(t);
            }
            //dynamic dynjson = json.Result;
            ///rewrite this for dynamic usage?
            //throw new Exception(dynjson.movies[0].id);
            //movies.AddRange(
            //    from movie
            //    in dynjson.Children()
            //    select new Title()
            //    {
            //        Id = movie["id"].ToString(),
            //        FullTitle = movie["title"].ToString(),
            //        Year = (String)movie["year"] != "" ? (Int32)movie["year"] : 0,
            //        Rating = (MpaaRating)Enum.Parse(typeof(MpaaRating), movie["mpaa_rating"].ToString().Replace("-","")),
            //        RunTime = (String)movie["runtime"] != "" ? (Int32)movie["runtime"] : 0,
            //        CriticsConsensus = (String)movie["critics_consensus"],
            //        Ratings = movie["ratings"].HasValues ? new List<Rating>
            //        {
            //            new Rating{ 
            //                Type = RatingType.Critic,
            //                RottenTomatoRating = movie["ratings"]["critics_rating"] != null && (String)movie["ratings"]["critics_rating"] != "" 
            //                ? (RottenRating)Enum.Parse(typeof(RottenRating), movie["ratings"]["critics_rating"].ToString().Replace(" ","")) : RottenRating.None,
            //                Score = movie["ratings"]["critics_score"] != null && (String)movie["ratings"]["critics_score"] != ""
            //                ? (Int32)movie["ratings"]["critics_score"] : -1
            //            },
            //            new Rating{ 
            //                Type = RatingType.Audience,
            //                RottenTomatoRating = movie["ratings"]["audience_rating"] != null && (String)movie["ratings"]["audience_rating"] != "" 
            //                ? (RottenRating)Enum.Parse(typeof(RottenRating), movie["ratings"]["audience_rating"].ToString().Replace(" ","")) : RottenRating.None,
            //                Score = movie["ratings"]["audience_score"] != null && (String)movie["ratings"]["audience_score"] != "" 
            //                ? (Int32)movie["ratings"]["audience_score"] : -1
            //            }
            //        } : new List<Rating>(),
            //        ReleaseDates = movie["release_dates"].HasValues ? new List<ReleaseDate>
            //        {///this is dumb, but I can't easily access the JProperty.Name value in a LINQ statement, for some reason
            //            new ReleaseDate
            //            {
            //                ReleaseType = ReleaseDateType.Theater,
            //                Date = movie["release_dates"]["theater"] != null ? (DateTime?)DateTime.Parse(movie["release_dates"]["theater"].ToString()) : null
            //            },
            //            new ReleaseDate
            //            {
            //                ReleaseType = ReleaseDateType.DVD,
            //                Date = movie["release_dates"]["dvd"] != null ? (DateTime?)DateTime.Parse(movie["release_dates"]["dvd"].ToString()) : null
            //            }
            //        } : new List<ReleaseDate>(),
            //        Synopsis = movie["synopsis"].ToString(),
            //        AlternateIds = movie["alternate_ids"] != null && movie["alternate_ids"].HasValues ? new List<AlternateId>
            //        {
            //            new AlternateId
            //            {
            //                Type = AlternateIdType.Imdb,
            //                Id = movie["alternate_ids"]["imdb"] != null && (String)movie["alternate_ids"]["imdb"] != "" 
            //                ? movie["alternate_ids"]["imdb"].ToString() : ""
            //            }
            //        } : new List<AlternateId>(),
            //        RottenTomatoesSiteUrl = movie["links"].HasValues ? movie["links"]["alternate"].ToString() : "",
            //        Studio = movie["studio"] != null ? movie["studio"].ToString() : "",
            //        Posters = movie["posters"] != null && movie["posters"].HasValues ? new List<Poster>
            //        {
            //            new Poster
            //            {
            //                 Type = PosterType.Thumbnail,
            //                 Url = (String)movie["posters"]["thumbnail"]
            //            },
            //            new Poster
            //            {
            //                 Type = PosterType.Profile,
            //                 Url = (String)movie["posters"]["profile"]
            //            },
            //            new Poster
            //            {
            //                 Type = PosterType.Detailed,
            //                 Url = (String)movie["posters"]["detailed"]
            //            },
            //            new Poster
            //            {
            //                 Type = PosterType.Original,
            //                 Url = (String)movie["posters"]["original"]
            //            }
            //        } : new List<Poster>(),
            //        Actors = movie["abridged_cast"] != null && movie["abridged_cast"].HasValues 
            //            ? new List<Person>(from actor 
            //                               in movie["abridged_cast"]
            //                               select new Person
            //                               {
            //                                    Id = (String)actor["id"],
            //                                    Name = (String)actor["name"],
            //                                    Characters = actor["characters"] != null
            //                                        ? new List<String>(actor["characters"].HasValues 
            //                                            ? actor["characters"].Select(a => a.ToString()) : new String[] { }) 
            //                                            : new List<String>()
            //                               }) : new List<Person>(),
            //        Directors = movie["abridged_directors"] != null && movie["abridged_directors"].HasValues
            //            ? new List<String>(from director
            //                                in movie["abridged_directors"]
            //                               select (String)director["name"]) : new List<String>(),
            //        Genres = movie["genres"] != null && movie["genres"].HasValues
            //            ? new List<String>(movie["genres"].Select(g => g.ToString())) : new List<String>()
            //    });

            return movies;
        }
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
