using FlixSharp.Queries;
using FlixSharp.Queries.RottenTomatoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp
{
    public class RottenTomatoes
    {
        public RottenTomatoes()
        { }

        public static Search Search { get { return search; } }
        private static Search search = new Search();

        public static Fill Fill { get { return fill; } }
        private static Fill fill = new Fill();

        public static Login Login { get { return login; } }
        private static Login login = new Login();
    }
}
