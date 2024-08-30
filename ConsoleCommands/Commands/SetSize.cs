using EquinoxsModUtils;
using FIMSpace.Generating.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ConsoleCommands
{
    internal static partial class Commands {
        internal static Command setSize = new Command() {
            name = "Set Size",
            description = "Multiplies your character's size by the given amount",
            examples = new List<string>() {
                "setsize 2 true",
                "SetSize 0.5 False"
            },
            arguments = new List<Argument>() {
                new Argument() {
                    name = "Multiplier",
                    description = "Value to multiply scale by",
                    type = typeof(float),
                    optional = false
                },
                new Argument() {
                    name = "Scale Player Parameters",
                    description = "Whether to multiply player parameters like walk speed too",
                    type = typeof(bool),
                    optional = false
                }
            },
            Validate = () => {
                if (!setSize.ValidateNumProvidedArguments(2)) return false;
                if (!setSize.ValidateFloatArgument(0, 0.01f)) return false;
                if (!setSize.ValidateBoolArgument(1)) return false;

                return true;
            },
            Execute = () => {
                float multiplier = float.Parse(setSize.argumentValues[0]);
                bool scaleParams = bool.Parse(setSize.argumentValues[1]);

                Player.instance.transform.localScale = new Vector3(multiplier, multiplier, multiplier);
                Player.instance.cam.transform.localScale = new Vector3(multiplier, multiplier, multiplier);

                if (!scaleParams) return;

                PlayerFirstPersonController.instance.maxWalkSpeed = 5f * multiplier;
                PlayerFirstPersonController.instance.maxRunSpeed = 8f * multiplier;
                PlayerFirstPersonController.instance.gravity = 20f * multiplier;
                PlayerFirstPersonController.instance.maxFallSpeed = -15f * multiplier;

                ModUtils.SetPrivateField("inAirDuration", PlayerFirstPersonController.instance, 2f * multiplier);

                ModUtils.SetPrivateField("peakMinHeight", PlayerFirstPersonController.instance, 1f * multiplier);
                ModUtils.SetPrivateField("peakMaxHeight", PlayerFirstPersonController.instance, 2f * multiplier);
                
                ModUtils.SetPrivateField("_stiltHeight", Player.instance.equipment.hoverPack, 3f * multiplier);
                ModUtils.SetPrivateField("_raiseSpeed", Player.instance.equipment.hoverPack, 5f * multiplier);
            }
        };
    }
}
