using EquinoxsDebuggingTools;
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
            string lowerName = command.name.Replace(" ", "").ToLower();
            if (commands.ContainsKey(lowerName)) {
                ConsoleCommandsPlugin.Log.LogError($"Command name '{lowerName}' is already taken");
                return;
            }

            if(command.arguments.Count != 0 && command.Validate == null) {
                ConsoleCommandsPlugin.Log.LogError($"You must provide a Validate() function for command '{command.name}' as it has arguments");
                return;
            }

            commands.Add(lowerName, command);
        }

        internal static List<string> GetNames() {
            return commands.Keys.ToList();
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

            if (!command.Validate()) {
                string error = string.IsNullOrEmpty(command.validationError) ? "Command has invalid arguments" : command.validationError;
                Notify(error);
                return;
            }

            command.Execute();
        }

        internal static void LoadDefaultCommands() {
            AddCommand(ref Commands.bind);
            AddCommand(ref Commands.echo);
            AddCommand(ref Commands.gameSpeed);
            AddCommand(ref Commands.gps);
            AddCommand(ref Commands.instamole);
            AddCommand(ref Commands.openSesame);
            AddCommand(ref Commands.quit);
            AddCommand(ref Commands.setExplosiveSize);
            AddCommand(ref Commands.setMoleDimensions);
            AddCommand(ref Commands.setPlayerParam);
            AddCommand(ref Commands.setSize);
            AddCommand(ref Commands.unlock);
            AddCommand(ref Commands.weightless);

            AddCommand(ref Commands.freeCam);
            AddCommand(ref Commands.noClip);

            AddCommand(ref Commands.give);
            AddCommand(ref Commands.remove);

            AddCommand(ref Commands.warp);
            AddCommand(ref Commands.saveWarpPoint);
            AddCommand(ref Commands.deleteWarpPoint);
        }

        internal static void Notify(string message) {
            UIManager.instance.systemLog.FlashMessage(new CustomNotification(message));
            EDT.Log("Command Notifications", message);
        }
    }

    public class CustomNotification : SystemMessageInfo {
        public CustomNotification(string message) : base(message){}
        public override MessageType type => MessageType.Tutorial;
    }
}
