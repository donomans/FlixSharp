using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp.Queries.RottenTomatoes
{
    public class Login
    {
        public static String ConsumerKey { get { return consumerkey; } }
        private static String consumerkey;

        public static Boolean InformationSet { get { return informationset; } }
        private static Boolean informationset = false;


        public Login SetCredentials(String ConsumerKey)
        {
            consumerkey = ConsumerKey;
            informationset = true;
            return this;
        }

        internal static void CheckInformationSet()
        {
            if (!informationset)
                throw new ArgumentException("Rotten Tomatoes API key has not been set.");
        }
    }
}
