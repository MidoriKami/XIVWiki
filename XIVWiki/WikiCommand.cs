using Dalamud.Game.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVWiki
{
    internal class WikiCommand : IDisposable
    {
        private readonly string _command = "/wiki";

        public WikiCommand()
        {
            RegisterCommand();

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
            Service.Chat.Print("Command Received");
            Service.Chat.Print($"Command: {command}");
            Service.Chat.Print($"Args: {args}");

            var url = "https://ffxiv.consolegameswiki.com/wiki/" + args;

            Service.Chat.Print($"FullURL: {url}");

            var psi = new System.Diagnostics.ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = url
            };

            System.Diagnostics.Process.Start(psi);
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
