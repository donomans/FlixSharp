using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FlixSharp.OAuth;
using FlixSharp.Constants;
using FlixSharp.Holders;
using FlixSharp.Queries;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace FlixSharp
{
    public class Netflix
    {

        #region Get User Info method stuff
        /// <summary>
        /// Method to provide the current user's Netflix account Token, Token Secret, and User Id.
        /// Used for Protected Netflix API requests.  Can return null if there is no Netflix account linked
        /// for the currently logged in user for this web request.
        /// </summary>
        /// <returns></returns>
        public delegate Account GetCurrentUserNetflixUserInfo();
        private static GetCurrentUserNetflixUserInfo _GetUserInfo = null;

        internal static Account SafeReturnUserInfo()
        {
            if (Netflix._GetUserInfo != null)
            {
                try
                {
                    Account na;
                    na = Netflix._GetUserInfo();
                    return na;
                }
                catch (Exception ex) 
                {
                }
            }
            return null;
        }
        public static void SetMethodForGettingCurrentUserAccount(GetCurrentUserNetflixUserInfo GetUserInfo)
        {
            _GetUserInfo = GetUserInfo;
        }
        #endregion

        #region On Demand Loading stuf
        private static Boolean _FillObjectsOnDemand = true;
        internal static Boolean FillObjectsOnDemand { get { return _FillObjectsOnDemand; } }
        #endregion

        /// <summary>
        /// Instantiate a new Netflix client to send requests.
        /// [Note] Although GetUserInfo method is only required the first time,
        /// it may be provided in every instantiation (or can be changed for each 
        /// instantiation if so desired)
        /// </summary>
        /// <param name="FillObjectsOnDemand">
        /// FlixSharp Movie and Person objects will fill themselves upon access of fields
        /// that are outside the current scope of information
        /// Ex: Accessing Awards on a Movie object with a "Completeness" set at Minimal will result
        /// in Awards being loaded on access.
        /// </param>
        /// <param name="GetUserInfo">A static method that returns a Netflix Account 
        /// (presumably for the current logged in user to make Protected requests)</param>
        public Netflix(Boolean FillObjectsOnDemand = true, GetCurrentUserNetflixUserInfo GetUserInfo = null)
        {
            _FillObjectsOnDemand = FillObjectsOnDemand;
            
            NetflixLogin.CheckInformationSet();
            if (_GetUserInfo == null)
                _GetUserInfo = GetUserInfo;
        }

        public NetflixSearch Search { get { return search; } }
        private NetflixSearch search = new NetflixSearch();

        public NetflixFill Fill { get { return fill; } }
        private NetflixFill fill = new NetflixFill();

        public static NetflixLogin Login { get { return login; } }
        private static NetflixLogin login = new NetflixLogin();

    }
}
