using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ConsoleCommands
{
    internal static partial class Commands {
        internal static Command quit = new Command() {
            name = "Quit",
            description = "A fast way to quit the game",
            examples = new List<string>() {
                "quit",
                "quit true"
            },
            arguments = new List<Argument>() {
                new Argument() {
                    name = "Should Save",
                    description = "Whether the game should be saved before quitting.",
                    type = typeof(bool),
                    optional = true
                }
            },
            Validate = () => {
                if (quit.NumProvidedArguments() == 0) return true;
                
                if (!quit.ValidateNumProvidedArguments(1)) return false;
                if (!quit.ValidateBoolArgument(0)) return false;

                return true;
            },
            Execute = () => {
                if(quit.NumProvidedArguments() == 1 && bool.Parse(quit.argumentValues[0])) {
                    ConsoleCommandsPlugin.quitOnSaveFinished = true;
                    SaveState.QuickSave(null);
                    return;
                }

                Application.Quit();
            }
        };
    }
}
