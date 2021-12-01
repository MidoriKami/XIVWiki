using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVWiki.SearchModules
{
    public interface iSearchable
    {
        abstract KeyValuePair<string, string>? FindMatch(string searchTerm);
    }
}
