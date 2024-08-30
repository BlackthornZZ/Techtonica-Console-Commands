using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCommands
{
    internal static partial class GlobalData {
        internal static bool noClipEnabled;
    }

    internal static partial class Commands {
        internal static Command noClip = new Command() {
            name = "No Clip",
            description = "Toggles clipping for the player",
            examples = new List<string>() {
                "noclip",
                "NoClip"
            },
            Execute = () => {
                GlobalData.noClipEnabled = !GlobalData.noClipEnabled;
                freeCam.Execute();

                string action = GlobalData.noClipEnabled ? "Enabled" : "Disabled";
                CommandManager.Notify($"{action} noclip");
            }
        };
    }
}
