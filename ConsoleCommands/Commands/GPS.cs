using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCommands
{
    internal static partial class Commands {
        internal static Command gps = new Command() {
            name = "gps",
            description = "Triggers a notification with your current position",
            examples = new List<string>() { "gps" },
            Execute = () => {
                CommandManager.Notify(Player.instance.transform.position.ToString());
            }
        };
    }
}
