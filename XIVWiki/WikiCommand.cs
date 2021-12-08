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
        private PopupSearchBox PopupSearchBox;
        public WikiCommand(PopupSearchBox popupSearchBox)
        {
            RegisterCommand();

            this.PopupSearchBox = popupSearchBox;
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
            // Load the relevent data tables.
            var territoryTypeTable = Service.DataManager.GetExcelSheet<TerritoryType>();

            // Get the territory we are currently in by RowID
            ushort currentTerritoryID = Service.ClientState.TerritoryType;

            // Get the entire row for the territory we are currently in
            TerritoryType currentTerritoryTypeRow = territoryTypeTable!.GetRow(currentTerritoryID)!;

            // Get the PlaceName RowID from TerritoryType
            uint placeNameRowID = currentTerritoryTypeRow!.PlaceName.Row;

            // Get the PlaceName Row
            PlaceName? placeNameRow = PlaceNameSheet!.GetRow(placeNameRowID);

            return placeNameRow!.Name;
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

        // Performs a lazy regex match, uses the first PlaceName that contains the search term in any part of the name.
        // ie: "prae" = "The Praetorium"
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
