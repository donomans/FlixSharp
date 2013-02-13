using FlixSharp.Async;
using FlixSharp.Holders;
using FlixSharp.Holders.RottenTomatoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp.Queries
{
    public class RottenTomatoesSearch
    {
        public async Task<Titles> SearchTitles(String Title, Int32 Limit = 10)
        {
            var moviejson = AsyncHelpers.RottenTomatoesLoadXDocumentAsync("");

            Titles movies = new Titles();

            movies.AddRange(
                from movie
                in (await moviejson)["movies"].Children()
                select new Title()
                {
                    Id = movie["id"].ToString(),
                    FullTitle = movie["title"].ToString()
                });

            return movies;

        }
    }
}
