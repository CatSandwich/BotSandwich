using System;
using System.Threading.Tasks;
using Discord;

namespace BotSandwich.Data.Commands
{
    /// <summary>
    /// Tells the command to listen for an argument and populate this field when provided.
    /// </summary>
    public class ArgumentAttribute : Attribute
    {
        public string[] Names;
        public string Description;
        public bool Required;
        public bool Provided;

        public ArgumentAttribute(bool required, params string[] names)
        {
            Names = names;
            Required = required;
            Description = "Descriptions are temporarily broken.";
        }

        /// <summary>
        /// Parses the string through custom type-based parsers.
        /// </summary>
        /// <param name="s">The value parsed</param>
        /// <param name="type">The type returned by the parse</param>
        /// <param name="context">Required context to parse properly</param>
        /// <returns>The parsed data</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<object> TryParse(string s, Type type, ParseContext context)
        {
            if (type == typeof(uint))
            {
                if(!uint.TryParse(s, out var temp)) throw new ArgumentException("Could not parse value as uint");
                return temp;
            }

            if (type == typeof(ulong))
            {
                if(!ulong.TryParse(s, out var temp)) throw new ArgumentException("Could not parse value as ulong");
                return temp;
            }

            if (type == typeof(string))
            {
                return s;
            }

            if (type == typeof(IMessage))
            {
                if(!ulong.TryParse(s, out var id)) throw new ArgumentException("Could not parse message id as ulong");
                var message = await context.Message.Channel.GetMessageAsync(id);
                if (message is null) throw new ArgumentException("Could not find a message of that id");
                return message;
            }

            throw new ArgumentException("Argument type not parseable. Spam ping Josh cause he messed up.");
        }
    }

    public struct ParseContext
    {
        public IMessage Message;

        public ParseContext(IMessage message)
        {
            Message = message;
        }
    }
}
