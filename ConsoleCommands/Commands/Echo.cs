using FluffyUnderware.DevTools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCommands
{
    internal static partial class Commands {
        private static List<string> levels = new List<string>() {
            "info",
            "warning",
            "error",
            "fatal"
        };

        internal static Command echo = new Command() {
            name = "Echo",
            description = "Writes the given text to the console and logs it",
            examples = new List<string>() {
                "echo about to test a new command",
                "echo warning about to test a new command"
            },
            arguments = new List<Argument>() {
                new Argument() {
                    name = "Level",
                    description = $"Level of the message. One of: {string.Join(", ", levels)}",
                    type = typeof(string),
                    optional = true
                }
            },
            Validate = () => {
                return echo.ValidateNumProvidedArguments(1, int.MaxValue);
            },
            Execute = () => {
                string level = "info";
                if (levels.Contains(echo.argumentValues[0])) {
                    level = echo.argumentValues[0];
                    echo.argumentValues = echo.argumentValues.RemoveAt(0);
                }

                string message = string.Join(" ", echo.argumentValues);
                switch (level) {
                    case "info": ConsoleCommandsPlugin.Log.LogInfo(message); break;
                    case "warning": ConsoleCommandsPlugin.Log.LogWarning(message); break;
                    case "error": ConsoleCommandsPlugin.Log.LogError(message); break;
                    case "fatal": ConsoleCommandsPlugin.Log.LogFatal(message); break;
                }
            }
        };
    }
}
