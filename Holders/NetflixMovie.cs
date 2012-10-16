using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp.Holders
{
    public class NetflixMovie
    {
        public TitleExpansion RecordType { get; set; }

        #region Basic
        public Int32 Id { get; set; }
        public Uri IdUrl { get; set; }
        public String ShortTitle { get; set; }
        public String Title { get; set; }
        public Int32 Year { get; set; }
        public Single AverageRating { get; set; }
        public String BoxArtUrlSmall { get; set; }
        public String BoxArtUrlLarge { get; set; }
        #endregion

        #region Expanded
        public NetflixMovies SimilarTitles { get; set; } ///not sure on this yet
        #endregion

        #region Full
        #endregion
    }

    [Flags]
    public enum TitleExpansion
    {
        Basic = 1,
        Expanded = 2,
        Full = 4
    }

}
