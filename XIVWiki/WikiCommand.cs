using Dalamud.Game.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using XIVWiki.SearchModules;
using Dalamud.Game.ClientState;
using Lumina.Excel.GeneratedSheets;
using System.Linq;
using System.Text.RegularExpressions;

namespace XIVWiki
{
    internal class WikiCommand : IDisposable
    {
        private readonly RegexOptions regexOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;
        private readonly string rootURL = "https://ffxiv.consolegameswiki.com";
        private readonly string _command = "/wiki";

        private readonly Lumina.Excel.ExcelSheet<PlaceName> PlaceNameSheet;

        public WikiCommand()
        {
            RegisterCommand();

            PlaceNameSheet = Service.DataManager.GetExcelSheet<PlaceName>()!;

            Service.Chat.Enable();
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

                default:
                    FindMatchAndLaunchPage(args);
                    break;
            }
        }

        private void FindMatchAndLaunchPage(string args)
        {
            var properSearchString = FindMatch(args);

            if (properSearchString == null)
            {
                LaunchPage(GetURLWithSearchQuery(args));
            }
            else
            {
                LaunchPage(GetURLWithSearchQuery(properSearchString));
            }
        }

        private string GetCurrentInstanceName()
        {
            var territoryTypeTable = Service.DataManager.GetExcelSheet<TerritoryType>();
            var placeNameTable = Service.DataManager.GetExcelSheet<PlaceName>();

            var currentTerritoryID = Service.ClientState.TerritoryType;

            var currentTerritoryPlaceNameID = territoryTypeTable!.GetRow(currentTerritoryID)!.PlaceName.Row;
            var currentTerritoryName = placeNameTable!.GetRow(currentTerritoryPlaceNameID)!.Name;

            return currentTerritoryName;
        }

        private string GetURLWithSearchQuery(string searchTerm)
        {
            var urlBase = "/mediawiki/";
            var queryString = $"index.php?search={searchTerm}&title=Special%3ASearch&go=Go";

            return rootURL + urlBase + queryString;
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

        private string? SearchPlaceName(string searchTerm)
        {
            var results = PlaceNameSheet.Where(r => Regex.Match(r.Name, searchTerm, regexOptions).Success);

            if( results.Any() )
            {
                return results.First().Name.ToString();
            }

            return null;
        }

        public string? FindMatch(string searchTerm)
        {
            // Search all known instance names for a match
            var searchResult = SearchPlaceName(searchTerm);
            if(searchResult != null) return searchResult;

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
