using EquinoxsModUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCommands
{
    internal static partial class Commands {
        internal static Command setExplosiveSize = new Command() {
            name = "Set Explosive Size",
            description = "Set the diameter and depth of mining charges",
            examples = new List<string>() {
                "setexplosivesize 5 15",
                "SetExplosiveSize 11 20"
            },
            arguments = new List<Argument>() {
                new Argument() {
                    name = "Diameter",
                    description = "Width and height of the explosion area",
                    type = typeof(int),
                    optional = false
                },
                new Argument() {
                    name = "Depth",
                    description = "Depth of the explosion area",
                    type = typeof(int),
                    optional = true
                }
            },
            Validate = () => {
                if (!setExplosiveSize.ValidateNumProvidedArguments(2)) return false;
                if (!setExplosiveSize.ValidateIntArgument(0, 1, int.MaxValue)) return false;
                if (!setExplosiveSize.ValidateIntArgument(1, 1, int.MaxValue)) return false;
                return true;
            },
            Execute = () => {
                int diameter = int.Parse(setExplosiveSize.argumentValues[0]);
                int depth = int.Parse(setExplosiveSize.argumentValues[1]);
                
                ExplosiveDefinition explosive = (ExplosiveDefinition)ModUtils.GetResourceInfoByName(ResourceNames.MiningCharge);
                explosive.explosionRadius = diameter;
                explosive.explosionDepth = depth;

                CommandManager.Notify($"Set explosive size to {diameter}x{diameter}x{depth}");
            }
        };
    }
}
