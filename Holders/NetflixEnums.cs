using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FlixSharp.Holders
{
    /// <summary>
    /// Minimal, Basic, Expanded, and Complete in order of information included, from least to most.
    /// </summary>
    public enum TitleExpansion
    {
        Minimal = 1,
        Expanded = 2,
        Complete = 4
    }

    public enum PersonExpansion
    {
        Minimal = 1,
        //Expanded = 2,
        Complete = 2
    }

    public struct Rating
    {
        public Rating(String rating)
        {
            MpaaRating = Holders.MpaaRating.None;
            TvRating = Holders.TvRating.None;
        }

        internal Rating(IEnumerable<XElement> rating)
        {
            var r = (from ratingattrib
                        in rating
                    select new
                    {
                        Rating = (String)ratingattrib.Attribute("label") ?? "None",
                        Type = (String)ratingattrib.Attribute("scheme")
                    }).FirstOrDefault();

            MpaaRating = Holders.MpaaRating.None;
            TvRating = Holders.TvRating.None;

            if (r == null || r.Rating == "None")
            {
                ///this is bads
            }
            else
            {
                if (r.Type.Contains("mpaa"))
                {
                    switch (r.Rating)
                    {
                        case "R":
                            MpaaRating = Holders.MpaaRating.R;
                            break;
                        case "PG-13":
                            MpaaRating = Holders.MpaaRating.PG13;
                            break;
                        case "PG":
                            MpaaRating = Holders.MpaaRating.PG;
                            break;
                        case "NR":
                        case "UR":
                            MpaaRating = Holders.MpaaRating.Unrated;
                            break;
                        case "G":
                            MpaaRating = Holders.MpaaRating.G;
                            break;
                        default:
                            MpaaRating = Holders.MpaaRating.None;
                            break;
                    }
                }
                else
                {
                    switch (r.Rating)
                    {
                        case "PG":
                            TvRating = Holders.TvRating.PG;
                            break;
                        case "TV-14":
                            TvRating = Holders.TvRating.TV14;
                            break;
                        default:
                            TvRating = Holders.TvRating.None;
                            break;
                    }

                }
            }

        }
        public RatingType RatingType { get { return TvRating == Holders.TvRating.None ? RatingType.Mpaa : RatingType.TV; } }// = RatingType.Mpaa;
        public MpaaRating MpaaRating;
        public TvRating TvRating;
    }

    public enum RatingType
    {
        Mpaa,
        TV
    }

    [Flags]
    public enum Format
    {
        Streaming = 1,
        Bluray = 2,
        DVD = 4
    }

    public struct FormatAvailability
    {
        public DateTime? AvailableFrom;
        public DateTime? AvailableUntil;
        public String Format;
    }

    public struct ScreenFormats
    {
        public String Format;
        public String ScreenFormat;
    }
    //public enum ScreenFormat
    //{
    //    Widescreen16by9,
    //    Widescreen24by10,
    //    Standard
    //}

    public enum NetflixType
    {
        Movie,
        Series,
        SeriesSeason,
        Programs,
        People
    }

    public enum MpaaRating
    {
        Unrated,
        R,
        PG13,
        PG,
        G,
        None
    }
    public enum TvRating
    {
        TV14,
        PG,
        None
    }


    public struct Award
    {
        public Int32? Year;
        public AwardType Type;
        public Boolean Winner;
        public String PersonIdUrl;
        public String AwardName;
    }

    public enum AwardType
    {
        AFI,
        Baftas,
        AcademyAwards,
        Razzie,
        Time,
        GoldenGlobeAwards
    }
}
