using Dalamud.Interface.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Keys;
using System.Numerics;
using ImGuiNET;

namespace XIVWiki
{
    internal class PopupSearchBox : Window, IDisposable
    {
        public bool Active { get; set; }

        private readonly Vector2 WindowSize = new(500, 25);

        private readonly ImGuiWindowFlags defaultWindowFlags =
                    ImGuiWindowFlags.NoScrollbar |
                    ImGuiWindowFlags.NoScrollWithMouse |
                    ImGuiWindowFlags.NoTitleBar |
                    ImGuiWindowFlags.NoBackground |
                    ImGuiWindowFlags.NoCollapse |
                    ImGuiWindowFlags.NoResize;

        public PopupSearchBox() : base("XIVWiki Popup Search Box")
        {
            SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new(WindowSize.X, WindowSize.Y),
                MaximumSize = new(WindowSize.X, WindowSize.Y)
            };

            this.IsOpen = true;
        }

        public override void PreDraw()
        {
            base.PreDraw();

            Flags = defaultWindowFlags;
        }

        public override void Draw()
        {
            if (IsKeyBindPressed())
            {
                this.Active = true;
            }

            if ( Active )
            {
                if(Service.KeyState[VirtualKey.ESCAPE] == true)
                {
                    this.Active = false;
                }

                ImGui.Text("Combo Pressed");
            }
        }

        public bool IsKeyBindPressed()
        {
            if (!Service.Configuration.KeybindEnabled) 
                return false;

            var primaryKey = Service.Configuration.PrimaryKey;
            var modifierKey = Service.Configuration.ModifierKey;

            if (modifierKey == VirtualKey.NO_KEY)
            {
                return Service.KeyState[primaryKey];
            }

            return Service.KeyState[modifierKey] && Service.KeyState[primaryKey];
        }

        public void Dispose()
        {
        }
    }
}
