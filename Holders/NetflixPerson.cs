using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlixSharp.Holders
{
    public class Person : IResult
    {
        public Person(PersonExpansion Completeness)
        {
            completeness = Completeness;
            _Parents = new HashSet<People>();
        }

        public ResultType Type { get { return ResultType.Person; } }

        public PersonExpansion Completeness { get { return completeness;} }
        public PersonExpansion completeness = PersonExpansion.Minimal;

        #region Basic
        public String Id
        {
            get
            {
                if (id != "")
                    return id;
                else
                {
                    String[] splits = IdUrl.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    String newid = splits[splits.Length - 1];
                    Match m = Regex.Match(newid, "[0-9]{4,10}");
                    if (m.Success)
                    {
                        id = newid;
                        return id;
                    }
                    else
                        return id;
                }
            }
        }
        private String id = "";
        public String IdUrl { get; set; }
        public String Name { get; set; }
        public String Bio { get; set; }
        #endregion

        #region Complete
        public String ImageUrl { get; set; }
        public List<Title> Filmography { get; set; }
        #endregion

        #region fill
        private async Task FillOutPerson()
        {
            ///take the
        }

        /// <summary>
        /// Used for lazy loading ?
        /// </summary>
        private HashSet<People> _Parents { get; set; }
        internal Person AddParent(People parent)
        {
            if (!_Parents.Contains(parent))
                _Parents.Add(parent);
            return this;
        }
        #endregion
    }
}
