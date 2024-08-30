using EquinoxsModUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ConsoleCommands
{
    internal static partial class Commands {
        internal static Command setMoleDimensions = new Command() {
            name = "Set MOLE Dimensions",
            description = "Sets the size of the MOLE's digging area. Large sizes cause performance issues.",
            examples = new List<string>() {
                "setmoledimensions 10 10 10",
                "SetMoleDimensions 25 25 25"
            },
            arguments = new List<Argument>() {
                new Argument() {
                    name = "x",
                    description = "X (east/west) size of digging area",
                    type = typeof(uint),
                    optional = false
                },
                new Argument() {
                    name = "y",
                    description = "Y (up/down) size of digging area",
                    type = typeof(uint),
                    optional = false
                },
                new Argument() {
                    name = "z",
                    description = "Z (north/south) size of digging area",
                    type = typeof(uint),
                    optional = false
                }
            },
            Validate = () => {
                if (!setMoleDimensions.ValidateNumProvidedArguments(3)) return false;
                if (!setMoleDimensions.ValidateUIntArgument(0)) return false;
                if (!setMoleDimensions.ValidateUIntArgument(1)) return false;
                if (!setMoleDimensions.ValidateUIntArgument(2)) return false;

                return true;
            },
            Execute = () => {
                TerrainManipulator mole = Player.instance.equipment.GetEquipment<TerrainManipulator>();
                Vector3Int dimensions = new Vector3Int() {
                    x = int.Parse(setMoleDimensions.argumentValues[0]),
                    y = int.Parse(setMoleDimensions.argumentValues[1]),
                    z = int.Parse(setMoleDimensions.argumentValues[2])
                };
                
                mole.tunnelMode.SetCurrentDimensions(dimensions);
                mole.flattenMode.SetCurrentDimensions(dimensions);

                CommandManager.Notify($"Set MOLE size to {dimensions}");
            }
        };
    }
}
