using System.Collections.Generic;

namespace XIVWiki.SearchModules
{
    public interface iSearchable
    {
        abstract KeyValuePair<string, string>? FindMatch(string searchTerm);
    }
}
