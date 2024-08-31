using FIMSpace.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

    public class Argument {
        public string name;
        public string description;
        public List<string> options = new List<string>();
        public bool optional;
        public Type type;
    }
}
