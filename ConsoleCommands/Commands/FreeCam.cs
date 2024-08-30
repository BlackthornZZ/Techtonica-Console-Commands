using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCommands
{
    internal static partial class Commands {
        internal static Command freeCam = new Command() {
            name = "Free Cam",
            description = "Toggles free camera. You probably want to use noclip instead of this.",
            examples = new List<string>() {
                "freecam",
                "FreeCam true"
            },
            arguments = new List<Argument>() {
                new Argument() {
                    name = "Teleport",
                    description = "Whether to teleport the player to the camera position when disabling",
                    type = typeof(bool),
                    optional = true
                }
            },
            Validate = () => {
                if (GlobalData.noClipEnabled) {
                    freeCam.validationError = $"Can't execute while noclip is enabled";
                    return false;
                }

                if (!freeCam.ValidateNumProvidedArguments(0, 1)) return false;
                if (!freeCam.ValidateBoolArgument(0)) return false;

                return true;
            },
            Execute = () => {
                bool teleport = false;
                if(freeCam.NumProvidedArguments() == 1) {
                    teleport = bool.Parse(freeCam.argumentValues[0]);
                }

                MethodInfo toggleFreeCamInfo = typeof(PlayerCheats).GetMethod("ToggleFreeCameraMode", BindingFlags.NonPublic | BindingFlags.Instance);
                toggleFreeCamInfo.Invoke(Player.instance.cheats, new object[] { teleport });

                string action = Player.instance.cheats.freeCameraMode == PlayerCheats.FreeCameraMode.Free ? "Enabled" : "Disabled";
                CommandManager.Notify($"{action} FreeCam");
            }
        };
    }   
}
