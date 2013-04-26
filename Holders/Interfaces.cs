using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixSharp.Holders
{
    public interface ITitle
    {
        String Id { get; }
        String FullId { get; }
        String FullTitle { get; }
        Int32 Year { get; set; }
        TitleSource Source { get; }
    }

    public enum TitleSource
    {
        Netflix,
        RottenTomatoes,
        Imdb
    }

    public interface IPerson
    {
        String Id { get; }
        String Name { get; }
    }

}
