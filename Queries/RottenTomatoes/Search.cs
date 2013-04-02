using FlixSharp.Helpers.Async;
using FlixSharp.Helpers.RottenTomatoes;
using FlixSharp.Holders;
using FlixSharp.Holders.RottenTomatoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp.Queries.RottenTomatoes
{
    public class Search
    {
        public async Task<Titles> SearchTitles(String Title, Int32 Limit = 10)
        {
            Login.CheckInformationSet();
            var moviejson = AsyncHelpers.RottenTomatoesLoadXDocumentAsync(UrlBuilder.SearchUrl(Title, Limit));
           
            return new Titles(await Fill.GetBaseTitleInfo(moviejson));
        }
    }
}
