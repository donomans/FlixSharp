using FlixSharp.Helpers.Async;
using FlixSharp.Helpers.RottenTomatoes;
using FlixSharp.Holders;
using FlixSharp.Holders.RottenTomatoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp.Queries.RottenTomatoes
{
    public class Search
    {
        public async Task<Titles> SearchTitles(String Title, Int32 Limit = 10)
        {
            var moviejson = await AsyncHelpers.RottenTomatoesLoadXDocumentAsync(UrlBuilder.SearchUrl(Title, Limit));

            Titles movies = new Titles();

            //foreach (var movie in moviejson["movies"].Children())
            //{
            //    var actors = from actor
            //                 in movie["abridged_cast"]
            //                 select new Person
            //                 {
            //                     Id = (String)actor["id"],
            //                     Name = (String)actor["name"],
            //                     Characters = actor["characters"] != null 
            //                     ? new List<String>(actor["characters"].HasValues ? actor["characters"].Select(a => a.ToString()) : new String[] { }) : new List<String>()
            //                 };
            //    var Actors = movie["abridged_cast"] != null && movie["abridged_cast"].HasValues
            //            ? new List<Person>(from actor
            //                               in movie["abridged_cast"]
            //                               select new Person
            //                               {
            //                                   Id = (String)actor["id"],
            //                                   Name = (String)actor["name"],
            //                                   Characters = actor["characters"] != null
            //                     ? new List<String>(actor["characters"].HasValues ? actor["characters"].Select(a => a.ToString()) : new String[] { }) : new List<String>()
            //                               }) : new List<Person>();
            //}

            movies.AddRange(
                from movie
                in (moviejson)["movies"].Children()
                select new Title()
                {
                    Id = movie["id"].ToString(),
                    FullTitle = movie["title"].ToString(),
                    Year = (String)movie["year"] != "" ? (Int32)movie["year"] : 0,
                    Rating = (MpaaRating)Enum.Parse(typeof(MpaaRating), movie["mpaa_rating"].ToString()),
                    RunTime = (String)movie["runtime"] != "" ? (Int32)movie["runtime"] : 0,
                    CriticsConsensus = (String)movie["critics_consensus"],
                    Ratings = movie["ratings"].HasValues ? new List<Rating>
                    {
                        new Rating{ 
                            Type = RatingType.Critic,
                            RottenTomatoRating = movie["ratings"]["critics_rating"] != null && (String)movie["ratings"]["critics_rating"] != "" 
                            ? (RottenRating)Enum.Parse(typeof(RottenRating), movie["ratings"]["critics_rating"].ToString().Replace(" ","")) : RottenRating.None,
                            Score = movie["ratings"]["critics_score"] != null && (String)movie["ratings"]["critics_score"] != ""
                            ? (Int32)movie["ratings"]["critics_score"] : -1
                        },
                        new Rating{ 
                            Type = RatingType.Audience,
                            RottenTomatoRating = movie["ratings"]["audience_rating"] != null && (String)movie["ratings"]["audience_rating"] != "" 
                            ? (RottenRating)Enum.Parse(typeof(RottenRating), movie["ratings"]["audience_rating"].ToString().Replace(" ","")) : RottenRating.None,
                            Score = movie["ratings"]["audience_score"] != null && (String)movie["ratings"]["audience_score"] != "" 
                            ? (Int32)movie["ratings"]["audience_score"] : -1
                        }
                    } : new List<Rating>(),
                    ReleaseDates = movie["release_dates"].HasValues ? new List<ReleaseDate>
                    {
                        new ReleaseDate
                        {
                            ReleaseType = ReleaseDateType.Theatrical,
                            Date = movie["release_dates"]["theater"] != null ? DateTime.Parse(movie["release_dates"]["theater"].ToString()) : DateTime.MinValue
                        },
                        new ReleaseDate
                        {
                            ReleaseType = ReleaseDateType.DVD,
                            Date = movie["release_dates"]["dvd"] != null ? DateTime.Parse(movie["release_dates"]["dvd"].ToString()) : DateTime.MinValue
                        }
                    } : new List<ReleaseDate>(),
                    Synopsis = movie["synopsis"].ToString(),
                    AlternateIds = movie["alternate_ids"] != null && movie["alternate_ids"].HasValues ? new List<AlternateId>
                    {
                        new AlternateId
                        {
                            Type = AlternateIdType.Imdb,
                            Id = movie["alternate_ids"]["imdb"] != null && (String)movie["alternate_ids"]["imdb"] != "" 
                            ? movie["alternate_ids"]["imdb"].ToString() : ""
                        }
                    } : new List<AlternateId>(),
                    RottenTomatoesSiteUrl = movie["links"].HasValues ? movie["links"]["alternate"].ToString() : "",
                    Studio = movie["studio"] != null ? movie["studio"].ToString() : "",
                    Posters = movie["posters"] != null && movie["posters"].HasValues ? new List<Poster>
                    {
                        new Poster
                        {
                             Type = PosterType.Thumbnail,
                             Url = (String)movie["posters"]["thumbnail"]
                        },
                        new Poster
                        {
                             Type = PosterType.Profile,
                             Url = (String)movie["posters"]["profile"]
                        },
                        new Poster
                        {
                             Type = PosterType.Detailed,
                             Url = (String)movie["posters"]["detailed"]
                        },
                        new Poster
                        {
                             Type = PosterType.Original,
                             Url = (String)movie["posters"]["original"]
                        }
                    } : new List<Poster>(),
                    Actors = movie["abridged_cast"] != null && movie["abridged_cast"].HasValues 
                        ? new List<Person>(from actor 
                                           in movie["abridged_cast"]
                                           select new Person
                                           {
                                                Id = (String)actor["id"],
                                                Name = (String)actor["name"],
                                                Characters = actor["characters"] != null
                                                    ? new List<String>(actor["characters"].HasValues 
                                                        ? actor["characters"].Select(a => a.ToString()) : new String[] { }) 
                                                        : new List<String>()
                                           }) : new List<Person>(),
                    Directors = movie["abridged_directors"] != null && movie["abridged_directors"].HasValues
                        ? new List<String>(from director
                                            in movie["abridged_directors"]
                                           select (String)director["name"]) : new List<String>(),
                    Genres = movie["genres"] != null && movie["genres"].HasValues
                        ? new List<String>(movie["genres"].Select(g => g.ToString())) : new List<String>()
                });

            return movies;

        }
    }
}
