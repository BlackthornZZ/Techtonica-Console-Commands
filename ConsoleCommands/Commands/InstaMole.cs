using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCommands
{
    internal static partial class Commands {
        internal static Command instamole = new Command() {
            name = "InstaMole",
            description = "Toggles instant digging for the M.O.L.E",
            examples = new List<string>() {
                "instamole",
                "InstaMole"
            },
            Execute = () => {
                CommandSettings.instamole = !CommandSettings.instamole;
                CommandSettings.Save();

                string action = CommandSettings.instamole ? "Enabled" : "Disabled";
                CommandManager.Notify($"{action} instamole");
            }
        };
    }
}
