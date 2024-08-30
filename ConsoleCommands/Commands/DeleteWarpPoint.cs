using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ConsoleCommands
{
    internal static partial class Commands {
        internal static Command deleteWarpPoint = new Command() {
            name = "Delete Warp Point",
            description = "Deletes the Warp Point with the provided name",
            examples = new List<string>() { 
                "deletewarppoint biobrickfactory",
                "DeleteWarpPoint BiobrickFactory" 
            },
            arguments = new List<Argument>() {
                new Argument() {
                    name = "Name",
                    description = "The name of the Warp Point to delete",
                    type = typeof(string),
                    optional = false
                }
            },
            Validate = () => {
                if (!saveWarpPoint.ValidateNumProvidedArguments(1)) return false;
                
                string name = saveWarpPoint.argumentValues[0];
                if (!WarpManager.GetWarpPoint(name, out Vector3 point, out string error)) {
                    saveWarpPoint.validationError = $"There is no saved Warp Point called '{name}'";
                    return false;
                }

                return true;
            },
            Execute = () => {
                string name = saveWarpPoint.argumentValues.First();
                if (saveWarpPoint.NumProvidedArguments() > 1) {
                    name = string.Join(" ", saveWarpPoint.argumentValues);
                }

                if(WarpManager.DeleteWarpPoint(name, out string error)) {
                    CommandManager.Notify($"Deleted Warp Point '{name}'");
                }
                else {
                    CommandManager.Notify(error);
                }
            }
        };
    }
}
