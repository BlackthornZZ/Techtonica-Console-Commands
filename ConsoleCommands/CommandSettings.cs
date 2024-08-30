using EquinoxsDebuggingTools;
using EquinoxsModUtils;
using EquinoxsModUtils.Additions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleCommands
{
    internal static class CommandSettings
    {
        internal static float maxWalkSpeed = 5f;
        internal static float maxRunSpeed = 8f;
        internal static float maxFlySpeed = 15f;
        internal static float jumpSpeed = 10f;
        internal static float scanSpeed = 1f;
        internal static float gravity = 20f;
        internal static float maxFlyHeight = 3f;
        internal static float railRunnerSpeed = 2f;

        internal static bool weightless = false;
        internal static bool instamole = false;

        // Internal Functions

        internal static void Save() {
            EMUAdditions.CustomData.Update(0, "maxWalkSpeed", maxWalkSpeed);
            EMUAdditions.CustomData.Update(0, "maxRunSpeed", maxRunSpeed);
            EMUAdditions.CustomData.Update(0, "maxFlySpeed", maxFlySpeed);
            EMUAdditions.CustomData.Update(0, "jumpSpeed", jumpSpeed);
            EMUAdditions.CustomData.Update(0, "scanSpeed", scanSpeed);
            EMUAdditions.CustomData.Update(0, "gravity", gravity);
            EMUAdditions.CustomData.Update(0, "maxFlyHeight", maxFlyHeight);
            EMUAdditions.CustomData.Update(0, "railRunnerSpeed", railRunnerSpeed);

            EMUAdditions.CustomData.Update(0, "weightless", weightless);
            EMUAdditions.CustomData.Update(0, "instamole", instamole);
        }

        internal static void Load() {
            if(EMUAdditions.CustomData.FieldExists<float>(0, "maxWalkSpeed")) {
                maxWalkSpeed = EMUAdditions.CustomData.Get<float>(0, "maxWalkSpeed");
            }

            if (EMUAdditions.CustomData.FieldExists<float>(0, "maxRunSpeed")) {
                maxRunSpeed = EMUAdditions.CustomData.Get<float>(0, "maxRunSpeed");
            }

            if (EMUAdditions.CustomData.FieldExists<float>(0, "maxFlySpeed")) {
                maxFlySpeed = EMUAdditions.CustomData.Get<float>(0, "maxFlySpeed");
            }

            if (EMUAdditions.CustomData.FieldExists<float>(0, "jumpSpeed")) {
                jumpSpeed = EMUAdditions.CustomData.Get<float>(0, "jumpSpeed");
            }

            if (EMUAdditions.CustomData.FieldExists<float>(0, "scanSpeed")) {
                scanSpeed = EMUAdditions.CustomData.Get<float>(0, "scanSpeed");
            }
            
            if (EMUAdditions.CustomData.FieldExists<float>(0, "gravity")) {
                gravity = EMUAdditions.CustomData.Get<float>(0, "gravity");
            }

            if (EMUAdditions.CustomData.FieldExists<float>(0, "maxFlyHeight")) {
                maxFlyHeight = EMUAdditions.CustomData.Get<float>(0, "maxFlyHeight");
            }

            if (EMUAdditions.CustomData.FieldExists<float>(0, "railRunnerSpeed")) {
                railRunnerSpeed = EMUAdditions.CustomData.Get<float>(0, "railRunnerSpeed");
            }

            if (EMUAdditions.CustomData.FieldExists<bool>(0, "weightless")) {
                weightless = EMUAdditions.CustomData.Get<bool>(0, "weightless");
            }

            if(EMUAdditions.CustomData.FieldExists<bool>(0, "instamole")) {
                instamole = EMUAdditions.CustomData.Get<bool>(0, "instamole");
            }
        }

        internal static void Apply() {
            Player.instance.fpcontroller.maxWalkSpeed = CommandSettings.maxWalkSpeed;
            Player.instance.fpcontroller.maxRunSpeed = CommandSettings.maxRunSpeed;
            Player.instance.fpcontroller.maxFlySpeed = CommandSettings.maxFlySpeed;
            Player.instance.fpcontroller.jumpSpeed = CommandSettings.jumpSpeed;
            Player.instance.fpcontroller.gravity = CommandSettings.gravity;

            ModUtils.SetPrivateField("_stiltHeight", Player.instance.equipment.hoverPack, CommandSettings.maxFlyHeight);
            ModUtils.SetPrivateField("_hookSpeed", Player.instance.equipment.railRunner, CommandSettings.railRunnerSpeed);
        }
    }
}
