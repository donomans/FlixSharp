using FlixSharp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlixSharp.Holders
{
    public class Title : IResult
    {
        public Title()
        {
            Genres = new List<String>();
            ScreenFormats = new List<ScreenFormats>();
            Formats = new List<FormatAvailability>();
            Awards = new List<Award>();
            SimilarTitles = new List<Title>();
            BonusMaterials = new List<String>();
        }
        public Title(TitleExpansion Completeness)
            : this()
        {
            completeness = Completeness;
        }

        public ResultType Type { get { return ResultType.Movie; } }

        public TitleExpansion Completeness { get { return completeness;} }
        internal TitleExpansion completeness;

        public String Id
        {
            get
            {
                if (id != "")
                    return id;
                else
                {
                    var tup = GeneralHelpers.GetIdFromUrl(IdUrl);
                    id = tup.Id;
                    seasonid = tup.SeasonId;
                    return id;
                }
            }
        }
        private String id = "";
        public String SeasonId
        {
            get
            {
                if (seasonid != "")
                    return seasonid;
                else
                {
                    var tup = GeneralHelpers.GetIdFromUrl(IdUrl);
                    id = tup.Id;
                    seasonid = tup.SeasonId;
                    return seasonid;
                }
            }
        }
        private String seasonid = "";
        public String IdUrl { get; set; }

        public NetflixType NetflixType { get; set; }

        #region Minimal
            public String ShortTitle { get; set; }
            public String FullTitle { get; set; }
            public Int32 Year { get; set; }
            public String BoxArtUrlSmall { get; set; }
            public String BoxArtUrlLarge { get; set; }
            public List<String> Genres { get; set; }
            public Single AverageRating { get; set; }
            public Int32? RunTime { get; set; }
            public Rating Rating { get; set; }
            public String NetflixSiteUrl { get; set; }
            public String OfficialWebsite { get; set; }
            public Boolean HasAwards { get; set; }
            public Boolean HasBonusMaterials { get; set; }
            public Boolean HasDiscs { get; set; }
        #endregion

        
        #region Expanded
            public String Synopsis { get; set; }
            public People Actors { get; set; }
            public People Directors { get; set; } 
            public List<ScreenFormats> ScreenFormats { get; set; }
            public List<FormatAvailability> Formats { get; set; }
        #endregion

        #region Complete
            public Single UserRating { get; set; }
            public List<Award> Awards { get; set; }
            public List<Title> SimilarTitles { get; set; } 
            //public List<Title> RelatedTitles { get; set; }
            public List<String> BonusMaterials { get; set; }
            public List<String> Discs { get; set; }
        #endregion

        #region fill
        //private async Task FillOutMovie()
        //{
        //    if (Netflix.FillObjectsOnDemand)
        //    {///change this to the getters -- load the specific one and then await load all the other ones in the same level (Expanded or Complete)
        //        //switch (completeness)
        //        //{
        //        //    case TitleExpansion.Expanded:
        //        //        Title e = await Netflix.Fill.Titles.GetExpandedTitle(this.IdUrl, Netflix.OnUserBehalf);
        //        //        ///fill the title out now
        //        //        this.Synopsis = e.Synopsis;
        //        //        break;
        //        //    case TitleExpansion.Complete:
        //        //        Title c = null;
        //        //        if (completeness == TitleExpansion.Expanded)
        //        //            c = await Netflix.Fill.Titles.GetCompleteTitle(this.IdUrl, Netflix.OnUserBehalf);
        //        //        else
        //        //            c = await Netflix.Fill.Titles.GetCompleteTitleFromExpanded(this.IdUrl, Netflix.OnUserBehalf);
        //        //        break;
        //        //}
        //    }
        //}

        ///// <summary>
        ///// Used for lazy loading ?
        ///// -- need to think about this, as a single Movie can conceivably be part of
        ///// multiple Movies objects... is that a problem?
        ///// </summary>
        //private HashSet<Titles> _Parents { get; set; }
        //internal Title AddParent(Titles parent)
        //{
        //    if(!_Parents.Contains(parent))
        //        _Parents.Add(parent);
        //    return this;
        //}
        #endregion
    }

}

