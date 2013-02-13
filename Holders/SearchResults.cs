using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlixSharp.Helpers;

namespace FlixSharp.Holders
{
    public class SearchResults //: IEnumerable<IResult>
    {
        public Titles MovieResults;
        public People PeopleResults;

        public String SearchTerm = "";
        
        //public IEnumerator<IResult> GetEnumerator()
        //{
        //    List<IResult> results = new List<IResult>();
        //    if (MovieResults != null)
        //        results.AddRange(MovieResults.ToArray());
        //    if (PeopleResults != null)
        //        results.AddRange(PeopleResults.ToArray());
        //    return results.GetEnumerator();            
        //}

        //public IEnumerable<IResult> ShuffledEnumerator()
        //{
        //    return MovieResults.MergeShuffle(PeopleResults);
        //}

        //System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        //{
        //    return GetEnumerator();
        //}
    }


    //public interface IResult
    //{
    //    String Id { get; }

    //    ResultType Type { get; }
    //}

    //public enum ResultType
    //{
    //    Movie,
    //    Person
    //}
}
