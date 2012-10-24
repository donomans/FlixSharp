using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp.Holders
{
    public class Movies : IEnumerable<Movie>
    {
        Dictionary<String, Movie> _movies = new Dictionary<String, Movie>();

        public Movie Find(String id)
        {
            return _movies[id];
        }

        public void AddRange(IEnumerable<Movie> movies)
        {
            foreach (Movie m in movies)
                if (_movies.ContainsKey(m.Id))
                    _movies[m.Id] = m.AddParent(this);
                else
                    _movies.Add(m.Id, m.AddParent(this));
        }

        public IEnumerator<Movie> GetEnumerator()
        {
            return _movies.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


    }
}
