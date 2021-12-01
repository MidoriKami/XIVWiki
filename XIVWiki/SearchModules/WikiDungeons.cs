using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;

namespace XIVWiki.SearchModules
{
    internal class WikiDungeons : iSearchable
    {
        private readonly RegexOptions regexOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;

        private Dictionary<string, string> lookuptable = new();

        public WikiDungeons()
        {
            InitializeDataTable();

            Service.Chat.Print("[XIV Wiki][dungeons] init complete");
        }

        public void InitializeDataTable()
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var filePath = Path.Combine(Path.GetDirectoryName(assemblyLocation)!, "dungeons.json");

            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                lookuptable = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
        }

        internal void PrintDatabase()
        {
            foreach(var pair in lookuptable)
            {
                Service.Chat.Print(pair.Key);
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
