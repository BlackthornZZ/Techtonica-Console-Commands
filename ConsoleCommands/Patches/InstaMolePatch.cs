using EquinoxsModUtils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCommands.Patches
{
    internal class InstaMolePatch
    {
        [HarmonyPatch(typeof(TerrainManipulator), nameof(TerrainManipulator.OnUpdate))]
        [HarmonyPostfix]
        public static void Postfix(ref TerrainManipulator __instance) {
            if (!CommandSettings.instamole) return;

            __instance.currentManipulatorMode.basePerVoxelActionDuration = 0.0001f;
            ModUtils.SetPrivateField("_isOverheated", __instance, false);
            ModUtils.SetPrivateField("currentSlowDownSpeed", __instance, 2f);
        }
    }
}
