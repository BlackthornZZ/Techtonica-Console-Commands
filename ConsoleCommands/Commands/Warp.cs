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
            name = "warp",
            description = "Teleports you to the provided coordinates / warp point",
            examples = new List<string>() {
                "warp 100 10 -250",
                "warp victor"
            },
            arguments = new List<Argument>() {
                new Argument() {
                    name = "x",
                    type = typeof(float),
                    description = "X (east/west) coordinate of warp point",
                    optional = true,
                },
                new Argument() {
                    name = "y",
                    type = typeof(float),
                    description = "Y (up/down) coordinate of warp point",
                    optional = true,
                },
                new Argument() {
                    name = "z",
                    type = typeof(float),
                    description = "Z (north/south) coordinate of warp point",
                    optional = true,
                },
                new Argument() {
                    name = "warp point",
                    type = typeof(string),
                    description = "Name of saved warp point"
                }
            },
            Validate = () => {
                if (warp.argumentValues.Count() != 1 && warp.argumentValues.Count() != 3) {
                    warp.validationError = $"Invalid number of arguments: {warp.NumProvidedArguments()}";
                    return false;
                }

                if (warp.NumProvidedArguments() == 1) {
                    string warpPointName = warp.argumentValues[0];
                    return WarpManager.GetWarpPoint(warpPointName, out Vector3 point, out warp.validationError);
                }

                else if (warp.NumProvidedArguments() == 3) {
                    float x, y, z;
                    if (!float.TryParse(warp.argumentValues[0], out x)) return false;
                    if (!float.TryParse(warp.argumentValues[1], out y)) return false;
                    if (!float.TryParse(warp.argumentValues[2], out z)) return false;

                    // ToDo: Check if x, y, z in bounds of map
                }

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
