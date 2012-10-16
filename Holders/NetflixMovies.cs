using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp.Holders
{
    public class Movies : IEnumerable<Movie>
    {
        List<Movie> _movies = new List<Movie>();

        public IEnumerator<Movie> GetEnumerator()
        {
            return _movies.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
