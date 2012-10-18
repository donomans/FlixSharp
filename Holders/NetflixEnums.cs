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
        Basic = 2,
        Expanded = 4,
        Complete = 8
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

    public enum Format
    {
        Streaming,
        Bluray,
        DVD
    }

    public enum ScreenFormat
    {
        Widescreen,
        Standard
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

}
