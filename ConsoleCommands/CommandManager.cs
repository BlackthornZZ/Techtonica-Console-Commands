using FluffyUnderware.DevTools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCommands
{
    public static class CommandManager
    {
        // Objects & Variables
        private static Dictionary<string, Command> commands = new Dictionary<string, Command>();

        // Public Functions

        public static void AddCommand(ref Command command) {
            if (commands.ContainsKey(command.name)) {
                ConsoleCommandsPlugin.Log.LogError($"Command name '{command.name}' is already taken");
                return;
            }

            if(command.arguments.Count != 0 && command.Validate == null) {
                ConsoleCommandsPlugin.Log.LogError($"You must provide a Validate() function for command '{command.name}' as it has arguments");
                return;
            }

            commands.Add(command.name, command);
        }

        internal static void ParseAndExecute(string userInput) {
            if (string.IsNullOrEmpty(userInput)) return;

            string[] parts = userInput.Split(' ');
            string name = parts[0];

            if (!commands.ContainsKey(name)) {
                Notify($"Unknown command: '{name}'");
                return;
            }

            parts = parts.RemoveAt(0);
            Command command = commands[name];
            command.argumentValues = parts;

            if (command.HasArguments() && !command.Validate()) {
                string error = string.IsNullOrEmpty(command.validationError) ? "Command has invalid arguments" : command.validationError;
                Notify(error);
                return;
            }

            command.Execute();
        }

        internal static void LoadDefaultCommands() {
            AddCommand(ref Commands.gps);
            AddCommand(ref Commands.warp);
            AddCommand(ref Commands.saveWarpPoint);
            AddCommand(ref Commands.deleteWarpPoint);
        }

        internal static void Notify(string message) {
            UIManager.instance.systemLog.FlashMessage(new CustomNotification(message));
        }
    }

    public class CustomNotification : SystemMessageInfo {
        public CustomNotification(string message) : base(message){}
        public override MessageType type => MessageType.Tutorial;
    }
}
