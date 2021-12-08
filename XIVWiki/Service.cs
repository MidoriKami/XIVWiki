using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Game.ClientState;
using Dalamud.Data;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Windowing;

namespace XIVWiki
{
    public class Service
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [PluginService] public static DalamudPluginInterface PluginInterface { get; set; }
        [PluginService] public static ChatGui Chat { get; private set; }
        [PluginService] public static CommandManager Commands { get; private set; }
        [PluginService] public static ClientState ClientState { get; private set; }
        [PluginService] public static DataManager DataManager { get; private set; }
        [PluginService] public static KeyState KeyState { get; private set; }
        public static WindowSystem WindowSystem { get; private set; } = new WindowSystem("XIVWiki");
        public static Configuration Configuration { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}