using EquinoxsModUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCommands
{
    internal static partial class Commands {
        internal static Dictionary<string, int> unlocksCache = new Dictionary<string, int>();

        internal static Command unlock = new Command() {
            name = "Unlock",
            description = "Activates the specified tech tree unlock",
            examples = new List<string>() {
                $"unlock {FormatUnlockName(UnlockNames.MiningDrillMKII)}",
                $"unlock {FormatUnlockName(UnlockNames.CoreClustering)} true",
                 "unlock all"
            },
            arguments = new List<Argument>() {
                new Argument() {
                    name = "Unlock Name",
                    description = "Name of the unlock or 'all'",
                    type = typeof(string),
                    optional = false
                },
                new Argument() {
                    name = "Should Draw Power",
                    description = "Whether power should be drained from accumulators on unlock",
                    type = typeof(bool),
                    optional = true
                }
            },
            Validate = () => {
                if (!unlock.ValidateNumProvidedArguments(1, 2)) return false;

                string name = unlock.argumentValues[0];
                if (name != "all" && !unlocksCache.ContainsKey(name)) {
                    unlock.validationError = $"Unknown unlock: '{name}'";
                    return false;
                }

                if(!unlock.ValidateBoolArgument(1)) return false;

                return true;
            },
            Execute = () => {
                bool drawPower = false;
                if(unlock.NumProvidedArguments() == 2) {
                    drawPower= bool.Parse(unlock.argumentValues[1]);
                }
                
                if (unlock.argumentValues[0] == "all") {
                    List<Unlock> toUnlock = GameDefines.instance.unlocks.Where(
                        tech => !TechTreeState.instance.IsUnlockActive(tech.uniqueId)
                    ).ToList();

                    foreach(Unlock tech in toUnlock) {
                        UnlockTech(tech.uniqueId, drawPower);
                    }

                    return;
                }
                
                int id = unlocksCache[unlock.argumentValues[0]];
                UnlockTech(id, drawPower);

                CommandManager.Notify($"Unlocked {unlock.argumentValues[0]}");
            }
        };

        internal static void InitialiseUnlocksCache() {
            foreach(Unlock unlock in GameDefines.instance.unlocks) {
                string name = LocsUtility.TranslateStringFromHash(unlock.displayNameHash);
                unlocksCache.Add(FormatUnlockName(name), unlock.uniqueId);
            }

            List<string> names = unlocksCache.Keys.ToList();
            names.Insert(0, "all");
            unlock.arguments[0].options = names;
        }

        private static string FormatUnlockName(string name) {
            return name.Replace(" ", "").ToLower();
        }

        private static void UnlockTech(int id, bool consumePower) {
            NetworkMessageRelay.instance.SendNetworkAction(new UnlockTechAction() {
                info = new UnlockTechInfo() {
                    unlockID = id,
                    drawPower = consumePower
                }
            });
        }
    }
}
