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
    internal class URLDatabase
    {
        private readonly RegexOptions regexOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;

        Dictionary<string, string> Lookuptable { get; set; } = new Dictionary<string, string>();

        public URLDatabase(string jsonfilename)
        {
            LoadJsonFromFile(jsonfilename);
        }

        private void LoadJsonFromFile(string jsonfilename)
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var filePath = Path.Combine(Path.GetDirectoryName(assemblyLocation)!, $@"data\{jsonfilename}");

            using (StreamReader r = new(filePath))
            {
                string json = r.ReadToEnd();
                Lookuptable = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            }
        }
        public void PrintDatabase()
        {
            foreach (var pair in Lookuptable)
            {
                Service.Chat.Print(pair.Key);
            }
        }

        public KeyValuePair<string, string>? FindMatch(string searchTerm)
        {
            foreach (var pair in Lookuptable)
            {
                var strippedKey = Regex.Replace(pair.Key, "[^a-zA-Z0-9 ]", string.Empty);
                var strippedSearchTerm = Regex.Replace(searchTerm, "[^a-zA-Z0-9 ]", string.Empty);

                if (Regex.Match(strippedKey, strippedSearchTerm, regexOptions).Success)
                {
                    return pair;
                }
            }

            return null;
        }
    }
}
