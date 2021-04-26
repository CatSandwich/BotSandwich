using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace BotSandwich.Data.Commands
{
    public abstract class Command
    {
        public abstract string Name { get; }
        public virtual string Description => "No description set.";
        public virtual string[] Examples => new string[0];
        
        public readonly Tuple<ArgumentAttribute, FieldInfo>[] ArgumentFields;

        protected Command()
        {
            ArgumentFields = (
                from field in GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                from a in Attribute.GetCustomAttributes(field)
                let fromArgument = a as ArgumentAttribute
                where fromArgument != null
                select new Tuple<ArgumentAttribute, FieldInfo>(fromArgument, field)
            ).ToArray();
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

            foreach (var (arg, field) in ArgumentFields)
            {
                var aliases = string.Join(", ", arg.Names);
                var title = $"Argument: {aliases} ({field.FieldType.Name}, {(arg.Required ? "Required" : "Optional")}) ";
                
                var descriptionAtt = field
                                     .GetCustomAttributes()
                                     .FirstOrDefault(a => a.GetType() == typeof(ArgumentDescriptionAttribute))
                                     as ArgumentDescriptionAttribute;
                var description = descriptionAtt?.Description ?? "No description provided.";
                
                temp.AddField(title, description);
            }
            
            foreach(var e in Examples) temp.AddField("Example:", $"{prefix}{Name} {e}");

            return temp.Build();
        }

        /// <summary>
        /// Populates the command's argument fields given the command message
        /// </summary>
        /// <param name="message">The message searched through</param>
        /// <returns>False if errors, false if required argument not provided, else true</returns>
        public async Task ParseArguments(SocketMessage message)
        {
            // foreach argument attribute
            foreach (var (arg, field) in ArgumentFields)
            {
                arg.Provided = false;

                // foreach accepted name in the argument
                foreach (var name in arg.Names)
                {
                    // check if provided
                    if(!_tryGetArgValue(message.Content, name, out var value)) continue;

                    var context = new ParseContext(message);
                    
                    // try to parse the value
                    try
                    {
                        var result = await arg.TryParse(value, field.FieldType, context);
                        field.SetValue(this, result);
                        arg.Provided = true;
                    }
                    catch (ArgumentException e)
                    {
                        throw new ParseException(name, e.Message);
                    }
                }

                if (arg.Required && !arg.Provided)
                {
                    throw new ParseException(arg.Names[0], "Argument is required but was not provided.");
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
                result = null;
                return false;
            }

            var match = quoteMatch.Success ? quoteMatch.Value : noQuoteMatch.Value;
            result = match.Replace("\"", "").Substring(name.Length + 2);
            return true;
        }
        
        public bool HasArgument(string name) => ArgumentFields.Any(a => a.Item1.Names.Contains(name) && a.Item1.Provided);
        public abstract Task Run(Module module, SocketMessage sm, string msg);
    }
}
