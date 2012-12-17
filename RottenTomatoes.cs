using FlixSharp.Queries;
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

        public static RottenTomatoesSearch Search { get { return search; } }
        private static RottenTomatoesSearch search = new RottenTomatoesSearch();

        public static RottenTomatoesFill Fill { get { return fill; } }
        private static RottenTomatoesFill fill = new RottenTomatoesFill();

        public static RottenTomatoesLogin Login { get { return login; } }
        private static RottenTomatoesLogin login = new RottenTomatoesLogin();
    }
}
