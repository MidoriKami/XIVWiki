using Dalamud.IoC;
using Dalamud.Plugin;
using System;

namespace XIVWiki
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "XIVWiki";

        private WikiCommand WikiCommand;
        private PopupSearchBox PopupSearchBox;

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface)
        {
            // Create Static Services for use everywhere
            pluginInterface.Create<Service>();
            Service.PluginInterface = pluginInterface;

            Service.Configuration = Service.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Service.Configuration.Initialize(Service.PluginInterface);

            this.PopupSearchBox = new PopupSearchBox();
            this.WikiCommand = new WikiCommand(PopupSearchBox);

            Service.WindowSystem.AddWindow(PopupSearchBox);

            Service.PluginInterface.UiBuilder.Draw += DrawUI;
        }

        private void DrawUI()
        {
            Service.WindowSystem.Draw();
        }

        public void Dispose()
        {
            Service.WindowSystem.RemoveAllWindows();
            WikiCommand.Dispose();
            PopupSearchBox.Dispose();
        }
    }
}
