using Dalamud.Game.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using XIVWiki.SearchModules;
using Dalamud.Game.ClientState;
using Lumina.Excel.GeneratedSheets;
using System.Linq;
using System.Text.RegularExpressions;
using Lumina.Excel;

namespace XIVWiki
{
    internal class WikiCommand : IDisposable
    {
        private readonly RegexOptions regexOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;
        private readonly string rootURL = "https://ffxiv.consolegameswiki.com";
        private readonly string _command = "/wiki";

        private HashSet<string> InstanceNames = new();
        private HashSet<string> InstanceNamesFallback = new();
        private List<HashSet<string>> Databases = new();

        private PopupSearchBox PopupSearchBox;


        public WikiCommand(PopupSearchBox popupSearchBox)
        {
            RegisterCommand();

            this.PopupSearchBox = popupSearchBox;

            InitializeDataSheets();

            Service.Chat.Enable();
        }

        private void InitializeDataSheets()
        {
            var ContentFinderCondition = Service.DataManager.GetExcelSheet<ContentFinderCondition>()!;

            // 2 - Dungeon 4 - Tiral 5 - Raid (Includes Alliance Raids) 28 - Ultimate Raids
            InstanceNames = ContentFinderCondition
                // Get rows that are instances
                .Where(r => r.ContentType.Value!.RowId is 2 or 4 or 5 or 28)
                // Reference TerritoryTypes
                .Select(r => r.TerritoryType.Value)
                // Reference PlaceName
                .Select(r => r!.PlaceName.Value!.Name.ToString())
                .ToHashSet();

            Databases.Add(InstanceNames);


            // Generate Fallback List, some of these have malformed names
            InstanceNamesFallback = ContentFinderCondition
                .Select(r => r.Name.ToString())
                .ToHashSet();

            Databases.Add(InstanceNamesFallback);
            
        }

        private void RegisterCommand()
        {
            Service.Commands.AddHandler(_command, new CommandInfo(CommandCallback)
            {
                HelpMessage = "Search the gamewiki"
            });
        }

        private void CommandCallback(string command, string args)
        {
            switch (args)
            {
                case "alive":
                    Service.Chat.Print("[XIV Wiki] Still Alive");
                    return;

                case "here":
                    var instanceName = GetCurrentInstanceName();
                    LaunchPage( GetURLWithSearchQuery(instanceName) );
                    break;

                case "":
                    PopupSearchBox.Active = true;
                    break;

                default:
                    FindMatchAndLaunchPage(args);
                    break;
            }
        }


        // Attempts to match the user typed args to an entry found in the data tables we use
        // If a match is found, we will search for the full exact name of the entry
        // If a match is not found, we will search for whatever the user typed in
        private void FindMatchAndLaunchPage(string args)
        {
            var properSearchString = FindMatch(args);

            if (properSearchString == null)
            {
                // Launches a search query with whatever terms the user input
                LaunchPage(GetURLWithSearchQuery(args));
            }
            else
            {
                // Should always launch a page that will redirect directly to a valid FFXIV thing
                LaunchPage(GetURLWithSearchQuery(properSearchString));
            }
        }

        private string GetCurrentInstanceName()
        {
            var territoryTypeTable = Service.DataManager.GetExcelSheet<TerritoryType>();
            ushort currentTerritoryID = Service.ClientState.TerritoryType;

            return territoryTypeTable
                !.GetRow(currentTerritoryID)
                !.PlaceName.Value!.Name;
        }

        private string GetURLWithSearchQuery(string searchTerm)
        {
            var queryString = $"/mediawiki/index.php?search={searchTerm}&title=Special%3ASearch&go=Go";

            return rootURL + queryString;
        }

        private void LaunchPage(string page)
        {
            var psi = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = page
            };

            Process.Start(psi);
        }

        // Performs a lazy regex match
        // ie: "prae" = "The Praetorium"
        public string? FindMatch(string searchTerm)
        {
            foreach(var database in Databases)
            {
                var result = SearchHastsetForTerm(searchTerm, database);
                if (result != null)
                {
                    Service.Chat.Print($"[XIVWiki][Search Result] Match Found: {result}");
                    return result;
                }
            }

            Service.Chat.Print($"[XIVWiki][Search Result] No Matches Found for: {searchTerm}");
            return null;
        }

        private string? SearchHastsetForTerm(string searchTerm, HashSet<string> InstanceNames)
        {
            foreach(var instanceName in InstanceNames)
            { 
                if(Regex.Match(instanceName, searchTerm, regexOptions).Success)
                {
                    return instanceName;
                }
            }

            return null;
        }

        private void UnregisterCommand()
        {
            Service.Commands.RemoveHandler(_command);
        }

        public void Dispose()
        {
            UnregisterCommand();
        }
    }
}
