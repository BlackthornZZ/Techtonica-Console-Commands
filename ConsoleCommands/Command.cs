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
        public Func<bool> Validate;

        public bool HasArguments() {
            return arguments.Any();
        }

        public int NumProvidedArguments() {
            if(argumentValues == null) return 0;
            return argumentValues.Length;
        }

        public int MaxArguments() {
            return arguments.Count;
        }
    }

    public class Argument {
        public string name;
        public string description;
        public bool optional;
        public Type type;
    }
}
