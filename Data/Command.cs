using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.VisualBasic;

namespace BotSandwich.Data
{
    public abstract class Command
    {
        public abstract string Name { get; }
        public virtual string Description => "No description set.";
        public virtual string Example => "No example set.";
        
        public readonly List<Argument> Arguments = new List<Argument>();

        // Runs all argument callbacks and returns whether or not they all succeed
        public async Task<bool> RunArguments(SocketMessage sm, string msg)
        {
            var success = true;
            foreach (var a in Arguments)
            {
                a.Supplied = await _checkArgument(a, msg);
                if (a.Required && !a.Supplied) success = false;
            }
            return success;
        }

        protected void AddArgument(Argument argument) => Arguments.Add(argument);

        private static async Task<bool> _checkArgument(Argument arg, string msg)
        {
            // Try Regex on all name variations of the argument
            foreach (var name in arg.Name)
            {
                var quoteMatch = Regex.Match(msg, $"-{name} \"[^\"]*");
                var noQuoteMatch = Regex.Match(msg, $"-{name} [^ ]*");

                // If no success, try next
                if (!quoteMatch.Success && !noQuoteMatch.Success) continue;
                
                // Else get value, run callback and relay its return value
                var match = quoteMatch.Success ? quoteMatch.Value : noQuoteMatch.Value;
                var value = _getArgValue(match, name);
                
                // If no callback provided, just return true
                if (arg.Invoke == null) return true;
                
                return await arg.Invoke(value);
            }

            // If no matches
            return false;
        }

        // Remove space, dash, and arg name from content
        private static string _getArgValue(string str, string argName) => str.Replace("\"", "").Substring(argName.Length + 2);
        protected bool HasArgument(string name) => Arguments.First(a => a.Name.Any(n => n == name)).Supplied;

        public abstract Task Run(SocketMessage sm, string msg);
    }
}
