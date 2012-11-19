using FlixSharp.Holders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlixSharp.Helpers
{
    internal static class GeneralHelpers
    {
        //public static DateTime FromUnixTime(Int32 seconds)
        //{
        //    return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(seconds);
        //}
        public static DateTime FromUnixTime(Int64 seconds)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(seconds);
        }
        //public static DateTime FromUnixTime(this DateTime source, Int32 seconds)
        //{
        //    if (source == null)
        //        return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(seconds);
        //    else
        //        return source.AddSeconds(seconds);
        //}
        public static DateTime FromUnixTime(this DateTime source, Int64 seconds)
        {
            if (source == null)
                return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(seconds);
            else
                return source.AddSeconds(seconds);
        }



        public static NetflixId GetIdFromUrl(String NetflixIdUrl)
        {
            MatchCollection m = Regex.Matches(NetflixIdUrl, "[0-9]{3,10}");
            var r = m.Cast<Match>().Select(f=>f.Value).Take(2);
            return new NetflixId() { Id = r.First(), SeasonId = r.Count() > 1 ? r.LastOrDefault() : "" };
            //var r = m.Cast<Match>().Select(r => r.Value);
            //var r = from regex in m.Cast<Match>().AsQueryable() select regex.Value;
            //return Regex.Match(NetflixIdUrl, "[0-9]{4,10}").Value;
            ///Netflix IDs seem to be 5-9 characters, but there may be some 4 character or 10 character that I don't know about
        }
    }

    public static class SearchExtensions
    {
        public static IEnumerable<IResult> MergeResults(this IEnumerable<IResult> source, IEnumerable<IResult> tomerge)
        {
            List<IResult> ret = new List<IResult>();
            if (source != null)
                ret.AddRange(source.ToList());
            if (tomerge != null)
                ret.AddRange(tomerge);
            return ret;
        }
        public static IEnumerable<IResult> ShuffleResults(this IEnumerable<IResult> source)
        {
            List<IResult> list = new List<IResult>(source);
            Random rng = new Random();
            Int32 n = list.Count;
            while (n > 1)
            {
                n--;
                Int32 k = rng.Next(n + 1);
                IResult value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        public static IEnumerable<IResult> MergeShuffle(this IEnumerable<IResult> source, IEnumerable<IResult> tomerge)
        {
            List<IResult> s = source.ToList();
            Int32 scount = 0;

            List<IResult> t = tomerge.ToList();
            Int32 tcount = 0;

            List<IResult> ret = new List<IResult>(t.Count + s.Count);

            Random r = new Random();

            while ((scount + tcount) < (s.Count + t.Count))
            {
                if (r.Next(1, 10000) < 5000)
                {
                    if (scount < s.Count)
                        ret.Add(s[scount++]);
                    else
                    {
                        if (tcount < t.Count)
                            ret.Add(t[tcount++]);
                    }
                }
                else
                {
                    if (tcount < t.Count)
                        ret.Add(t[tcount++]);
                    else
                    {
                        if (scount < s.Count)
                            ret.Add(s[scount++]);
                    }
                }
            }

            return ret;
        }
    }
}
