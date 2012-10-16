using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp.Holders
{
    public class NetflixMovies : IEnumerable<NetflixMovie>
    {
        List<NetflixMovie> _movies = new List<NetflixMovie>();

        public IEnumerator<NetflixMovie> GetEnumerator()
        {
            return _movies.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
