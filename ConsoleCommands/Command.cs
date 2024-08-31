using FIMSpace.Graph;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ConsoleCommands
{
    public class Command
    {
        public string name;
        public string description;
        public List<string> examples = new List<string>();
        public List<Argument> arguments = new List<Argument>();
        public string[] argumentValues { get; internal set; }
        public string validationError;
        
        public Action Execute;
        public Func<bool> Validate = () => { return true; };

        public bool HasArguments() {
            return arguments.Any();
        }

        public int NumProvidedArguments() {
            if(argumentValues == null) return 0;
            return argumentValues.Length;
        }

        public bool ValidateNumProvidedArguments(int expected) {
            if (NumProvidedArguments() < expected) {
                validationError = $"Not enough arguments: {NumProvidedArguments()} - Expected {expected}";
                return false;
            }

            if (NumProvidedArguments() > expected) {
                validationError = $"Too many arguments: {NumProvidedArguments()} - Expected {expected}";
                return false;
            }

            return true;
        }

        public bool ValidateNumProvidedArguments(int min, int max) {
            if(NumProvidedArguments() < min) {
                validationError = $"Not enough arguments: {NumProvidedArguments()} - Expected > {min}";
                return false;
            }

            if(NumProvidedArguments() > max) {
                validationError = $"Too many arguments: {NumProvidedArguments()} - Expected < {max}";
                return false;
            }

            return true;
        }

        public bool ValidateNumProvidedArguments(List<int> acceptableQuantities) {
            if (!acceptableQuantities.Contains(NumProvidedArguments())) {
                validationError = $"Invalid number of arguments: {NumProvidedArguments()} - Expected one of {string.Join(", ", acceptableQuantities)}";
                return false;
            }

            return true;
        }

        public bool ValidateUIntArgument(int index, uint min = 0, uint max = uint.MaxValue) {
            if (!uint.TryParse(argumentValues[index], out uint value)) {
                validationError = $"Invalid positive integer argument: {argumentValues[index]}";
                return false;
            }
            
            if(min != 0 && value < min) {
                validationError = $"'{arguments[index].name}' argument must be > {min}";
                return false;
            }

            if(max != uint.MaxValue && value > max) {
                validationError = $"'{arguments[index].name}' argument must be < {max}";
                return false;
            }

            return true;
        }

        public bool ValidateIntArgument(int index, int min = int.MinValue, int max = int.MaxValue) {
            if (!int.TryParse(argumentValues[index], out int value)) {
                validationError = $"Invalid integer argument: {argumentValues[index]}";
                return false;
            }

            if(min != int.MinValue && value < min) {
                validationError = $"'{arguments[index].name}' argument must be > {min}";
                return false;
            }

            if(max != int.MaxValue && value > max) {
                validationError = $"'{arguments[index].name}' argument must be < {max}";
                return false;
            }

            return true;
        }

        public bool ValidateFloatArgument(int index, float min = float.MinValue, float max = float.MaxValue) {
            if (!float.TryParse(argumentValues[index], out float value)) {
                validationError = $"Invalid decimal argument: {argumentValues[index]}";
                return false;
            }

            if(min != float.MinValue && value < min) {
                validationError = $"'{arguments[index].name}' argument must be > {min}";
                return false;
            }

            if(max !=  float.MaxValue && value > max) {
                validationError = $"'{arguments[index].name}' argument must be < {max}";
                return false;
            }

            return true;
        }

        public bool ValidateBoolArgument(int index) {
            if (!bool.TryParse(argumentValues[index], out _)) {
                validationError = $"Invalid bool argument: {argumentValues[index]}";
                return false;
            }

            return true;
        }

        public bool ValidateOptionsBasedArgument(int index) {
            Argument argument = arguments[index];
            if(argument.options.Count == 0) {
                CommandManager.Notify($"Command error in '{name}'. Please notify the developer.");
                ConsoleCommandsPlugin.Log.LogError($"ValidateOptionsBasedArgument() was called on argument '{argument.name}' which has no options");
                return true;
            }

            if (!argument.options.Contains(argumentValues[index])) {
                validationError = $"'{argumentValues[index]}' is not a valid value for argument '{argument.name}'";
                return false;
            }

            return true;
        }

        public int MaxArguments() {
            return arguments.Count;
        }

        public void LogDocumentation() {
            Debug.Log($"Documentation for Command '{name}':\n\n{GenerateDocumentation()}\n\n");
        }

        public string GenerateDocumentation() {
            string markup = $"#### {name}\n\n{description}\n\n";
            if(arguments.Count > 0) {
                bool joinOptions = name != "Give" && name != "Remove" && name != "Unlock" && name != "Bind";

                markup += "Arguments:\n\n";
                foreach(Argument argument in arguments) {
                    markup += $"{argument.GenerateDocumentation(joinOptions)}\n\n";
                }
            }

            if(examples.Count > 0) {
                markup += $"Examples:\n\n";
                foreach (string example in examples) {
                    markup += $"- `{example}`\n";
                }
            }

            if(markup.EndsWith("\n")) markup = markup.Substring(0, markup.Length - 1);

            return markup;
        }
    }

    public class Argument {
        public string name;
        public string description;
        public List<string> options = new List<string>();
        public bool optional;
        public Type type;

        internal string GenerateDocumentation(bool joinOptions) {
            string markup = $"- {name}\n  - Description: {description}\n";
            if(options.Count > 0) {
                string optionsList = joinOptions ? string.Join(", ", options) : "**INSERT LINK TO OPTIONS**";
                markup += $"  - Options: {optionsList}\n";
            }

            string optionalText = optional ? "Yes" : "No";
            markup += $"  - Type: {GetPrettyType(type)}\n  - Optional: {optionalText}";

            return markup;
        }

        private static string GetPrettyType(Type type) {
            if (type == typeof(string)) return "String";
            if (type == typeof(uint)) return "Positive Integer";
            if (type == typeof(int)) return "Interger";
            if (type == typeof(float)) return "Decimal Number";
            if (type == typeof(bool)) return "Bool";
            if (type == typeof(KeyCode)) return "Key Code";

            return type.ToString();
        }
    }
}
