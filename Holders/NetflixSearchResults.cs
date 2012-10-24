using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp.Holders
{
    public class SearchResults : IEnumerable<IResult>
    {
        public Movies MovieResults;
        public People PeopleResults;

        public String SearchTerm = "";
        
        public IEnumerator<IResult> GetEnumerator()
        {
            List<IResult> results = new List<IResult>();
            if (MovieResults != null)
                results.AddRange(MovieResults.ToArray());
            if (PeopleResults != null)
                results.AddRange(PeopleResults.ToArray());
            return results.GetEnumerator();            
        }

        public IEnumerable<IResult> ShuffledEnumerator()
        {
            return MovieResults.MergeShuffle(PeopleResults);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public static class SearchExtensions
    {
        public static IEnumerable<IResult> MergeResults(this IEnumerable<IResult> source, IEnumerable<IResult> tomerge)
        {
            List<IResult> ret = new List<IResult>();
            if(source != null)
                ret.AddRange(source.ToList());
            if(tomerge != null)
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

            while((scount + tcount) < (s.Count + t.Count))
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

    public interface IResult
    {
        String Id { get; }

        ResultType Type { get; }
    }

    public enum ResultType
    {
        Movie,
        Person
    }
}
