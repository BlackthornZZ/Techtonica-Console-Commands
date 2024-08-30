using ArchieAndrews.PrefabBrush;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ConsoleCommands
{
    internal static partial class Commands {
        internal static Command saveWarpPoint = new Command() {
            name = "Save Warp Point",
            description = "Saves your current location as a Warp Point.",
            examples = new List<string>() { "SaveWarpPoint BiobrickFactory" },
            arguments = new List<Argument>() {
                new Argument() {
                    name = "Name",
                    description = "The name of your Warp Point",
                    type = typeof(string),
                    optional = false
                }
            },
            Validate = () => {
                if (!saveWarpPoint.ValidateNumProvidedArguments(1)) return false;

                string name = saveWarpPoint.argumentValues[0];
                if (name.Contains("|")) {
                    saveWarpPoint.validationError = "You cannot have a | character in the Warp Point's name";
                    return false;
                }

                if (name.Contains("/")) {
                    saveWarpPoint.validationError = "You cannot have a / character in the Warp Point's name";
                    return false;
                }

                if (name.Contains("\\")) {
                    saveWarpPoint.validationError = "You cannot have a \\ character in the Warp Point's name";
                    return false;
                }

                if (WarpManager.GetWarpPoint(name, out _, out _)) {
                    saveWarpPoint.validationError = $"A warp point named '{name}' already exists. Use 'DeleteWarpPoint {name}' to delete it.";
                    return false;
                }
                
                return true;
            },
            Execute = () => {
                string name = saveWarpPoint.argumentValues.First();
                if(saveWarpPoint.NumProvidedArguments() > 1) {
                    name = string.Join(" ", saveWarpPoint.argumentValues);
                }

                if (WarpManager.AddWarpPoint(name, Player.instance.transform.position, out string error)) {
                    CommandManager.Notify($"Saved warp point '{name}'");
                }
                else {
                    CommandManager.Notify(error);
                }
            }
        };
    }
}
