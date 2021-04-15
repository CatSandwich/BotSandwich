using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.VisualBasic;
using Attribute = System.Attribute;

namespace BotSandwich.Data
{
    public abstract class Command
    {
        public abstract string Name { get; }
        public virtual string Description => "No description set.";
        public virtual string Example => "No example set.";

        public readonly Dictionary<ArgumentAttribute, FieldInfo> ArgumentFields;

        protected Command()
        {
            ArgumentFields = new Dictionary<ArgumentAttribute, FieldInfo>(
                from field in GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                from a in Attribute.GetCustomAttributes(field)
                let fromArgument = a as ArgumentAttribute
                where fromArgument != null
                select new KeyValuePair<ArgumentAttribute, FieldInfo>(fromArgument, field)
            );
        }

        /// <summary>
        /// Populates the command's argument fields given the command message
        /// </summary>
        /// <param name="message">The message searched through</param>
        /// <param name="error">Description of any errors</param>
        /// <returns>False if errors, false if required argument not provided, else true</returns>
        public bool ParseArguments(string message, out string error)
        {
            var success = true;
            error = "";
            
            // foreach argument attribute
            foreach (var attribute in ArgumentFields)
            {
                var fromArg = attribute.Key;
                var field = attribute.Value;
                fromArg.Provided = false;

                // foreach accepted name in the argument
                foreach (var name in fromArg.Names)
                {
                    // check if provided
                    if(!_tryGetArgValue(message, name, out var value)) continue;

                    // try to parse the value
                    if (fromArg.TryParse(value, field.FieldType, out var result))
                    {
                        field.SetValue(this, result);
                        fromArg.Provided = true;
                        continue;
                    }

                    error += $"Failed to parse param '-{name}'.\n";
                    success = false;
                }

                if (fromArg.Required && !fromArg.Provided)
                {
                    error += $"Required param '-{fromArg.Names[0]}' not provided\n";
                    success = false;
                }
            }

            return success;
        }

        /// <summary>
        /// Checks for an argument in a message.
        /// </summary>
        /// <param name="str">The message searched through</param>
        /// <param name="name">The argument name</param>
        /// <param name="result">The value if the argument is provided</param>
        /// <returns>Whether or not the argument was found</returns>
        private static bool _tryGetArgValue(string str, string name, out string result)
        {
            var quoteMatch = Regex.Match(str, $"-{name} \"[^\"]*");
            var noQuoteMatch = Regex.Match(str, $"-{name} [^ ]*");

            // If no success, try next
            if (!quoteMatch.Success && !noQuoteMatch.Success)
            {
                result = "";
                return false;
            }

            var match = quoteMatch.Success ? quoteMatch.Value : noQuoteMatch.Value;
            result = match.Replace("\"", "").Substring(name.Length + 2);
            return true;
        }
        
        public bool HasArgument(string name) => ArgumentFields.Any(a => a.Key.Names.Contains(name) && a.Key.Provided);
        public abstract Task Run(SocketMessage sm, string msg);
    }
}
