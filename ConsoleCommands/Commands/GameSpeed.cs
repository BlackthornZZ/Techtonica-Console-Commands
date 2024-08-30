using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCommands
{
    internal static partial class Commands {
        internal static Command gameSpeed = new Command() {
            name = "Game Speed",
            description = "Sets a multiplier for the game's speed. Must be greater than 0.",
            examples = new List<string>() {
                "gamespeed 1.5",
                "GameSpeed 2"
            },
            arguments = new List<Argument>() {
                new Argument() {
                    name = "Multiplier",
                    description = "Multiplies the game's simulation speed",
                    type = typeof(float),
                    optional = false
                }
            },
            Validate = () => {
                if (!gameSpeed.ValidateNumProvidedArguments(1)) return false;
                if (!gameSpeed.ValidateFloatArgument(0, 0.001f, float.MaxValue)) return false;

                return true;
            },
            Execute = () => {
                float multiplier = float.Parse(gameSpeed.argumentValues[0]);
                Player.instance.cheats.simSpeed = multiplier;
                CommandManager.Notify($"Set gameSpeed to '{multiplier}'");
            }
        };
    }
}
