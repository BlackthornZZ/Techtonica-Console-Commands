using EquinoxsModUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCommands
{
    internal static partial class Commands {
        internal static Command setPlayerParam = new Command() {
            name = "Set Player Param",
            description = "Sets various parameters for the player controller",
            examples = new List<string>() {
                "setplayerparam maxrunspeed 10.5",
                "SetPlayerParam MaxRunSpeed 10.5"
            },
            arguments = new List<Argument>() {
                new Argument() {
                    name = "Parameter",
                    description = "The parameter to set",
                    options = new List<string>() {
                        "maxrunspeed",
                        "maxwalkspeed",
                        "maxflyspeed",
                        "jumpspeed",
                        "scanspeed",
                        "gravity",
                        "maxflyheight",
                        "railrunnerspeed"
                    },
                    type = typeof(string),
                    optional = false
                },
                new Argument() {
                    name = "New Value",
                    description = "The new value to set the parameter to",
                    type = typeof(float),
                    optional = false
                }
            },
            Validate = () => {
                if (!setPlayerParam.ValidateNumProvidedArguments(2)) return false;
                if (!setPlayerParam.ValidateOptionsBasedArgument(0)) return false;
                if (!setPlayerParam.ValidateFloatArgument(1)) return false;

                return true;
            },
            Execute = () => {
                string parameter = setPlayerParam.argumentValues[0];
                float newValue = float.Parse(setPlayerParam.argumentValues[1]);

                switch(parameter) {
                    case "maxrunspeed": CommandSettings.maxRunSpeed = newValue; break;
                    case "maxwalkspeed": CommandSettings.maxWalkSpeed = newValue; break;
                    case "maxflyspeed": CommandSettings.maxFlySpeed = newValue; break;
                    case "jumpspeed": CommandSettings.jumpSpeed = newValue; break;
                    case "scanspeed": CommandSettings.scanSpeed = newValue; break;
                    case "gravity": CommandSettings.gravity = newValue; break;
                    case "maxflyheight": CommandSettings.maxFlyHeight = newValue; break;
                    case "railrunnerspeed": CommandSettings.railRunnerSpeed = newValue; break;
                }

                CommandSettings.Save();
                CommandSettings.Apply();
                CommandManager.Notify($"Set {parameter} to {newValue}");
            }
        };
    }
}
