using Dalamud.Game.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XIVWiki.SearchModules;

namespace XIVWiki
{
    internal class WikiCommand : IDisposable
    {
        private readonly string rootURL = "https://ffxiv.consolegameswiki.com";
        private readonly string _command = "/wiki";
        private List<iSearchable> databases = new();

        public WikiCommand()
        {
            RegisterCommand();

            InitializeDatabases();

            Service.Chat.Enable();
        }

        private void InitializeDatabases()
        {
            databases.Add(new WikiDungeons());
            databases.Add(new WikiTrials());
            databases.Add(new WikiRaids());
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

            if(result != null)
            {
                LaunchPage(result.Value.Value);
            }
            else
            {
                Service.Chat.Print($"[XIV Wiki][error] Term not Found: {args}");
            }
        }

        private KeyValuePair<string, string>? SearchDatabase(string searchTerm)
        {
            KeyValuePair<string, string>? result = null;

            foreach(var database in databases)
            {
                result = database.FindMatch(searchTerm);

                if(result != null)
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
