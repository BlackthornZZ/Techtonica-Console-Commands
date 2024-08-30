using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCommands.Patches
{
    internal class ScannableDataPatch
    {
        [HarmonyPatch(typeof(ScannableData), "GetScanDuration")]
        [HarmonyPostfix]
        static void ApplyScanSpeedModification(ref float __result) {
            __result /= CommandSettings.scanSpeed;
        }
    }
}
