using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.VisualBasic;
using Attribute = System.Attribute;

namespace BotSandwich.Data
{
    public abstract class Command
    {
        public abstract string Name { get; }
        public virtual string Description => "No description set.";
        public virtual string[] Examples => new string[0];
        
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
        /// Creates a help embed.
        /// </summary>
        /// <param name="prefix">The prefix used in the embed</param>
        /// <returns>The built embed object</returns>
        public Embed BuildEmbed(string prefix)
        {            // Add command's help embed
            var temp = new EmbedBuilder
            {
                Title = $"Command: {prefix}{Name}",
                Description = Description
            };

            foreach (var argument in ArgumentFields)
            {
                var att = argument.Key;
                var field = argument.Value;
                var aliases = string.Join(", ", att.Names);
                var title = $"Argument: {aliases} ({field.FieldType.Name}, {(att.Required ? "Required" : "Optional")}) ";
                
                var descriptionAtt = field
                                     .GetCustomAttributes()
                                     .FirstOrDefault(a => a.GetType() == typeof(ArgumentDescriptionAttribute))
                                     as ArgumentDescriptionAttribute;
                var description = descriptionAtt?.Description ?? "No description set.";
                
                temp.AddField(title, description);
            }
            
            foreach(var e in Examples) temp.AddField("Example:", $"{prefix}{Name} {e}");

            return temp.Build();
        }

        /// <summary>
        /// Populates the command's argument fields given the command message
        /// </summary>
        /// <param name="message">The message searched through</param>
        /// <param name="error">Description of any errors</param>
        /// <returns>False if errors, false if required argument not provided, else true</returns>
        public async Task ParseArguments(SocketMessage message)
        {
            
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
                    if(!_tryGetArgValue(message.Content, name, out var value)) continue;

                    var context = new ArgumentAttribute.ParseContext(message);
                    
                    // try to parse the value
                    try
                    {
                        var result = await fromArg.TryParse(value, field.FieldType, context);
                        field.SetValue(this, result);
                        fromArg.Provided = true;
                    }
                    catch (ArgumentException e)
                    {
                        throw new ParseException(name, e.Message);
                    }
                }

                if (fromArg.Required && !fromArg.Provided)
                {
                    throw new ParseException(fromArg.Names[0], "Argument is required but was not provided.");
                }
            }
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
