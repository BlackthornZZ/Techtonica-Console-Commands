using EMU.Additions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ConsoleCommands
{
    internal static class WarpManager
    {
        // Objects & Variables
        private static Dictionary<string, Vector3> warpPoints = new Dictionary<string, Vector3>();

        // Internal Functions

        internal static bool AddWarpPoint(string name , Vector3 point, out string error) {
            error = "";
            
            if (warpPoints.ContainsKey(name)) {
                error = $"A warp point named '{name}' already exists. Use 'DeleteWarpPoint {name}' to delete it";
                return false;
            }

            warpPoints.Add(name, point);
            SaveData();
            return true;
        }

        internal static bool GetWarpPoint(string name, out Vector3 point, out string error) {
            error = "";
            point = Vector3.zero;

            if (!warpPoints.ContainsKey(name)) {
                error = $"A warp point named '{name}' has not been saved. Use 'SaveWarpPoint' to create it";
                return false;
            }

            point = warpPoints[name];
            return true;
        }

        internal static bool DeleteWarpPoint(string name, out string error) {
            error = "";
            if (warpPoints.ContainsKey(name)) {
                error = $"A warp point named '{name}' has not been saved. Use 'SaveWarpPoint' to create it";
                return false;
            }

            warpPoints.Remove(name);
            SaveData();
            return true;
        }

        // Private Functions

        internal static void SaveData() {
            List<string> points = new List<string>();
            foreach(KeyValuePair<string, Vector3> pair in warpPoints) {
                points.Add($"{pair.Key}/{pair.Value}");
            }

            string joined = string.Join("|", points);
            EMUAdditions.CustomData.Update(0, "WarpPoints", joined);
        }

        internal static void LoadData() {
            string joined = EMUAdditions.CustomData.Get<string>(0, "WarpPoints");
            if (string.IsNullOrEmpty(joined)) return;

            string[] points = joined.Split('|');
            foreach(string point in points) {
                string[] parts = point.Split('/');
                AddWarpPoint(parts[0], ParseVector3(parts[1]), out string error);
            }
        }

        internal static Vector3 ParseVector3(string input) {
            input = input.Replace("(", "").Replace(")", "");
            string[] parts = input.Split(',');
            return new Vector3() {
                x = float.Parse(parts[0]),
                y = float.Parse(parts[1]),
                z = float.Parse(parts[2]),
            };
        }
    }
}
