using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp.Holders
{
    public class People : IEnumerable<Person>
    {
        List<Person> _people = new List<Person>();

        public IEnumerator<Person> GetEnumerator()
        {
            return _people.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
