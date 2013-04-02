using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp.Holders
{
    public class Titles : IEnumerable<ITitle>
    {
        Dictionary<String, ITitle> _movies = new Dictionary<String, ITitle>();

        public Titles() { }
        public Titles(IEnumerable<ITitle> movies)
        {
            this.AddRange(movies);
        }

        public ITitle Find(String id)
        {
            if (_movies.ContainsKey(id))
                return _movies[id];
            else
                return null;
        }

        public void AddRange(IEnumerable<ITitle> movies)
        {
            foreach (ITitle m in movies)
            {
                String id = m.Id + 
                    ///gosh this is gross.
                    (m is Netflix.Title ? ((m as Netflix.Title).SeasonId != "" ? ";" + (m as Netflix.Title).SeasonId : "") : "");
                if (_movies.ContainsKey(id))
                    _movies[id] = m;//.AddParent(this);
                else
                    _movies.Add(id, m);//.AddParent(this));
            }
        }

        public IEnumerator<ITitle> GetEnumerator()
        {
            return _movies.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


    }
}
