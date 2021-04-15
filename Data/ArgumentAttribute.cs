using System;
using System.Collections.Generic;
using System.Text;
using Discord;

namespace BotSandwich.Data
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
        }

        /// <summary>
        /// Parses the string through custom type-based parsers.
        /// </summary>
        /// <param name="s">The value parsed</param>
        /// <param name="type">The type returned by the parse</param>
        /// <param name="result">The parsed result</param>
        /// <returns>Whether or not the parse was successful</returns>
        public bool TryParse(string s, Type type, out object result)
        {
            if (type == typeof(uint))
            {
                var success = uint.TryParse(s, out var temp);
                result = temp;
                return success;
            }

            if (type == typeof(ulong))
            {
                var success = ulong.TryParse(s, out var temp);
                result = temp;
                return success;
            }

            if (type == typeof(string))
            {
                result = s;
                return true;
            }

            result = null;
            return false;
        }
    }
}
