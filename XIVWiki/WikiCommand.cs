using Dalamud.Game.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using XIVWiki.SearchModules;

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
            if (args == "alive")
            {
                Service.Chat.Print("[XIV Wiki][alive] still alive.");
                return;
            }

            KeyValuePair<string, string>? result = SearchDatabase(args);

            if (result != null)
            {
                LaunchPage(result.Value.Value);
            }
            else
            {
                Service.Chat.Print($"[XIV Wiki][error] No Matches Found: {args}");
            }
        }

        private KeyValuePair<string, string>? SearchDatabase(string searchTerm)
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
