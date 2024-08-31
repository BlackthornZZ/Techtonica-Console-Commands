using BepInEx;
using EquinoxsModUtils;
using EquinoxsModUtils.Additions;
using FluffyUnderware.DevTools.Extensions;
using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ConsoleCommands
{
    internal static partial class Commands {
        private static Dictionary<string, KeyCode> keyCodeMap = new Dictionary<string, KeyCode>();
        internal static Dictionary<KeyCode, string> boundCommands = new Dictionary<KeyCode, string>();

        internal static Command bind = new Command() {
            name = "Bind",
            description = "Binds a command to be executed each time you press a key",
            examples = new List<string>() {
                "bind numpad1 weightless",
                "Bind NumPad2 SetMoleSize 15 15 15"
            },
            arguments = new List<Argument>() {
                new Argument() {
                    name = "Key",
                    description = "The key to press to trigger the command",
                    type = typeof(KeyCode),
                    optional = false
                },
                new Argument() {
                    name = "Command",
                    description = "The command to execute when the key is pressed",
                    type = typeof(string),
                    optional = false
                }
            },
            Validate = () => {
                if (!bind.ValidateNumProvidedArguments(2, int.MaxValue)) return false;
                if (!bind.ValidateOptionsBasedArgument(0)) return false;

                return true;
            },
            Execute = () => {
                KeyCode key = keyCodeMap[bind.argumentValues[0]];
                string command = string.Join(" ", bind.argumentValues.RemoveAt(0));
                boundCommands[key] = command;
                CommandManager.Notify($"Bound '{command}' to '{key}'");

                SaveBoundCommands();
            }
        };

        internal static void InitialiseKeyCodeMap() {
            foreach(KeyCode key in Enum.GetValues(typeof(KeyCode))) {
                string lowerName = key.ToString().Replace(" ", "").ToLower();
                keyCodeMap[lowerName] = key;
            }

            bind.arguments[0].options = keyCodeMap.Keys.ToList();
        }

        private static float sSinceCommandExecute = 0f;
        internal static void ExecuteBoundCommands() {
            sSinceCommandExecute += Time.deltaTime;
            if (sSinceCommandExecute < 0.2f) return;

            foreach(KeyValuePair<KeyCode, string> pair in boundCommands) {
                if (UnityInput.Current.GetKey(pair.Key)) {
                    CommandManager.ParseAndExecute(pair.Value);
                    sSinceCommandExecute = 0f;
                }
            }
        }

        private static void SaveBoundCommands() {
            foreach (KeyValuePair<KeyCode, string> pair in boundCommands) {
                EMUAdditions.CustomData.Update(0, $"boundCommand-{pair.Key}", pair.Value);
            }
        }

        internal static void LoadBoundCommands() {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode))) {
                if (EMUAdditions.CustomData.FieldExists<string>(0, $"boundCommand-{key}")) {
                    string command = EMUAdditions.CustomData.Get<string>(0, $"boundCommand-{key}");
                    boundCommands[key] = command;
                }
            }
        }
    }
}
