using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp.Constants
{
    internal static class NetflixConstants
    {
        public const String RequestUrl = "http://api-public.netflix.com/oauth/request_token";
        public const String AccessUrl = "http://api-public.netflix.com/oauth/access_token";
        public const String LoginUrl = "https://api-user.netflix.com/oauth/login";

        public const String CatalogTitleSearchUrl = "http://api-public.netflix.com/catalog/titles";
        public const String CatalogPeopleSearcUrl = "http://api-public.netflix.com/catalog/people";
        public const String CatalogTitleAutoCompleteUrl = "http://api-public.netflix.com/catalog/titles/autocomplete";


        public const String MoviesBaseInfo = "http://api-public.netflix.com/catalog/titles/movies/{0}";
        public const String MoviesSimilars = "http://api-public.netflix.com/catalog/titles/movies/{0}/similars";
        public const String MoviesSynopsis = "http://api-public.netflix.com/catalog/titles/movies/{0}/synopsis";
        public const String MoviesCast = "http://api-public.netflix.com/catalog/titles/movies/{0}/cast";
        public const String MoviesDirectors = "http://api-public.netflix.com/catalog/titles/movies/{0}/directors";
        public const String MoviesFormatAvailability = "http://api-public.netflix.com/catalog/titles/movies/{0}/format_availability";
        public const String MoviesScreenFormat = "http://api-public.netflix.com/catalog/titles/movies/{0}/screen_formats";
        public const String MoviesAwards = "http://api-public.netflix.com/catalog/titles/movies/{0}/awards";

        public const String SeriesBaseInfo = "http://api-public.netflix.com/catalog/titles/series/{0}";
        public const String SeriesSimilars = "http://api-public.netflix.com/catalog/titles/series/{0}/similars";
        public const String SeriesSynopsis = "http://api-public.netflix.com/catalog/titles/series/{0}/synopsis";
        public const String SeriesCast = "http://api-public.netflix.com/catalog/titles/series/{0}/cast";
        public const String SeriesDirectors = "http://api-public.netflix.com/catalog/titles/series/{0}/directors";
        public const String SeriesFormatAvailability = "http://api-public.netflix.com/catalog/titles/series/{0}/format_availability";
        public const String SeriesScreenFormat = "http://api-public.netflix.com/catalog/titles/series/{0}/screen_formats";
        public const String SeriesAwards = "http://api-public.netflix.com/catalog/titles/series/{0}/awards";

        public const String PeopleFilmography = "http://api-public.netflix.com/catalog/people/{0}/filmography";

        public const String RelatedTitles = "";
        
        internal static class Schemas
        {
            public const String CategoryGenre = "http://api-public.netflix.com/categories/genres";
            public const String CategoryMpaaRating = "http://api-public.netflix.com/categories/mpaa_ratings";
            public const String CategoryTvRating = "http://api-public.netflix.com/categories/tv_ratings";

            public const String LinkCast = "http://schemas.netflix.com/catalog/people.cast";
            public const String LinkDirectors = "http://schemas.netflix.com/catalog/people.directors";
            public const String LinkFormatAvailability = "http://schemas.netflix.com/catalog/titles/format_availability";
            public const String LinkScreenFormat = "http://api-public.netflix.com/categories/screen_formats";
            public const String LinkTitleFormat = "http://api-public.netflix.com/categories/title_formats";
            public const String LinkLanguagesAndAudio = "http://schemas.netflix.com/catalog/titles/languages_and_audio";
            

            public const String LinkTitlesSimilar = "http://schemas.netflix.com/catalog/titles.similars";
            public const String LinkTitlesSeries = "http://schemas.netflix.com/catalog/titles.series";
            public const String LinkTitlesDiscs = "http://schemas.netflix.com/catalog/titles.discs";
            public const String LinkTitlesFilmography = "http://schemas.netflix.com/catalog/titles.filmography";
        }
    }
}
