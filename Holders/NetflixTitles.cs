using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp.Holders
{
    public class Titles : IEnumerable<Title>
    {
        Dictionary<String, Title> _movies = new Dictionary<String, Title>();

        public Title Find(String id)
        {
            return _movies[id];
        }

        public void AddRange(IEnumerable<Title> movies)
        {
            foreach (Title m in movies)
                if (_movies.ContainsKey(m.Id))
                    _movies[m.Id] = m.AddParent(this);
                else
                    _movies.Add(m.Id, m.AddParent(this));
        }

        public IEnumerator<Title> GetEnumerator()
        {
            return _movies.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


    }
}
