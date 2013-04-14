using FlixSharp.Helpers.OAuth;
using FlixSharp.Holders.RottenTomatoes;
using FlixSharp.Queries.RottenTomatoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp.Helpers.RottenTomatoes
{
    internal class UrlBuilder
    {
        public static String SearchUrl(String SearchTerm, Int32 Limit = 10, Int32 Page = 1)
        {
            return String.Format(Constants.SearchUrl, Login.ConsumerKey, OAuthHelpers.Encode(SearchTerm), Limit, Page);
        }

        public static String BoxOfficeUrl(String Country, Int32 Limit = 10)
        {
            return String.Format(Constants.BoxOfficeUrl, Login.ConsumerKey, Country, Limit);
        }
        public static String InTheatersUrl(String Country, Int32 Limit = 10, Int32 Page = 1)
        {
            return String.Format(Constants.InTheatersUrl, Login.ConsumerKey, Country, Limit, Page);
        }
        public static String OpeningMoviesUrl(String Country, Int32 Limit = 10)
        {
            return String.Format(Constants.OpeningMoviesUrl, Login.ConsumerKey, Country, Limit);
        }
        public static String UpcomingMoviesUrl(String Country, Int32 Limit = 10, Int32 Page = 1)
        {
            return String.Format(Constants.UpcomingMoviesUrl, Login.ConsumerKey, Country, Limit, Page);
        }

        public static String TopRentalsUrl(String Country, Int32 Limit = 10)
        {
            return String.Format(Constants.TopRentalsUrl, Login.ConsumerKey, Country, Limit);
        }
        public static String CurrentReleaseDVDsUrl(String Country, Int32 Limit = 10, Int32 Page = 1)
        {
            return String.Format(Constants.CurrentReleaseDVDsUrl, Login.ConsumerKey, Country, Limit, Page);
        }
        public static String NewReleaseDVDsUrl(String Country, Int32 Limit = 10, Int32 Page = 1)
        {
            return String.Format(Constants.NewReleaseDVDsUrl, Login.ConsumerKey, Country, Limit, Page);
        }
        public static String UpcomingDVDsUrl(String Country, Int32 Limit = 10, Int32 Page = 1)
        {
            return String.Format(Constants.UpcomingDVDsUrl, Login.ConsumerKey, Country, Limit, Page);
        }

        public static String MoviesInfoUrl(String Id)
        {
            return String.Format(Constants.MoviesInfoUrl, Login.ConsumerKey, Id);
        }

        public static String CastInfoUrl(String Id)
        {
            return String.Format(Constants.CastInfoUrl, Login.ConsumerKey, Id);
        }
        public static String ClipsUrl(String Id)
        {
            return String.Format(Constants.ClipsUrl, Login.ConsumerKey, Id);
        }
        public static String ReviewsUrl(String Id, ReviewType ReviewType = ReviewType.All, String Country = "us", Int32 Limit = 10, Int32 Page = 1)
        {
            return String.Format(Constants.ReviewsUrl, Login.ConsumerKey, Id, 
                ReviewType.ToString().ToLower(), Country, Limit, Page);
        }
        public static String SimilarMoviesUrl(String Id, Int32 Limit = 5)
        {
            return String.Format(Constants.SimilarMoviesUrl, Login.ConsumerKey, Id, Limit);
        }

        public static String MovieAliasUrl(String Id, AlternateIdType IdType)
        {
            return String.Format(Constants.MovieAliasUrl, Login.ConsumerKey, Id, IdType.ToString().ToLower());
        }
    }
}
