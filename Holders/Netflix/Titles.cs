using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp.Holders.Netflix
{
    public class Titles : IEnumerable<Title>
    {
        Dictionary<String, Title> _movies = new Dictionary<String, Title>();

        public Title Find(String id)
        {
            if (_movies.ContainsKey(id))
                return _movies[id];
            else
                return null;
        }

        public void AddRange(IEnumerable<Title> movies)
        {
            foreach (Title m in movies)
            {
                String id = m.Id + (m.SeasonId != "" ? ";" + m.SeasonId : "");
                if (_movies.ContainsKey(id))
                    _movies[id] = m;//.AddParent(this);
                else
                    _movies.Add(id, m);//.AddParent(this));
            }
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
