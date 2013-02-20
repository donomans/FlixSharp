using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp.Holders.RottenTomatoes
{
    internal static class Constants
    {
        public const String BaseV1Url = "http://api.rottentomatoes.com/api/public/v1.0.json?apikey={0}";
        public const String SearchUrl = "http://api.rottentomatoes.com/api/public/v1.0/movies.json?apikey={0}&q={1}";
    }
}
