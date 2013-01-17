using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp.Holders.RottenTomatoes
{
    public class Title
    {
        public String Id { get; set; }
        
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
        public List<Person> Directors { get; set; }
    }

}
