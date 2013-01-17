using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp.Constants.Netflix
{
    internal static class NetflixConstants
    {
        public const String RequestUrl = "http://api-public.netflix.com/oauth/request_token";
        public const String AccessUrl = "http://api-public.netflix.com/oauth/access_token";
        public const String LoginUrl = "https://api-user.netflix.com/oauth/login";

        public const String CatalogTitleSearchUrl = "http://api-public.netflix.com/catalog/titles";
        public const String CatalogPeopleSearcUrl = "http://api-public.netflix.com/catalog/people";
        public const String CatalogTitleAutoCompleteUrl = "http://api-public.netflix.com/catalog/titles/autocomplete";

        public const String TitlesDiscs = "http://api-public.netflix.com/catalog/titles/discs/{0}";///?

        public const String MoviesBaseInfo = "http://api-public.netflix.com/catalog/titles/movies/{0}";
        public const String MoviesSimilars = "http://api-public.netflix.com/catalog/titles/movies/{0}/similars";
        public const String MoviesSynopsis = "http://api-public.netflix.com/catalog/titles/movies/{0}/synopsis";
        public const String MoviesDiscs = "http://api-public.netflix.com/catalog/titles/movies/{0}/discs";
        public const String MoviesCast = "http://api-public.netflix.com/catalog/titles/movies/{0}/cast";
        public const String MoviesDirectors = "http://api-public.netflix.com/catalog/titles/movies/{0}/directors";
        public const String MoviesFormatAvailability = "http://api-public.netflix.com/catalog/titles/movies/{0}/format_availability";
        public const String MoviesScreenFormat = "http://api-public.netflix.com/catalog/titles/movies/{0}/screen_formats";
        public const String MoviesAwards = "http://api-public.netflix.com/catalog/titles/movies/{0}/awards";
        public const String MoviesBonusMaterials = "http://api-public.netflix.com/catalog/titles/movies/{0}/bonus_materials";
        public const String MoviessLanguagesAndAudio = "http://api-public.netflix.com/catalog/titles/movies/{0}/languages_and_audio";


        public const String SeriesBaseInfo = "http://api-public.netflix.com/catalog/titles/series/{0}";
        public const String SeriesSimilars = "http://api-public.netflix.com/catalog/titles/series/{0}/similars";
        public const String SeriesDiscs = "http://api-public.netflix.com/catalog/titles/series/{0}/discs";
        public const String SeriesSynopsis = "http://api-public.netflix.com/catalog/titles/series/{0}/synopsis";
        public const String SeriesCast = "http://api-public.netflix.com/catalog/titles/series/{0}/cast";
        public const String SeriesDirectors = "http://api-public.netflix.com/catalog/titles/series/{0}/directors";
        public const String SeriesFormatAvailability = "http://api-public.netflix.com/catalog/titles/series/{0}/format_availability";
        public const String SeriesScreenFormat = "http://api-public.netflix.com/catalog/titles/series/{0}/screen_formats";
        public const String SeriesAwards = "http://api-public.netflix.com/catalog/titles/series/{0}/awards";
        public const String SeriesBonusMaterials = "http://api-public.netflix.com/catalog/titles/series/{0}/bonus_materials";
        public const String SeriesLanguagesAndAudio = "http://api-public.netflix.com/catalog/titles/series/{0}/languages_and_audio";


        public const String SeriesSeasonsBaseInfo = "http://api-public.netflix.com/catalog/titles/series/{0}/seasons/{1}";
        public const String SeriesSeasonsCast = "http://api-public.netflix.com/catalog/titles/series/{0}/seasons/{1}/cast";
        public const String SeriesSeasonsDirectors = "http://api-public.netflix.com/catalog/titles/series/{0}/seasons/{1}/directors";
        public const String SeriesSeasonsFormatAvailability = "http://api-public.netflix.com/catalog/titles/series/{0}/seasons/{1}/format_availability";
        public const String SeriesSeasonsScreenFormat = "http://api-public.netflix.com/catalog/titles/series/{0}/seasons/{1}/screen_formats";
        public const String SeriesSeasonsSynopsis = "http://api-public.netflix.com/catalog/titles/series/{0}/seasons/{1}/synopsis";
        public const String SeriesSeasonsSimilars = "http://api-public.netflix.com/catalog/titles/series/{0}/seasons/{1}/similars";
        public const String SeriesSeasonsDiscs = "http://api-public.netflix.com/catalog/titles/series/{0}/seasons/{1}/discs";
        public const String SeriesSeasonsAwards = "http://api-public.netflix.com/catalog/titles/series/{0}/seasons/{1}/awards";
        public const String SeriesSeasonsLanguagesAndAudio = "http://api-public.netflix.com/catalog/titles/series/{0}/seasons/{1}/languages_and_audio";//?
        public const String SeriesSeasonsBonusMaterials = "http://api-public.netflix.com/catalog/titles/series/{0}/seasons/{1}/bonus_materials";
        public const String SeriesSeasonsEpisodes = "http://api-public.netflix.com/catalog/titles/series/{0}/seasons/{1}/episodes";

        public const String DiscsBaseInfo = "http://api-public.netflix.com/catalog/titles/discs/{0}";
        public const String DiscsSynopsis = "http://api-public.netflix.com/catalog/titles/discs/{0}/synopsis";
        public const String DiscsCast = "http://api-public.netflix.com/catalog/titles/discs/{0}/cast";
        public const String DiscsFormatAvailability = "http://api-public.netflix.com/catalog/titles/discs/{0}/format_availability";
        public const String DiscsScreenFormat = "http://api-public.netflix.com/catalog/titles/discs/{0}/screen_formats";
        public const String DiscsSimilars = "http://api-public.netflix.com/catalog/titles/discs/{0}/similars";
        public const String DiscsLanguagesAndAudio = "http://api-public.netflix.com/catalog/titles/discs/{0}/languages_and_audio";

        public const String PeopleBaseInfo = "http://api-public.netflix.com/catalog/people/{0}";
        public const String PeopleFilmography = "http://api-public.netflix.com/catalog/people/{0}/filmography";

        //public const String RelatedTitles = "";
        
        internal static class Schemas
        {
            public const String CategoryGenre = "http://api-public.netflix.com/categories/genres";
            public const String CategoryMpaaRating = "http://api-public.netflix.com/categories/mpaa_ratings";
            public const String CategoryTvRating = "http://api-public.netflix.com/categories/tv_ratings";

            public const String TitleOfficialUrl = "http://schemas.netflix.com/catalog/titles/official_url";

            public const String TitlesSynopsis = "http://schemas.netflix.com/catalog/titles/synopsis";

            public const String LinkCast = "http://schemas.netflix.com/catalog/people.cast";
            public const String LinkDirectors = "http://schemas.netflix.com/catalog/people.directors";
            public const String LinkFormatAvailability = "http://schemas.netflix.com/catalog/titles/format_availability";
            public const String LinkScreenFormat = "http://api-public.netflix.com/categories/screen_formats";
            public const String LinkTitleFormat = "http://api-public.netflix.com/categories/title_formats";
            public const String LinkLanguagesAndAudio = "http://schemas.netflix.com/catalog/titles/languages_and_audio";
            public const String LinkAwards = "http://schemas.netflix.com/catalog/titles/awards";
            public const String LinkBonusMaterials = "http://schemas.netflix.com/catalog/titles/bonus_materials";
            public const String LinkDiscs = "http://schemas.netflix.com/catalog/titles.discs";

            public const String LinkTitlesSimilar = "http://schemas.netflix.com/catalog/titles.similars";
            public const String LinkTitlesSeries = "http://schemas.netflix.com/catalog/titles.series";
            public const String LinkTitlesSeason = "http://schemas.netflix.com/catalog/titles.season";
            public const String LinkTitlesDiscs = "http://schemas.netflix.com/catalog/titles.discs";
            public const String LinkTitlesFilmography = "http://schemas.netflix.com/catalog/titles.filmography";
            public const String LinkTitlesPrograms = "http://schemas.netflix.com/catalog/titles.programs";
        }
    }
}
