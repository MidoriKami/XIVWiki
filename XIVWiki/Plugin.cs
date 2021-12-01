﻿using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using System.Reflection;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Party;

namespace XIVWiki
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "XIVWiki";

        private WikiCommand WikiCommand;

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface)
        {
            // Create Static Services for use everywhere
            pluginInterface.Create<Service>();
            Service.PluginInterface = pluginInterface;

            Service.Configuration = Service.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Service.Configuration.Initialize(Service.PluginInterface);

            this.WikiCommand = new WikiCommand();
        }

        public void Dispose()
        {
            WikiCommand.Dispose();
        }
    }
}