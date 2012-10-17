using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public Int32 Id { get; set; }
        public Uri IdUrl { get; set; }
        public String ShortTitle { get; set; }
        public String Title { get; set; }
        public Int32 Year { get; set; }
        public String BoxArtUrlSmall { get; set; }
        public String BoxArtUrlLarge { get; set; }
        public Single AverageRating { get; set; }
        #endregion

        #region Basic
        public MpaaRating MpaaRating { get; set; }
        public String Synopsis { get; set; }
        public Int32 Length { get; set; }
        public Single Stars { get; set; }
        public String OfficialWebsite { get; set; }
        #endregion

        #region Expanded
        public People Actors { get; set; }
        public People Directors { get; set; }
        #endregion

        #region Full
        public List<String> Awards { get; set; }
        public Movies SimilarTitles { get; set; } 
        public Movies RelatedTitles { get; set; }
        #endregion
    }

    /// <summary>
    /// Minimal, Basic, Expanded, and Complete in order of information included, from least to most.
    /// </summary>
    public enum TitleExpansion
    {
        Minimal = 1,
        Basic = 2,
        Expanded = 4,
        Complete = 8
    }

    public enum MpaaRating
    {
        Unrated,
        R,
        PG13,
        PG,
        G
    }

}
