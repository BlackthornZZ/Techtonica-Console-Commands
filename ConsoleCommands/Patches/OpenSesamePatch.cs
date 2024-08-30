using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCommands.Patches
{
    internal class OpenSesamePatch
    {
        [HarmonyPatch(typeof(ResourceGateInstance), nameof(ResourceGateInstance.CheckForRequiredResources))]
        [HarmonyPrefix]
        public static void setDoorToFree(ref ResourceGateInstance __instance) {
            if (__instance.commonInfo.instanceId != GlobalData.doorToOpen.commonInfo.instanceId) return;

            for (int i = 0; i < __instance.resourcesRequired.Length; i++) {
                __instance.resourcesRequired[i].quantity = 0;
            }
        }
    }
}
