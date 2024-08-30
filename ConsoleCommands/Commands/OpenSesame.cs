using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DoorList = MachineInstanceList<ResourceGateInstance, ResourceGateDefinition>;

namespace ConsoleCommands
{
    internal static partial class GlobalData {
        internal static ResourceGateInstance doorToOpen;
    }

    internal static partial class Commands {
        const int maxRange = 8;

        internal static Command openSesame = new Command() {
            name = "Open Sesame",
            description = $"Opens the nearest door that is within {maxRange} voxels / power floors",
            examples = new List<string>() { 
                "opensesame",
                "OpenSesame"
            },
            Validate = () => {
                if (!GetNearestDoor(out ResourceGateInstance door)) {
                    openSesame.validationError = $"No closed doors within max range: {maxRange} voxels";
                    return false;
                }

                return true;
            },
            Execute = () => {
                GetNearestDoor(out ResourceGateInstance door);
                AddRequiredResources(ref door);
                OpenDoor(ref door);
                door.ProcessUpgrade();
                door.interactionState = 2;
                CommandManager.Notify($"Opened Door '{door.myConfig.displayName}'");
            }
        };

        private static bool GetNearestDoor(out ResourceGateInstance closestDoor) {
            float closestDoorDistance = float.MaxValue;
            closestDoor = default;

            DoorList doorsList = (DoorList)MachineManager.instance.GetMachineList(MachineTypeEnum.ResourceGate);
            foreach (ResourceGateInstance instance in doorsList.myArray) {
                if (instance.beenActivated) continue;

                float distance = instance.gridInfo.Center.Distance(Player.instance.transform.position);
                if(distance < closestDoorDistance) {
                    closestDoorDistance = distance;
                    closestDoor = instance;
                }
            }

            if(closestDoorDistance <= maxRange) {
                GlobalData.doorToOpen = closestDoor;
                return true;
            }
            else {
                return false;
            }
        }

        private static void AddRequiredResources(ref ResourceGateInstance door) {
            for(int i = 0; i < door.resourcesRequired.Length; i++) {
                int resId = door.resourcesRequired[i].resType.uniqueId;
                int count = door.resourcesRequired[i].quantity;
                door.GetInputInventory().AddResourcesToSlot(resId, i, out _, count);
            }
        }

        private static void OpenDoor(ref ResourceGateInstance door) {
            if (!door.CheckForRequiredResources()) {
                Player.instance.audio.error.PlayRandomClip();
                return;
            }

            CompleteResourceGateAction action = new CompleteResourceGateAction() {
                info = new CompleteResourceGateInfo() {
                    machineId = door.commonInfo.instanceId,
                    unlockLevel = 1
                }
            };

            NetworkMessageRelay.instance.SendNetworkAction(action);
            Player.instance.audio.productionTerminalTierUpgrade.PlayRandomClip();
        }
    }
}
