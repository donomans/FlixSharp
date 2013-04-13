using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp.Holders
{
    public class People : IEnumerable<IPerson>
    {
        Dictionary<String, IPerson> _people = new Dictionary<String, IPerson>();

        public void AddRange(IEnumerable<IPerson> people)
        {
            foreach (IPerson p in people)
                if (_people.ContainsKey(p.Id))
                    _people[p.Id] = p;//.AddParent(this);
                else
                    _people.Add(p.Id, p);//.AddParent(this));
        }
        public void Add(IPerson person)
        {
            if (_people.ContainsKey(person.Id))
                _people[person.Id] = person;
            else
                _people.Add(person.Id, person);
        }

        public IPerson Find(String id)
        {
            if (_people.ContainsKey(id))
                return _people[id];
            else
                return null;
        }

        public IEnumerator<IPerson> GetEnumerator()
        {
            return _people.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
