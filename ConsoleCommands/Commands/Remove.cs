using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCommands
{
    internal static partial class Commands {
        internal static Command remove = new Command() {
            name = "Remove",
            description = "Removes the given amount of an item from your inventory",
            examples = new List<string>() {
                "remove ironingot",
                "remove ironingot 10",
                "remove IronIngot max",
                "remove all"
            },
            arguments = new List<Argument>() {
                new Argument() {
                    name = "Item",
                    description = "The item you want to remove or 'all' to clear inventory",
                    type = typeof(string),
                    optional = false
                },
                new Argument() {
                    name = "Count",
                    description = "The number of the item to remove. Defaults to 1. 'max' to remove all",
                    type = typeof(string),
                    optional = true
                }
            },
            Validate = () => {
                if (!remove.ValidateNumProvidedArguments(1, 2)) return false;
                if (!remove.ValidateOptionsBasedArgument(0)) return false;

                // Don't check the count argument if it wasn't provided or if it's "max"
                if (remove.NumProvidedArguments() == 1 || remove.argumentValues[1] == "max") return true;
                if (!remove.ValidateUIntArgument(1)) return false;

                return true;
            },
            Execute = () => {
                string itemName = FormatItemName(remove.argumentValues[0]);

                if(itemName == "all") {
                    ClearInventory();
                    return;
                }

                ResourceInfo item = itemsCache[itemName];

                uint count = 1;
                if(remove.NumProvidedArguments() == 2) {
                    string countString = remove.argumentValues[1];
                    if (countString == "max") count = (uint)Player.instance.inventory.GetResourceCount(item);
                    else count = uint.Parse(countString);
                }

                Player.instance.inventory.TryRemoveResources(item, (int)count);
                CommandManager.Notify($"Removed {count} {item.displayName}");
            }
        };

        private static void ClearInventory() {
            for(int i = 0; i < Player.instance.inventory.myInv.myStacks.Length; i++) {
                Player.instance.inventory.myInv.myStacks[i] = ResourceStack.CreateEmptyStack();
            }

            CommandManager.Notify("Cleared inventory");
        }
    }
}
