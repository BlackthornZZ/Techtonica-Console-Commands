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
                if (!deleteWarpPoint.ValidateNumProvidedArguments(1)) return false;

                // Avoiding ValidateOptionsBasedArgument() for the more detailed error message
                string name = deleteWarpPoint.argumentValues[0];
                if (!WarpManager.GetWarpPoint(name, out Vector3 point, out string error)) {
                    deleteWarpPoint.validationError = $"There is no saved Warp Point called '{name}'";
                    return false;
                }

                return true;
            },
            Execute = () => {
                string name = deleteWarpPoint.argumentValues.First();
                if (deleteWarpPoint.NumProvidedArguments() > 1) {
                    name = string.Join(" ", deleteWarpPoint.argumentValues);
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
