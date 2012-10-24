using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlixSharp.Holders
{
    public class Movie : IResult
    {
        public Movie(TitleExpansion Completeness)
        {
            completeness = Completeness;
            _Parents = new HashSet<Movies>();
        }

        public ResultType Type { get { return ResultType.Movie; } }

        public TitleExpansion Completeness { get { return completeness;} }
        public TitleExpansion completeness;

        public String Id
        {
            get
            {
                if (id != "")
                    return id;
                else
                {
                    String[] splits = IdUrl.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    String newid = splits[splits.Length - 1];
                    Match m = Regex.Match(newid, "[0-9]{5,9}");
                    if (m.Success)
                    {
                        id = newid;
                        return id;
                    }
                    else
                        return id;
                }
            }
        }
        private String id = "";
        public String IdUrl { get; set; }

        #region Minimal
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

        #region Complete
            public List<String> Awards { get; set; }
            public List<Movie> SimilarTitles { get; set; } 
            public List<Movie> RelatedTitles { get; set; }
        #endregion

        #region fill
        private async Task FillOutMovie()
        {
            ///take the
            switch (completeness)
            {
                case TitleExpansion.Minimal:
                    break;
                case TitleExpansion.Basic:
                    break;
                case TitleExpansion.Expanded:
                    break;
                case TitleExpansion.Complete:
                    break;
            }
        }

        /// <summary>
        /// Used for lazy loading ?
        /// -- need to think about this, as a single Movie can conceivably be part of
        /// multiple Movies objects... is that a problem?
        /// </summary>
        private HashSet<Movies> _Parents { get; set; }
        internal Movie AddParent(Movies parent)
        {
            if(!_Parents.Contains(parent))
                _Parents.Add(parent);
            return this;
        }
        #endregion
    }

}

