using Dalamud.Configuration;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Plugin;
using System;

namespace XIVWiki
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;
        public bool KeybindEnabled { get; internal set; } = true;
        public VirtualKey PrimaryKey { get; internal set; } = VirtualKey.P;
        public VirtualKey ModifierKey { get; internal set; } = VirtualKey.SHIFT;

        [NonSerialized]
        private DalamudPluginInterface? pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.pluginInterface!.SavePluginConfig(this);
        }
    }
}