using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XIVWiki.SearchModules
{
    internal class WikiTrials : iSearchable
    {
        private readonly RegexOptions regexOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;

        private Dictionary<string, string> lookuptable = new();

        public WikiTrials()
        {
            InitializeDataTable();

            Service.Chat.Print("[XIV Wiki][trials] init complete");
        }

        public void InitializeDataTable()
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var filePath = Path.Combine(Path.GetDirectoryName(assemblyLocation)!, "trials.json");

            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                lookuptable = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
        }

        public KeyValuePair<string, string>? FindMatch(string searchTerm)
        {
            foreach (var pair in lookuptable)
            {
                var strippedKey = Regex.Replace(pair.Key, "[^a-zA-Z0-9 ]", String.Empty);
                var strippedSearchTerm = Regex.Replace(searchTerm, "[^a-zA-Z0-9 ]", String.Empty);

                if (Regex.Match(strippedKey, strippedSearchTerm, regexOptions).Success)
                {
                    return pair;
                }
            }

            return null;
        }
    }
}
