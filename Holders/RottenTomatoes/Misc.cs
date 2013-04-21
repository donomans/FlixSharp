using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp.Holders.RottenTomatoes
{
    public class ReleaseDate
    {
        public ReleaseDateType ReleaseType { get; set; }
        public DateTime? Date { get; set; }
    }

    public class Rating
    {
        public RottenRatingType Type { get; set; }
        public RottenRating RottenTomatoRating { get; set; }
        public Int32 Score { get; set; }
    }

    public enum ReleaseDateType
    {
        Theater,
        DVD
    }

    public enum RottenRatingType
    {
        Critic,
        Audience
    }

    public enum RottenRating
    {
        Fresh,
        CertifiedFresh,
        Upright,
        /// some others in here
        Spilled,
        Rotten,
        None
    }


    public class Poster
    {
        public PosterType Type { get; set; }
        public String Url { get; set; }
    }

    public enum PosterType
    {
        Thumbnail,
        Profile,
        Detailed,
        Original
    }

    public class AlternateId
    {
        public AlternateIdType Type { get; set; }
        public String Id { get; set; }
    }

    public enum AlternateIdType
    {
        Imdb
    }

    public class Clip
    {
        public String Title { get; set; }
        public Int32 Duration { get; set; }
        public String ThumbnailUrl { get; set; }
        public String SourceUrl { get; set; }
    }

    public class Review
    {
        public String Critic { get; set; }
        public DateTime? Date { get; set; }
        public RottenRating Freshness { get; set; }
        public String Publication { get; set; }
        public String Quote { get; set; }
        public String SourceUrl { get; set; }
    }

    public enum ReviewType
    {
        All,
        Top_Critic,
        DVD
    }
}
