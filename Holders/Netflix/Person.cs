﻿using FlixSharp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlixSharp.Holders.Netflix
{
    public class Person : IPerson
    {
        public Person()
        {
            Filmography = new List<Title>();
        }
        public Person(PersonExpansion Completeness)
            : this()
        {
            completeness = Completeness;
            //_Parents = new HashSet<People>();
        }

       // public ResultType Type { get { return ResultType.Person; } }

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
                    id = GeneralHelpers.GetIdFromUrl(IdUrl).Id;
                    return id;
                }
            }
            private set { }
        }
        private String id = "";
        public String IdUrl { get; set; }
        public String NetflixSiteUrl { get; set; }
        public String Name { get; set; }
        public String Bio { get; set; }
        #endregion

        #region Complete
        public String ImageUrl { get; set; }
        public List<Title> Filmography { get; set; }
        #endregion

        #region fill
        //private async Task FillOutPerson()
        //{
        //}

        ///// <summary>
        ///// Used for lazy loading ?
        ///// </summary>
        //private HashSet<People> _Parents { get; set; }
        //internal Person AddParent(People parent)
        //{
        //    if (!_Parents.Contains(parent))
        //        _Parents.Add(parent);
        //    return this;
        //}
        #endregion
    }
}
