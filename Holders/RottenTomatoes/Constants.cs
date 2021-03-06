﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp.Holders.RottenTomatoes
{
    internal static class Constants
    {
        public const String BaseV1Url = "http://api.rottentomatoes.com/api/public/v1.0.json?apikey={0}";
        public const String SearchUrl = "http://api.rottentomatoes.com/api/public/v1.0/movies.json?apikey={0}&q={1}&page_limit={2}&page={3}";

        public const String BoxOfficeUrl = "http://api.rottentomatoes.com/api/public/v1.0/lists/movies/box_office.json?limit={2}&country={1}&apikey={0}";
        public const String InTheatersUrl = "http://api.rottentomatoes.com/api/public/v1.0/lists/movies/in_theaters.json?page_limit={2}&&page={3}&country={1}&apikey={0}";
        public const String OpeningMoviesUrl = "http://api.rottentomatoes.com/api/public/v1.0/lists/movies/opening.json?limit={2}&country={1}&apikey={0}";
        public const String UpcomingMoviesUrl = "http://api.rottentomatoes.com/api/public/v1.0/lists/movies/upcoming.json?page_limit={2}&&page={3}&country={1}&apikey={0}";

        public const String TopRentalsUrl = "http://api.rottentomatoes.com/api/public/v1.0/lists/dvds/top_rentals.json?limit={2}&country={1}&apikey={0}";
        public const String CurrentReleaseDVDsUrl = "http://api.rottentomatoes.com/api/public/v1.0/lists/dvds/current_releases.json?page_limit={2}&&page={3}&country={1}&apikey={0}";
        public const String NewReleaseDVDsUrl = "http://api.rottentomatoes.com/api/public/v1.0/lists/dvds/new_releases.json?page_limit={2}&&page={3}&country={1}&apikey={0}";
        public const String UpcomingDVDsUrl = "http://api.rottentomatoes.com/api/public/v1.0/lists/dvds/upcoming.json?page_limit={2}&&page={3}&country={1}&apikey={0}";

        public const String MoviesInfoUrl = "http://api.rottentomatoes.com/api/public/v1.0/movies/{1}.json?apikey={0}";
        
        public const String CastInfoUrl = "http://api.rottentomatoes.com/api/public/v1.0/movies/{1}/cast.json?apikey={0}";
        public const String ClipsUrl = "http://api.rottentomatoes.com/api/public/v1.0/movies/{1}/clips.json?apikey={0}";
        public const String ReviewsUrl = "http://api.rottentomatoes.com/api/public/v1.0/movies/{1}/reviews.json?apikey={0}&review_type={2}&country={3}&page_limit={4}&page={5}";
        public const String SimilarMoviesUrl = "http://api.rottentomatoes.com/api/public/v1.0/movies/{1}/similar.json?limit={2}&apikey={0}";

        public const String MovieAliasUrl = "http://api.rottentomatoes.com/api/public/v1.0/movie_alias.json?id={1}&type={2}&apikey={0}";
    }
}
