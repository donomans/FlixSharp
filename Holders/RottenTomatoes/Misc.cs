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
        public DateTime Date { get; set; }
    }

    public class Rating
    {
        public RatingType Type { get; set; }
        public RottenRating RottenTomatoRating { get; set; }
        public Int32 Score { get; set; }
    }

    public enum ReleaseDateType
    {
        Theatrical,
        DVD
    }

    public enum RatingType
    {
        Critic,
        Audience
    }

    public enum RottenRating
    {
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

    //public enum RoleType
    //{
    //    Actor,
    //    Director
    //}

    public class AlternateId
    {
        public AlternateIdType Type { get; set; }
        public String Id { get; set; }
    }
    public enum AlternateIdType
    {
        Imdb
    }
}
