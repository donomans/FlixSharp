using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp.Holders.RottenTomatoes
{
    public class Title: ITitle
    {
        public Title()
        {
            Genres = new List<String>();
            AlternateIds = new List<AlternateId>();
            ReleaseDates = new List<ReleaseDate>();
            Ratings = new List<Rating>();
            Posters = new List<Poster>();
            Actors = new List<Person>();
            Directors = new List<String>();
        }
        public String Id { get; set; }

        public String FullId { get { return Id; } }
        public String FullTitle { get; set; }
        public Int32 Year { get; set; }
        public List<String> Genres { get; set; }
        public MpaaRating Rating { get; set; }
        public Int32 RunTime { get; set; } //Minutes
        public String Studio { get; set; }
        public List<AlternateId> AlternateIds { get; set; }

        public String CriticsConsensus { get; set; }
        public List<ReleaseDate> ReleaseDates { get; set; }
        public List<Rating> Ratings { get; set; }
        public String Synopsis { get; set; }
        
        public List<Poster> Posters { get; set; }
        public String RottenTomatoesSiteUrl { get; set; }

        public List<Person> Actors { get; set; }
        public List<String> Directors { get; set; }

        public List<Clip> Clips { get; set; }
        public List<Review> Reviews { get; set; }
    }
}
