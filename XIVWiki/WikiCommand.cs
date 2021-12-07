using Dalamud.Game.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using XIVWiki.SearchModules;
using Dalamud.Game.ClientState;
using Lumina.Excel.GeneratedSheets;
using System.Linq;

namespace XIVWiki
{
    internal class WikiCommand : IDisposable
    {
        private readonly string rootURL = "https://ffxiv.consolegameswiki.com";
        private readonly string _command = "/wiki";
        private readonly List<URLDatabase> databases = new();

        public WikiCommand()
        {
            RegisterCommand();

            InitializeDatabases();

            Service.Chat.Enable();
        }

        private void InitializeDatabases()
        {
            databases.Add(new URLDatabase("dungeons.json"));
            databases.Add(new URLDatabase("trials.json"));
            databases.Add(new URLDatabase("raids.json"));
            databases.Add(new URLDatabase("trialbosses.json"));
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
                    Service.Chat.Print("[XIV Wiki][alive] still alive.");
                    return;

                case "here":
                    var instanceName = GetCurrentInstanceName();
                    TrySearch(instanceName);
                    break;

                default:
                    TrySearch(args);
                    break;

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

        private void TrySearch(string args)
        {
            KeyValuePair<string, string>? result = SearchLocalDatabases(args);

            if (result != null)
            {
                LaunchPage(result.Value.Value);
            }
            else
            {
                Service.Chat.Print($"[XIV Wiki][error] No Matches Found: {args}");
                Service.Chat.Print($"[XIV Wiki][debug] Invoking Search Query");

                InvokeSearchQuery(args);
            }
        }

        private KeyValuePair<string, string>? SearchLocalDatabases(string searchTerm)
        {
            KeyValuePair<string, string>? result = null;

            foreach (var database in databases)
            {
                result = database.FindMatch(searchTerm);

                if (result != null)
                {
                    break;
                }
            }

            return result;
        }

        private void InvokeSearchQuery(string searchterm)
        {
            var urlBase = "/mediawiki/";
            var queryString = $"index.php?search={searchterm}&title=Special%3ASearch&go=Go";

            LaunchPage(urlBase + queryString);
        }

        private void LaunchPage(string page)
        {
            string fullURL = rootURL + page;

            var psi = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = fullURL
            };

            Process.Start(psi);
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
