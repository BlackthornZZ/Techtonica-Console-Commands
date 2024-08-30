using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ConsoleCommands
{
    internal static partial class Commands {
        internal static Command warp = new Command() {
            name = "Warp",
            description = "Teleports you to the provided coordinates / warp point",
            examples = new List<string>() {
                "warp 100 10 -250",
                "warp victor"
            },
            arguments = new List<Argument>() {
                new Argument() {
                    name = "x",
                    description = "X (east/west) coordinate of warp point",
                    type = typeof(float),
                    optional = true,
                },
                new Argument() {
                    name = "y",
                    description = "Y (up/down) coordinate of warp point",
                    type = typeof(float),
                    optional = true,
                },
                new Argument() {
                    name = "z",
                    description = "Z (north/south) coordinate of warp point",
                    type = typeof(float),
                    optional = true,
                },
                new Argument() {
                    name = "Warp Point",
                    description = "Name of saved warp point (converted to lower case)",
                    type = typeof(string),
                    optional = true,
                }
            },
            Validate = () => {
                if (!warp.ValidateNumProvidedArguments(new List<int>() { 1, 3 })) return false;

                if (warp.NumProvidedArguments() == 1) {
                    string warpPointName = warp.argumentValues[0];
                    return WarpManager.GetWarpPoint(warpPointName, out Vector3 point, out warp.validationError);
                }

                if (!warp.ValidateFloatArgument(0, -388, 478)) return false;
                if (!warp.ValidateFloatArgument(1, -117, 175)) return false;
                if (!warp.ValidateFloatArgument(2, -538, 328)) return false;

                return true;
            },
            Execute = () => {
                if (warp.NumProvidedArguments() == 1) {
                    string warpPointName = warp.argumentValues[0];
                    if (WarpManager.GetWarpPoint(warpPointName, out Vector3 point, out warp.validationError)) {
                        Player.instance.transform.position = point;
                    }
                }
                else {
                    float x = float.Parse(warp.argumentValues[0]);
                    float y = float.Parse(warp.argumentValues[1]);
                    float z = float.Parse(warp.argumentValues[2]);
                    Player.instance.transform.position = new Vector3(x, y, z);
                }
            }
        };
    }
}
