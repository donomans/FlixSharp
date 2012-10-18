using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlixSharp.Holders
{
    public class Movie
    {
        public Movie(TitleExpansion Completeness)
        {
            completeness = Completeness;
        }

        public TitleExpansion Completeness { get { return completeness;} }
        public TitleExpansion completeness = TitleExpansion.Minimal;

        #region Basic
        public String Id
        {
            get
            {
                if (id != "")
                    return id;
                else
                {
                    Match m = Regex.Match(IdUrl, "[0-9]{4,9}");
                    if (m.Success)
                    {
                        id = m.Value;
                        return id;
                    }
                    else
                        return id;
                }
            }
        }
        private String id = "";
        public String IdUrl { get; set; }
        public String ShortTitle { get; set; }
        public String Title { get; set; }
        public Int32 Year { get; set; }
        public String BoxArtUrlSmall { get; set; }
        public String BoxArtUrlLarge { get; set; }
        #endregion

        #region Basic
        public Single AverageRating { get; set; }
        public Rating Rating { get; set; }
        public String Synopsis { get; set; }
        public Int32? RunTime { get; set; }
        #endregion

        #region Expanded
        public Single UserRating { get; set; }
        public People Actors { get; set; }
        public People Directors { get; set; } 
        public String OfficialWebsite { get; set; }
        public List<String> Genres { get; set; }
        public Format Format { get; set; }
        public ScreenFormat ScreenFormat { get; set; }
        #endregion

        #region Full
        public List<String> Awards { get; set; }
        public Movies SimilarTitles { get; set; } 
        public Movies RelatedTitles { get; set; }
        #endregion
    }

}

