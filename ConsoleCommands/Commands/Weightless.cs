using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCommands
{
    internal static partial class Commands {
        internal static Command weightless = new Command() {
            name = "Weightless",
            description = "Toggles encumbrance",
            examples = new List<string>() { 
                "weightless",
                "Weightless"
            },
            Execute = () => {
                Player.instance.cheats.disableEncumbrance = !Player.instance.cheats.disableEncumbrance;
                CommandSettings.weightless = Player.instance.cheats.disableEncumbrance;
                CommandSettings.Save();

                string action = Player.instance.cheats.disableEncumbrance ? "Enabled" : "Disabled";
                CommandManager.Notify($"{action} weightlessness");
            }
        };
    }
}
