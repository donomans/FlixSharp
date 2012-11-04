using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp.Holders
{
    public class People : IEnumerable<Person>
    {
        Dictionary<String, Person> _people = new Dictionary<String, Person>();

        public void AddRange(IEnumerable<Person> people)
        {
            foreach (Person p in people)
                if (_people.ContainsKey(p.Id))
                    _people[p.Id] = p;//.AddParent(this);
                else
                    _people.Add(p.Id, p);//.AddParent(this));
        }

        public Person Find(String id)
        {
            return _people[id];
        }

        public IEnumerator<Person> GetEnumerator()
        {
            return _people.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
