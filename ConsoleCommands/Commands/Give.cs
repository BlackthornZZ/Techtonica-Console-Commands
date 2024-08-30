using EquinoxsModUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolBuddy.ThirdParty.VectorGraphics;

namespace ConsoleCommands
{
    internal static partial class Commands {
        internal static Dictionary<string, ResourceInfo> itemsCache = new Dictionary<string, ResourceInfo>();

        internal static Command give = new Command() {
            name = "Give",
            description = "Adds the specified amount of the specified item to your inventory",
            examples = new List<string>() {
                "give all",
                "give IronIngot",
                "give IronIngot 10",
                "give ironingot max",
            },
            arguments = new List<Argument>() {
                new Argument() {
                    name = "Item",
                    description = "Either 'all' or the item you want to add",
                    type = typeof(string),
                    optional = false
                },
                new Argument() {
                    name = "Count",
                    description = "The number of the item to give. Defaults to 1. 'max' to give 1 stack.",
                    type = typeof(string),
                    optional = true
                }
            },
            Validate = () => {
                if (!give.ValidateNumProvidedArguments(1, 2)) return false;

                string itemName = FormatItemName(give.argumentValues[0]);
                if (itemName != "all" && !itemsCache.ContainsKey(itemName)) {
                    give.validationError = $"Unknown item '{itemName}'";
                    return false;
                }

                // Don't check the count argument if it wasn't provided
                if (give.NumProvidedArguments() == 1) return true;
                if (!give.ValidateUIntArgument(1, 1, uint.MaxValue)) return false;

                return true;
            },
            Execute = () => {
                string itemName = FormatItemName(give.argumentValues[0]);

                if(itemName == "all") {
                    GiveAllItems();
                    return;
                }

                ResourceInfo item = itemsCache[itemName];

                uint count = 1;
                if (give.NumProvidedArguments() == 2) {
                    string countString = give.argumentValues[1];
                    if (countString == "max") count = (uint)item.maxStackCount;
                    else count = uint.Parse(countString);
                }

                Player.instance.inventory.AddResources(item, (int)count);
                CommandManager.Notify($"Added {count} {item.displayName}");
            }
        };

        internal static void InitialiseItemsCache() {
            foreach(string name in ResourceNames.SafeResources) {
                ResourceInfo item = ModUtils.GetResourceInfoByName(name);
                if (item != null) itemsCache.Add(FormatItemName(name), item);
            }
        }

        private static string FormatItemName(string name) {
            return name.Replace(" ", "").ToLower();
        }

        private static void GiveAllItems() {
            foreach(ResourceInfo item in itemsCache.Values) {
                int count = item.maxStackCount;
                if(give.NumProvidedArguments() == 2) {
                    string countString = give.argumentValues[1];
                    if (countString == "max") count = item.maxStackCount;
                    else count = int.Parse(countString);
                }

                Player.instance.inventory.AddResources(item, count);
            }

            CommandManager.Notify("Added all items");
        }
    }
}
