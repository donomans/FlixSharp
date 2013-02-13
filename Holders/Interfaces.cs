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
        String FullTitle { get; }
    }

    public interface IPerson
    {
        String Id { get; }
        String Name { get; }
    }

}
