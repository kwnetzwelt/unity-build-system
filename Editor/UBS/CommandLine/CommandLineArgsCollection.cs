using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor.UBS.Commandline
{
    [Serializable]
    public class CommandLineArgsCollection
    {
        public CommandLineArgsCollection(CommandLineArgsCollection other)
        {
            foreach (var argument in other.arguments)
            {
                arguments.Add(new CommandLineArgsParser.Argument(argument));
            }
        }
        public bool HasArgument(string argument)
        {
            return arguments.Find( (a) => a.Name == argument) != null;
        }

        /// <summary>
        /// Will extract the value of a commandline argument. 
        /// </summary>
        /// <returns>The value of the argument or null if the argument was not present or found. </returns>
        public string GetValue(string argument)
        {
            var found = arguments.Find((a) => a.Name == argument);
            if (found == null)
                return null;
            return found.Value;
        }
            
        public bool TryGetValue(string argument, out bool value)
        {
            value = false;
            var found = arguments.Find((a) => a.Name == argument);
            if (found == null)
                return false;
            return bool.TryParse(found.Value, out value);
        }
        public bool TryGetValue(string argument, out float value)
        {
            value = 0;
            var found = arguments.Find((a) => a.Name == argument);
            if (found == null)
                return false;
            return float.TryParse(found.Value, out value);
        }
        public bool TryGetValue(string argument, out int value)
        {
            value = 0;
            var found = arguments.Find((a) => a.Name == argument);
            if (found == null)
                return false;
            return int.TryParse(found.Value, out value);
        }
        public bool TryGetValue(string argument, ref string value)
        {
            var found = arguments.Find((a) => a.Name == argument);
            if (found == null)
                return false;
            value = found.Value;
            return true;
        }
        [SerializeField]
        internal List<CommandLineArgsParser.Argument> arguments = new List<CommandLineArgsParser.Argument>();

        public CommandLineArgsCollection()
        {
        }

        public List<CommandLineArgsParser.Argument> Arguments => arguments;
        internal void Next()
        {
            arguments.Add(new CommandLineArgsParser.Argument());
        }
        internal CommandLineArgsParser.Argument Last
        {
            get
            {
                if (arguments.Count == 0)
                {
                    Next();
                }

                return arguments[^1];
            }
        }

        public int Count => arguments.Count;
    }
}